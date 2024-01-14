using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Framework;
using Logic;
using System;
using Table;
using Packet;

namespace Logic
{
    public class UI_ChatBg_Item
    {
        public uint id;
        public EHeadViewType type;
        private PictureFrameMap.Types.FraInfo map;
        private ClientHeadData clientHeadData;

        private Transform transform;
        private Image bgImage;
        private Text name;
        public Button selectBtn;

        public GameObject select;
        public GameObject lockTag;
        public GameObject useTag;
        public GameObject redPoint;

        public void BingGameObject(GameObject go)
        {
            this.transform = go.transform;
            bgImage = transform.Find("Image_Icon").GetComponent<Image>();
            name = transform.Find("Image_Bottom/Text_Empty").GetComponent<Text>();
            select = transform.Find("Image_Select").gameObject;
            lockTag = transform.Find("Image_Lock").gameObject;
            useTag = transform.Find("Image_Useing").gameObject;
            redPoint = transform.Find("Image_Dot").gameObject;
            selectBtn = transform.GetComponent<Button>();
            selectBtn.onClick.AddListener(OnselectBtnClick);
            useTag.SetActive(false);
            select.gameObject.SetActive(false);
        }

        public void SetData(uint _id, EHeadViewType _type)
        {
            id = _id;
            type = _type;
            clientHeadData = Sys_Head.Instance.clientHead;
            CSVChatBack.Data csvChatBack = CSVChatBack.Instance.GetConfData(id);
            if (csvChatBack != null)
            {
                ImageHelper.SetIcon(bgImage, csvChatBack.BackIcon);
                useTag.SetActive(clientHeadData.chatBackId == id);
            }
            name.text = LanguageHelper.GetTextContent(csvChatBack.BackName);
            map = Sys_Head.Instance.GetActivePictureFrameMap(id, type);
            lockTag.SetActive(map == null && csvChatBack.Lock == 0);
            redPoint.SetActive(false);
            if (Sys_Head.Instance.activeInfosCheckDic.ContainsKey(id))
            {
                redPoint.SetActive(!Sys_Head.Instance.activeInfosCheckDic[id]);
            }
            select.gameObject.SetActive(false);
        }

        public void AddRefresh()
        {
            map = Sys_Head.Instance.GetActivePictureFrameMap(id, type);
            if (map != null)
                lockTag.SetActive(map == null);
            if (Sys_Head.Instance.activeInfosCheckDic.ContainsKey(id))
            {
                redPoint.SetActive(!Sys_Head.Instance.activeInfosCheckDic[id]);
            }
        }

        private void OnselectBtnClick( )
        {
            select.gameObject.SetActive(true);
            redPoint.gameObject.SetActive(false);
            Sys_Head.Instance.eventEmitter.Trigger<uint, EHeadViewType>(Sys_Head.EEvents.OnSelectItem, id, type);       
        }
    }
}