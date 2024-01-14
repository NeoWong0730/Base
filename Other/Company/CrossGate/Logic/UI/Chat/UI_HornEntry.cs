using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_HornEntry
    {
        private const string lightName = "chat_Bottom_light";
        private const string darkName = "chat_Bottom4";

        private Transform mTrans;
        private GameObject mBaseItemRoot;
        private Text txt_CostNum_Text;
        private Text txt_Name_Text;
        private Text txt_Count_Text;
        private Button tempItem_Button;
        private Image tempItem_Image;

        private Item0_Layout BaseLayout = new Item0_Layout();
        private HornItemData mHornItem;

        public void BindGameObject(GameObject gameObject)
        {
            mTrans = gameObject.transform;
            mBaseItemRoot = mTrans.Find("_Btn_Item02").gameObject;
            txt_CostNum_Text = mTrans.Find("_txt_CostNum").GetComponent<Text>();
            txt_Name_Text = mTrans.Find("_txt_Name").GetComponent<Text>();
            txt_Count_Text = mTrans.Find("_txt_Count").GetComponent<Text>();

            tempItem_Button = mTrans.GetComponent<Button>();
            tempItem_Image = mTrans.GetComponent<Image>();

            BaseLayout.BindGameObject(mBaseItemRoot);

            tempItem_Button.onClick.AddListener(OnButtonClicked);
        }

        public void SetData(HornItemData hornItem)
        {
            mHornItem = hornItem;
            BaseLayout.SetData(hornItem.mItemData, true);
            txt_CostNum_Text.text = hornItem.mHornData.price.ToString();
            txt_Name_Text.text = LanguageHelper.GetTextContent(hornItem.mItemData.name_id);

            long itemCount = Sys_Bag.Instance.GetItemCount(hornItem.mHornData.id);
            txt_Count_Text.text = itemCount.ToString();
            txt_Count_Text.color = itemCount > 0 ? Color.white : Color.red;

            if (hornItem.mItemData.id == Sys_Chat.Instance.nLastSelectedFullServerHorn || hornItem.mItemData.id == Sys_Chat.Instance.nLastSelectedSingleServerHorn)
            {
                //TODO:配置到图标表
                ImageHelper.SetIcon(tempItem_Image, 992121);
                //ImageHelper.SetIcon(tempItem_Image, GlobalAssets.sAtlas_Common, "Common_Frame-selected");                
            }
            else
            {
                //TODO:配置到图标表
                ImageHelper.SetIcon(tempItem_Image, 992120);
                //ImageHelper.SetIcon(tempItem_Image, GlobalAssets.sAtlas_Common, "Common_bottom2");                
            }
        }

        public void OnButtonClicked()
        {
            Sys_Chat.Instance.SelectedHorn(mHornItem.mItemData);
        }
    }
}