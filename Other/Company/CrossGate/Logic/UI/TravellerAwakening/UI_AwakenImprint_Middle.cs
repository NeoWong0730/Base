using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Text;
using System;
using logic;
using Lib.Core;
using UnityEngine.UI.Extensions;

namespace Logic
{
    public class UI_AwakenImprint_Middle : UIComponent
    {
        private GameObject view_Postion;
        private LineRenderer lineRendererProto;
        private GameObject updateFx;
        private GameObject activeFX;
        private IListener listener;
        private List<UIImprintNode> imprintNodes = new List<UIImprintNode>();
        private ImprintEntry iEntry;
        private GameObject ui_Line;
        private UILineRenderer ui_LineShine;
        private UILineRenderer ui_LineGray;
        private uint labelId;
        private ImprintNode showNode;
        private bool IsActiveFxFlash;
        private Timer m_timer;
        protected override void Loaded()
        {
            view_Postion = transform.Find("View_Position").gameObject;

            ui_Line= transform.Find("UILine").gameObject;
            ui_LineShine = transform.Find("UILine/UILineRendererShine").GetComponent<UILineRenderer>();
            ui_LineGray = transform.Find("UILine/UILineRendererGray").GetComponent<UILineRenderer>();
            updateFx = transform.Find("Fx_ui_Energyspar04").gameObject;
            activeFX = transform.Find("Fx_ui_Energyspar01").gameObject;

        }

        public override void Show()
        {
            base.Show();
            labelId = Sys_TravellerAwakening.Instance.SelectIndex + 1;
            LabelRefresh(labelId - 1);
            view_Postion.transform.GetComponent<Canvas>().sortingOrder = Sys_TravellerAwakening.Instance.panelOrder + 2;
            ui_Line.transform.GetComponent<Canvas>().sortingOrder = Sys_TravellerAwakening.Instance.panelOrder + 1;

        }

        public void PanelOrderFresh()
        {
            view_Postion.transform.GetComponent<Canvas>().sortingOrder = Sys_TravellerAwakening.Instance.panelOrder + 2;
            ui_Line.transform.GetComponent<Canvas>().sortingOrder = Sys_TravellerAwakening.Instance.panelOrder + 1;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            m_timer?.Cancel();
            imprintNodes.Clear();
            FrameworkTool.DestroyChildren(ui_LineShine.transform.parent.gameObject, ui_LineShine.transform.name, ui_LineGray.transform.name);
        }
        private void InitNode()
        {
            iEntry = Sys_TravellerAwakening.Instance.GetImprintEntry(labelId);
            imprintNodes.Clear();
            int childCount = view_Postion.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                view_Postion.transform.GetChild(i).gameObject.SetActive(false);

            }

            for (int i = 0; i < iEntry.imprintList.Count; i++)
            {
                view_Postion.transform.GetChild((int)iEntry.imprintList[i].GetPositionIndex()).gameObject.SetActive(true);
                UIImprintNode node = new UIImprintNode();
                node.InitNode(view_Postion.transform.GetChild((int)iEntry.imprintList[i].GetPositionIndex()));
                node.SetNodeData(iEntry.imprintList[i]);
                node.AddRefreshListener(OnClickNode);
                imprintNodes.Add(node);

            }
            updateFx.SetActive(false);
            activeFX.SetActive(false);

        }

        #region Original

