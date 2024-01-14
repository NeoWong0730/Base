using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Logic
{
    /// <summary>
    ///pvp 新赛季
    /// </summary>
    public partial class UI_Pvp_NewSeason : UIBase, UI_Pvp_NewSeason_Layout.IListener
    {
        UI_Pvp_NewSeason_Layout m_Layout = new UI_Pvp_NewSeason_Layout();


        private RaycastHit[] m_RaycastHit = new RaycastHit[5];

        private bool m_bShowNowSeasonInfo = false;

        private bool m_bHadOpenBox = false;

        private float RewardOpenTime = 0;
        private float RankOpenTime = 0;
        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Pvp.Instance.eventEmitter.Handle(Sys_Pvp.EEvents.ShowSeasonAward, OnShowAward, toRegister);

        }

        private void OnTouchDown(Vector2 pos)
        {
            if (CameraManager.mUICamera != null)
            {
                Ray ray = CameraManager.mUICamera.ScreenPointToRay(pos);

                int layerMask = (int)(ELayerMask.UI);

                int count = Physics.RaycastNonAlloc(ray, m_RaycastHit, 500f, layerMask);

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        RaycastHit hit = m_RaycastHit[i];

                        if (LayerMaskUtil.ContainLayerInt(ELayerMask.UI, hit.collider.gameObject.layer))
                        {
                            if (hit.collider.gameObject.name == "96100")
                            {
                                OnClickRewardBox();
                                break;
                            }
                        }
                    }
                }
            }
        }
        protected override void OnShow()
        {            
            m_Layout.SetStarActive(true);
            m_Layout.PlayNormal();

        }
        protected override void OnHide()
        {            
            m_bShowNowSeasonInfo = false;

            m_bHadOpenBox = false;
        }

        protected override void OnDestroy()
        {
            m_Layout.OnDestory();
        }

        private void RefreshReward()
        {
            if (Sys_Pvp.Instance.SewasonAwardInfo == null)
                return;

            m_Layout.SetReward(Sys_Pvp.Instance.SewasonAwardInfo.ItemIds, Sys_Pvp.Instance.SewasonAwardInfo.ItemNums);
        }


        private void RefreshRankWithOld()
        {
            if (Sys_Pvp.Instance.SewasonAwardInfo == null)
                return;

            var info = Sys_Pvp.Instance.SewasonAwardInfo;

            CSVArenaSegmentInformation.Data item = CSVArenaSegmentInformation.Instance.GetConfData((uint)info.OldDanLv);

            m_Layout.SetRankDanLv(LanguageHelper.GetTextContent(item.RankDisplay));

            m_Layout.SetRankIcon(item.RankIcon);

            m_Layout.SetRankTitle(LanguageHelper.GetTextContent(10165, info.OldSeasonId.ToString()));

            m_Layout.SetRankStar((int)info.OldStar);

            m_Layout.SetRankNewSeasonTips(false, string.Empty);
        }

        private void RefreshRankWithNew()
        {
            m_bShowNowSeasonInfo = true;

            if (Sys_Pvp.Instance.SewasonAwardInfo == null)
                return;

            var info = Sys_Pvp.Instance.SewasonAwardInfo;

            CSVArenaSegmentInformation.Data item = CSVArenaSegmentInformation.Instance.GetConfData((uint)info.NewDanLv);

            string rankName = LanguageHelper.GetTextContent(item.RankDisplay);
            m_Layout.SetRankDanLv(rankName);

            m_Layout.SetRankIcon(item.RankIcon);

            m_Layout.SetRankTitle(LanguageHelper.GetTextContent(10164));

            m_Layout.SetRankStar((int)info.NewStar);

            m_Layout.SetRankNewSeasonTips(true, LanguageHelper.GetTextContent(10198, rankName));
        }


    }


    public partial class UI_Pvp_NewSeason : UIBase, UI_Pvp_SingleLoading_Layout.IListener
    {
        public void OnClickClose()
        {

        }

        /// <summary>
        /// 开宝箱动画结束
        /// </summary>
        /// <param name="playable"></param>
        public void OnOpenEnd(PlayableDirector playable)
        {
            //Debug.LogError("OnOpenEnd");

            m_Layout.PlayReward();

            Sys_Pvp.Instance.Apply_GetBoxAward();

            RewardOpenTime = Time.time;
        }

        /// <summary>
        /// 宝箱奖励动画结束，然后打开奖励界面
        /// </summary>
        /// <param name="playable"></param>
        public void OnRewardEnd(PlayableDirector playable)
        {
           
        }

        
        public void OnStartStateTouch(Vector3 pos)
        {
            if (m_bHadOpenBox)
                return;

            OnTouchDown(pos);
        }

        public void OnStartStateLongTouch(Vector3 pos)
        {

        }

        public void OnClickRewardClose()
        {
            if (RewardOpenTime == 0 || Time.time - RewardOpenTime < 2)
            {
              //  Debug.LogError("OnClickRewardClose return " + (Time.time - RewardOpenTime).ToString());
                return;
            }
                

            //Debug.LogError("OnClickRewardClose");

            m_Layout.SetRewardActive(false);

            RefreshRankWithOld();

            RewardOpenTime = 0;

            RankOpenTime = Time.time;

            m_Layout.SetRankActive(true);
        }

        public void OnClickRankClose()
        {
            if (Time.time - RankOpenTime < 2)
            {
               // Debug.LogError("OnClickRankClose return " + (Time.time - RankOpenTime).ToString());
                return;
            }


            RankOpenTime = 0;

           // Debug.LogError("OnClickRankClose");

            if (m_bShowNowSeasonInfo == false)
            {
                m_Layout.PlayRankAnimator("Old");

                RankOpenTime = Time.time;
                return;
            }

            

            m_Layout.SetRankActive(false);

            CloseSelf();
        }
        /// <summary>
        /// 点击宝箱
        /// </summary>
        private void OnClickRewardBox()
        {
           // Debug.LogError("OnClickRewardBox");

            m_bHadOpenBox = true;

            m_Layout.CloseNormal();

            m_Layout.PlayOpen();
        }

        public void RankAnimationEnd( string name)
        {
            if (name.Equals("Old"))
            {
                m_Layout.PlayRankAnimator("New");
                RefreshRankWithNew();
            }
        }

       

        private void OnShowAward()
        {
            RefreshReward();

            m_Layout.SetStarActive(false);

            m_Layout.SetRewardActive(true);

            var info = Sys_Pvp.Instance.SewasonAwardInfo;

            m_Layout.SetRewardTitle(LanguageHelper.GetTextContent(10126, info.OldSeasonId.ToString()));

            RewardOpenTime = Time.time;
        }

    }
}
