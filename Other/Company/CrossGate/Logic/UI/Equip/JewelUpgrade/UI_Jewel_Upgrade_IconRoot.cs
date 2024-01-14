using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using System;

namespace Logic
{
    public class UI_Jewel_Upgrade_IconRoot
    {
        private Transform transform;
 
        private Item0_Layout layout;
        private Text jewelNum; //宝石数量
        private Text textName;
        private Text textCostNum;  //消耗数量
        private Button btnSub;
        private Button btnAdd;

        private Sys_Equip.SubJewelData subJewelData;
        private uint totalNum;
        private uint useNum;
        private uint singleOneCount;

        public void Init(Transform trans)
        {
            transform = trans;

            layout = new Item0_Layout();
            layout.BindGameObject(transform.Find("PropItem/Btn_Item").gameObject);
            layout.btnItem.onClick.AddListener(OnClickJewel);

            jewelNum = transform.Find("PropItem/Text_Number").GetComponent<Text>();
            jewelNum.gameObject.SetActive(true);

            textName = transform.Find("Text_Name").GetComponent<Text>();
            textCostNum = transform.Find("Image1/Text").GetComponent<Text>();

            btnSub = transform.Find("Image1/Button_Sub").GetComponent<Button>();
            btnSub.onClick.AddListener(OnClickSub);

            btnAdd = transform.Find("Image1/Button_Add").GetComponent<Button>();
            btnAdd.onClick.AddListener(OnClickAdd);
        }

        private void OnClickJewel()
        {

        }

        private void OnClickSub()
        {
            if (subJewelData.jewelUseNum <= 0u)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4190u));
            }
            else
            {
                subJewelData.jewelUseNum--;
                Sys_Equip.Instance.OnAddLeftUpgradeCount(singleOneCount);
                RefreshNum();
            }
        }

        private void OnClickAdd()
        {
            if (totalNum <= subJewelData.jewelUseNum)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4192u));
            }
            else
            {
                if (singleOneCount > Sys_Equip.Instance.LeftUpgradeCount)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4191u));
                }
                else
                {
                    subJewelData.jewelUseNum++;
                    Sys_Equip.Instance.OnMinusLeftUpgradeCount(singleOneCount);
                    RefreshNum();
                }
            }
        }

        public void UpdateInfo(uint jewelId)
        {
            singleOneCount = Sys_Equip.Instance.TransJewelMinCount(jewelId);

            CSVItem.Data jewelItem = CSVItem.Instance.GetConfData(jewelId);
            layout.SetData(jewelItem, false);
            textName.text = LanguageHelper.GetTextContent(jewelItem.name_id);

            subJewelData = Sys_Equip.Instance.GetSubJewelData(jewelId);
            if (subJewelData == null)
                Debug.LogErrorFormat("{}  is Error", jewelId);

            totalNum = subJewelData.groupData != null ? subJewelData.groupData.count : 0u;
            jewelNum.text = string.Format("x{0}", totalNum.ToString());

            RefreshNum();
        }

        private void RefreshNum()
        {
            useNum = subJewelData.jewelUseNum;
            textCostNum.text = useNum.ToString();
        }
    }
}


