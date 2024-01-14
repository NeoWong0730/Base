using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Pvp_Single_Layout
    {
        private Button m_BtnClose;
        private Button m_BtnRuleDetail;

        private Button m_BtnRankReward;
        private Button m_BtnLevelUpReward;

        private Transform m_TransFxLevelUpReward;

        private Button m_BtnRank;
        private Button m_BtnMatch;

        //排名信息
        private Text m_TexRankNum;
        private Image m_ImLevelIcon;
        private Text m_TexLevel;
        private Transform m_TransLevelIcon;


        private Transform m_TransStar;
        private List<Transform> m_TransStarList = new List<Transform>(5);

        private List<Transform> m_TransStarLightList = new List<Transform>(5);

        private Transform m_TransBigStar;
        private Text m_TexBigStarNum;

        //赛季时间信息
        private Text m_TexSeasonNum;
        private Text m_TextSeasonDate;


        private Text m_TexServerName;

        private Text m_TexRemainingTime;

        private Text m_TexRemaining;
        //累胜奖励


        ClickItemGroup<CumulativeVictoryItem> m_CumulativeGroup = new ClickItemGroup<CumulativeVictoryItem>() { AutoClone = false };
        //我的队伍


        List<MateItem> m_MateItems = new List<MateItem>();

        ClickItemGroup<MateItem> m_MateItemGroup = new ClickItemGroup<MateItem>() { AutoClone = false };
        private Transform m_TransMemberAdd;
        private Button m_BtnMemberAdd;

        private Button m_BtnMemerChange;

        private Text m_TexLimite401;
        private Text m_TexLimite601;


        private Animator m_AnLevelEffect;

        UIPvpLevelIcon m_UIPvplevelIcon = new UIPvpLevelIcon();
        UIPvpLevelIcon m_UIPvpOldlevelIcon = new UIPvpLevelIcon();

        private Transform m_TransLevelOld;
        private Transform m_TransLevelNew;

        private Transform m_TransStarUpEffect;
        private Transform m_TransStarDownEffect;

        private ParticleSystem m_PsStarUpEffect;
        private ParticleSystem m_PsStarDownEffect;

        UICircleLayoutGroupEx m_CircleLayout;

        public Transform m_TransOpenTips;
        public void Load(Transform root)
        {
            m_BtnClose = root.Find("Animator/View_Title02/Btn_Close").GetComponent<Button>();

            m_BtnRuleDetail = root.Find("Animator/Button_Detail").GetComponent<Button>();

            //排名信息
            m_TexRankNum = root.Find("Animator/Image_List/Text_List").GetComponent<Text>();
            m_ImLevelIcon = root.Find("Animator/Image_circle/Image_level").GetComponent<Image>();
            m_TexLevel = root.Find("Animator/Image_circle/Text_level").GetComponent<Text>();

            //   m_TransLevelIcon = root.Find("Animator/Image_circle/Image_level");


            m_TransStar = root.Find("Animator/Image_circle/Star");

            m_CircleLayout = m_TransStar.GetComponent<UICircleLayoutGroupEx>();

            int starCount = m_TransStar.childCount;
            for (int i = 0; i < starCount; i++)
            {
                var item = m_TransStar.Find("Image_Star" + i);
                if (item != null)
                {
                    m_TransStarList.Add(item);

                    var itemchild = item.Find("Image_Star");

                    if (itemchild != null)
                        m_TransStarLightList.Add(itemchild);
                }
                    
            }

            m_TransBigStar = root.Find("Animator/Image_circle/Star_Big");

            m_TexBigStarNum = m_TransBigStar.Find("Num/Text_num").GetComponent<Text>();

            m_TransLevelOld = root.Find("Animator/Image_circle/Image_level/Level_Old");
            m_TransLevelNew = root.Find("Animator/Image_circle/Image_level/Level_New");


            m_UIPvplevelIcon.Parent = m_TransLevelNew;
            m_UIPvpOldlevelIcon.Parent = m_TransLevelOld;

            //赛季时间信息
            m_TexSeasonNum = root.Find("Animator/Image_Label/Text_Week").GetComponent<Text>();
            m_TextSeasonDate = root.Find("Animator/Image_Label/Text_Date").GetComponent<Text>();
            m_TexRemainingTime = root.Find("Animator/Text_time").GetComponent<Text>();
            m_TexRemaining = root.Find("Animator/Text").GetComponent<Text>();

            //累胜奖励
            Transform victoryTrans = root.Find("Animator/Image_today/Grid");
            int victoryItemCount = victoryTrans.childCount;
            for (int i = 0; i < victoryItemCount; i++)
            {
                Transform item = victoryTrans.Find("Image_item" + i);
                if (item != null)
                {
                    m_CumulativeGroup.AddChild(item);
                }

            }

            //我的队伍

            Transform mateTrans = root.Find("Animator/Image_team/Grid_team");
            m_TransMemberAdd = mateTrans.Find("MemberAdd");
            m_BtnMemberAdd = m_TransMemberAdd.Find("Image_bg/Button_add").GetComponent<Button>();

            m_BtnMemerChange = root.Find("Animator/Image_team/Button").GetComponent<Button>();

            int matecount = mateTrans.childCount;
            for (int i = 0; i < matecount; i++)
            {
                Transform item = mateTrans.Find("Member" + i);
                m_MateItemGroup.AddChild(item);
            }


            m_BtnRankReward = root.Find("Animator/Button_Chest").GetComponent<Button>();
            m_BtnLevelUpReward = root.Find("Animator/Button_Reward").GetComponent<Button>();

            m_TransFxLevelUpReward = root.Find("Animator/Button_Reward/FxReward");

            m_BtnRank = root.Find("Animator/Btn_check").GetComponent<Button>();
            m_BtnMatch = root.Find("Animator/Btn_start").GetComponent<Button>();


            m_TexServerName = root.Find("Animator/ServerName/Text_List").GetComponent<Text>();

            m_TexLimite401 = root.Find("Animator/Image_team/Grid_type/item/Text_num").GetComponent<Text>();
            m_TexLimite601 = root.Find("Animator/Image_team/Grid_type/item (1)/Text_num").GetComponent<Text>();

            m_AnLevelEffect = root.Find("Animator/Image_circle/Image_level").GetComponent<Animator>();

            m_TransStarDownEffect = root.Find("Animator/Image_circle/Fx_ui_pvp_star02");
            m_TransStarUpEffect = root.Find("Animator/Image_circle/Fx_ui_pvp_star");

            m_PsStarDownEffect = root.Find("Animator/Image_circle/Fx_ui_pvp_star02/Particle System").GetComponent<ParticleSystem>();
            m_PsStarUpEffect = root.Find("Animator/Image_circle/Fx_ui_pvp_star/glow").GetComponent<ParticleSystem>();

            m_TransOpenTips = root.Find("Animator/Text_Promote");
        }


        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
            m_BtnRuleDetail.onClick.AddListener(listener.OnClickRuleDetail);
            m_BtnRankReward.onClick.AddListener(listener.OnClickRandkReward);
            m_BtnLevelUpReward.onClick.AddListener(listener.OnClickLevelUpReward);
            m_BtnRank.onClick.AddListener(listener.OnClickRank);
            m_BtnMatch.onClick.AddListener(listener.OnClickMatch);
            m_BtnMemberAdd.onClick.AddListener(listener.OnClickMemberAdd);
            m_BtnMemerChange.onClick.AddListener(listener.OnClickMemberAdd);

            int count = m_CumulativeGroup.Size;

            for (int i = 0; i < count; i++)
            {
                var item = m_CumulativeGroup.getAt(i);

                if (item != null)
                    item.clickItemEvent.AddListener(listener.OnClickCumulative);
            }

            int pcount = m_MateItemGroup.Size;

            for (int i = 0; i < pcount; i++)
            {
                var item = m_MateItemGroup.getAt(i) ;

                if (item != null)
                {
                    item.SetLisitener(listener);
                }
            }
        }

        public void OnDestory()
        {
            m_UIPvplevelIcon.Destory();
            m_UIPvpOldlevelIcon.Destory();

            DestoryStarEffectTimer();


        }
        #region 我的队伍
        public void SetPartnerSize(int size)
        {
            bool addShow = (size == 1);

            //if (m_TransMemberAdd.gameObject.activeSelf != addShow)
            //    m_TransMemberAdd.gameObject.SetActive(addShow);

            m_MateItemGroup.SetChildSize(size);
        }

        /// <summary>
        /// 伙伴头像
        /// </summary>
        /// <param name="index"></param>
        /// <param name="iconID"></param>
        public void SetParnerIcon(int index, uint iconID)
        {
            var item = m_MateItemGroup.getAt(index);

            if (item == null)
                return;

            item.SerPartnerIcon(iconID);
        }

        public void SetParnerRoleIcon(int index, ulong roleID, bool bNativeSize)
        {
            var item = m_MateItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetRoleIcon(roleID, bNativeSize);
        }

        /// <summary>
        /// 伙伴名字
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tex"></param>
        public void SetParnerName(int index, uint tex)
        {
            var item = m_MateItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetName(tex);
        }

        public void SetParnerName(int index, string tex)
        {
            var item = m_MateItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetName(tex);
        }

        public void SetParnerProfession(int index, uint icon)
        {
            var item = m_MateItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetProfessionIcon(icon);
        }

        public void SetParnerAddMode(int index, bool b)
        {
            var item = m_MateItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetAddMode(b);
        }
        public void SetLimiteOcci401(int count)
        {
            m_TexLimite401.text = count.ToString();
        }
        public void SetLimiteOcci601(int count)
        {
            m_TexLimite601.text = count.ToString();
        }


        #endregion


        #region 赛季信息

        public void SetLevelUpEffect(string oldLevel,string newLevel)
        {
            if (string.IsNullOrEmpty(oldLevel) == false)
                m_UIPvpOldlevelIcon.ShowLevelIcon(oldLevel);

            m_UIPvplevelIcon.ShowLevelIcon(newLevel);
            m_AnLevelEffect.enabled = true;
            m_AnLevelEffect.Play("Open");

        }

        /// <summary>
        /// 我的排名
        /// </summary>
        /// <param name="num"></param>
        public void SetMineRankNum(int num)
        {
            m_TexRankNum.text = num == 0 ? LanguageHelper.GetTextContent(10168) : num.ToString();
        }

        /// <summary>
        /// 我的排名等级icon
        /// </summary>
        /// <param name="id"></param>
        public void SetMineLevelIcon(string id)
        {
            m_AnLevelEffect.enabled = false;

            m_UIPvplevelIcon.ShowLevelIcon(id);
        }

        public void SetMineLevelTex(string tex)
        {
            m_TexLevel.text = tex;
        }
        /// <summary>
        /// 我的等级星星数量
        /// </summary>
        /// <param name="num"></param>
        public void SetMineLevleStar(int num)
        {
            int count = m_TransStarList.Count;

            for (int i = 0; i < count; i++)
            {
                bool bshow = (i < num);

                if (bshow != m_TransStarLightList[i].gameObject.activeSelf)
                    m_TransStarLightList[i].gameObject.SetActive(bshow);
            }
        }

        private Timer mLevelStarEffectTimer = null;

        private void DestoryStarEffectTimer()
        {
            if (mLevelStarEffectTimer == null)
                return;

            mLevelStarEffectTimer.Cancel();

            mLevelStarEffectTimer = null;
        }
        public void SetMineLevelStarEffect(int from,int to,int maxcount)
        {
            if (from == to)
                return;

            SetMineLevleStar(from);

            int index = to > from ? (to - 1) : (from - 1);

            Vector2 offsetPos = m_CircleLayout.GetChildPosition(index, maxcount);
            
            Vector3 toPos = offsetPos + m_CircleLayout.rectTransform.anchoredPosition;

            DestoryStarEffectTimer();


            if (from < to)
            {
               var rect = m_TransStarUpEffect as RectTransform;

                rect.anchoredPosition = toPos;

                m_TransStarUpEffect.gameObject.SetActive(true);
                m_PsStarUpEffect.Play(true);

                mLevelStarEffectTimer = Timer.Register(1.4f, () => {
                    SetMineLevleStar(to);
                    m_TransStarUpEffect.gameObject.SetActive(false);
                });
            }

            else
            {
                var rect = m_TransStarDownEffect as RectTransform;

                rect.anchoredPosition = toPos;

                m_TransStarDownEffect.gameObject.SetActive(true);
                m_PsStarDownEffect.Play(true);

                mLevelStarEffectTimer = Timer.Register(1.3f, () => {
                    SetMineLevleStar(to);
                    m_TransStarDownEffect.gameObject.SetActive(false);
                });
            }
        }
        public void SetMineLevleStarMax(int num)
        {
            int count = m_TransStarList.Count;

            for (int i = 0; i < count; i++)
            {
                bool bshow = (i < num);
                if (bshow != m_TransStarList[i].gameObject.activeSelf)
                    m_TransStarList[i].gameObject.SetActive(bshow);
            }
        }
        public void SetMineBigStarActiv(bool state)
        {
            if (m_TransBigStar.gameObject.activeSelf != state)
                m_TransBigStar.gameObject.SetActive(state);

            if (m_TransStar.gameObject.activeSelf == state)
                m_TransStar.gameObject.SetActive(!state);
        }


        public void SetBigStarNum(int num)
        {
            m_TexBigStarNum.text = num.ToString();
         }
        /// <summary>
        /// 第几赛季
        /// </summary>
        /// <param name="tex"></param>
        public void SetSeasonNum(string tex)
        {
            m_TexSeasonNum.text = tex;
        }

        /// <summary>
        /// 赛季时间，开始---结束
        /// </summary>
        /// <param name="tex"></param>
        public void SetSeasonDate(string tex)
        {
            m_TextSeasonDate.text = tex;
        }

        public void SetRemainingTime(string tex)
        {
            m_TexRemainingTime.text = tex;
        }

        public void SetRemainingTex(string tex)
        {
            m_TexRemaining.text = tex;
        }


        public void SetServerName(string tex)
        {
            m_TexServerName.text = tex;
        }
        #endregion


        #region 累计胜利奖励

        public void SetCumulativeVictory(int index, int state)
        {
            var item = m_CumulativeGroup.getAt(index);

            if (item == null)
                return;

            item.SetHadUsed(state == 2);

            item.ShowFx(state == 1);
        }

        public void SetCumulativeVictoryID(int index, int id)
        {
            var item = m_CumulativeGroup.getAt(index);

            if (item == null)
                return;

            item.Index = id;
        }

        public void SetCumulativeVictoryItemLostFocus(int index)
        {
            var item = m_CumulativeGroup.getAt(index);

            if (item == null)
                return;

            item.SetToggleState(false);
        }
        #endregion


        public void SetLevelUpRewardFxActive(bool active)
        {
            m_TransFxLevelUpReward.gameObject.SetActive(active);
        }
    }


    /// <summary>
    /// 监听
    /// </summary>
    public partial class UI_Pvp_Single_Layout
    {
        public interface IListener
        {
            /// <summary>
            /// 关闭
            /// </summary>
            void OnClickClose();

            /// <summary>
            /// 规则
            /// </summary>
            void OnClickRuleDetail();

            /// <summary>
            /// 排行奖励
            /// </summary>
            void OnClickRandkReward();

            /// <summary>
            /// 升级奖励
            /// </summary>
            void OnClickLevelUpReward();
            /// <summary>
            /// 查看排行
            /// </summary>
            void OnClickRank();
            /// <summary>
            /// 开始匹配
            /// </summary>
            void OnClickMatch();

            /// <summary>
            /// 设置伙伴
            /// </summary>
            void OnClickMemberAdd();

            /// <summary>
            /// 累计胜利奖励
            /// </summary>
            /// <param name="id"></param>
            void OnClickCumulative(int index);

        }
    }

    public partial class UI_Pvp_Single_Layout
    {
        /// <summary>
        /// 累胜奖励子条目
        /// </summary>
        class CumulativeVictoryItem : IntClickItem
        {
            private Transform m_Transform;

            private Image m_ImHadUsed;

            private Toggle m_Btn;

            private Transform m_TransFx;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Transform = root;

                m_ImHadUsed = root.Find("Image_Blank").GetComponent<Image>();

                m_TransFx = root.Find("Fx_ui_PropItem02");

                m_Btn = root.GetComponent<Toggle>();

                m_Btn.onValueChanged.AddListener(OnClick);

            }

            private void OnClick(bool value)
            {
                if (value)
                    clickItemEvent.Invoke(Index);
            }

            public void SetHadUsed(bool state)
            {
                if (m_ImHadUsed.gameObject.activeSelf != state)
                    m_ImHadUsed.gameObject.SetActive(state);
            }

            public void ShowFx(bool state)
            {
                if (m_TransFx.gameObject.activeSelf != state)
                    m_TransFx.gameObject.SetActive(state);
            }
            public void SetToggleState(bool b)
            {
                m_Btn.isOn = b;
            }
        }

        /// <summary>
        /// 伙伴子条目
        /// </summary>
        class MateItem : ClickItem
        {
            private Transform m_Transform;

            private Transform m_TransRoleIcon;
            private Image mImageRole;

            private Transform m_TransName;

            private Text m_TexName;

            private Text m_TexName0;

            private Image m_ImType;

            private Image m_ImPartnertip;

            private Button m_BtnAdd;
            private Transform m_TransAddTex;

            private Transform m_TransTips;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Transform = root;

                m_TransRoleIcon = root.Find("Image_bg/Image_role");
                if (m_TransRoleIcon != null)
                    mImageRole = m_TransRoleIcon.GetComponent<Image>();

                if (mImageRole == null)
                {
                    m_TransRoleIcon = root.Find("Image_bg/Image_Mask");
                    mImageRole = root.Find("Image_bg/Image_Mask/Image_role").GetComponent<Image>();
                }
                   


                m_TexName = root.Find("Image_bg/Text_name").GetComponent<Text>();

                m_TransName = root.Find("Image_bg/Image_partnertip");
                m_TexName0 = root.Find("Image_bg/Image_partnertip/Text").GetComponent<Text>();

                m_ImType = root.Find("Image_bg/Image_type").GetComponent<Image>();

                m_ImPartnertip = root.Find("Image_bg/Image_partnertip").GetComponent<Image>();


                m_TransTips = root.Find("Image_bg/Text_tip");

                m_BtnAdd = root.Find("Image_bg/Button_add").GetComponent<Button>();
            }

            public void SetLisitener(IListener listener)
            {
                m_BtnAdd.onClick.AddListener(listener.OnClickMemberAdd);
            }
            public void SetRoleIcon(ulong roleID, bool bNativeSize)
            {
                RoleIconHelper.SetRoleIcon(roleID, mImageRole, bNativeSize);
            }

            public void SerPartnerIcon(uint iconID)
            {
                ImageHelper.SetIcon(mImageRole, iconID);
            }
            public void SetName(uint nameID)
            {
                TextHelper.SetText(m_TexName0, nameID);
            }

            public void SetName(string tex)
            {
                m_TexName0.text = tex;
            }

            public void SetProfessionIcon(uint icon)
            {
                if (icon == 0)
                {
                    m_ImType.gameObject.SetActive(false);
                    return;
                }


                if (m_ImType.gameObject.activeSelf == false)
                    m_ImType.gameObject.SetActive(true);

                ImageHelper.SetIcon(m_ImType, icon);
            }

            public void SetAddMode(bool b)
            {

                m_TransRoleIcon.gameObject.SetActive(!b);
                m_ImType.gameObject.SetActive(!b);
                m_TransName.gameObject.SetActive(!b);


                m_BtnAdd.gameObject.SetActive(b);
                m_TransTips.gameObject.SetActive(b);
            }
        }
    }


    public static class RoleIconHelper
    {
        /// <summary>
        /// 带时装的人物半身头像
        /// </summary>
        /// <param name="roleId">场景中实例化后的roleid，否则取不到相关信息</param>
        /// <param name="image"></param>
        /// <param name="bNativeSize"></param>
        public static void SetRoleIcon(ulong roleId, Image image, bool bNativeSize = false)
        {
            uint fashionIconId = 0;
            uint heroID = 0;
            string roleName = "";
            if (roleId == Sys_Role.Instance.Role.RoleId)
            {
                roleName = Sys_Role.Instance.Role.Name.ToStringUtf8();
                fashionIconId = Sys_Fashion.Instance.GetClothId(GameCenter.mainHero?.heroBaseComponent?.fashData);
                heroID = Sys_Role.Instance.Role.HeroId;
            }
            else
            {
                //Hero hero = GameCenter.mainWorld.GetActor(Hero.Type, roleId) as Hero;
                Hero hero = GameCenter.GetSceneHero(roleId);
                if (hero != null)
                {
                    roleName = hero.heroBaseComponent.Name;
                    fashionIconId = Sys_Fashion.Instance.GetClothId(hero.heroBaseComponent?.fashData);
                    heroID = hero.heroBaseComponent.HeroID;
                }
            }

            SetRoleIcon(image, fashionIconId, heroID, bNativeSize);
        }

        /// <summary>
        /// 带时装的人物半身头像
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fashionId"></param>
        /// <param name="heroID"></param>
        /// <param name="bNativeSize"></param>
        public static void SetRoleIcon(Image image, uint fashionId, uint heroID, bool bNativeSize = false)
        {
            uint iconID = fashionId * 10000 + heroID;

            CSVFashionIcon.Data cSVFashionIconData = CSVFashionIcon.Instance.GetConfData(iconID);

            if (cSVFashionIconData != null)
            {
                ImageHelper.SetIcon(image, null, cSVFashionIconData.Icon_Path, bNativeSize);

                if (bNativeSize)
                {
                    image.transform.localScale = new Vector3(cSVFashionIconData.Arena_scale, cSVFashionIconData.Arena_scale, 1);
                    (image.transform as RectTransform).anchoredPosition = new Vector2(cSVFashionIconData.Arena_pos[0], cSVFashionIconData.Arena_pos[1]);
                }

            }
        }
    }


}
