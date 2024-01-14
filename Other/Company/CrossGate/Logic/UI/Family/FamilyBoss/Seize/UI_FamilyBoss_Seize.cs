
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_FamilyBoss_Seize : UIBase
    {
        public class Cell
        {
            private Transform transform;

            private Image m_imgRank;
            private Image m_Head;
            private Image m_imgOcc;

            private Text m_textNumber;
            private Text m_textLv;
            private Text m_textName;
            private Text m_textFamily;
            private Text m_textScore;
            private UIButtonCD m_btnSeize;

            private ulong m_roleId;

            public void Init(Transform trans)
            {
                transform = trans;

                m_imgRank = transform.Find("Image_Rank").GetComponent<Image>();
                m_Head = transform.Find("Head").GetComponent<Image>();
                m_imgOcc = transform.Find("Image_Prop").GetComponent<Image>();

                m_textNumber = transform.Find("Text_Number").GetComponent<Text>();
                m_textLv = transform.Find("Text_Lv").GetComponent<Text>();
                m_textName = transform.Find("Text_Name").GetComponent<Text>();
                m_textFamily = transform.Find("Text_Family/Text").GetComponent<Text>();
                m_textScore = transform.Find("Text_Score/Text").GetComponent<Text>();

                m_btnSeize = new UIButtonCD();
                m_btnSeize.Init(transform.Find("Button"));
                m_btnSeize.OnClick = OnClickSeize;
                //m_btnSeize = transform.Find("Button").GetComponent<Button>();
                //m_btnSeize.onClick.AddListener(OnClickSeize);
            }

            public void UpdateInfo(CmdRankRole rankRole)
            {
                m_roleId = rankRole.RoleId;

                CharacterHelper.SetHeadAndFrameData(m_Head, rankRole.Hero, rankRole.HeadPhoto, rankRole.Headframe);

                if (rankRole.Rank <= 3)
                {
                    m_imgRank.gameObject.SetActive(true);
                    m_textNumber.text = "";
                    ImageHelper.SetIcon(m_imgRank, 993900 + rankRole.Rank);
                }
                else
                {
                    m_imgRank.gameObject.SetActive(false);
                    m_textNumber.text = rankRole.Rank.ToString();
                }

                ImageHelper.SetIcon(m_imgOcc, OccupationHelper.GetCareerLogoIcon(rankRole.Occ));
                //m_textNumber.text = rankRole.Rank.ToString();
                m_textLv.text = rankRole.Level.ToString();
                m_textName.text = rankRole.RoleName.ToStringUtf8();
                m_textFamily.text = rankRole.GuildName.ToStringUtf8();
                m_textScore.text = rankRole.Score.ToString();

                m_btnSeize.transform.gameObject.SetActive(m_roleId != Sys_Role.Instance.RoleId);
                m_btnSeize.Start(rankRole.ProtectEnd);
            }

            private void OnClickSeize(bool isInCD)
            {
                if (isInCD)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010171));
                else
                    Sys_FamilyBoss.Instance.OnGuildBossAttackReq(m_roleId);
            }

            public void OnDispose()
            {
                m_btnSeize?.Dispose();
            }
        }

        private InfinityGrid _infinityGrid;
        private Dictionary<GameObject, Cell> dicCells = new Dictionary<GameObject, Cell>();
        private List<CmdRankRole> m_listRankRoles;

        protected override void OnLoaded()
        {
            Button btnClose = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);

            _infinityGrid = transform.Find("Animator/Scroll_Rank").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        protected override void OnDestroy()
        {
            //ui_CurrencyTitle.Dispose();
        }
        protected override void OnOpen(object arg)
        {
            //eApplyFamilyMenu = null == arg ? EApplyFamilyMenu.Join : (EApplyFamilyMenu)System.Convert.ToInt32(arg);
        }

        protected override void OnShow()
        {
            Sys_FamilyBoss.Instance.OnGuildBossGetRoleListReq();
        }

        protected override void OnHide()
        {
            foreach (var data in dicCells)
                data.Value.OnDispose();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_FamilyBoss.Instance.eventEmitter.Handle(Sys_FamilyBoss.EEvents.OnSeizeRolesInfo, this.UpdateInfo, toRegister);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            Cell entry = new Cell();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            Cell entry = cell.mUserData as Cell;
            entry.UpdateInfo(m_listRankRoles[index]);
        }

        private void OnClickClose()
        {
            CloseSelf();
        }

        private void UpdateInfo()
        {
            m_listRankRoles = Sys_FamilyBoss.Instance.GetSeizeRoles();
            _infinityGrid.CellCount = m_listRankRoles.Count;
        }
    }
}