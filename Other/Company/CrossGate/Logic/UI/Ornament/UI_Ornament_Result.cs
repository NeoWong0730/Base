using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;
using Lib.Core;

namespace Logic
{
    public class OrnamentResultPrama
    {
        public uint type = 1;
        public ulong itemUuid;
    }
    public class UI_Ornament_Result : UIBase
    {
        private ulong uuid = 0;
        private uint pageType = 1;

        private Button btnClose;
        private Image imgIcon;
        private Text txtName;
        private Transform parant;
        private GameObject goImgUpgrade;
        private GameObject goImgRecast;
        #region 系统函数
        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(OrnamentResultPrama))
            {
                OrnamentResultPrama prama = arg as OrnamentResultPrama;
                pageType = prama.type;
                uuid = prama.itemUuid;
            }
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            UpdateView();
        }        
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Image_Black").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            imgIcon = transform.Find("Animator/Image_Successbg/Image_Result/Icon").GetComponent<Image>();
            txtName = transform.Find("Animator/Image_Successbg/Image_Result/Name").GetComponent<Text>();
            parant = transform.Find("Animator/Grid");
            goImgRecast = transform.Find("Animator/Image_Successbg/Image_Result/Image1").gameObject;
            goImgUpgrade = transform.Find("Animator/Image_Successbg/Image_Result/Image2").gameObject;
        }
        private void UpdateView()
        {
            ItemData itemData =  Sys_Bag.Instance.GetItemDataByUuid(uuid);
            if (itemData != null)
            {
                ImageHelper.SetIcon(imgIcon, itemData.cSVItemData.icon_id);
                txtName.text = LanguageHelper.GetTextContent(4811, itemData.cSVItemData.lv.ToString()) + " " + LanguageHelper.GetTextContent(itemData.cSVItemData.name_id);
                CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(itemData.Id);
                //var baseAttrs = ornament.base_attr;
                var extAttrs = itemData.ornament.ExtAttr;
                var extSkills = itemData.ornament.ExtSkill;
                int allCount = extAttrs.Count + extSkills.Count;
                FrameworkTool.CreateChildList(parant, allCount);
                for (int i = 0; i < allCount; i++)
                {
                    GameObject go = parant.GetChild(i).gameObject;
                    UI_ornament_RecastResultCellView cell = new UI_ornament_RecastResultCellView();
                    cell.Init(go.transform);
                    if (i < extAttrs.Count)
                    {
                        var extAttr = extAttrs[i];
                        CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(extAttr.AttrId);
                        cell.txtAttr.text = LanguageHelper.GetTextContent(attrData.name);
                        cell.txtValue.text = Sys_Ornament.Instance.GetExtAttrShowString(extAttr.AttrValue, extAttr.InfoId);
                        cell.SetBestState(Sys_Ornament.Instance.CheckExtAttrIsBest(extAttr.InfoId, extAttr.AttrValue));
                    }
                    else 
                    {
                        var extSkill = extSkills[i - extAttrs.Count];
                        CSVPassiveSkillInfo.Data skillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(extSkill.SkillId);
                        cell.txtAttr.text = LanguageHelper.GetTextContent(skillInfoData.name);
                        cell.txtValue.text = Sys_Ornament.Instance.GetPassiveSkillShowString(extSkill.InfoId, skillInfoData);
                        cell.SetBestState(Sys_Ornament.Instance.CheckExtSkillIsBest(extSkill.SkillId));
                    }
                }
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "未找到Uuid为 " + uuid + " 的饰品");
            }
            UpdateTitleView();
        }
        private void UpdateTitleView()
        {
            bool isUpgrade = false;
            bool isRecaset = false;
            if(pageType == (uint)EOrnamentPageType.Upgrade)
            {
                isUpgrade = true;
            }
            else if (pageType == (uint)EOrnamentPageType.Recast)
            {
                isRecaset = true;
            }
            goImgUpgrade.gameObject.SetActive(isUpgrade);
            goImgRecast.gameObject.SetActive(isRecaset);
        }
        #endregion
        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        #endregion

        public class UI_ornament_RecastResultCellView : UIComponent
        {
            public Text txtAttr;
            public Text txtValue;
            private GameObject goImgBest;//极品

            protected override void Loaded()
            {
                txtAttr = transform.Find("Attr").GetComponent<Text>();
                txtValue = transform.Find("Value").GetComponent<Text>();
                goImgBest = transform.Find("Image").gameObject;
            }

            public void UpdateView(uint key, uint value = 0)
            {
                if (key > 0)
                {
                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData((uint)key);
                    txtAttr.text = LanguageHelper.GetTextContent(attrData.name);
                    txtValue.text = Sys_Attr.Instance.GetAttrValue(attrData, value);
                }
            }

            public void SetBestState(bool isBest = false)
            {
                goImgBest.SetActive(isBest);
            }
        }
    }
}
