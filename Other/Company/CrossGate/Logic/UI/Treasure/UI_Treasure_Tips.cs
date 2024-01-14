using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Treasure_Tips : UIBase
    {
        //UI
        private Image mImgTips;

        private Image mIcon;
        private Text mTxtName;
        private Text mTxtLevel;
        private Button mBtnDetails;

        private Text mTxtExp;

        private GameObject mAttrTemplate;
        private string lineStr = "Image_Title_Describle01";

        private Text mDes;

        private uint mInfoId;

        protected override void OnLoaded()
        {            
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("Animator/Image_Black").gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (_) => { UIManager.CloseUI(EUIID.UI_Treasure_Tips); });

            mImgTips = transform.Find("Animator/Image_Tips").GetComponent<Image>();
            mImgTips.gameObject.SetActive(false);

            mIcon = transform.Find("Animator/Background_Root/collect_Descripration/Image_TresureBG/Image").GetComponent<Image>();
            mTxtName = transform.Find("Animator/Background_Root/collect_Descripration/Image_TresureBG/Text_Name").GetComponent<Text>();
            mTxtLevel = transform.Find("Animator/Background_Root/collect_Descripration/Image_TresureBG/Text_Rarity").GetComponent<Text>();
            mBtnDetails = transform.Find("Animator/Background_Root/collect_Descripration/Image_TresureBG/Btn_Details").GetComponent<Button>();
            mBtnDetails.onClick.AddListener(OnClickDetails);

            mTxtExp = transform.Find("Animator/Background_Root/Scroll_View/View/Image_Title_Sever/Text_Num").GetComponent<Text>();

            mAttrTemplate = transform.Find("Animator/Background_Root/Scroll_View/View/View_Describle01/Object_Property").gameObject;
            mAttrTemplate.SetActive(false);

            mDes = transform.Find("Animator/Background_Root/Scroll_View/View/View_Describle02/Describle01").GetComponent<Text>();
        }

        protected override void OnOpen(object arg)
        {            
            if (arg != null)
            {
                mInfoId = (uint)arg;
            }
        }      

        protected override void OnShow()
        {            
            UpdateInfo(mInfoId);
        }

        protected override void OnHide()
        {
            mImgTips.gameObject.SetActive(false);
        }        

        private void OnClickDetails()
        {
            mImgTips.gameObject.SetActive(!mImgTips.gameObject.activeSelf);
        }

        public void UpdateInfo(uint infoId)
        {
            CSVTreasures.Data data = CSVTreasures.Instance.GetConfData(infoId);

            ImageHelper.SetIcon(mIcon, data.icon_id);

            mTxtName.text = LanguageHelper.GetTextContent(data.name_id);
            mTxtLevel.text = LanguageHelper.GetTextContent(2009204, data.level.ToString());

            mTxtExp.text = data.unlock_exp.ToString();
            bool isUnlock = Sys_Treasure.Instance.IsUnlock(infoId);
            if (isUnlock)
            {
                mDes.text = LanguageHelper.GetTextContent(data.des_id);
            }
            else
            {
                mDes.text = LanguageHelper.GetTextContent(data.unlock_des_id);
            }

            FrameworkTool.DestroyChildren(mAttrTemplate.transform.parent.gameObject, mAttrTemplate.name, lineStr);

            for (int i = 0; i < data.display_attr.Count; ++i)
            {
                GameObject entryGo = GameObject.Instantiate<GameObject>(mAttrTemplate, mAttrTemplate.transform.parent);
                entryGo.SetActive(true);

                Text name = entryGo.transform.Find("Describle_Left").GetComponent<Text>();
                Text value = entryGo.transform.Find("Describle_Left/Text_Number").GetComponent<Text>();

                uint attrId = data.display_attr[i][0];
                uint attrValue = data.display_attr[i][1];

                CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                name.text = LanguageHelper.GetTextContent(attrData.name);
                if (attrData.show_type == 1)
                {
                    value.text = attrValue.ToString();
                }
                else if (attrData.show_type == 2)
                {
                    value.text = string.Format("{0}%", attrValue / 100f);
                }
            }
        }
    }
}


