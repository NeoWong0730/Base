using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_TerroristWeek_Team : UIComponent
    {
        #region Cell
        private class TeamCell
        {
            public GameObject gameObject;
            public Image imgHead;
            public Text textLv;
            public Text textRoleName;
            public Image imgCarrier;
            public Text textCarrier;

            public List<PropItem> listItems = new List<PropItem>();

            public void Bind(GameObject go)
            {
                gameObject = go;
                Transform trans = gameObject.transform;

                imgHead = trans.Find("Head").GetComponent<Image>();
                textLv = trans.Find("Text_Lv/Text_Num").GetComponent<Text>();
                textRoleName = trans.Find("Text_Name").GetComponent<Text>();
                imgCarrier = trans.Find("Image_Profession").GetComponent<Image>();
                textCarrier = trans.Find("Image_Profession/Text").GetComponent<Text>();

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
                uint heroId = 0;
                uint rolelevel = 0;
                string roleName = "";
                uint carrer = 0;
                uint rank = 0;
                uint headId = 0;
                uint headFrameId = 0;

                if (mem.RoleId == Sys_Role.Instance.RoleId)
                {
                    heroId = Sys_Role.Instance.Role.HeroId;
                    rolelevel = Sys_Role.Instance.Role.Level;
                    roleName = Sys_Role.Instance.sRoleName;
                    carrer = Sys_Role.Instance.Role.Career;
                    rank= Sys_Role.Instance.Role.CareerRank;
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

                CharacterHelper.SetHeadAndFrameData(imgHead, heroId, headId, headFrameId);

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

                uint textCarrierId = OccupationHelper.GetTextID(carrer,rank);
                if (textCarrierId != 0u)
                    textCarrier.text = LanguageHelper.GetTextContent(textCarrierId);
                else
                    textCarrier.text = "";

                for (int i = 0; i < listItems.Count; ++i)
                {
                    if (i < mem.Items.Count && mem.Items[i].Count > 0)
                    {
                        listItems[i].transform.gameObject.SetActive(true);

                        PropIconLoader.ShowItemData itemInfo = new PropIconLoader.ShowItemData(mem.Items[i].InfoId, mem.Items[i].Count, false, false, false, false, false, false, false);
                        listItems[i].SetData(new MessageBoxEvt(EUIID.UI_TerroristWeek, itemInfo));
                    }
                    else
                    {
                        listItems[i].transform.gameObject.SetActive(false);
                    }
                }
            }
        }
        #endregion

        private List<TeamCell> listCells = new List<TeamCell>();        

        protected override void Loaded()
        {
            for (int i = 0; i < 5; ++i)
            {
                string str = string.Format("Content/Item{0}", i);

                TeamCell cell = new TeamCell();
                cell.Bind(transform.Find(str).gameObject);

                listCells.Add(cell);
            }
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
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
    }
}


