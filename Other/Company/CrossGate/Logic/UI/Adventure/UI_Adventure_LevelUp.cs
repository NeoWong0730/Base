using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Adventure_LevelUp : UIBase
    {
        private uint level;
        #region 界面组件
        private Image imgLevelIcon1;
        private Image imgLevelIcon2;
        private Text txtLevelName;
        private Button closeBtn;
        private GameObject attrNormalCell;
        private GameObject attrSpecialCell;
        #endregion

        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnOpen(object arg)
        {
            level = (uint)arg;
        }
        protected override void OnShow()
        {
            UpdateView();
        }
        protected override void OnHide()
        {
            //UIManager.CloseUI(EUIID.UI_Adventure_LevelUp);
        }
        #endregion

        #region 初始化
        private void OnParseComponent()
        {
            imgLevelIcon1 = transform.Find("Animator/View_Left/Icon_1").GetComponent<Image>();
            imgLevelIcon2 = transform.Find("Animator/View_Left/Icon_2").GetComponent<Image>();
            txtLevelName = transform.Find("Animator/View_Left/Image/Text_Name").GetComponent<Text>();
            attrNormalCell = transform.Find("Animator/View_Right/Attr_Grid01/Viewport/Content/Attr").gameObject;
            attrNormalCell.SetActive(false);
            attrSpecialCell = transform.Find("Animator/View_Right/Attr_Grid02/Viewport/Content/Attr").gameObject;
            attrSpecialCell.SetActive(false);
            closeBtn = transform.Find("Blank").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseBtn);
        }
        #endregion

        #region 界面刷新
        private void UpdateView()
        {
            CSVAdventureLevel.Data oldData = CSVAdventureLevel.Instance.GetConfData(level-1);
            CSVAdventureLevel.Data data = CSVAdventureLevel.Instance.GetConfData(level);
            txtLevelName.text = LanguageHelper.GetTextContent(data.name);
            ImageHelper.SetIcon(imgLevelIcon1, oldData.icon);
            ImageHelper.SetIcon(imgLevelIcon2, data.icon);
            UpdateNormalAttrView(data.addAttribute);
            UpdateSpecialAttrView(data);
        }

        private void UpdateNormalAttrView(List<List<uint>> attrs)
        {
            FrameworkTool.DestroyChildren(attrNormalCell.transform.parent.gameObject, attrNormalCell.name);
            int len = attrs.Count;
            for (int i = 0; i < len; i++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(attrNormalCell, attrNormalCell.transform.parent);
                go.SetActive(true);
                List<uint> attrData = attrs[i];
                UI_NormalAttr normalAttr = new UI_NormalAttr(attrData[0], attrData[1]);
                normalAttr.Init(go.transform);
                normalAttr.UpdateCellView();
            }
        }

        private void UpdateSpecialAttrView(CSVAdventureLevel.Data data)
        {
            FrameworkTool.DestroyChildren(attrSpecialCell.transform.parent.gameObject, attrSpecialCell.name);
            if (data.addPrivilegeAttribute != null)
            {
                List<List<uint>> attrs = data.addPrivilegeAttribute;
                int len = attrs.Count;
                for (int i = 0; i < len; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(attrSpecialCell, attrSpecialCell.transform.parent);
                    go.SetActive(true);
                    List<uint> attrData = attrs[i];
                    UI_SpecialAttr specialAttr = new UI_SpecialAttr(attrData[0], attrData[1]);
                    specialAttr.Init(go.transform);
                    specialAttr.UpdateCellView();
                }
            }
            if (data.addPrivilege != null)
            {
                List<List<uint>> attrs = data.addPrivilege;
                int len = attrs.Count;
                for (int i = 0; i < len; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(attrSpecialCell, attrSpecialCell.transform.parent);
                    go.SetActive(true);
                    List<uint> attrData = attrs[i];
                    UI_SpecialAttr specialAttr = new UI_SpecialAttr(attrData[0], attrData[1], data.privilegeText[i]);
                    specialAttr.Init(go.transform);
                    specialAttr.UpdatePrivilegeCellView();
                }
            }
        }
        #endregion

        #region 响应事件
        private void OnCloseBtn()
        {
            Sys_Adventure.Instance.eventEmitter.Trigger(Sys_Adventure.EEvents.OnTryDoMainTaskByAdventureLevelUp);
            UIManager.CloseUI(EUIID.UI_Adventure_LevelUp);
        }
        #endregion
    }

    public class UI_NormalAttr : UIComponent
    {
        private uint attrId;
        private uint attrValue;
        private Text txtProperty;
        private Text txtNum;

        public UI_NormalAttr(uint id,uint value) : base()
        {
            attrId = id;
            attrValue = value;
        }
        protected override void Loaded()
        {
            txtProperty = transform.Find("Text_Property").GetComponent<Text>();
            txtNum = transform.Find("Text_Num").GetComponent<Text>();
        }

        public void UpdateCellView()
        {
            txtProperty.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(attrId).name);
            txtNum.text = attrValue.ToString();
        }
    }
    public class UI_SpecialAttr : UIComponent
    {
        private uint attrId;
        private uint attrValue;
        private uint attrDesc;
        private Text txtProperty;

        public UI_SpecialAttr(uint id,uint value,uint desc = 0) : base()
        {
            attrId = id;
            attrValue = value;
            attrDesc = desc;
        }
        protected override void Loaded()
        {
            txtProperty = transform.Find("Text_Property").GetComponent<Text>();
        }

        public void UpdateCellView()
        {
            CSVAttr.Data data = CSVAttr.Instance.GetConfData(attrId);
            string str;
            if (data.show_type == 1)
            {
                str = attrValue.ToString();
            }
            else
            {
                str = ((float)attrValue / 100f).ToString() + "%";
            }
            txtProperty.text = LanguageHelper.GetTextContent(data.name) + str;
        }

        public void UpdatePrivilegeCellView()
        {
            txtProperty.text = LanguageHelper.GetTextContent(attrDesc, attrValue.ToString());
        }
    }
}
