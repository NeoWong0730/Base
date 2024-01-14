using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class TipOrnamentBtnRoot : UIParseCommon
    {
        public Button btnIntensify; //升级
        public Button btnRecast;    //重铸
        public Button btnReplace;   //替换
        public Button btnDecompose; //分解
        public Button btnSale;      //出售
        public Button btnTrade;     //上架
        public Button btnBagSource;    //来源(在背包里打开)
        public bool bSourceActive;

        //private Text txtIntensify;
        private Text txtReplace;

        protected override void Parse()
        {
            btnIntensify = transform.Find("Button_Intensify").GetComponent<Button>();
            btnRecast = transform.Find("Button_Recast").GetComponent<Button>();
            btnReplace = transform.Find("Button_Replace").GetComponent<Button>();
            btnDecompose = transform.Find("Button_Decompose").GetComponent<Button>();
            btnSale = transform.Find("Button_Sell").GetComponent<Button>();
            btnTrade = transform.Find("Button_PutOn").GetComponent<Button>();
            btnBagSource = transform.Find("Button_Source").GetComponent<Button>();

            //txtIntensify = btnIntensify.transform.Find("Text").GetComponent<Text>();
            txtReplace = btnReplace.transform.Find("Text").GetComponent<Text>();
        }

        public override void UpdateInfo(ItemData item)
        {
            if (Sys_Ornament.Instance.IsEquiped(item))
            {
                //身上装备
                txtReplace.text = LanguageHelper.GetTextContent(4020);
            }
            else
            {
                ItemData tempItem = null;
                if (Sys_Ornament.Instance.IsShowCompare(item, ref tempItem))
                {
                    txtReplace.text = LanguageHelper.GetTextContent(4019);
                }
                else
                {
                    txtReplace.text = LanguageHelper.GetTextContent(4027);
                }
            }

            bool isEquiped = Sys_Ornament.Instance.IsEquiped(item);
            bool isBind = item.bBind;
            CSVOrnamentsUpgrade.Data ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(item.Id);
            bool isMaxLv = ornamentData.nextlevelid <= 0;
            bool canDecompose = (ornamentData != null && ornamentData.decompose_item != null);
            bool showSource = item.cSVItemData.ItemSource != 0 && bSourceActive;

            int itemMapUse = Sys_Bag.Instance.GetItemMapUseState(item);
            bool onlyUse = itemMapUse == 1;//只有使用/替换
            bool allShow = itemMapUse == 2;//全部显示

            //满足重铸条件
            bool canRecast = Sys_Ornament.Instance.CheckCanRecast(item.Id);

            btnIntensify.gameObject.SetActive(!isMaxLv && allShow);

            btnRecast.gameObject.SetActive(canRecast && allShow);

            btnReplace.gameObject.SetActive(allShow || onlyUse);

            btnDecompose.gameObject.SetActive(!isEquiped && !isBind && canDecompose && allShow);

            btnSale.gameObject.SetActive(!isEquiped && !canDecompose && allShow);

            btnTrade.gameObject.SetActive(!isEquiped && allShow); //穿戴的装备，不显示上架按钮(策划再三确认)

            btnBagSource.gameObject.SetActive(showSource && allShow);

            Lib.Core.FrameworkTool.ForceRebuildLayout(gameObject);
        }
    }
}