        private void RefreshLines()
        {
            if (Sys_TravellerAwakening.Instance.nowNode == null || Sys_TravellerAwakening.Instance.nowNode.csv.Node_Type == 1 || Sys_TravellerAwakening.Instance.nowNode.ThisNodeIsMaxGrade())
            {
                CreateLines();

            }
        }
        private void CreateLines()
        {
            FrameworkTool.DestroyChildren(ui_LineShine.transform.parent.gameObject, ui_LineShine.transform.name, ui_LineGray.transform.name);
            int totalCount = iEntry.imprintList.Count - 1;
            for (int i = 0, j = 1; i < totalCount; i++, j++)
            {
                UIImprintNode nextNode;
                if (imprintNodes[j]._nodeType == 1)
                {
                    nextNode = imprintNodes[totalCount];
                }
                else
                {
                    nextNode = imprintNodes[j];
                }
                DrawLine(imprintNodes[i], nextNode);
            }


        }
        private void DrawLine(UIImprintNode left, UIImprintNode right)
        {

            UILineRenderer line = null;
            if (left._isActive && right._isActive)
            {
                GameObject go = GameObject.Instantiate<GameObject>(ui_LineShine.gameObject,ui_LineShine.transform.parent);
            line = go.transform.GetComponent<UILineRenderer>();

            }
            else
            {
                GameObject go = GameObject.Instantiate<GameObject>(ui_LineGray.gameObject, ui_LineGray.transform.parent);
                line = go.transform.GetComponent<UILineRenderer>();
            }
            line.Points[0] = left.nodeTran.anchoredPosition;
            line.Points[1] = right.nodeTran.anchoredPosition;
        }
        #endregion
        private void OnClickNode()
        {
            listener?.OnSelectNode();
            RefreshSelectNode();
        }
        private void RefreshSelectNode()
        {
            int j = 0;
            IsActiveFxFlash = true;
            for (int i = 0; i < imprintNodes.Count; i++)
            {
                imprintNodes[i].SetSelected(false);
                if (imprintNodes[i].theId == Sys_TravellerAwakening.Instance.nowNode.id)
                {
                    imprintNodes[i].SetSelected(true);
                    updateFx.transform.position = imprintNodes[i].nodePos;
                    j = i + 1;
                    j = (j == imprintNodes.Count) ? i : j;
                    if (imprintNodes[j]._nodeType != 2)
                    {//下个节点是头结点时
                        j = (imprintNodes[imprintNodes.Count - 1]._isActive) ? i : (imprintNodes.Count - 1);
                    }
                    if (!imprintNodes[i].CanActiveNext)
                    {
                        j = i;
                        IsActiveFxFlash = false;
                    }
                }
            }
            activeFX.transform.position = imprintNodes[j].nodePos;
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {//通知右边面板
            void OnSelectNode();
        }
        public void LabelRefresh(uint selectLabel)
        {
            labelId = selectLabel + 1;
            InitNode();
            Sys_TravellerAwakening.Instance.nowNode = iEntry.imprintList[0];
            RefreshLines();
            RefreshSelectNode();
            listener?.OnSelectNode();
        }
        public void RefreshPanel(uint selectLabel)
        {
            labelId = selectLabel + 1;
            InitNode();
            RefreshLines();
            updateFx.SetActive(true);
            activeFX.SetActive(IsActiveFxFlash);
            listener?.OnSelectNode();
            if (Sys_TravellerAwakening.Instance.nextNode!=null&& Sys_TravellerAwakening.Instance.nowNode.ThisNodeIsMaxGrade())
            {
                float animTime = 0.9f;
                m_timer?.Cancel();
                m_timer = Timer.Register(animTime, OnFresh);

            }
            else
            {
                RefreshSelectNode(); 
            }
            
        }
        private void OnFresh()
        {
            Sys_TravellerAwakening.Instance.nowNode = (Sys_TravellerAwakening.Instance.nextNode == null) ? Sys_TravellerAwakening.Instance.nowNode : Sys_TravellerAwakening.Instance.nextNode;
            OnClickNode();
        }

        private float CaculateParticleSystemTime(GameObject go)
        {
            float maxDuration = 0f;
            var pts = go.transform.GetComponentsInChildren<ParticleSystem>();
            foreach (var p in pts)
            {
                if (p.emission.enabled)
                {
                    if (p.main.loop)
                    {
                        return -1f;
                    }
                    float dura = 0f;
                    if (p.emission.rateOverTimeMultiplier <= 0)
                    {
                        dura = p.main.startDelayMultiplier + p.main.startLifetimeMultiplier;
                    }
                    else
                    {
                        dura = p.main.startDelayMultiplier + Mathf.Max(p.main.duration, p.main.startLifetimeMultiplier);
                    }
                    if (dura > maxDuration)
                    {
                        maxDuration = dura;
                    }
                }
            }

            return maxDuration;
        }

    }

    public class UIImprintNode
    {
        private Image frame;
        private Image icon;
        private Image selected;
        public Text level;
        public Button btn;
        ImprintNode iNode;
        private Action onOver;
        public Vector3 nodePos;
        public RectTransform nodeTran;
        public uint theId;
        public uint frameId;
        public bool _isActive;
        public uint _nodeType;
        public bool CanActiveNext;
        public enum EStatus
        {
            AllReached,
            AnyReached,
            NoReached,
        }
        public void InitNode(Transform trans)
        {
            btn = trans.Find("Image").GetComponent<Button>();
            frame = trans.Find("Image").GetComponent<Image>();
            icon = trans.Find("Image_Icon").GetComponent<Image>();
            selected = trans.Find("Image_Selected").GetComponent<Image>();
            level = trans.Find("Text_Level").GetComponent<Text>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnButtonClicked);
            selected.gameObject.SetActive(false);
            nodePos = trans.position;
            nodeTran = trans as RectTransform;
        }
        public void SetNodeData(ImprintNode _iNode)
        {
            iNode = _iNode;
            theId = iNode.id;
            frameId = iNode.csv.Farme;
            _isActive = iNode.isActive;
            _nodeType = iNode.csv.Node_Type;
            CanActiveNext = (iNode.csv.Level_Cap - iNode.level <= 1) ? true : false;
            ImageHelper.SetIcon(icon, iNode.csv.Node_Icon);
            ImageHelper.SetImageGray(icon, !iNode.isActive);
            if (iNode.csv.Node_Type==3)
            {
                ImageHelper.SetIcon(frame, 6091);
                frame.color = Sys_TravellerAwakening.Instance.FrameShow(65);
                frame.rectTransform.sizeDelta = new Vector2(76,76);
                frame.rectTransform.anchoredPosition = new Vector3(0f,-3f,0f);
            }
            else
            {
                ImageHelper.SetIcon(frame, 6090);
                frame.color = Sys_TravellerAwakening.Instance.FrameShow(iNode.csv.Farme);
                frame.rectTransform.sizeDelta = new Vector2(66, 66);
                frame.rectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
            }
            
            level.text = iNode.level.ToString() + "/" + iNode.csv.Level_Cap.ToString();
            int styleId = iNode.ThisNodeIsMaxGrade() ? 74 : 65;
            level.color = Sys_TravellerAwakening.Instance.FrameShow((uint)styleId);

        }
        public void SetSelected(bool isSelected)
        {
            selected.gameObject.SetActive(isSelected);
        }
        public void SetButtonEnable(bool isEnable)
        {
            btn.enabled = isEnable;
        }
        private void OnButtonClicked()
        {
            Sys_TravellerAwakening.Instance.nowNode = iNode;
            onOver?.Invoke();

        }

        public void AddRefreshListener(Action onOvered = null)
        {
            onOver = onOvered;
        }

    }


}