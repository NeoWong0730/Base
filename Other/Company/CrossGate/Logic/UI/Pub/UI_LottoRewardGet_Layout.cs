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
    public partial class UI_LottoRewardGet_Layout
    {
       
        private Button m_BtnClose;

        private ClickItemGroup<FiveRectRewardItem> m_FiveRewardGroup = new ClickItemGroup<FiveRectRewardItem>() { AutoClone = false};
        private ClickItemGroup<RectRewardItem> m_OneRewardGroup = new ClickItemGroup<RectRewardItem>() { AutoClone = false};

        private FiveRectRewardItem m_ShowRewradItem = new FiveRectRewardItem();

        private Transform m_TransResult;
        private Transform m_TransFive;
        private Transform m_TransOne;

        private Transform m_TransShow;

        private Transform m_TransShowItem;
        private ParticleGroupLoader m_ParticleGroup = new ParticleGroupLoader();

        private Animator m_AniShowItem;
        private AnimationEndTrigger m_AniShowItemEndTrigger;

        private Animator m_AniResult;
        private IListener m_Listener;

        private  AdSpriteLoader m_SpirteLoader = new AdSpriteLoader();

        private Button m_BtnSkip;


        private Transform m_TransExtra;

        RewardExtra m_rewardExtra = new RewardExtra();
        public void Load(Transform root)
        {
            m_BtnClose = root.Find("Animator/result/GameObject").GetComponent<Button>();

            m_TransResult = root.Find("Animator/result");

            m_AniResult = m_TransResult.GetComponent<Animator>();

            m_TransOne = root.Find("Animator/result/View_Once");
            m_TransFive = root.Find("Animator/result/View_Five");

            var onceTrans = m_TransOne.Find("item");
            m_OneRewardGroup.AddChild(onceTrans);

            int count = m_TransFive.childCount;
            for (int i = 0; i < count; i++)
            {
                string strname = "Item0" + (i + 1);

                var item = m_TransFive.Find(strname);

                if (item != null)
                    m_FiveRewardGroup.AddChild(item);
            }


            m_TransShow = root.Find("Animator/View_Show");


            m_TransShowItem = m_TransShow.Find("item");

            m_AniShowItem = m_TransShow.GetComponent<Animator>();
            m_AniShowItemEndTrigger = m_TransShow.GetComponent<AnimationEndTrigger>();

            m_ShowRewradItem.Load(m_TransShowItem);

            m_BtnSkip = m_TransShow.Find("Btn_Skip").GetComponent<Button>();

            m_TransExtra = root.Find("Animator/result/View_Extra");
            m_rewardExtra.Load(m_TransExtra);
        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);

            m_ParticleGroup.ActionFinal = listener.OnParticleLoadFinally;

            m_AniShowItemEndTrigger.onAnimationEnd += listener.OnShowRewardItemAnimEnd;

            m_BtnSkip.onClick.AddListener(listener.OnClickSkip);

            m_Listener = listener;
        }

        public void SetReward(int mode,IList<uint>ids, IList<uint> counts,IList<uint> rewardIds)
        {

            if (mode == 0)
                SetOneReward(ids,counts, rewardIds);
            else if (mode == 1)
                SetFiveReward(ids,counts, rewardIds);
        }

        private void SetOneReward(IList<uint> ids, IList<uint> counts, IList<uint> rewardIds)
        {
            int count = ids.Count;

            for (int i = 0; i < count; i++)
            {
                var item = m_OneRewardGroup.getAt(i);

                if (item != null)
                {
                    item.SetReward(ids[i], counts[i]);

                    var data = CSVAward.Instance.GetConfData(rewardIds[i]);

                    item.SetBgIcon(data.backgroundType, m_SpirteLoader);

                    var qualitydata = ItemQualityEffectHelper.GetItemQualityEffectData(rewardIds[i]);

                    if (string.IsNullOrEmpty(qualitydata.effects5_path) == false)
                        item.SetBgIconEffect(qualitydata.effects5_path + ".prefab");

                    item.SetEffectImage(data.backgroundlightType);
                }
                    
            }
        }

        private void SetFiveReward(IList<uint> ids, IList<uint> counts, IList<uint> rewardIds)
        {
            int count = ids.Count;

            for (int i = 0; i < count; i++)
            {
                var item = m_FiveRewardGroup.getAt(i);

                if (item != null)
                {
                    item.SetReward(ids[i], counts[i]);

                    var data = CSVAward.Instance.GetConfData(rewardIds[i]);

                    item.SetBgIcon(data.backgroundType, m_SpirteLoader);

                    var qualitydata = ItemQualityEffectHelper.GetItemQualityEffectData(rewardIds[i]);

                    if (string.IsNullOrEmpty(qualitydata.effects5_path) == false)
                        item.SetBgIconEffect(qualitydata.effects5_path + ".prefab");

                    item.SetEffectImage(data.backgroundlightType);
                }
                    
            }
        }
        /// <summary>
        /// 单次抽奖结果，五连抽奖结果
        /// </summary>
        /// <param name="mode"> 0 单次 1 五连抽</param>
        public void SetMode(int mode)
        {
            if (m_TransShow.gameObject.activeSelf)
                m_TransShow.gameObject.SetActive(false);


            if (m_TransResult.gameObject.activeSelf == false)
            {
                m_AniResult.enabled = true;
                m_AniResult.Play("Open");
                m_TransResult.gameObject.SetActive(true);
            }
               

            bool bone = (mode == 0);
            if (bone != m_TransOne.gameObject.activeSelf)
                m_TransOne.gameObject.SetActive(bone);

            bool bfive = (mode == 1);
            if (bfive != m_TransFive.gameObject.activeSelf)
                m_TransFive.gameObject.SetActive(bfive);
        }

        public void ShowRewardEffect()
        {
            if (m_TransShow.gameObject.activeSelf == false)
                m_TransShow.gameObject.SetActive(true);

            if (m_TransResult.gameObject.activeSelf)
                m_TransResult.gameObject.SetActive(false);
        }

        public void SetShowRewardActive(bool b)
        {
            if (b)
            {
               // Debug.Log("SetShowRewardActive");

                m_AniShowItem.Play("Open");
            }
              

            m_TransShowItem.gameObject.SetActive(b);

            m_AniShowItem.enabled = b;
        }

        public void SetSkip()
        {
           m_AniShowItem.Play("empty");
        }
        public void LoadParticle(string name)
        {
           GameObject particle = m_ParticleGroup.GetGameObject(name);

            if (particle != null)
            {
                m_Listener.OnParticleLoadFinally(name, particle.GetComponentInChildren<ParticleSystem>());
                return;
            }
         
             m_ParticleGroup.Load(name,m_TransShow);
        }

        public void PlayPraticleSystem(ParticleSystem particle)
        {
            particle.Play(true);
        }

        public void SetShowReward(uint id, uint count)
        {
            m_ShowRewradItem.SetReward(id, count);
        }

        public void SetShowRewardBgIcon(string path)
        {
            m_ShowRewradItem.SetBgIcon(path,m_SpirteLoader);
        }

        public void SetShowIconEffect(string path)
        {
            m_ShowRewradItem.SetEffectImage(path);
        }

        public void OnCloseLayout()
        {
            m_ParticleGroup.OnDestory();

            m_SpirteLoader.OnDestory();

            int oneCount = m_OneRewardGroup.Size;
            for (int i = 0; i < oneCount; i++)
            {
                var item = m_OneRewardGroup.getAt(i);
                if (item != null)
                {
                    item.OnDestory();
                }
            }

            int fiveCount = m_FiveRewardGroup.Size;
            for (int i = 0; i < fiveCount; i++)
            {
                var item = m_FiveRewardGroup.getAt(i);
                if (item != null)
                {
                    item.OnDestory();
                }
            }

        }

        public void SetRewardExtra(uint id, uint count)
        {
            if ((count == 0 || id == 0) && m_TransExtra.gameObject.activeSelf)
            {
                m_TransExtra.gameObject.SetActive(false);
                return;
            }
                

            if(m_TransExtra.gameObject.activeSelf == false)
                m_TransExtra.gameObject.SetActive(true);

            m_rewardExtra.SetReward(id, count);
        }
    }


    public partial class UI_LottoRewardGet_Layout
    {
        public interface IListener
        {
            void OnClickClose();

            void OnParticleLoadFinally(string name, ParticleSystem particleSystem);

            void OnShowRewardItemAnimEnd(string name);

            void OnClickSkip();
        }
    }

    public partial class UI_LottoRewardGet_Layout
    {
        private class RectRewardItem : IntClickItem
        {
            protected PropItem m_Item;
            protected PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);

            protected RawImage m_ImgIcon;

            protected RawImage m_ImgEffect;
            public UI_LottoRewardGet_Layout ParentLayout { get; set; }

            private ParticleGroupLoader particleGroupLoader = new ParticleGroupLoader();
            public override void Load(Transform root)
            {
                base.Load(root);

                var propitem = root.Find("PropItem");

                m_Item = new PropItem();

                m_Item.BindGameObject(propitem.gameObject);

                m_ImgIcon = root.Find("Image_Icon").GetComponent<RawImage>();

                m_ImgEffect = root.Find("Image_Effect").GetComponent<RawImage>();
            }

            public void SetReward(uint id, uint count)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Lotto_RewardGet, m_ItemData));
            }

            public void SetBgIcon(string name, AdSpriteLoader loader)
            {
                loader.SetImage(name, m_ImgIcon);

           
            }

            public void SetEffectImage(string name)
            {
                ImageHelper.SetTexture(m_ImgEffect, name);
            }
            public void SetBgIconEffect(string name)
            {
                particleGroupLoader.Load(name, mTransform);
            }

            public void OnDestory()
            {
                particleGroupLoader.OnDestory();
            }
        }

        private class FiveRectRewardItem : RectRewardItem
        {
            public override void Load(Transform root)
            {
                mTransform = root;

                var propitem = root.Find("PropItem");

                m_Item = new PropItem();

                m_Item.BindGameObject(propitem.gameObject);

                m_ImgIcon = root.Find("Image_Icon").GetComponent<RawImage>();

                m_ImgEffect = root.Find("Image_Effect").GetComponent<RawImage>();
            }
        }
    }

    public partial class UI_LottoRewardGet_Layout
    {


        class RewardExtra:ClickItem
        {
            protected PropItem m_Item;
            protected PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);

            public override void Load(Transform root)
            {
                base.Load(root);

                var propitem = root.Find("PropItem");

                m_Item = new PropItem();

                m_Item.BindGameObject(propitem.gameObject);
            }

            public void SetReward(uint id, uint count)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Lotto_RewardGet, m_ItemData));


                var data = CSVItem.Instance.GetConfData(id);
                if (data != null)
                    m_Item.txtName.text = LanguageHelper.GetTextContent(data.name_id);
            }
        }

  
    }
   


}
