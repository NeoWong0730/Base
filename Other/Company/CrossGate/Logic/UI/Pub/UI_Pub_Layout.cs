using System;
using System.Collections.Generic;
using Framework;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Pub_Layout
    {
        public class RawardItem : IntClickItem
        {
            PropItem m_Item;

            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);

            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }

            public override ClickItem Clone()
            {
                return Clone<RawardItem>(this);
            }

            public void SetItem(ItemIdCount item)
            {
                m_ItemData.id = item.id;
                m_ItemData.count = item.count;

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Pub, m_ItemData));
            }
        }
    }
    public partial class UI_Pub_Layout
    {
        public interface IListener
        {
            void OnClickRewardSrue();
            void OnClickRewardClose();

            void OnClickSkipCardAnim();
            void OnCardAnimEnd();

            //void OnPageChange(int index,Transform transform);

            //void OnGridCell(ScrollGridCell cell);

            void OnCellChange(InfinityGridCell cell, int index);
        }
    }
    public partial class UI_Pub_Layout
    {
      

        List<RawardItem> mRawardItems = new List<RawardItem>();

        InfinityGridLayoutGroup mInfinityGridLayoutGroup;

        Transform mRewardTrans;
        UIAnimatorControl mAnimReward;

        Transform mAnimatorTrans;
        RawImage mRIAnimator;

     //   private PlayableDirector mUIMapTimeline;
        private Button m_BtnSkipMapTimeline;

        private Button mBtnRewardSure;
        private Button mBtnRewardClose;

        private IListener mListener;

        private PubShowScene mPubShowScene = null;
        private GameObject mPubShowSceneOrgine = null;

        private InfinityGrid mScrollGridVertical;
        public void Load(Transform root)
        {
            mRewardTrans = root.Find("Animator/Common");

            mAnimReward = mRewardTrans.GetComponent<UIAnimatorControl>();


            mScrollGridVertical = root.Find("Animator/Common/Image_bg03/Rect").GetComponent<InfinityGrid>();

            Transform rawardParent = root.Find("Animator/Common/Image_bg03/Rect/Rectlist");

            //int childCount = rawardParent.childCount;
            //for(int i = 0; i < childCount; i++)
            //{
            //    Transform item = rawardParent.Find(string.Format("PropItem ({0})", i + 1));

            //    if(item != null)
            //    {
            //        RawardItem rawardItem = new RawardItem();
            //        rawardItem.Load(item);
            //        mRawardItems.Add(rawardItem);
            //    }
            //}

            mBtnRewardSure = root.Find("Animator/Common/Btn_01").GetComponent<Button>();
            mBtnRewardClose = root.Find("Animator/Common/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();

           // mInfinityGridLayoutGroup = rawardParent.GetComponent<InfinityGridLayoutGroup>();

            mAnimatorTrans = root.Find("Animator/RawImage");
            mRIAnimator = mAnimatorTrans.GetComponent<RawImage>();

            var assetDep =mAnimatorTrans.GetComponent<AssetDependencies>();
            if (assetDep != null && assetDep.mCustomDependencies.Count > 0)
                mPubShowSceneOrgine = assetDep.mCustomDependencies[0] as GameObject;

            m_BtnSkipMapTimeline = root.Find("Animator/SkipButton").GetComponent<Button>();

            mAnimatorTrans.gameObject.SetActive(false);
        }


        public void SetListener(IListener listener)
        {
            mListener = listener;

            mBtnRewardSure.onClick.AddListener(mListener.OnClickRewardSrue);
            mBtnRewardClose.onClick.AddListener(mListener.OnClickRewardClose);

            m_BtnSkipMapTimeline.onClick.AddListener(mListener.OnClickSkipCardAnim);
            //  mUIMapTimeline.stopped += OnCardAnimEnd;

            // mInfinityGridLayoutGroup.updateChildrenCallback += OnPageChange;

            // mScrollGridVertical

            mScrollGridVertical.onCreateCell = OnItemCellCreate;
            mScrollGridVertical.onCellChange = listener.OnCellChange;
        }

        private void OnItemCellCreate(InfinityGridCell cell)
        {
            RawardItem rawardItem = new RawardItem();

            rawardItem.Load(cell.mRootTransform);

            cell.BindUserData(rawardItem);
        }
    }

    public partial class UI_Pub_Layout
    {
        public void SetRewardListActive(bool active)
        {
            mRewardTrans.gameObject.SetActive(active);
        }


        public void PlayEnterRewardAnim()
        {
            mAnimReward.PlayEnter();
        }

        public void PlayEndRewardAnim()
        {
            mAnimReward.PlayExit();
        }

        public void SetReward(int count)
        {
            mScrollGridVertical.CellCount = count;
            mScrollGridVertical.ForceRefreshActiveCell();

            mScrollGridVertical.MoveToIndex(0);
   
        }

        public void SetReward( Transform transform, ItemIdCount itemIdCount)
        {

            var item = mRawardItems.Find(o => o.mTransform == transform);

            if (item == null)
            {

                RawardItem rawardItem = new RawardItem();
                rawardItem.Load(transform);
                mRawardItems.Add(rawardItem);

                item = rawardItem;

            }
            if(item != null)
            item.SetItem(itemIdCount);

        }
    }

    public partial class UI_Pub_Layout
    {
        public void ShowCard()
        {
            mAnimatorTrans.gameObject.SetActive(true);

            CreateShowScene();

            if(mPubShowScene != null)
             mPubShowScene.Play();

            m_BtnSkipMapTimeline.gameObject.SetActive(true);
        }

        public void HidCard()
        {
            mAnimatorTrans.gameObject.SetActive(false);

            if (mPubShowScene != null)
                mPubShowScene.Pause();

            m_BtnSkipMapTimeline.gameObject.SetActive(false);
        }

        private void CreateShowScene()
        {
            if (mPubShowScene != null)
                return;

           var scenego = GameObject.Instantiate(mPubShowSceneOrgine);

            scenego.transform.SetParent(GameCenter.sceneShowRoot.transform);

            mPubShowScene = new PubShowScene();

            mPubShowScene.BindImage = mRIAnimator;
            mPubShowScene.mListener = mListener;

            mPubShowScene.Load(scenego);


        }

        public void DestoryShowScene()
        {
            if (mPubShowScene == null)
                return;

            mPubShowScene.Destory();

            mPubShowScene = null;
        }
    }

    public partial class UI_Pub_Layout
    {
        class PubShowScene
        {
            ShowSceneControl mShowSceneControl = new ShowSceneControl();

            PlayableDirector mUIMapTimeline;

            public RawImage BindImage { get; set; }

           public UI_Pub_Layout.IListener mListener { get; set; }
            public void Load(GameObject gameObject)
            {
                mShowSceneControl.Parse(gameObject);

                mUIMapTimeline = gameObject.transform.Find("Timeline/Card").GetComponent<PlayableDirector>();


              BindImage.texture =  mShowSceneControl.GetTemporary((int)BindImage.rectTransform.rect
                    .width, (int)BindImage.rectTransform.rect.height, 0, RenderTextureFormat.ARGB32, 1);

                mUIMapTimeline.stopped += OnCardAnimEnd;
            }


            private void OnCardAnimEnd(PlayableDirector playable)
            {
                mListener.OnCardAnimEnd();

                HidCard();
            }

            public void Play()
            {
                mUIMapTimeline.Play();
            }
            public void Pause()
            {
                mUIMapTimeline.Pause();
            }
            private void HidCard()
            {
                mUIMapTimeline.Pause();
            }

            public void Destory()
            {
                mShowSceneControl.Dispose();


            }
        }
    }
}
