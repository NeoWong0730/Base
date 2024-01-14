//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using Lib.Core;
//using Logic.Core;
//using Table;

//namespace Logic
//{
//    public class JewelQuickImproveData
//    {
//        public uint jewelInfoId;
//        public ulong equipUId;
//        public uint slotIndex;
//    }

//    public class UI_Jewel_Upgrade : UIBase
//    {
//        private Button btnClose;
//        private PropItem leftJewel;
//        private Text leftName;
//        private PropItem rightJewel;
//        private Text rightName;

//        private GameObject attrParent;
//        private GameObject attrTemplate;
//        private List<string> tempList = new List<string>();

//        private Button btnUpgrade;

//        private JewelQuickImproveData improveData;

//        private Animator animator;
//        private Timer timer;

//        protected override void OnLoaded()
//        {
//            base.OnLoaded();

//            btnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
//            btnClose.onClick.AddListener(OnClickClose);

//            animator = transform.Find("Animator/View_Left").GetComponent<Animator>();

//            leftJewel = new PropItem();
//            leftJewel.BindGameObject(transform.Find("Animator/View_Left/PropItem").gameObject);
//            leftName = transform.Find("Animator/View_Left/Text_Name").GetComponent<Text>();

//            rightJewel = new PropItem();
//            rightJewel.BindGameObject(transform.Find("Animator/View_Left/View_Right/PropItem").gameObject);
//            rightName = transform.Find("Animator/View_Left/View_Right/Text_Name").GetComponent<Text>();

//            attrParent = transform.Find("Animator/View_Attribute/Attr_Grid").gameObject;
//            attrTemplate = attrParent.transform.Find("Attr").gameObject;
//            attrTemplate.SetActive(false);
//            tempList.Add(attrTemplate.name);

//            btnUpgrade = transform.Find("Animator/Button_One").GetComponent<Button>();
//            btnUpgrade.onClick.AddListener(OnClickUpgrade);
//        }

//        protected override void OnOpen(object arg)
//        {
//            base.OnOpen(arg);

//            improveData = null;
//            if (arg != null)
//                improveData = (JewelQuickImproveData)arg;
//        }

//        protected override void ProcessEventsForEnable(bool toRegister)
//        {
//            base.ProcessEventsForEnable(toRegister);
//            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfQuickCompose, OnJewelComposeNtf, toRegister);
//        }

//        protected override void OnShow()
//        {
//            base.OnShow();

//            UpdatePanel();
//        }

//        protected override void OnHide()
//        {
//            timer?.Cancel();
//            timer = null;

//            base.OnHide();
//        }

//        protected override void OnDestroy()
//        {
//            timer?.Cancel();
//            timer = null;

//            base.OnDestroy();
//        }

//        private void UpdatePanel()
//        {
//            if (improveData == null || improveData.jewelInfoId == 0)
//            {
//                Debug.LogError("jewel is  null");
//                return;
//            }

//            CSVJewel.Data leftJewelInfo = CSVJewel.Instance.GetConfData(improveData.jewelInfoId);
//            CSVJewel.Data rightJewelInfo = CSVJewel.Instance.GetConfData(leftJewelInfo.next_id);

//            PropIconLoader.ShowItemData leftItem = new PropIconLoader.ShowItemData(leftJewelInfo.id, 1, true, false, false, false, false, false, true);
//            leftJewel.SetData(new MessageBoxEvt( EUIID.UI_Jewel_Upgrade, leftItem));
//            leftName.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(leftJewelInfo.id).name_id);

//            string temp = string.Format("x{0}", leftJewelInfo.num);
//            bool isCanCompond = Sys_Bag.Instance.GetItemCount(improveData.jewelInfoId) >= leftJewelInfo.num - 1;
//            uint costColorId = isCanCompond ? (uint)2007201 : 2007202;
//            leftJewel.txtNumber.gameObject.SetActive(true);
//            leftJewel.txtNumber.text = LanguageHelper.GetLanguageColorWordsFormat(temp, costColorId);

//            PropIconLoader.ShowItemData rightItem = new PropIconLoader.ShowItemData(rightJewelInfo.id, 1, true, false, false, false, false, false, true, true, null, false);
//            rightJewel.SetData(new MessageBoxEvt( EUIID.UI_Jewel_Upgrade, rightItem));
//            rightName.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(rightJewelInfo.id).name_id);

//            attrParent.DestoryAllChildren(tempList, true);

//            if (leftJewelInfo.percent != 0) //策划确认，百分比属性只有一个
//            {
//                GameObject attrGo = GameObject.Instantiate<GameObject>(attrTemplate, attrParent.transform);
//                attrGo.SetActive(true);

//                Text attrName = attrGo.transform.Find("Text_Property").GetComponent<Text>();
//                Text curValue = attrGo.transform.Find("Text_Num").GetComponent<Text>();
//                Text desValue = attrGo.transform.Find("Text_Num1").GetComponent<Text>();
//                Text diffValue = attrGo.transform.Find("Text_Num2").GetComponent<Text>();

//                attrName.text = LanguageHelper.GetTextContent(4050);
//                curValue.text = string.Format("+{0}%", leftJewelInfo.percent);
//                desValue.text = string.Format("+{0}%", rightJewelInfo.percent);
//                diffValue.text = string.Format("+{0}%", rightJewelInfo.percent - leftJewelInfo.percent);
//            }
//            else
//            {
//                for (int i = 0; i < leftJewelInfo.attr.Count; ++i)
//                {
//                    GameObject attrGo = GameObject.Instantiate<GameObject>(attrTemplate, attrParent.transform);
//                    attrGo.SetActive(true);

//                    Text attrName = attrGo.transform.Find("Text_Property").GetComponent<Text>();
//                    Text curValue = attrGo.transform.Find("Text_Num").GetComponent<Text>();
//                    Text desValue = attrGo.transform.Find("Text_Num1").GetComponent<Text>();
//                    Text diffValue = attrGo.transform.Find("Text_Num2").GetComponent<Text>();

//                    List<uint> attr = leftJewelInfo.attr[i];
//                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attr[0]);
//                    attrName.text = LanguageHelper.GetTextContent(attrData.name);
//                    curValue.text = Sys_Attr.Instance.GetAttrValue(attrData, attr[1]);
//                    desValue.text = Sys_Attr.Instance.GetAttrValue(attrData, rightJewelInfo.attr[i][1]);
//                    diffValue.text = Sys_Attr.Instance.GetAttrValue(attrData, rightJewelInfo.attr[i][1] - attr[1]);
//                }
//            }
//        }

//        private void OnClickClose()
//        {
//            UIManager.CloseUI(EUIID.UI_Jewel_Upgrade);
//        }

//        private void OnClickUpgrade()
//        {
//            Sys_Equip.Instance.OnQuickComposeReq(improveData.equipUId, improveData.slotIndex);
//        }

//        private void OnJewelComposeNtf()
//        {
//            animator.enabled = true;
//            animator.Play("UpgradeMosaic", 0, 0f);

//            timer = Timer.Register(1.55f, () =>
//            {
//                UIManager.CloseUI(EUIID.UI_Jewel_Upgrade);
//            });
//        }
//    }
//}
