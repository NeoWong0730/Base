using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Logic.Core;
using System.Threading;
using Lib.Core;
using System;
using UnityEngine.UI;
using Table;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace Logic
{
    public class OrderPreShow : IHUDComponent
    {
        public GameObject mRootGameObject;
        private RectTransform rectTransform
        {
            get
            {
                return mRootGameObject.transform as RectTransform;
            }
        }

        private CanvasGroup mCanvasGroup;

        private Image icon;
        public uint sender;

        private Vector3 startPosition;
        private Vector3 endPosition;
        private Lib.Core.Timer timer;
        private float startTime = 2;
        private float moveTime = 1.5f;
        private float GrayedTime = 3;
        private float startCarouseTime = 2;
        private Action<OrderPreShow> onComplete;
        private Action _startCarouse;
        private uint iconId;
        TweenerCore<Vector3, Vector3, VectorOptions> moveTweener;
        TweenerCore<float, float, FloatOptions> fadeTweener;
        private bool bValid;

        public void Construct(GameObject gameObject, uint _iconId)
        {
            mRootGameObject = gameObject;
            iconId = _iconId;
            ParseCp();
            string str = CSVParam.Instance.GetConfData(640).str_value;
            string[] strs = str.Split('|');
            startTime = float.Parse(strs[0]) / 1000f;
            moveTime = float.Parse(strs[1]) / 1000f;
            GrayedTime = float.Parse(strs[2]) / 1000f;
            string _str = CSVParam.Instance.GetConfData(641).str_value;
            string[] _strs = _str.Split('|');
            startCarouseTime = float.Parse(_strs[0]) / 1000f;
        }

        private void ParseCp()
        {
            icon = mRootGameObject.GetComponent<Image>();
            mCanvasGroup = mRootGameObject.GetComponent<CanvasGroup>();
        }


        public void ShowPreOrder(uint sender, Vector3 startWorldPosition, Vector3 endWorldPosition, Action<OrderPreShow> action, Action startCarouse)
        {
            this.sender = sender;
            bValid = true;
            mCanvasGroup.alpha = 1;
            onComplete = action;
            _startCarouse = startCarouse;
            startPosition = startWorldPosition;
            ImageHelper.SetIcon(icon, iconId);
            CalPos();
            Vector3 _endPos = CameraManager.GetUIPositionByWorldPosition(endWorldPosition, CameraManager.mCamera, UIManager.mUICamera);
            endPosition = new Vector3(_endPos.x, _endPos.y, UIManager.mUICamera.transform.position.z + 100);
            timer = Lib.Core.Timer.Register(startTime, () =>
            {
                moveTweener = rectTransform.DOMove(endPosition, moveTime);
                moveTweener.onComplete += MoveEnd;
            });
        }

        private void MoveEnd()
        {
            fadeTweener = DOTween.To(() => mCanvasGroup.alpha, x => mCanvasGroup.alpha = x, 0, GrayedTime);
            fadeTweener.onComplete += OnComplete;
        }

        private void OnComplete()
        {
            onComplete?.Invoke(this);
            timer?.Cancel();
            timer = Lib.Core.Timer.Register(startCarouseTime, () =>
            {
                _startCarouse?.Invoke();
            });
        }

        private void CalPos()
        {
            if (CameraManager.mSkillPlayCamera == null)
            {
                CameraManager.World2UI(mRootGameObject, startPosition, CameraManager.mCamera, UIManager.mUICamera);
                rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, UIManager.mUICamera.transform.position.z + 100);
            }
            else
            {
                CameraManager.World2UI(mRootGameObject, startPosition, CameraManager.mSkillPlayCamera, UIManager.mUICamera, 800, 480,
               CameraManager.relativePos.x, CameraManager.relativePos.y);
                rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, UIManager.mUICamera.transform.position.z + 100);
            }
        }


        public void Dispose()
        {
            bValid = false;
            moveTweener?.Kill();
            fadeTweener?.Kill();
            mRootGameObject.SetActive(false);
            timer?.Cancel();
            onComplete = null;
            mCanvasGroup.alpha = 1;
        }
    }

    public class OrderQueue
    {
        //private List<CreateOrderHUDEvt> orders = new List<CreateOrderHUDEvt>();
        private SkillCarouselQueue skillCarouselQueue;//战斗内技能轮播队列
        private List<OrderPreShow> orderPreShows = new List<OrderPreShow>();
        //private bool canPop = true;
        private Func<GameObject> getOrder;
        private Action<GameObject> recycleOrder;
        private Func<GameObject> getSkillCarousel;
        private Action<GameObject> recycleSkillCarousel;
        private GameObject template_OrderShow;
        private GameObject template_SkillCarouselShow;

        public OrderQueue(Func<GameObject> getOrder, Action<GameObject> recycleOrder, Func<GameObject> getSkillCarousel, Action<GameObject> recycleSkillCarousel,
            GameObject template_OrderShow, GameObject template_SkillCarouselShow)
        {
            this.getOrder = getOrder;
            this.recycleOrder = recycleOrder;
            this.getSkillCarousel = getSkillCarousel;
            this.recycleSkillCarousel = recycleSkillCarousel;
            this.template_OrderShow = template_OrderShow;
            this.template_SkillCarouselShow = template_SkillCarouselShow;
        }

        public void AddOrder(CreateOrderHUDEvt createOrderHUDEvt)
        {
            //canPop = true;
            if (skillCarouselQueue == null)
            {
                skillCarouselQueue = new SkillCarouselQueue(getSkillCarousel, recycleSkillCarousel, template_SkillCarouselShow);
            }
            skillCarouselQueue.AddCarousel(createOrderHUDEvt.senderId, createOrderHUDEvt.receiverId, createOrderHUDEvt.iconId);

            CreatePreOrder(createOrderHUDEvt);
            //orders.Add(createOrderHUDEvt);
        }

        public void Undo(uint senderId)
        {
            for (int i = orderPreShows.Count - 1; i >= 0; i--)
            {
                OrderPreShow orderPreShow = orderPreShows[i];
                if (orderPreShow.sender == senderId)
                {
                    orderPreShows.RemoveAt(i);
                    orderPreShow.Dispose();
                    HUDFactory.Recycle(orderPreShow);
                    recycleOrder(orderPreShow.mRootGameObject);
                }
            }
            skillCarouselQueue.Undo(senderId);
            //for (int i = orders.Count - 1; i >= 0; i--)
            //{
            //    CreateOrderHUDEvt evt = orders[i];
            //    if (evt.senderId == senderId)
            //    {
            //        orders.RemoveAt(i);
            //    }
            //}
        }

        public void CreatePreOrder(CreateOrderHUDEvt createOrderHUDEvt)
        {
            GameObject go;
            go = getOrder();
            go.SetActive(true);
            OrderPreShow orderPreShow = HUDFactory.Get<OrderPreShow>();
            orderPreShow.Construct(go, createOrderHUDEvt.iconId);
            Vector3 start = MobManager.Instance.GetPosByMobBind(createOrderHUDEvt.senderId, 2).position;
            Vector3 end = MobManager.Instance.GetPosByMobBind(createOrderHUDEvt.receiverId, 2).position;
            orderPreShow.ShowPreOrder(createOrderHUDEvt.senderId, start, end, OnOnePreOrderPlayOver, StartCarousel);
            orderPreShows.Add(orderPreShow);
        }

        private void OnOnePreOrderPlayOver(OrderPreShow orderPreShow)
        {
            orderPreShow.Dispose();
            HUDFactory.Recycle(orderPreShow);
            recycleOrder(orderPreShow.mRootGameObject);
            orderPreShows.Remove(orderPreShow);
            //canPop = true;
        }

        private void StartCarousel()
        {
            skillCarouselQueue.StartCarousel();
        }

        public void Update()
        {
            skillCarouselQueue?.Update();
            //if (!canPop)
            //    return;
            //if (orders.Count > 0)
            //{
            //    canPop = false;

            //    CreateOrderHUDEvt createOrderHUDEvt = orders[0];
            //    CreatePreOrder(createOrderHUDEvt);
            //    orders.RemoveAt(0);
            //}
        }



        public void Dispose()
        {
            //orders.Clear();
            //canPop = false;
            skillCarouselQueue?.Dispose();
            foreach (var item in orderPreShows)
            {
                item.Dispose();
                HUDFactory.Recycle(item);
                recycleOrder(item.mRootGameObject);
            }
            orderPreShows.Clear();
        }
    }
}


