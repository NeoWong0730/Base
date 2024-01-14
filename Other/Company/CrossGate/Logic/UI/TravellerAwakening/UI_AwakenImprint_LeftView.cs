using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System.Text;
using System;
using System.Linq;

namespace Logic {

    public class ImprintLabel
    {
        public uint nameId;
        public bool IsOpen;

    }
    public class UI_AwakenImprint_LeftCeil : UIParseCommon
    {
        private GameObject btn_Menu_Dark;
        private GameObject image_Menu_Light;
        private Text txt_Menu_Dark;
        private Text txt_Menu_Light;
        private GameObject go_Lock;
        private GameObject go_RedPoint;
        private CP_Toggle iToggle;
        private int gridIndex;
        private bool IsHint;
        protected override void Parse()
        {

            btn_Menu_Dark = transform.Find("Btn_Menu_Dark").gameObject;
            txt_Menu_Dark = transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
            image_Menu_Light = transform.Find("Image_Menu_Light").gameObject;
            txt_Menu_Light = transform.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();
            go_RedPoint = transform.Find("Image_Dot").gameObject; 
            go_Lock = transform.Find("Lock").gameObject;
            iToggle = transform.gameObject.GetComponent<CP_Toggle>();
            iToggle.onValueChanged.AddListener(OnClickToggle);
            go_RedPoint.SetActive(false);
        }
        public override void Show()
        {
        }

        public override void Hide()
        {

        }

        private void OnClickToggle(bool isOn)
        {
            if (IsHint&&isOn)
            {
                CSVTravellerAwakening.Data tData = CSVTravellerAwakening.Instance.GetConfData((uint)(gridIndex+ Sys_TravellerAwakening.Instance.GetAwakenOpenLevel()+1));
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010172,LanguageHelper.GetTextContent(tData.NameId)));
                iToggle.SetSelected(false,false);
            }
        }
        public void UpdateRedPoint(bool isShow)
        {
            go_RedPoint.SetActive(isShow);
        }

        public void SetData(ImprintLabel iLabel,int index)
        {
            gridIndex = index;
            iToggle.id = index;
            go_RedPoint.SetActive(Sys_TravellerAwakening.Instance.imprintLabelRedPointList[index]);
            txt_Menu_Dark.text = LanguageHelper.GetTextContent(iLabel.nameId);
            txt_Menu_Light.text = LanguageHelper.GetTextContent(iLabel.nameId);
            go_Lock.SetActive(!iLabel.IsOpen);
            IsHint = !iLabel.IsOpen;
            iToggle.SetSelected(Sys_TravellerAwakening.Instance.SelectIndex == index, true);
            
        }
    }
    public class UI_AwakenImprint_LeftView : UIComponent
    {
        
        private CP_ToggleRegistry viewToggle;
        private IListener listener;
        private InfinityGrid imprintInfinityGrid;
        private List<ImprintLabel> leftList = new List<ImprintLabel>();
        private List<UI_AwakenImprint_LeftCeil> ceilList = new List<UI_AwakenImprint_LeftCeil>();
        private uint nowType;

        protected override void Loaded()
        {
            
            viewToggle = this.transform.GetComponent<CP_ToggleRegistry>();
            imprintInfinityGrid = transform.Find("Scroll_Rank").GetComponent<InfinityGrid>();
            viewToggle.onToggleChange += OnToggleChange;
            imprintInfinityGrid.onCreateCell += OnCreateCell;
            imprintInfinityGrid.onCellChange += OnCellChange;
            
        }

        public override void Show()
        {
            nowType = Sys_TravellerAwakening.Instance.SelectIndex;
            InitList();
            
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            leftList.Clear();
            ceilList.Clear();
        }

        private void InitList()
        {
            leftList.Clear();
            ceilList.Clear();
            for (int i=1;i<=CSVImprintLabel.Instance.Count;i++)
            {
                CSVImprintLabel.Data iData= CSVImprintLabel.Instance.GetConfData((uint)i);
                ImprintLabel _label=new ImprintLabel();
                _label.nameId = iData.Label_Name;

                if (i == 1)
                {
                    _label.IsOpen = true;
                }
                else
                {
                    _label.IsOpen = (Sys_TravellerAwakening.Instance.awakeLevel >= iData.Title_Des) ? true : false;
                }

                leftList.Add(_label);

            }
            imprintInfinityGrid.CellCount = leftList.Count;
            imprintInfinityGrid.ForceRefreshActiveCell();
            imprintInfinityGrid.MoveToIndex(0);//这里首次跳转

        }
        public void RefreshRedPoint()
        {
            for (int i=0;i<ceilList.Count;i++)
            {
                if (i>=Sys_TravellerAwakening.Instance.imprintLabelRedPointList.Count) return;
                ceilList[i].UpdateRedPoint(Sys_TravellerAwakening.Instance.imprintLabelRedPointList[i]);
            }
        }

        private void OnToggleChange(int current, int old)
        {
            if ((int)nowType == current)
                return;
            if (current>Sys_TravellerAwakening.Instance.awakeLevel- (Sys_TravellerAwakening.Instance.GetAwakenOpenLevel()+1))
            {
                return;
            }
            nowType = (uint)current;
            Sys_TravellerAwakening.Instance.SelectIndex = (uint)current;
            listener?.OnSelectListIndex(nowType);

        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_AwakenImprint_LeftCeil entry = new UI_AwakenImprint_LeftCeil();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            //cell.mRootTransform.name = index.ToString();
            UI_AwakenImprint_LeftCeil entry = cell.mUserData as UI_AwakenImprint_LeftCeil;
            entry.SetData(leftList[index],index);
            ceilList.Add(entry);
        }
        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnSelectListIndex(uint selectLabel);
        }

    }


}
