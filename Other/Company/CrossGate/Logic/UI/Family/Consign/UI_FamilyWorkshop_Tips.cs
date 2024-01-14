using Lib.Core;
using Logic.Core;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;


namespace Logic
{
    public class OpenFamilyConsignTipsParm
    {
        public ItemData equip;
        public uint formulaId;
        public string roleName;
    }

    public class UI_FamilyWorkshop_Tips : UIBase
    {
        private GameObject m_PropItemGo;
        private PropItem m_PropItem;
        private Text m_Desc;
        private Text m_Exp;
        private Text m_FamlimyContribute;
        private Text m_ItemName;
        private Button m_ClickClose;

        private OpenFamilyConsignTipsParm m_OpenFamilyConsignTipsParm;
        private uint m_FormulaId;
        private CSVFormula.Data cSVFormulaData;
        private ItemData m_Equip;

        protected override void OnOpen(object arg)
        {
            m_OpenFamilyConsignTipsParm = arg as OpenFamilyConsignTipsParm;
            m_FormulaId = m_OpenFamilyConsignTipsParm.formulaId;
            cSVFormulaData = CSVFormula.Instance.GetConfData(m_FormulaId);
            m_Equip = m_OpenFamilyConsignTipsParm.equip;
        }

        protected override void OnLoaded()
        {
            m_PropItemGo = transform.Find("Animator/View_Left/PropItem").gameObject;
            m_Desc = transform.Find("Animator/View_Right/Text_Message").GetComponent<Text>();
            m_Exp = transform.Find("Animator/View_Right/Grid_Award/AwardItem/Text_Number").GetComponent<Text>();
            m_FamlimyContribute = transform.Find("Animator/View_Right/Grid_Award/AwardItem (1)/Text_Number").GetComponent<Text>();
            m_ItemName = transform.Find("Animator/View_Left/PropItem/Text_Name").GetComponent<Text>();
            m_ClickClose = transform.Find("Animator/Image_BG").GetComponent<Button>();

            m_PropItem = new PropItem();
            m_PropItem.BindGameObject(m_PropItemGo);

            m_ClickClose.onClick.AddListener(OnCloseButtonClicked);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_FamilyWorkshop_Tips);
        }

        private void UpdateInfo()
        {
            PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                               (_id: m_Equip.cSVItemData.id,
                               _count: 0,
                               _bUseQuailty: true,
                               _bBind: false,
                               _bNew: false,
                               _bUnLock: false,
                               _bSelected: false,
                               _bShowCount: false,
                               _bShowBagCount: false,
                               _bUseClick: true,
                               _onClick: OnClickEquip,
                               _bshowBtnNo: false,
                               _bUseTips: false);
            showItem.SetQuality(m_Equip.Quality);
            m_PropItem.SetData(new MessageBoxEvt(EUIID.UI_FamilyWorkshop_Tips, showItem));

            TextHelper.SetText(m_Desc, LanguageHelper.GetTextContent(590002026, m_OpenFamilyConsignTipsParm.roleName));
            TextHelper.SetText(m_Exp, cSVFormulaData.proficiency.ToString());
            TextHelper.SetText(m_FamlimyContribute, cSVFormulaData.assist_reward.ToString());
            TextHelper.SetText(m_ItemName, m_Equip.cSVItemData.name_id);
        }

        private void OnClickEquip(PropItem propItem)
        {
            EquipTipsData tipData = new EquipTipsData();
            tipData.equip = m_Equip;
            tipData.isShowOpBtn = false;
            tipData.isCompare = false;
            UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
        }
    }
}


