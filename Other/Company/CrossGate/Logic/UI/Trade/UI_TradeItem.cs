using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_TradeItem 
    {
        public Transform transform;

        public Button BtnBg;
        public Button BtnMask;
        public Text TextName;
        public Text TextTip;

        public PropSpecialItem propItem;
        public GameObject goDomestication_0;
        public GameObject goDomestication_1;

        public Text TextPriceInfo;
        public Text TextPrice;

        public Button BtnWatch;
        public Image ImgWatch;

        public Button BtnHeart;
        public Image ImgHeart;

        public Image ImgTip;

        public Text TextWatchNum;

        public Button BtnTip;
        public Text TextState;
        public Text TextTime;

        public Image ImgCrossTip;
        public Transform transFx; //特效

        public void Init(Transform trans)
        {
            transform = trans;

            Loaded();
        }

        private  void Loaded()
        {
            BtnBg = transform.Find("Image_BG").GetComponent<Button>();
            BtnMask = transform.Find("Image_mask").GetComponent<Button>();
            BtnMask.gameObject.SetActive(false);

            TextName = transform.Find("Text_name").GetComponent<Text>();
            TextTip = transform.Find("Text_name/Text_tip").GetComponent<Text>();

            propItem = new PropSpecialItem();
            propItem.BindGameObject(transform.Find("PropItem").gameObject);
            propItem.transform.gameObject.SetActive(true);

            goDomestication_0 = transform.Find("Image_Domestication_0").gameObject;
            goDomestication_1 = transform.Find("Image_Domestication_1").gameObject;

            TextPriceInfo = transform.Find("Text_Price").GetComponent<Text>();
            TextPrice = transform.Find("Text_Price/Text_Num").GetComponent<Text>();

            BtnWatch = transform.Find("Image_star").GetComponent<Button>();
            ImgWatch = transform.Find("Image_star/Image").GetComponent<Image>();
            BtnWatch.gameObject.SetActive(false);

            BtnHeart = transform.Find("Image_heart").GetComponent<Button>();
            ImgHeart = transform.Find("Image_heart/Image").GetComponent<Image>();
            BtnHeart.gameObject.SetActive(false);

            ImgTip = transform.Find("Image_tip").GetComponent<Image>();

            TextWatchNum = transform.Find("Text_WatchNum").GetComponent<Text>();
            TextWatchNum.gameObject.SetActive(false);

            BtnTip = transform.Find("ButtonTip").GetComponent<Button>();
            BtnTip.gameObject.SetActive(false);

            TextState = transform.Find("Text_State").GetComponent<Text>();
            TextTime = transform.Find("Text_Time").GetComponent<Text>();

            ImgCrossTip = transform.Find("Image_ServerTip").GetComponent<Image>();

            transFx = transform.Find("Fx_ui_quan_mandang");
            transFx.gameObject.SetActive(false);
        }

        public void SetDetailType(Sys_Trade.DetailSourceType srcType)
        {
            propItem.detailSrcType = srcType;
        }

        public void ShowFx(bool isShow)
        {
            transFx.gameObject.SetActive(isShow);
        }
    }
}


