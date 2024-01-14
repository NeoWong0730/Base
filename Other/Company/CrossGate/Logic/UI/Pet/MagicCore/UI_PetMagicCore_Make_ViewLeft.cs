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
    public enum EPetMagicCoreType
    {
        Type1 = 1,//魔纹
        Type2 = 2,//魔晶
        Type3 = 3,//墨粹
    }

    public class UI_PetMagicCore_Make_ViewLeft : UIComponent
    {
        private Transform content;
        private RectTransform contentRect;
        private GameObject itemCell;
        private IListener listener;
        private List<UI_PetMagicCore_Make_ViewLeftItem> listCell = new List<UI_PetMagicCore_Make_ViewLeftItem>();
        private uint lastShowType = 1;
        private uint lastShowId = 2;
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
            var types = Enum.GetValues(typeof(EPetMagicCoreType));
            int count = types.Length;
            FrameworkTool.CreateChildList(content, count);
            for (int i = 0; i < count; i++)
            {
                GameObject go = content.GetChild(i).gameObject;
                UI_PetMagicCore_Make_ViewLeftItem cell = new UI_PetMagicCore_Make_ViewLeftItem();
                cell.Init(go.transform);
                cell.AddListener(RefreshTypeViewByType, OnSelectClick);
                cell.UpdateView((uint)i + 1);
                listCell.Add(cell);
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
                UI_PetMagicCore_Make_ViewLeftItem cell = listCell[i];
                cell.SetState(0);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }
        private void RefreshTypeView()
        {
            for (int i = 0; i < listCell.Count; i++)
            {
                UI_PetMagicCore_Make_ViewLeftItem cell = listCell[i];
                cell.SetState(lastShowType, lastShowId);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
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
                UI_PetMagicCore_Make_ViewLeftItem cell = listCell[i];
                cell.ResetShowState();
            }
        }

        public void InitItemInfo(uint type, uint id)
        {
            lastShowType = type;
            lastShowId = id;
        }

        #endregion

        #region event
        private void OnSelectClick(uint type, uint id)
        {
            lastShowType = type;
            lastShowId = id;
            listener?.OnSelectClick(type, id);
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

        public class UI_PetMagicCore_Make_ViewLeftItem : UIComponent
        {
            private uint type;
            private Text txtType;
            private Button btn;
            private Transform contentMin;
            private GameObject toggleCell;
            private Image imgArrow;
            private List<UI_PetMagicCore_Make_ViewLeftToggle> listMinCells = new List<UI_PetMagicCore_Make_ViewLeftToggle>();
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
            }

            public void UpdateView(uint _type)
            {
                type = _type;
                listMinCells.Clear();
                txtType.text = LanguageHelper.GetTextContent(680010000u + _type);
                GameObject.Instantiate<GameObject>(toggleCell, contentMin);
                List<uint> vs = Sys_Pet.Instance.GetPetEquipsByType(_type);
                int count = vs.Count;
                FrameworkTool.CreateChildList(contentMin, count);
                for (int i = 0; i < count; i++)
                {
                    GameObject go = contentMin.GetChild(i).gameObject;
                    UI_PetMagicCore_Make_ViewLeftToggle cell = new UI_PetMagicCore_Make_ViewLeftToggle();
                    cell.Init(go.transform);
                    cell.AddListener(OnCellSelect);
                    listMinCells.Add(cell);
                    cell.UpdateView(type, vs[i]);
                }
            }

            public int GetIndexById(uint id)
            {
                for (int i = 0; i < listMinCells.Count; i++)
                {
                    if(listMinCells[i].id == id)
                    {
                        return i;
                    }
                }
                return 0;
            }

            public void SetState(uint showType, uint id = 0)
            {
                if (type == showType && !isShow)
                {
                    contentMin.gameObject.SetActive(true);
                    listMinCells[GetIndexById(id)].OpenTrigger();
                    isShow = true;
                }
                else
                {
                    contentMin.gameObject.SetActive(false);
                    isShow = false;
                }
                SetArrow(isShow);
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
            private void OnCellSelect(uint type, uint id)
            {
                selectAction?.Invoke(type, id);
            }
        }

        public class UI_PetMagicCore_Make_ViewLeftToggle : UIComponent
        {
            private uint type;
            public uint id;
            private Text nameText;
            private CP_ToggleEx parToggle;
            private Action<uint, uint> action;
            protected override void Loaded()
            {
                nameText = transform.Find("Text").GetComponent<Text>();
                parToggle = transform.GetComponent<CP_ToggleEx>();
                parToggle?.onValueChanged.AddListener(OnToggleClick);
            }

            public void UpdateView(uint _type, uint _id)
            {
                type = _type;
                id = _id;
                var config = CSVPetEquip.Instance.GetConfData(id);
                nameText.text = LanguageHelper.GetTextContent(config.des);
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
                action?.Invoke(type, id);
            }
        }
    }
}
