using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_LadderPvp_Layout
    {
        public interface IListener
        {
            void OnBtnBagClick();
            void OnBtnFriendClick();
            void OnBtnTeamMemsClick();
            void OnBtnChatClick();
            void OnBtnRankClick();
            void OnBtnShopClick();
            void OnBtnFreeMatchClick();
            void OnBtnDanMatchClick();
            void OnBtnRankRewardClick();
            void OnBtnLevelUpRewardClick();
            void OnBtnTaskRewardClick();
            void OnBtnCloseClick();

            void OnBtnTeamMemAdd();

            void OnBtnTeamMemHead(uint id);
            void OnBtnTeamMemBG(uint id,Vector3 position);
        }
        public class StarItem
        {
            public Transform transform;
            public Transform transLight;
        }

        public class TeamMemItem : ClickItem
        {
            public Image ImgIcon;
            public Image ImgIconFrame;
            public Text TexName;
            public Image ImgCareeIcon;
            public Text TexLevel;
            public Text TexDanLevel;

            public Transform TransLeave;
            public Transform TransOffline;

            public Transform TransInfo;
            public Transform TransAdd;
            public Button BtnAdd;

            public Button BtnHead;

            public uint ID { get; set; }
            public Action<uint> OnClickHead;

            public Button BtnBG;
            public Action<uint,Vector3> OnClickBG;
            public override void Load(Transform root)
            {
                base.Load(root);
                BtnBG = root.Find("Image_BG").GetComponent<Button>();
                TransLeave = root.Find("Image_BG/State/Image_State0");
                TransOffline = root.Find("Image_BG/State/Image_State1");
                ImgIcon = root.Find("Image_BG/Head").GetComponent<Image>();
                ImgIconFrame = root.Find("Image_BG/Head/Image_Before_Frame").GetComponent<Image>();
                TexName = root.Find("Image_BG/Text_Name").GetComponent<Text>();
                ImgCareeIcon = root.Find("Image_BG/Image_Job").GetComponent<Image>();
                TexLevel = root.Find("Image_BG/Text_LV").GetComponent<Text>();
                TexDanLevel = root.Find("Image_BG/Text_Rank").GetComponent<Text>();

                TransInfo = root.Find("Image_BG");
                TransAdd = root.Find("Image_Add");
                BtnAdd = TransAdd.GetComponent<Button>();
                BtnHead = root.Find("Image_BG/Head").GetComponent<Button>();

                BtnHead.onClick.AddListener(OnBtnClickHead);
                BtnBG.onClick.AddListener(OnBtnClickBG);
            }

            private void OnBtnClickHead()
            {
                if (OnClickHead != null)
                    OnClickHead.Invoke(ID);
            }

            private void OnBtnClickBG()
            {
                if (OnClickBG == null)
                    return;

                
               OnClickBG.Invoke(ID,BtnBG.transform.position);
            }
        }


        public Text TexLevel;
        public Transform TranFxStar;
        public Transform TranFxStar02;
        public Text TexRank;
        public Text TexSeasonNum;
        public Text TexSeasonTime;
        public Text TexTime;
        public Text TexTimeTips;

        public Text TexScore;
        public Text TexNextScore;

        public Button BtnBag;
        public Button BtnFriend;
        public Button BtnTeamMems;
        public Button BtnChat;

        public Button BtnRank;
        public Button BtnShop;
        public Button BtnFreeMatch;
        public Button BtnDanMatch;

        public Button BtnRankReward;
        public Button BtnLevleUpReward;
        public Button BtnTaskReward;

        public Text TexFreeMatchTip;
        public Text TexDanMatchTip;

        public ClickItemGroup<TeamMemItem> TeamMemGroup = new ClickItemGroup<TeamMemItem>() { AutoClone = false };

        public Button BtnClose;


        public Transform m_TransStarUpEffect;
        public Transform m_TransStarDownEffect;
        public ParticleSystem m_PsStarDownEffect;
        public ParticleSystem m_PsStarUpEffect;

        private Timer mLevelStarEffectTimer;

        private Animator m_AnLevelEffect;

        UIPvpLevelIcon m_UIPvplevelIcon = new UIPvpLevelIcon();
        UIPvpLevelIcon m_UIPvpOldlevelIcon = new UIPvpLevelIcon();

        public Transform TransLevleUpRewardRed;
        public Transform TransTaskRewardRed;
        public void Load(Transform root)
        {
            TexScore = root.Find("Animator/Image_Score/Text_List").GetComponent<Text>();
            TexNextScore = root.Find("Animator/Image_Score2/Text_List").GetComponent<Text>();

            TexLevel = root.Find("Animator/Image_circle/Text_level").GetComponent<Text>();
            TranFxStar = root.Find("Animator/Image_circle/Fx_ui_pvp_star");
            TranFxStar02 = root.Find("Animator/Image_circle/Fx_ui_pvp_star02");

            TexRank = root.Find("Animator/Image_List/Text_List").GetComponent<Text>();
            TexSeasonNum = root.Find("Animator/Image_Label/Text_Week").GetComponent<Text>();
            TexSeasonTime = root.Find("Animator/Image_Label/Text_Date").GetComponent<Text>();

            TexTime = root.Find("Animator/Image_Time/Text_time").GetComponent<Text>();
            TexTimeTips = root.Find("Animator/Image_Time/Text").GetComponent<Text>();

            BtnBag = root.Find("Animator/Buttons_Left/Button01").GetComponent<Button>();
            BtnFriend = root.Find("Animator/Buttons_Left/Button02").GetComponent<Button>();
            BtnTeamMems = root.Find("Animator/Buttons_Left/Button03").GetComponent<Button>();
            BtnChat = root.Find("Animator/Buttons_Left/Button04").GetComponent<Button>();

            BtnRank = root.Find("Animator/Buttons_Bottom/Btn_01").GetComponent<Button>();
            BtnShop = root.Find("Animator/Buttons_Bottom/Btn_02").GetComponent<Button>();
            BtnFreeMatch = root.Find("Animator/Buttons_Bottom/Btn_03").GetComponent<Button>();
            BtnDanMatch = root.Find("Animator/Buttons_Bottom/Btn_04").GetComponent<Button>();

            TexFreeMatchTip = BtnFreeMatch.transform.Find("Text").GetComponent<Text>();
            TexDanMatchTip = BtnDanMatch.transform.Find("Text").GetComponent<Text>();

            BtnRankReward = root.Find("Animator/Buttons_Right/Button_Chest").GetComponent<Button>();
            BtnLevleUpReward = root.Find("Animator/Buttons_Right/Button_Reward").GetComponent<Button>();
            BtnTaskReward = root.Find("Animator/Buttons_Right/Button_Task").GetComponent<Button>();

            for (int i = 0; i < 6; i++)
            {
                var memtrans = root.Find("Animator/Image_team/Grid_team/Member" + i.ToString());
                TeamMemGroup.AddChild(memtrans);
            }

            BtnClose = root.Find("Animator/View_Title02/Btn_Close").GetComponent<Button>();

            m_TransStarUpEffect = root.Find("Animator/Image_cricle/Fx_ui_pvp_star");
            m_TransStarDownEffect = root.Find("Animator/Image_cricle/Fx_ui_pvp_star02");
            m_PsStarDownEffect = root.Find("Animator/Image_circle/Fx_ui_pvp_star02/Particle System").GetComponent<ParticleSystem>();
            m_PsStarUpEffect = root.Find("Animator/Image_circle/Fx_ui_pvp_star/glow").GetComponent<ParticleSystem>();

            m_AnLevelEffect = root.Find("Animator/Image_circle/Image_level").GetComponent<Animator>();


             var TransLevelOld = root.Find("Animator/Image_circle/Image_level/Level_Old");
             var TransLevelNew = root.Find("Animator/Image_circle/Image_level/Level_New");


            m_UIPvplevelIcon.Parent = TransLevelNew;
            m_UIPvpOldlevelIcon.Parent = TransLevelOld;

            TransTaskRewardRed = BtnTaskReward.transform.Find("Image_Red");
            TransLevleUpRewardRed = BtnLevleUpReward.transform.Find("Image_Red");
        }

        public void SetListener(IListener listener)
        {
            BtnClose.onClick.AddListener(listener.OnBtnCloseClick);

            BtnBag.onClick.AddListener(listener.OnBtnBagClick);
            BtnFriend.onClick.AddListener(listener.OnBtnFriendClick); 
            BtnTeamMems.onClick.AddListener(listener.OnBtnTeamMemAdd);
            BtnChat.onClick.AddListener(listener.OnBtnChatClick);

            BtnRank.onClick.AddListener(listener.OnBtnRankClick);
            BtnShop.onClick.AddListener(listener.OnBtnShopClick);
            BtnFreeMatch.onClick.AddListener(listener.OnBtnFreeMatchClick);
            BtnDanMatch.onClick.AddListener(listener.OnBtnDanMatchClick);

            BtnRankReward.onClick.AddListener(listener.OnBtnRankRewardClick);
            BtnLevleUpReward.onClick.AddListener(listener.OnBtnLevelUpRewardClick);
            BtnTaskReward.onClick.AddListener(listener.OnBtnTaskRewardClick);

            int size = TeamMemGroup.Size;
            for (int i = 0; i < size; i++)
            {
                TeamMemGroup.items[i].BtnAdd.onClick.AddListener(listener.OnBtnTeamMemsClick);

                TeamMemGroup.items[i].OnClickHead = (listener.OnBtnTeamMemHead);
                TeamMemGroup.items[i].OnClickBG = listener.OnBtnTeamMemBG;

            }
        }
    

  

  

        private void DestoryStarEffectTimer()
        {
            if (mLevelStarEffectTimer == null)
                return;

            mLevelStarEffectTimer.Cancel();

            mLevelStarEffectTimer = null;
        }

       

        public void SetLevelUpEffect(string oldLevel, string newLevel)
        {
            if (string.IsNullOrEmpty(oldLevel) == false)
                m_UIPvpOldlevelIcon.ShowLevelIcon(oldLevel);

            m_UIPvplevelIcon.ShowLevelIcon(newLevel);
            m_AnLevelEffect.enabled = true;
            m_AnLevelEffect.Play("Open");

        }

        public void SetMineLevelIcon(string id)
        {
            m_AnLevelEffect.enabled = false;
            m_UIPvpOldlevelIcon.Parent?.gameObject.SetActive(false);
            m_UIPvplevelIcon.ShowLevelIcon(id);
        }
    }
}
