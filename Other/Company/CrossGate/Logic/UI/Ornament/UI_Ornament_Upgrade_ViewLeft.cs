using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;
using System;

namespace Logic
{
    public class UI_Ornament_Upgrade_ViewLeft : UIComponent
    {
        private Transform content;
        private RectTransform contentRect;
        private GameObject itemCell;
        private IListener listener;
        private List<UI_Ornament_Upgrade_ViewLeftItem> listCell = new List<UI_Ornament_Upgrade_ViewLeftItem>();
        private uint lastShowType = 1;
        private uint lastShowLv = 2;
        #region 系统函数
        protected override void Loaded()
        {
            Parse();
            InitView();
        }

        public override void Show()
        {
            base.Show();
        }
        public override void Hide()
        {
            base.Hide();
            ResetCellShowState();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        #endregion

        #region func
        private void Parse()
        {
            content = transform.Find("Content");
            contentRect = content.GetComponent<RectTransform>();
            itemCell = transform.Find("Item").gameObject;
            itemCell.SetActive(false);
        }
        private void InitView()
        {
            listCell.Clear();
            GameObject.Instantiate<GameObject>(itemCell, content);
            var types = Sys_Ornament.Instance.Types;
            int count = types.Count;
            FrameworkTool.CreateChildList(content, count);
            for (int i = 0; i < count; i++)
            {
                GameObject go = content.GetChild(i).gameObject;
                UI_Ornament_Upgrade_ViewLeftItem cell = new UI_Ornament_Upgrade_ViewLeftItem();
                cell.Init(go.transform);
                cell.AddListener(RefreshTypeViewByType, OnSelectClick);
                cell.UpdateView(types[i]);
                listCell.Add(cell);
            }
            if (Sys_Ornament.Instance.UpgradeTargetUuid > 0)
            {
                ItemData itemData = Sys_Bag.Instance.GetItemDataByUuid(Sys_Ornament.Instance.UpgradeTargetUuid);
                CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(itemData.Id);
                lastShowType = ornament.type;
                lastShowLv = ornament.lv + 1;
            }
        }
        public void UpdateView()
        {
            RefreshTypeView();
        }
        public void DefaultView()
        {
            for (int i = 0; i < listCell.Count; i++)
            {
                UI_Ornament_Upgrade_ViewLeftItem cell = listCell[i];
                cell.SetState(0);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }
        private void RefreshTypeView()
        {
            for (int i = 0; i < listCell.Count; i++)
            {
                UI_Ornament_Upgrade_ViewLeftItem cell = listCell[i];
                cell.SetState(lastShowType, lastShowLv);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
            UpdateScrollPos(lastShowType, lastShowLv);
        }
        private void RefreshTypeViewByType(uint _showType)
        {
            lastShowType = _showType;
            RefreshTypeView();
        }
        private void ResetCellShowState()
        {
            for (int i = 0; i < listCell.Count; i++)
            {
                UI_Ornament_Upgrade_ViewLeftItem cell = listCell[i];
                cell.ResetShowState();
            }
        }
        private void UpdateScrollPos(uint type,uint lv)
        {
            uint typeIndex = 1;
            uint lvIndex = lv - 2;
            var types = Sys_Ornament.Instance.Types;
            for (int i = 0; i < types.Count; i++)
            {
                if(type == types[i])
                {
                    typeIndex = (uint)i;
                    break;
                }
            }
            float offSetY = 65 * typeIndex + 63 * lvIndex;
            contentRect.anchoredPosition = new Vector3(0, offSetY, 0);
        }
        public void RefreshRedPoint()
        {
            for (int i = 0; i < listCell.Count; i++)
            {
                UI_Ornament_Upgrade_ViewLeftItem cell = listCell[i];
                cell.RefreshRedPoint();
            }
        }
        #endregion

        #region event
        private void OnSelectClick(uint type, uint lv)
        {
            lastShowType = type;
            lastShowLv = lv + 1;
            listener?.OnSelectClick(type, lv);
        }
        #endregion
        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }
        public interface IListener
        {
            void OnSelectClick(uint type, uint lv);
        }

        public class UI_Ornament_Upgrade_ViewLeftItem : UIComponent
        {
            private uint type;
            private uint maxLv;
            private Text txtType;
            private Button btn;
            private Transform contentMin;
            private GameObject toggleCell;
            private GameObject goRedPoint;
            private Image imgArrow;
            private List<UI_Ornament_Upgrade_ViewLeftToggle> listMinCells = new List<UI_Ornament_Upgrade_ViewLeftToggle>();
            private Action<uint> parentAction;
            private Action<uint, uint> selectAction;
            private bool isShow = false;
            protected override void Loaded()
            {
                btn = transform.Find("Button").GetComponent<Button>();
                btn.onClick.AddListener(OnBtnClick);
                txtType = transform.Find("Button/Text").GetComponent<Text>();
                contentMin = transform.Find("Content_Small");
                toggleCell = transform.Find("Toggle").gameObject;
                toggleCell.SetActive(false);
                imgArrow = transform.Find("Button/Image_Frame").GetComponent<Image>();
                goRedPoint = transform.Find("Button/Image_Dot").gameObject;
            }

            public void UpdateView(uint _type)
            {
                type = _type;
                listMinCells.Clear();
                CSVOrnamentsType.Data typeData = CSVOrnamentsType.Instance.GetConfData(type);
                maxLv = typeData.maxlevel;
                txtType.text = LanguageHelper.GetTextContent(typeData.name);
                GameObject.Instantiate<GameObject>(toggleCell, contentMin);
                int count = (int)maxLv - 1;
                FrameworkTool.CreateChildList(contentMin, count);
                for (int i = 0; i < count; i++)
                {
                    GameObject go = contentMin.GetChild(i).gameObject;
                    UI_Ornament_Upgrade_ViewLeftToggle cell = new UI_Ornament_Upgrade_ViewLeftToggle();
                    cell.Init(go.transform);
                    cell.AddListener(OnCellSelect);
                    listMinCells.Add(cell);
                    cell.UpdateView(type, (uint)(i + 1));
                }
                goRedPoint.SetActive(Sys_Ornament.Instance.GetTypeRedPoint(type));
            }

            public void SetState(uint showType, uint showLv = 2)
            {
                if (type == showType && !isShow)
                {
                    contentMin.gameObject.SetActive(true);
                    float index = Mathf.Clamp(showLv - 2, 0, listMinCells.Count - 1);
                    listMinCells[(int)index].OpenTrigger();
                    isShow = true;
                }
                else
                {
                    contentMin.gameObject.SetActive(false);
                    isShow = false;
                }
                SetArrow(isShow);
                RefreshRedPoint();
            }
            private void SetArrow(bool select)
            {
                float rotateZ = select ? 0f : 90f;
                imgArrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotateZ);
            }
            public void ResetShowState()
            {
                isShow = false;
            }
            public void AddListener(Action<uint> parAc, Action<uint, uint> selAc)
            {
                parentAction = parAc;
                selectAction = selAc;
            }
            private void OnBtnClick()
            {
                parentAction?.Invoke(type);
            }
            private void OnCellSelect(uint type, uint lv)
            {
                selectAction?.Invoke(type, lv);
            }
            public void RefreshRedPoint()
            {
                goRedPoint.SetActive(Sys_Ornament.Instance.GetTypeRedPoint(type));
                for (int i = 0; i < listMinCells.Count; i++)
                {
                    listMinCells[i].RefreshRedPoint();
                }
            }
        }

        public class UI_Ornament_Upgrade_ViewLeftToggle : UIComponent
        {
            private uint type;
            private uint lv;
            private Text txtLv;
            private GameObject goRedPoint;
            private CP_ToggleEx parToggle;
            private Action<uint, uint> action;
            protected override void Loaded()
            {
                txtLv = transform.Find("Text").GetComponent<Text>();
                goRedPoint = transform.Find("Image_Dot").gameObject;
                parToggle = transform.GetComponent<CP_ToggleEx>();
                parToggle?.onValueChanged.AddListener(OnToggleClick);
            }

            public void UpdateView(uint _type, uint _lv)
            {
                type = _type;
                lv = _lv;
                txtLv.text = LanguageHelper.GetTextContent(4811, (lv + 1).ToString());
                RefreshRedPoint();
            }
            public void AddListener(Action<uint, uint> ac)
            {
                action = ac;
            }
            private void OnToggleClick(bool isOn)
            {
                if (isOn)
                {
                    OpenTrigger();
                }
            }
            public void OpenTrigger()
            {
                parToggle.SetSelected(true, false);
                action?.Invoke(type, lv);
            }
            public void RefreshRedPoint()
            {
                goRedPoint.SetActive(Sys_Ornament.Instance.GetLevelRedPoint(type, lv));
            }
        }
    }
}
