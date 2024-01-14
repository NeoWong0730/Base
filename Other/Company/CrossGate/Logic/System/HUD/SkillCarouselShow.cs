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
    public class SkillCarouselShow : IHUDComponent
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
        public uint sender;
        private Image icon;
        private Vector3 position;
        private Action<SkillCarouselShow> onComplete;
        private float showTime = 1;
        private float hideTime = 1;
        private uint iconId;
        private bool bVaild = false;
        private TweenerCore<float, float, FloatOptions> tweener1;
        private TweenerCore<float, float, FloatOptions> tweener2;

        public void Construct(GameObject gameObject)
        {
            mRootGameObject = gameObject;
            ParseCp();
        }

        private void ParseCp()
        {
            icon = mRootGameObject.GetComponent<Image>();
            mCanvasGroup = mRootGameObject.GetComponent<CanvasGroup>();
        }

        public void SetPosition(Vector3 _pos)
        {
            position = _pos;
            CalPos();
        }

        public void Carousel(uint _sender, uint _iconId, Action<SkillCarouselShow> _onComplete)
        {
            bVaild = true;
            sender = _sender;
            iconId = _iconId;
            ImageHelper.SetIcon(icon, iconId);
            onComplete = _onComplete;
            mCanvasGroup.alpha = 0;
            tweener1 = DOTween.To(() => mCanvasGroup.alpha, x => mCanvasGroup.alpha = x, 1, showTime);
            tweener1.onComplete += OnTweener1Complete;
        }

        private void OnTweener1Complete()
        {
            tweener2 = DOTween.To(() => mCanvasGroup.alpha, x => mCanvasGroup.alpha = x, 0, hideTime);
            tweener2.onComplete += () =>
             {
                 onComplete.Invoke(this);
             };
        }

        public void Dispose()
        {
            bVaild = false;
            onComplete = null;
            tweener1.Kill();
            tweener2.Kill();
            mRootGameObject?.SetActive(false);
            mCanvasGroup.alpha = 1;
        }

        private void CalPos()
        {
            if (CameraManager.mSkillPlayCamera == null)
            {
                CameraManager.World2UI(mRootGameObject, position, CameraManager.mCamera, UIManager.mUICamera);
                rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, UIManager.mUICamera.transform.position.z + 100);
            }
            else
            {
                CameraManager.World2UI(mRootGameObject, position, CameraManager.mSkillPlayCamera, UIManager.mUICamera, 800, 480,
               CameraManager.relativePos.x, CameraManager.relativePos.y);
                rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, UIManager.mUICamera.transform.position.z + 100);
            }
        }
    }


    public class SkillCarouselQueue
    {
        private Func<GameObject> getSkillCarousel;
        private Action<GameObject> recycleSkillCarousel;
        private GameObject template_SkillCarouselShow;
        private Dictionary<uint, CarouselQueue> carouselQueues = new Dictionary<uint, CarouselQueue>();
        private bool bStartCarousel;

        public SkillCarouselQueue(Func<GameObject> getSkillCarousel, Action<GameObject> recycleSkillCarousel, GameObject template_SkillCarouselShow)
        {
            this.getSkillCarousel = getSkillCarousel;
            this.recycleSkillCarousel = recycleSkillCarousel;
            this.template_SkillCarouselShow = template_SkillCarouselShow;
        }

        public void AddCarousel(uint sender, uint receiver, uint iconId)
        {
            if (!carouselQueues.TryGetValue(receiver, out CarouselQueue carouselQueue))
            {
                carouselQueue = new CarouselQueue(receiver, getSkillCarousel, recycleSkillCarousel, template_SkillCarouselShow);
                carouselQueues.Add(receiver, carouselQueue);
            }
            carouselQueue.AddOrder(sender, iconId);
        }

        public void Undo(uint sender)
        {
            foreach (var item in carouselQueues)
            {
                item.Value.Undo(sender);
            }
        }

        public void StartCarousel()
        {
            bStartCarousel = true;
        }

        public void Update()
        {
            if (!bStartCarousel)
                return;
            foreach (var item in carouselQueues)
            {
                item.Value.Update();
            }
        }

        public void Dispose()
        {
            bStartCarousel = false;
            foreach (var item in carouselQueues)
            {
                item.Value.Dispose();
            }
            carouselQueues.Clear();
        }
    }

    /// <summary>
    /// 处理单个receiver头顶的逻辑
    /// </summary>
    public class CarouselQueue
    {
        public class CQData
        {
            public uint sender;
            public uint icon;

            public CQData(uint sender, uint icon)
            {
                this.sender = sender;
                this.icon = icon;
            }
        }

        private uint receiver;
        //用两个队列存储  队列表现完之后需要重头再表演一次
        private List<CQData> queue = new List<CQData>();
        private List<CQData> _queue = new List<CQData>();
        private bool canPop = true;
        private Lib.Core.Timer timer;
        private float interval = 0.5f;
        private List<SkillCarouselShow> skillCarouselShows = new List<SkillCarouselShow>();
        private Func<GameObject> getSkillCarousel;
        private Action<GameObject> recycleSkillCarousel;
        private GameObject template_SkillCarouselShow;

        public CarouselQueue(uint _receiver, Func<GameObject> getSkillCarousel, Action<GameObject> recycleSkillCarousel, GameObject template_SkillCarouselShow)
        {
            receiver = _receiver;
            this.getSkillCarousel = getSkillCarousel;
            this.recycleSkillCarousel = recycleSkillCarousel;
            this.template_SkillCarouselShow = template_SkillCarouselShow;
        }

        public void AddOrder(uint sender, uint icon)
        {
            CQData cQData = new CQData(sender, icon);
            queue.Add(cQData);
            _queue.Add(cQData);
        }

        public void Undo(uint sender)
        {
            for (int i = skillCarouselShows.Count - 1; i >= 0; i--)
            {
                SkillCarouselShow skillCarouselShow = skillCarouselShows[i];
                if (skillCarouselShow.sender == sender)
                {
                    skillCarouselShow.Dispose();
                    HUDFactory.Recycle(skillCarouselShow);
                    recycleSkillCarousel(skillCarouselShow.mRootGameObject);
                    skillCarouselShows.RemoveAt(i);
                    canPop = true;
                }
            }
            for (int i = queue.Count - 1; i >= 0; i--)
            {
                CQData cQData = queue[i];
                if (cQData.sender == sender)
                {
                    queue.RemoveAt(i);
                }
            }
            for (int i = _queue.Count - 1; i >= 0; i--)
            {
                CQData cQData = _queue[i];
                if (cQData.sender == sender)
                {
                    _queue.RemoveAt(i);
                }
            }
        }

        private void CreateCarouselOrder(CQData cQData)
        {
            Transform trans = MobManager.Instance.GetPosByMobBind(receiver, 2);
            if (trans == null)
            {
                DebugUtil.LogError($"trans=null , receiver={ receiver}    bindType:2");
                return;
            }
            GameObject go;
            go = getSkillCarousel(); ;
            go.SetActive(true);
            SkillCarouselShow skillCarouselShow = HUDFactory.Get<SkillCarouselShow>();
            skillCarouselShow.Construct(go);
            skillCarouselShow.SetPosition(trans.position);
            skillCarouselShow.Carousel(cQData.sender, cQData.icon, OnCarouselComplete);
            skillCarouselShows.Add(skillCarouselShow);
        }

        private void OnCarouselComplete(SkillCarouselShow skillCarouselShow)
        {
            skillCarouselShow.Dispose();
            HUDFactory.Recycle(skillCarouselShow);
            recycleSkillCarousel(skillCarouselShow.mRootGameObject);
            skillCarouselShows.Remove(skillCarouselShow);
            canPop = true;
        }

        public void Update()
        {
            if (!canPop)
                return;
            if (queue.Count > 0)
            {
                canPop = false;
                CQData cQData = queue[0];
                timer?.Cancel();
                timer = Lib.Core.Timer.Register(interval, () =>
                {
                    CreateCarouselOrder(cQData);
                });
                queue.RemoveAt(0);
            }
            else
            {
                for (int i = 0; i < _queue.Count; i++)
                {
                    queue.Add(_queue[i]);
                }
            }
        }

        public void Dispose()
        {
            timer?.Cancel();
            canPop = true;
            queue.Clear();
            _queue.Clear();
            foreach (var item in skillCarouselShows)
            {
                item.Dispose();
                HUDFactory.Recycle(item);
                recycleSkillCarousel(item.mRootGameObject);
            }
            skillCarouselShows.Clear();
        }
    }
}


