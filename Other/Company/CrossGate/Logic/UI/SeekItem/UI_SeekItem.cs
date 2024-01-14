using Packet;
using UnityEngine;
using Logic.Core;
using Lib.AssetLoader;
using System.Collections.Generic;
using UnityEngine.UI;
using Table;
using Framework;
using System;
using Lib.Core;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Logic
{
    public class UI_SeekItem : UIBase
    {
        public class ItemWarp
        {
            public uint id;
            private GameObject gameobject;
            private RectTransform rectTransform;
            Lib.Core.EventTrigger eventListener;
            public Action<ItemWarp> onclicked;
            public Action onRemoveListener;

            private float x_ratio;
            private float y_ratio;
            private float weith;
            private float height;
            private float scale;
            private float rotateangle;
            public bool isVaild;
            public bool canshowFx;
            private bool bRotate = false;
            private GameObject fx_item;

            public void BindGameObject(GameObject @object)
            {
                gameobject = @object;
                rectTransform = gameobject.transform as RectTransform;
                eventListener = Lib.Core.EventTrigger.Get(gameobject);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnPointerClick);
            }

            public void InitUi()
            {
                ImageHelper.SetIcon(gameobject.GetComponent<Image>(), CSVItem.Instance.GetConfData(id).icon_id);
                rectTransform.anchoredPosition = new Vector2(weith * x_ratio, height * y_ratio);
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.localEulerAngles += new Vector3(0, 0, rotateangle);
                rectTransform.localScale = new Vector3(scale, scale, 1);
            }

            public void PlayitemFx()
            {
                if (fx_item == null)
                {
                    fx_item = GameObject.Instantiate<GameObject>(GlobalAssets.GetAsset<GameObject>(GlobalAssets.sPrefab_Fx_item));
                    fx_item.transform.SetParent(gameobject.transform);
                    fx_item.transform.localScale = Vector3.one;
                    fx_item.transform.localPosition = Vector3.zero;
                }
            }

            public void StopitemFx()
            {
                if (null != fx_item)
                {
                    GameObject.Destroy(fx_item);
                    fx_item = null;
                }
            }

            public void BindData(uint _id, float _x, float _y, float _weith, float _height, bool _isVaild, bool _canshowFx, float _rotateangle, float _scale)
            {
                id = _id;
                x_ratio = _x;
                y_ratio = _y;
                weith = _weith;
                height = _height;
                isVaild = _isVaild;
                canshowFx = _canshowFx;
                rotateangle = _rotateangle;
                scale = _scale / 100;
            }

            public void BindListener(Action<ItemWarp> action)
            {
                onclicked = action;
                bRotate = false;
            }

            private void OnPointerClick(BaseEventData baseEventData)
            {
                onclicked?.Invoke(this);
            }

            public void PlayAmin(bool taskOver, Action onComplete)
            {
                bRotate = true;
                onRemoveListener = onComplete;
                if (taskOver)
                {
                    rectTransform.DOAnchorPos(new Vector2(weith / 2, height / 2), 1f).onComplete += OnComplete;
                    //Sys_Task.Instance.ReqStepGoalFinishEx(Sys_LittleGame_SeekItem.Instance._TaskId);
                }
                else
                {
                    rectTransform.DOAnchorPos(new Vector2(weith / 2, height / 2), 1f);
                }
            }

            public void OnComplete()
            {
                bRotate = false;
                onRemoveListener?.Invoke();
                //StopitemFx();
                UIManager.CloseUI(EUIID.UI_SeekItem);
               
                Sys_HUD.Instance.OpenHud();
                Sys_LittleGame_SeekItem.Instance.LeaveSeekItem();
                //任务完成
                StopitemFx();
            }

            public void OnRemoveListener()
            {
                GameObject.Destroy(eventListener);
            }

            public void Update(float deltaTime)
            {
                if (!bRotate)
                    return;
                gameobject.transform.Rotate(Vector3.forward, 300 * deltaTime);
            }
        }

        private Image _eventBg;
        private Text _taskName;
        private Text _taskinfo;
        private Transform _itemRoot;
        private Transform _itemtemp;
        private Transform _rewardIconRoot;
        private Transform _rewardIcontemp;
        private int _seededCount = 0;
        private CSVSeekItem.Data _cSVSeekItemData;
        private List<ItemWarp> _vaildItems = new List<ItemWarp>();
        private List<ItemWarp> _allItems = new List<ItemWarp>();
        List<uint> _showId = new List<uint>();
        List<uint> _vaildId = new List<uint>();
        private uint _seekId;
        private int _stepIndex;
        private uint _taskId;
        private Timer _timer;


        protected override void OnOpen(object arg)
        {
            Tuple<uint, object> tuple = arg as Tuple<uint, object>;
            if (tuple != null)
            {
                _taskId = tuple.Item1;
                _seekId = Convert.ToUInt32(tuple.Item2);
            }
            //UIManager.CloseUI(EUIID.HUD);
            Sys_HUD.Instance.CloseHud();
        }

        protected override void OnShow()
        {
            _cSVSeekItemData = CSVSeekItem.Instance.GetConfData(_seekId);
            Sys_LittleGame_SeekItem.Instance.EnterSeekItem(_taskId, _seekId);
            _showId = _cSVSeekItemData.showId;
            _vaildId = _cSVSeekItemData.itemId;
            _vaildItems.Clear();
            _allItems.Clear();
            _seededCount = 0;
            UpdateUI();
            ConstructItems();
        }

        protected override void OnLoaded()
        {
            _eventBg = transform.Find("View_Message/EventBG").GetComponent<Image>();
            _taskName = transform.Find("View_Message/dialogue/bg/TitleImage/TitleName").GetComponent<Text>();
            _taskinfo = transform.Find("View_Message/dialogue/bg/taskInfo").GetComponent<Text>();
            _rewardIconRoot = transform.Find("View_Message/dialogue/iconRoot");
            _rewardIcontemp = transform.Find("View_Message/dialogue/iconRoot/icon");
            _itemRoot = transform.Find("View_Message/itemroot");
            _itemtemp = _itemRoot.Find("item");

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(_eventBg);
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnPointerClick);
            GameCenter.mCameraController.RecordLastCameraData();
        }

        public void ConstructItems()
        {
            int childCount = _itemRoot.childCount;
            int dataCount = _showId.Count;
            if (dataCount > childCount)
            {
                for (int i = childCount; i < dataCount; i++)
                {
                    FrameworkTool.CreateGameObject(_itemtemp.gameObject, _itemRoot.gameObject);
                }
            }
            else
            {
                for (int i = dataCount; i < childCount; i++)
                {
                    _itemRoot.GetChild(i).gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < _showId.Count; i++)
            {
                ItemWarp itemWarp = new ItemWarp();
                itemWarp.BindGameObject(_itemRoot.GetChild(i).gameObject);
                bool isVaild = _vaildId.Contains(_showId[i]);
                itemWarp.BindData(_showId[i], _cSVSeekItemData.showSeat[i][0] / 1000f, _cSVSeekItemData.showSeat[i][1] / 1000f, _eventBg.rectTransform.rect.width,
                    _eventBg.rectTransform.rect.height, isVaild, _cSVSeekItemData.showItem != 0, (float)_cSVSeekItemData.showRotate[i], (float)_cSVSeekItemData.showScale[i]);
                itemWarp.BindListener(
                _ =>
                {
                    OnItemClicked(_);
                }
                );
                itemWarp.InitUi();
                _allItems.Add(itemWarp);
                if (isVaild)
                    _vaildItems.Add(itemWarp);
            }
        }

        private void UpdateUI()
        {
            TextHelper.SetText(_taskName, CSVLanguage.Instance.GetConfData(CSVSeekItem.Instance.GetConfData(_seekId).taskName).words);
            TextHelper.SetText(_taskinfo, CSVLanguage.Instance.GetConfData(CSVSeekItem.Instance.GetConfData(_seekId).taskDescribe).words);
            if (_cSVSeekItemData.showItem == 0)
            {
                _rewardIconRoot.gameObject.SetActive(false);
                return;
            }
            _rewardIconRoot.gameObject.SetActive(true);
            for (int i = 1; i < _vaildId.Count; i++)
            {
                FrameworkTool.CreateGameObject(_rewardIcontemp.gameObject, _rewardIconRoot.gameObject);
            }
            for (int i = 0; i < _vaildId.Count; i++)
            {
                ImageHelper.SetIcon(_rewardIconRoot.GetChild(i).transform.Find("icon").GetComponent<Image>(), CSVItem.Instance.GetConfData(_vaildId[i]).icon_id);
            }
        }

        private void OnItemClicked(ItemWarp itemWarp)
        {
            if (itemWarp.isVaild)
            {
                uint contentId = CSVSeekItem.Instance.GetConfData(_seekId).trueTips;
                Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(contentId).words);
                _seededCount++;
                itemWarp.PlayAmin(_seededCount == _vaildId.Count, OnRemoveItemWarpEvent);
            }
            else
            {
                uint contentId = CSVSeekItem.Instance.GetConfData(_seekId).errorTips;
                Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(contentId).words);
                for (int i = 0; i < _vaildItems.Count; i++)
                {
                    if (_cSVSeekItemData.effectType == 1)
                    {
                        if (_vaildItems[i].canshowFx)
                        {
                            _vaildItems[i].PlayitemFx();
                            //_timer = Timer.Register(1, _vaildItems[i].StopitemFx);
                        }
                    }
                }
            }
        }


        private void OnPointerClick(BaseEventData baseEventData)
        {
            uint contentId = CSVSeekItem.Instance.GetConfData(_seekId).spaceTips;
            Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(contentId).words);
            if (_cSVSeekItemData.effectType == 2)
            {
                for (int i = 0; i < _vaildItems.Count; i++)
                {
                    if (_vaildItems[i].canshowFx)
                    {
                        _vaildItems[i].PlayitemFx();
                        //_timer = Timer.Register(1, _vaildItems[i].StopitemFx);
                    }
                }
            }
        }

        protected override void OnUpdate()
        {
            foreach (var item in _vaildItems)
            {
                item.Update(deltaTime);
            }
        }

        private void OnRemoveItemWarpEvent()
        {
            foreach (var item in _allItems)
            {
                item.OnRemoveListener();
            }
        }
    }
}


