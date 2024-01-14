using UnityEngine;
using UnityEngine.UI;
using Table;
using Packet;

namespace Logic
{
    public class UI_Head_Item
    {
        public uint id;
        public EHeadViewType type;
        private PictureFrameMap.Types.FraInfo map;
        private ClientHeadData clientHeadData;

        private Transform transform;
        private Image headImage;
        private Image headFrameImage;
        private Image teamLogoImage;
        public Button selectBtn;

        private GameObject headGo;
        public GameObject select;
        public GameObject lockTag;
        public GameObject useTag;
        public GameObject redPoint;

        public void BingGameObject(GameObject go)
        {
            this.transform = go.transform;
            headImage = transform.Find("RawImage_BG/Image_Head").GetComponent<Image>();
            headFrameImage = transform.Find("RawImage_BG/Image_Frame").GetComponent<Image>();
            teamLogoImage = transform.Find("RawImage_BG/RawImage_Team/Icon").GetComponent<Image>();
            headGo = transform.Find("RawImage_BG").gameObject;
            select = transform.Find("Image_Select").gameObject;
            lockTag = transform.Find("Image_Lock").gameObject;
            useTag = transform.Find("Image_Useing").gameObject;
            redPoint  = transform.Find("Image_Dot").gameObject;
            selectBtn = transform.GetComponent<Button>();
            selectBtn.onClick.AddListener(OnselectBtnClick);

            headImage.gameObject.SetActive(false);
            headFrameImage.gameObject.SetActive(false);
            teamLogoImage.gameObject.SetActive(false);
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
                case EHeadViewType.HeadView:
                    CSVHead.Data csvHeadData = CSVHead.Instance.GetConfData(id);
                    if (csvHeadData != null)
                    {
                        headImage.gameObject.SetActive(true);
                        if (csvHeadData.HeadIcon[0]==0)
                        {
                            CSVCharacter.Data heroData = CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId);
                            if (heroData != null)
                            { 
                                ImageHelper.SetIcon(headImage, heroData.headid);
                            }
                        }
                        else
                        {
                            uint headIconId = Sys_Head.Instance.GetHeadIconIdByRoleType(csvHeadData.HeadIcon);
                            ImageHelper.SetIcon(headImage, headIconId);
                        }
                        useTag.SetActive(clientHeadData.headId == id);
                        lockTag.SetActive(map == null && csvHeadData.Lock==0);
                    }
                    break;
                case EHeadViewType.HeadFrameView:
                    CSVHeadframe.Data csvHeadframeData = CSVHeadframe.Instance.GetConfData(id);
                    if (csvHeadframeData != null)
                    {
                        headFrameImage.gameObject.SetActive(true);
                        if (csvHeadframeData.HeadframeIcon == 0)
                        {
                            ImageHelper.SetIcon(headFrameImage, 3002100);
                        }
                        else
                        {
                            ImageHelper.SetIcon(headFrameImage, csvHeadframeData.HeadframeIcon);
                        }
                        useTag.SetActive(clientHeadData.headFrameId == id);
                        lockTag.SetActive(map == null && csvHeadframeData.Lock == 0);
                    }
                    break;
                case EHeadViewType.TeamFalgView:
                    CSVTeamLogo.Data csvTeamLogoData = CSVTeamLogo.Instance.GetConfData(id);
                    if (csvTeamLogoData != null)
                    {
                        if (csvTeamLogoData.TeamIcon == 0)
                        {
                            teamLogoImage.gameObject.SetActive(false);
                        }
                        else
                        {
                            ImageHelper.SetIcon(teamLogoImage, csvTeamLogoData.TeamIcon);
                            teamLogoImage.gameObject.SetActive(true);
                        }
                        useTag.SetActive(clientHeadData.teamLogeId == id);
                        lockTag.SetActive(map == null && csvTeamLogoData.Lock == 0);
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
            if(map!=null)
            lockTag.SetActive(map == null);
            if (Sys_Head.Instance.activeInfosCheckDic.ContainsKey(id))
            {
                redPoint.SetActive(!Sys_Head.Instance.activeInfosCheckDic[id]);
            }
        }

        private void OnselectBtnClick()
        {
            select.gameObject.SetActive(true);
            redPoint.gameObject.SetActive(false);
            Sys_Head.Instance.eventEmitter.Trigger<uint, EHeadViewType>(Sys_Head.EEvents.OnSelectItem, id, type);           
        }
    }
}
