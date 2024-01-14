using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_TerroristWeek_Single : UIComponent
    {
        #region UI
        private Image m_ImgRoleHead;
        private Text m_TextRoleLevel;
        private Text m_TextRoleName;
        private Image m_ImgRoleCarrier;
        private Text m_TextRoleCarrier;


        private List<PropItem> listItems = new List<PropItem>();
        #endregion
        
        protected override void Loaded()
        {
            m_ImgRoleHead = transform.Find("role/Head").GetComponent<Image>();
            m_TextRoleLevel = transform.Find("role/Text_Lv/Text_Num").GetComponent<Text>();
            m_TextRoleName = transform.Find("role/Text_Name").GetComponent<Text>();
            m_ImgRoleCarrier = transform.Find("role/Image_Profession").GetComponent<Image>();
            m_TextRoleCarrier = transform.Find("role/Image_Profession/Text").GetComponent<Text>();

            for (int i = 0; i < 5; ++i)
            {
                string name = string.Format("Grid/PropItem ({0})", i + 1);

                PropItem prop = new PropItem();
                prop.BindGameObject(transform.Find(name).gameObject);

                listItems.Add(prop);
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

        public void UpdateInfo(TerrorSeriesMemItems.Types.Mem mem)
        {
            ClientHeadData headData = Sys_Head.Instance.clientHead;
            CharacterHelper.SetHeadAndFrameData(m_ImgRoleHead, Sys_Role.Instance.Role.HeroId, headData.headId, headData.headFrameIconId);
            m_TextRoleLevel.text = Sys_Role.Instance.Role.Level.ToString();
            m_TextRoleName.text = Sys_Role.Instance.sRoleName;

            ImageHelper.SetIcon(m_ImgRoleCarrier, OccupationHelper.GetLogonIconID(Sys_Role.Instance.Role.Career));
            uint textCarrierId = OccupationHelper.GetTextID(Sys_Role.Instance.Role.Career, Sys_Role.Instance.Role.CareerRank);
            if (textCarrierId != 0u)
                m_TextRoleCarrier.text = LanguageHelper.GetTextContent(textCarrierId);
            else
                m_TextRoleCarrier.text = "";

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
}


