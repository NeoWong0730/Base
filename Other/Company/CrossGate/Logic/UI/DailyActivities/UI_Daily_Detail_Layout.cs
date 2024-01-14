using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;

using UnityEngine.UI;
using Table;
using Framework;

namespace Logic
{
    public partial class UI_Daily_Detail_Layout
    {
        class RewardItem : ClickItem
        {
            PropItem m_Item;

            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);

            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }

            public void SetReward(ItemIdCount item)
            {
                m_ItemData.id = item.id;
                m_ItemData.count = item.count;
                m_ItemData.SetQuality(0);

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_DailyActivites_Detail, m_ItemData));
            }

            public override ClickItem Clone()
            {
                return Clone<RewardItem>(this);
            }
        }


    }
    public partial class UI_Daily_Detail_Layout
    {
        // private Text mTexName;

        //   private Text mTxtTimes;
        // private Text mTxtActivity;

        private Text mTxtActiviTime;
        private Text mTxtTaskType;
        private Text mTxtLevelLimite;

        private Text mTxtDetailInfo;
        public Button btnCamp;
        public Button mBtnCampShop;

        public Transform mTransCampRed;

        private ClickItemGroup<RewardItem> mGroup;


        private Button mBtnjoin;

        private IListener m_listener;

        private Button mClose;
        public Button btnTrialSkill;
        public GameObject trialRedPoint;

        public Transform bossTowerTrans;
        public Button btnBossTowerRank;
        public Transform qualifierTrans;
        public Transform qualifierPass, qualifierFailed;
        public Transform rnakTrans;
        public Text bossTowerRankNum;

        // private Button mBtnJoin;
        private UI_Daily_Common.DailyItem mDailyItem = new UI_Daily_Common.DailyItem();

        private Color mLevelTexOrginColor = Color.black;
        public void Load(Transform root)
        {
            Transform item = root.Find("Animator/View_Content/Scroll_View/Viewport/Item");
            mGroup = new ClickItemGroup<RewardItem>(item);

            // mTxtTimes = root.Find("Animator/View_Content/Text_Times/Text").GetComponent<Text>();

            //mTxtActivity = root.Find("Animator/View_Content/Text_Activity/Text").GetComponent<Text>();

            mTxtActiviTime = root.Find("Animator/View_Content/Text_Time/Text").GetComponent<Text>();

            mTxtTaskType = root.Find("Animator/View_Content/Text_Type/Text").GetComponent<Text>();

            mTxtLevelLimite = root.Find("Animator/View_Content/Text_Level/Text").GetComponent<Text>();
            mLevelTexOrginColor = mTxtLevelLimite.color;

            btnCamp = root.Find("Animator/View_Content/Button_Camp").GetComponent<Button>();
            mTransCampRed = root.Find("Animator/View_Content/Button_Camp/Image_Red");

            mBtnCampShop = root.Find("Animator/View_Content/Button_Shop").GetComponent<Button>();

            btnTrialSkill = root.Find("Animator/View_Content/Button_Skill").GetComponent<Button>();
            trialRedPoint = root.Find("Animator/View_Content/Button_Skill/Image_Red").gameObject;

            mTxtDetailInfo = root.Find("Animator/View_Content/Text_Tips").GetComponent<Text>();

            // mBtnjoin = root.Find("Animator/View_Content/Btn_01").GetComponent<Button>();

            mClose = root.Find("Black").GetComponent<Button>();

            //mTexName = root.Find("Animator/View_Content/Text_Title").GetComponent<Text>();

            Transform itemdaily = root.Find("Animator/ShopItem");

            mDailyItem.Load(itemdaily);

            mBtnjoin = root.Find("Animator/GameObject/Btn_01").GetComponent<Button>();

            bossTowerTrans = root.Find("Animator/View_Content/View_BossGame");
            btnBossTowerRank = bossTowerTrans.Find("Button_Skill").GetComponent<Button>();
            qualifierTrans = bossTowerTrans.Find("Qualification");
            qualifierPass = qualifierTrans.Find("Pass");
            qualifierFailed = qualifierTrans.Find("Failed");
            rnakTrans = bossTowerTrans.Find("Rank");
            bossTowerRankNum = rnakTrans.Find("Text").GetComponent<Text>();
        }

        public void SetListener(IListener listener)
        {
            m_listener = listener;

            mBtnjoin.onClick.AddListener(m_listener.OnClickJoin);
            btnCamp.onClick.AddListener(m_listener.OnClickCamp);
            btnTrialSkill.onClick.AddListener(m_listener.OnClickTrial);

            mClose.onClick.AddListener(m_listener.OnClickClose);
            mBtnCampShop.onClick.AddListener(m_listener.OnClickCampShop);

            btnBossTowerRank.onClick.AddListener(m_listener.OnClickBossTowerRank);
        }
        public interface IListener
        {
            void OnClickJoin();

            void OnClickClose();
            void OnClickCamp();
            void OnClickTrial();

            void OnClickCampShop();

            void OnClickBossTowerRank();
        }

        public void SetJoinBtnActive(bool active)
        {
            mBtnjoin.gameObject.SetActive(active);
        }

    }

    public partial class UI_Daily_Detail_Layout
    {

        public void SetName(uint langue)
        {
            mDailyItem.SetName(LanguageHelper.GetTextContent(langue));
        }

        public void SetDesc(uint langue)
        {
            mDailyItem.SetDesc(LanguageHelper.GetTextContent(langue));
        }
        /// <summary>
        /// 设置次数
        /// </summary>
        /// <param name="cur"></param>
        /// <param name="total"></param>
        public void SetTimes(int cur, int total)
        {
            if (cur > total)
                cur = total;

            string str = total == 0 ? LanguageHelper.GetTextContent(2010255) : (cur + "/" + total);

            mDailyItem.SetTimes(str);
        }

        /// <summary>
        /// 设置活跃度
        /// </summary>
        /// <param name="cur"></param>
        /// <param name="total"></param>
        public void SetActivity(int cur, int total)
        {
            uint styleid = cur >= total ? 74u : 128u;

            var styledata = CSVWordStyle.Instance.GetConfData(styleid);

            string value = total <= 0 ? string.Empty : (cur.ToString() + "/" + total.ToString());

            mDailyItem.SetActivity((uint)cur,(uint)total, styledata);
        }


        public void SetRightMark(UI_Daily_Common.DailyItem.EState eState,uint langueid = 0)
        {
            mDailyItem.SetState(eState, langueid);
        }

        /// <summary>
        /// 设置限制文字
        /// </summary>
        /// <param name="index"></param>
        /// <param name="active"></param>
        /// <param name="model"> 1 等级， 2 时间</param>
        /// <param name="value"></param>
        public void SetRightLimit( int model, string value, CSVWordStyle.Data textStyle)
        {
            //mDailyItem.SetLimitActive(active);

            if (model == 1)
                mDailyItem.SetLimitLevel(value, textStyle);

            if (model == 2)
                mDailyItem.SetLimitTime(value, textStyle);
        }

        /// <summary>
        /// 设置活动时间
        /// </summary>
        /// <param name="langue"></param>
        public void SetTime(uint langue)
        {
            mTxtActiviTime.text = langue == 0 ? string.Empty:LanguageHelper.GetTextContent(langue);
        }

        /// <summary>
        /// 设置人物形式
        /// </summary>
        /// <param name="langue"></param>
        public void SetTaskType(uint langue)
        {
            mTxtTaskType.text = LanguageHelper.GetTextContent(langue);
        }


        /// <summary>
        /// 设置等级限制
        /// </summary>
        /// <param name="level"></param>
        public void SetLevelLimit(string str)
        {
            mTxtLevelLimite.text = str;
            
        }


        /// <summary>
        /// 设置玩法描述
        /// </summary>
        /// <param name="langue"></param>
        public void SetDetailInfo(uint langue)
        {
            TextHelper.SetText(mTxtDetailInfo, LanguageHelper.GetTextContent(langue));
        }

        /// <summary>
        /// 设置玩法描述
        /// </summary>
        /// <param name="langue"></param>
        public void SetDetailInfo(string langue)
        {
            TextHelper.SetText(mTxtDetailInfo, langue);
        }


        /// <summary>
        /// 设置奖励
        /// </summary>
        public void SetReward(List<ItemIdCount> dropItems)
        {
            int count = dropItems.Count;

            mGroup.SetChildSize(count);

            for (int i = 0; i < count; i++)
            {
                var item = mGroup.getAt(i);

                item.SetReward(dropItems[i]);
            }
        }


        public void SetIcon(uint iconID)
        {
            mDailyItem.SetIcon(iconID);
        }

        public void SetDetailCoutType( uint iconid)
        {


            mDailyItem.SetPlayerType(iconid);
        }

        public void SetOpState( UI_Daily_Common.DailyItem.EOpState eOpState)
        {
            mDailyItem.SetOpState(eOpState);
        }

        public void SetDailyIsOpen(bool open)
        {
            mTxtLevelLimite.color = open ? mLevelTexOrginColor : Color.red;
        }

        public void SetAmount(string value)
        {
            mDailyItem.SetAmount(value);
        }
       
    }
}
