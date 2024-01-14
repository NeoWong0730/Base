using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Lib.Core;

namespace Logic
{

    public partial class UI_RideLottoRewardGet : UIBase, UI_RideLottoRewardGet_Layout.IListener
    {
        UI_RideLottoRewardGet_Layout m_Layout = new UI_RideLottoRewardGet_Layout();

        private uint mExtraID = 0;
        public class Parmas
        {
            public CmdLuckyDrawDrawRes Info { get; set; }

            public int Type { get; set; }
        }


        private Parmas m_Parmas;

        private int m_ShowEffectIndex = 0;

        private CSVAwardQualityEffect.Data m_CurEffectData;

        private Timer m_CurTimer;

        private Timer m_CurPetShowTimer;

        private int m_ItemCount;


        private Dictionary<int, uint> m_DicShowPetEffect = new Dictionary<int, uint>();
        private Dictionary<int, ulong> m_DicPetUUid = new Dictionary<int, ulong>();
        protected override  void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);


            //mPetGet.Init(gameObject.transform.Find("Animator/UI_Pet_Get"));

            m_Layout.SetListener(this);

            var parmas = CSVParam.Instance.GetConfData(1053);

            if (parmas != null)
            {
                mExtraID = uint.Parse(parmas.str_value);
            }
        }

        protected override void OnOpen(object arg)
        {            
            m_Parmas = arg as Parmas;

            if (m_Parmas != null && m_Parmas.Info != null)
            {
                m_ItemCount = m_Parmas.Info.Itemid.Count;

            }
        }

        protected override void OnShow()
        {
            m_Layout.OnCloseLayout();
            
            if (m_ShowEffectIndex == 0)
                Timer.Register(0.1f, RefreshShow);

            // RefreshShow();
        }

        protected override void OnHide()
        {            
            m_Layout.ShowRewardEffect();

            if (m_CurPetShowTimer != null && m_CurPetShowTimer.isDone == false)
            {
                m_CurPetShowTimer.Cancel();
                m_CurPetShowTimer = null;
            }

            CloseSelf();
        }


        protected override void OnClose()
        {            
            m_ItemCount = 0;

            m_ShowEffectIndex = 0;

            m_Layout.OnCloseLayout();

            m_DicShowPetEffect.Clear();

            m_DicPetUUid.Clear();

            if (m_CurTimer != null)
                m_CurTimer.Cancel();
            //DebugUtil.LogError("Lotto Reward Get OnClose !!!");
        }
        private void Refresh()
        {
            m_Layout.SetMode(m_Parmas.Type);

            m_Layout.SetReward(m_Parmas.Type,m_Parmas.Info.Itemid, m_Parmas.Info.Itemcount,m_Parmas.Info.AwardId);

            m_Layout.SetRewardExtra(mExtraID, m_Parmas.Info.ExtraGiftNum);
        }

        private void RefreshShow()
        {
            m_Layout.ShowRewardEffect();

            ShowNextEffect();

        }
        private void ShowNextEffect()
        {
            if (m_ShowEffectIndex >= m_ItemCount)
            {
                return;
            }

            m_Layout.SetShowRewardActive(false);

            uint itemid = m_Parmas.Info.Itemid[m_ShowEffectIndex];

            var itemdata = CSVItem.Instance.GetConfData(itemid);

           // DebugUtil.LogError("show item index : " + m_ShowEffectIndex);

            if (itemdata.type_id == 1103 && m_DicShowPetEffect.ContainsKey(m_ShowEffectIndex) == false)
            {
                m_DicShowPetEffect.Add(m_ShowEffectIndex, itemid);

                if (m_DicPetUUid.Count < m_Parmas.Info.PetUID.Count)
                {
                    var id = m_Parmas.Info.PetUID[m_DicPetUUid.Count];
                    m_DicPetUUid.Add(m_ShowEffectIndex, id);
                }
               

                ShowPetEffect();
            }
            else
            {
                ShowItemEffect();

                
            }

           
            //DebugUtil.LogError("加载特效 ：" + (m_ShowEffectIndex - 1));
        }

        private void ShowItemEffect()
        {
            //mPetGet.Hide();

            uint itemid = m_Parmas.Info.AwardId[m_ShowEffectIndex];

            m_CurEffectData = ItemQualityEffectHelper.GetItemQualityEffectData(itemid);

            m_ShowEffectIndex += 1;

            if (m_CurEffectData == null)
            {
                m_Layout.SetShowRewardActive(true);
                return;
            }
            m_Layout.LoadParticle(m_CurEffectData.effects4_path + ".prefab");
        }

        private void ShowPetEffect()
        {
            uint itemid = m_Parmas.Info.Itemid[m_ShowEffectIndex];

            //mPetGet.SetPetID(itemid);

            //mPetGet.Show();

            if (m_DicPetUUid.ContainsKey(m_ShowEffectIndex) == false)
            {
                OnWaitNextFinally();
                return;
            }



           var result = Sys_Pet.Instance.GetPetByUId((uint)m_DicPetUUid[m_ShowEffectIndex]);

            if (result == null)
            {
                OnWaitNextFinally();
                return;
            }

            UIManager.OpenUI(EUIID.UI_Pet_GetMix, false, result);

            if (m_CurPetShowTimer != null && m_CurPetShowTimer.isDone == false)
                m_CurPetShowTimer.Cancel();

            m_CurPetShowTimer = Timer.Register(4f, ShowOverPet);

        }

        private void ShowOverPet()
        {
            UIManager.CloseUI(EUIID.UI_Pet_GetMix);
            // mPetGet.Hide();

            OnWaitNextFinally();
        }
        private void StartShowQualityEffect()
        {
           // m_CurTimer = Timer.Register(m_CurEffectData.effectsDuration / 1000f - 0.5f, OnShowEffectEnd);

           
        }
    }

    public partial class UI_RideLottoRewardGet : UIBase, UI_RideLottoRewardGet_Layout.IListener
    {
        private void PushTips(uint id, uint count,uint rewardLevel = 0)
        {
            var item = CSVItem.Instance.GetConfData(id);

            string Name = LanguageHelper.GetTextContent(item.name_id);
            string content = string.Format(LanguageHelper.GetTextContent(1000935), Name, count.ToString());

            Sys_Hint.Instance.PushContent_GetReward(content, item.id);

            if (rewardLevel >= 4)
                return;

            string chatStr = string.Format("[@{0}]{1}", Sys_Role.Instance.sRoleName,content);

            Sys_Chat.Instance.PushMessage(ChatType.Person, null, chatStr);

        }

       
        private void GetItemsTips()
        {
            if (m_Parmas != null && m_Parmas.Info != null)
            {
                int count = m_Parmas.Info.Itemid.Count;
                for (int i = 0; i < count; i++)
                {
                    var data = CSVAward.Instance.GetConfData(m_Parmas.Info.AwardId[i]);
                    PushTips(m_Parmas.Info.Itemid[i], m_Parmas.Info.Itemcount[i],data.quality);
                }

                if (m_Parmas.Info.ExtraGiftNum > 0)
                    PushTips(mExtraID, m_Parmas.Info.ExtraGiftNum);
            }
        }
        public void OnClickClose()
        {
            GetItemsTips();

            CloseSelf();

            if (m_CurTimer != null)
            {
                m_CurTimer.Cancel();
            }
            //Debug.Log("onclose");
           
            UIManager.OpenUI(EUIID.UI_RideLotto);

            
        }

        public void OnParticleLoadFinally(string name, ParticleSystem particleSystem)
        {
           // StartShowQualityEffect();

            m_Layout.PlayPraticleSystem(particleSystem);

            m_Layout.SetShowRewardActive(true);

            m_Layout.SetShowReward(m_Parmas.Info.Itemid[m_ShowEffectIndex - 1], m_Parmas.Info.Itemcount[m_ShowEffectIndex - 1]);

            var id = m_Parmas.Info.AwardId[m_ShowEffectIndex - 1];

            var data = CSVAward.Instance.GetConfData(id);

            m_Layout.SetShowRewardBgIcon(data.backgroundType);

            m_Layout.SetShowIconEffect(data.backgroundlightType);
            //DebugUtil.LogError("加载特效 完成：" + (m_ShowEffectIndex - 1));
        }


        public void OnClickSkip()
        {
            m_Layout.SetSkip();

            m_ShowEffectIndex = m_ItemCount;

            OnWaitNextFinally();
        }
        public void OnShowRewardItemAnimEnd(string name)
        {

            //Debug.Log("OnShowRewardItemAnimEnd");

            if (m_CurTimer != null)
            {
                m_CurTimer.Cancel();
            }

            m_CurTimer = Timer.Register(2, OnWaitNextFinally);

            //DebugUtil.LogError("UI动画播放完成 ：" + (m_ShowEffectIndex - 1));
        }

        private void OnWaitNextFinally()
        {
            //DebugUtil.LogError("开始下一个 ：" + (m_ShowEffectIndex - 1));

            m_Layout.SetShowRewardActive(false);

            if (m_ShowEffectIndex >= (m_ItemCount))
            {
                Refresh();
                m_Layout.SetShowRewardActive(false);
                return;
            }
            ShowNextEffect();
        }

    }
}
