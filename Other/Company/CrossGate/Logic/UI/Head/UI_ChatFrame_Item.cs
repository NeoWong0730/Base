using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;

namespace Logic
{
    public class UI_ChatFrame_Item 
    {
        public uint id;
        public EHeadViewType type;
        private PictureFrameMap.Types.FraInfo map;
        private ClientHeadData clientHeadData;

        private Transform transform;
        private Image chatFrame;
        private Text chatWord;
        public Button selectBtn;

        public GameObject select;
        public GameObject lockTag;
        public GameObject useTag;
        public GameObject redPoint;

        public void BingGameObject(GameObject go)
        {
            this.transform = go.transform;
            chatFrame = transform.Find("Icon").GetComponent<Image>();
            chatWord = transform.Find("Icon/Text_Name").GetComponent<Text>();
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
            map = Sys_Head.Instance.GetActivePictureFrameMap(id, type);
            redPoint.SetActive(false);
            if (Sys_Head.Instance.activeInfosCheckDic.ContainsKey(id))
            {
                redPoint.SetActive(!Sys_Head.Instance.activeInfosCheckDic[id]);
            }
            switch (type)
            {
                case EHeadViewType.ChatFrameView:
                    CSVChatframe.Data csvChatFrameData = CSVChatframe.Instance.GetConfData(id);
                    if (csvChatFrameData != null)
                    {
                        ImageHelper.SetIcon(chatFrame, csvChatFrameData.ChatIcon);
                        CSVWordStyle.Data worldData = CSVWordStyle.Instance.GetConfData(csvChatFrameData.Word);
                        if (worldData != null)
                        {
                            TextHelper.SetText(chatWord, LanguageHelper.GetTextContent(csvChatFrameData.Text), worldData);
                        }
                        useTag.SetActive(clientHeadData.chatFrameId == id);
                        lockTag.SetActive(map == null && csvChatFrameData.Lock == 0);
                    }
                    break;
                case EHeadViewType.ChatTextView:
                    CSVChatWord.Data csvChatWordData = CSVChatWord.Instance.GetConfData(id);
                    if (csvChatWordData != null)
                    {
                        ImageHelper.SetIcon(chatFrame, CSVChatframe.Instance.GetConfData(300).ChatIcon);
                        CSVWordStyle.Data worldData = CSVWordStyle.Instance.GetConfData(csvChatWordData.WordIcon);
                        if (worldData != null)
                        {
                            TextHelper.SetText(chatWord, LanguageHelper.GetTextContent(csvChatWordData.Word), worldData);
                        }
                        useTag.SetActive(clientHeadData.chatTextId == id);
                        lockTag.SetActive(map == null && csvChatWordData.Lock == 0);
                    }
                    break;
                default:
                    break;
            }
            select.gameObject.SetActive(false);
        }

        public void AddRefresh()
        {
            map = Sys_Head.Instance.GetActivePictureFrameMap(id, type);
            if (map != null)
                lockTag.SetActive(map == null);
            lockTag.SetActive(map == null);
            if (Sys_Head.Instance.activeInfosCheckDic.ContainsKey(id))
            {
                redPoint.SetActive(!Sys_Head.Instance.activeInfosCheckDic[id]);
            }
        }

        private void OnselectBtnClick( )
        {
            select.gameObject.SetActive(true);
            redPoint.SetActive(false);
            Sys_Head.Instance.eventEmitter.Trigger<uint, EHeadViewType>(Sys_Head.EEvents.OnSelectItem, id, type);          
        }
    }
}
