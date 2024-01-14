using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class TipPetEquipBtnRoot : UIParseCommon
    {
        public Button btnDisboard;   //卸下
        public Button btnReplace;   //替换 装备
        public Button btnDecompose; //分解
        public Button btnTrade;     //上架
        public Button btnRemake;    //炼化
        //private Text txtIntensify;
        private Text txtReplace;

        protected override void Parse()
        {
            btnDisboard = transform.Find("Button_Descharge").GetComponent<Button>();
            btnReplace = transform.Find("Button_Replace").GetComponent<Button>();
            btnDecompose = transform.Find("Button_Resolve").GetComponent<Button>();
            btnTrade = transform.Find("Button_Added").GetComponent<Button>();
            btnRemake = transform.Find("Button_Artifice").GetComponent<Button>();
            txtReplace = btnReplace.transform.Find("Text").GetComponent<Text>();
        }

        public void  UpdateInfo(ItemData item, uint petUid, EUIID uiid)
        {
            bool isEquiped = Sys_Pet.Instance.IsEquiped(item.Uuid, petUid);
            if (isEquiped)
            {
                btnDisboard.gameObject.SetActive(true);
                //身上装备
                txtReplace.text = LanguageHelper.GetTextContent(4019);//替换
            }
            else
            {
                btnDisboard.gameObject.SetActive(false);
                txtReplace.text = LanguageHelper.GetTextContent(4027);//装备
            }
            btnTrade.gameObject.SetActive(uiid == EUIID.UI_Bag);
            btnDecompose.gameObject.SetActive(uiid == EUIID.UI_Bag);
            btnRemake.gameObject.SetActive(item.petEquip.SuitSkill > 0);
            CSVPetEquip.Data equipInfo = CSVPetEquip.Instance.GetConfData(item.Id);
           
            btnTrade.gameObject.SetActive(!isEquiped && equipInfo.sale_least == 1); //穿戴的装备，不显示上架按钮(策划再三确认)

            int mapUse = Sys_Bag.Instance.GetItemMapUseState(item);
            if (mapUse == 0) //全部隐藏
            {
                btnDisboard.gameObject.SetActive(false);
                btnRemake.gameObject.SetActive(false);
                btnReplace.gameObject.SetActive(false);
                btnDecompose.gameObject.SetActive(false);
                btnTrade.gameObject.SetActive(false);
            }
            else if (mapUse == 1) //1 只有使用/替换
            {
                btnRemake.gameObject.SetActive(false);
                btnDecompose.gameObject.SetActive(false);
                btnTrade.gameObject.SetActive(false);
            }

            Lib.Core.FrameworkTool.ForceRebuildLayout(gameObject);
        }
    }
}
