using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Packet;

namespace Logic
{
    public class UI_TerroristEnter_Left : UIComponent
    {
        #region UI

        private class TeamCell
        {
            public GameObject gameObject;
            private Image imgHead;
            private Text textLv;
            private Text textRoleName;
            private Image imgCarrier;
            private Image imgOk;
            private Image imgNo;
            private Image imgWait;
            private Image imgLeave;
            private Image imgLeaveMoment;
            //public Text textCarrier;

            public ulong roleId;

            public List<PropItem> listItems = new List<PropItem>();

            public void Bind(GameObject go)
            {
                gameObject = go;
                Transform trans = gameObject.transform;

                imgHead = trans.Find("Head").GetComponent<Image>();
                textLv = trans.Find("Text_Lv/Text_Num").GetComponent<Text>();
                textRoleName = trans.Find("Text_Name").GetComponent<Text>();
                imgCarrier = trans.Find("Image_Profession").GetComponent<Image>();
                imgOk = trans.Find("Image_Ok").GetComponent<Image>();
                imgNo = trans.Find("Image_No").GetComponent<Image>();
                imgWait = trans.Find("Image_Wait").GetComponent<Image>();
                imgLeave = trans.Find("Image_Leave").GetComponent<Image>();
                imgLeaveMoment = trans.Find("Image_LeaveMoment").GetComponent<Image>();

                for (int i = 0; i < 5; ++i)
                {
                    string name = string.Format("Grid/PropItem ({0})", i + 1);

                    PropItem prop = new PropItem();
                    prop.BindGameObject(trans.Find(name).gameObject);

                    listItems.Add(prop);
                }
            }

            public void UpdateInfo(TerrorSeriesMemItems.Types.Mem mem)
            {
                roleId = mem.RoleId;

                uint heroId = 0;
                uint rolelevel = 0;
                string roleName = "";
                uint carrer = 0;
                uint headId = 0;
                uint headFrameId = 0;

                if (mem.RoleId == Sys_Role.Instance.RoleId)
                {
                    heroId = Sys_Role.Instance.Role.HeroId;
                    rolelevel = Sys_Role.Instance.Role.Level;
                    roleName = Sys_Role.Instance.sRoleName;
                    carrer = Sys_Role.Instance.Role.Career;
                    headId = Sys_Head.Instance.clientHead.headId;
                    headFrameId = Sys_Head.Instance.clientHead.headFrameId;
                }
                else
                {
                    TeamMem roleInfo = Sys_Team.Instance.getTeamMem(mem.RoleId);
                    if (roleInfo != null)
                    {
                        heroId = roleInfo.HeroId;
                        rolelevel = roleInfo.Level;
                        roleName = roleInfo.Name.ToStringUtf8();
                        carrer = roleInfo.Career;
                        headId = roleInfo.Photo;
                        headFrameId = roleInfo.PhotoFrame;
                    }
                }

                CharacterHelper.SetHeadAndFrameData(imgHead, heroId,headId,headFrameId);
                uint carrierIconId = OccupationHelper.GetCareerLogoIcon(carrer);
                if (carrierIconId != 0u)
                {
                    imgCarrier.gameObject.SetActive(true);
                    ImageHelper.SetIcon(imgCarrier, carrierIconId);
                }
                else
                {
                    imgCarrier.gameObject.SetActive(false);
                }

                textLv.text = rolelevel.ToString();
                textRoleName.text = roleName;
                //textCarrier.text = LanguageHelper.GetTextContent(OccupationHelper.GetTextID(carrer));

                for (int i = 0; i < listItems.Count; ++i)
                {
                    if (i < mem.Items.Count && mem.Items[i].Count > 0)
                    {
                        listItems[i].transform.gameObject.SetActive(true);

                        PropIconLoader.ShowItemData itemInfo = new PropIconLoader.ShowItemData(mem.Items[i].InfoId, mem.Items[i].Count, false, false, false, false, false, false, false);
                        listItems[i].SetData(new MessageBoxEvt(EUIID.UI_TerroristEnter, itemInfo));
                    }
                    else
                    {
                        listItems[i].transform.gameObject.SetActive(false);
                    }
                }

                UpdateVoteState();
            }

            public void UpdateVoteState()
            {
                imgOk.gameObject.SetActive(false);
                imgNo.gameObject.SetActive(false);
                imgWait.gameObject.SetActive(false);
                imgLeave.gameObject.SetActive(false);
                imgLeaveMoment.gameObject.SetActive(false);

                VoterOpType voteState = Sys_TerrorSeries.Instance.GetVoteOp(roleId);

                switch (voteState)
                {
                    case VoterOpType.Agree:
                        imgOk.gameObject.SetActive(true);
                        break;
                    case VoterOpType.Disagree:
                        imgNo.gameObject.SetActive(true);
                        break;
                    case VoterOpType.None:
                        imgWait.gameObject.SetActive(true);
                        break;
                }
            }
        }
        #endregion

        private List<TeamCell> listCells = new List<TeamCell>();        
        protected override void Loaded()
        {
            for (int i = 0; i < 5; ++i)
            {
                string str = string.Format("Scroll View01/Viewport/Content/Item{0}", i);

                TeamCell cell = new TeamCell();
                cell.Bind(transform.Find(str).gameObject);

                listCells.Add(cell);
            }
        }

        public void UpdateInfo(TerrorSeriesMemItems mems)
        {
            for (int i = 0; i < listCells.Count; ++i)
            {
                if (i < mems.Mems.Count)
                {
                    listCells[i].gameObject.SetActive(true);
                    listCells[i].UpdateInfo(mems.Mems[i]);
                }
                else
                {
                    listCells[i].gameObject.SetActive(false);
                }
            }
        }

        public void OnVoteNtf(ulong roleId)
        {
            for (int i = 0; i < listCells.Count; ++i)
            {
                if (listCells[i].roleId == roleId)
                {
                    listCells[i].UpdateVoteState();
                    break;
                }
            }
        }
    }
}


