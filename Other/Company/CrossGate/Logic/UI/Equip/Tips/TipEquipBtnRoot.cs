using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class TipEquipBtnRoot : UIParseCommon
    {
        public Button btnIntensify; //升级
        public Button btnReplace;   //替换
        public Button btnDecompose; //分解
        public Button btnSale;      //出售
        public Button btnTrade;     //上架

        //private Text txtIntensify;
        private Text txtReplace;

        protected override void Parse()
        {
            btnIntensify = transform.Find("Button_Intensify").GetComponent<Button>();
            btnReplace = transform.Find("Button_Replace").GetComponent<Button>();
            btnDecompose = transform.Find("Button_Decompose").GetComponent<Button>();
            btnSale = transform.Find("Button_Sell").GetComponent<Button>();
            btnTrade = transform.Find("Button_PutOn").GetComponent<Button>();

            //txtIntensify = btnIntensify.transform.Find("Text").GetComponent<Text>();
            txtReplace = btnReplace.transform.Find("Text").GetComponent<Text>();
        }

        public override void  UpdateInfo(ItemData item)
        {
            if (Sys_Equip.Instance.IsEquiped(item))
            {
                //身上装备
                txtReplace.text = LanguageHelper.GetTextContent(4020);
            }
            else
            {
                ItemData tempItem = null;
                if (Sys_Equip.Instance.IsShowCompare(item.Id, ref tempItem))
                {
                    txtReplace.text = LanguageHelper.GetTextContent(4019);
                }
                else
                {
                    txtReplace.text = LanguageHelper.GetTextContent(4027);
                }
            }

            
            bool isEquiped = Sys_Equip.Instance.IsEquiped(item);
            bool isBind = item.bBind;
            bool isMake = item.Equip != null && item.Equip.BuildType != 0u;

            btnDecompose.gameObject.SetActive(!isEquiped && !isBind && isMake);

            btnSale.gameObject.SetActive(!isEquiped && !isMake);

            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(item.Id);
            //bool isFixedScore = equipInfo.sale_least != 0u && Sys_Equip.Instance.CalEquipQualityScore(item) >= equipInfo.sale_least;
            btnTrade.gameObject.SetActive(!isEquiped); //穿戴的装备，不显示上架按钮(策划再三确认)

            int mapUse = Sys_Bag.Instance.GetItemMapUseState(item);
            if (mapUse == 0) //全部隐藏
            {
                btnIntensify.gameObject.SetActive(false);
                btnReplace.gameObject.SetActive(false);
                btnDecompose.gameObject.SetActive(false);
                btnSale.gameObject.SetActive(false);
                btnTrade.gameObject.SetActive(false);
            }
            else if (mapUse == 1) //1 只有使用/替换
            {
                btnIntensify.gameObject.SetActive(false);
                //btnReplace.gameObject.SetActive(false);
                btnDecompose.gameObject.SetActive(false);
                btnSale.gameObject.SetActive(false);
                btnTrade.gameObject.SetActive(false);
            }

            Lib.Core.FrameworkTool.ForceRebuildLayout(gameObject);
        }
    }
}
