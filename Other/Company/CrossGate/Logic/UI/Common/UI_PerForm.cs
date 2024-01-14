using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;
using Lib.AssetLoader;
using DG.Tweening;

namespace Logic
{
    public class UI_PerForm : UIBase
    {
        private BagItemFlightPerForm bagItemFlightPerFormCompmonent;
        //private LifeSkillPerForm lifeSkillPerFormComponent;

        public class BagItemFlightPerForm
        {
            public class PerformItem
            {
                private Transform _transParent;
                private GameObject _gameObject;
                private Image _imgIcon;

                private uint _itemId;
                private Vector3 _posStart;
                private Vector3 _posEnd;
                private float _pauseTime;

                private Timer _timerPause;
                private Timer _timerRotate;

                private bool _isUsing = false;
                public bool IsUsing
                {
                    get
                    {
                        return _isUsing;
                    }
                }

                public void Init(Transform parent, GameObject go)
                {
                    _transParent = parent;
                    _gameObject = go;

                    _gameObject.SetActive(false);
                    _gameObject.transform.SetParent(_transParent);
                    _gameObject.transform.localScale = new Vector3(1, 1, 1);
                    _gameObject.transform.localRotation = Quaternion.identity;

                    _imgIcon = _gameObject.GetComponent<Image>();
                }

                public void Start(uint itemId, Vector3 startPos, Vector3 endPos, float pauseTime)
                {
                    _posStart = startPos;
                    _posEnd = endPos;
                    _pauseTime = pauseTime;

                    _isUsing = true;

                    _gameObject.transform.position = startPos;
                    _gameObject.SetActive(true);

                    CSVItem.Data itemData = CSVItem.Instance.GetConfData(itemId);
                    if (itemData != null)
                        ImageHelper.SetIcon(_imgIcon, itemData.icon_id);

                    _timerPause?.Cancel();
                    _timerPause = Timer.Register(_pauseTime, OnStartMove);
                }

                public void Stop()
                {
                    if (_isUsing)
                    {
                        _timerPause?.Cancel();
                        _timerRotate?.Cancel();
                        DOTween.Kill(_gameObject);
                        _gameObject.SetActive(false);
                    }
                    _isUsing = false;
                }

                private void OnStartMove()
                {
                    _gameObject.transform.DOMove(_posEnd, 1.2f).onComplete += OnEndMove;
                    _timerRotate?.Cancel();
                    _timerRotate = Timer.Register(1.2f, OnRotateEnd, OnRotateUpdate);
                }

                private void OnEndMove()
                {
                    _isUsing = false;
                    _gameObject.SetActive(false);
                    Sys_Bag.Instance.eventEmitter.Trigger(Sys_Bag.EEvents.OnPlayMenuBagAnim);
                }

                private void OnRotateEnd()
                {
                    _timerRotate?.Cancel();
                }

                private void OnRotateUpdate(float elaseTime)
                {
                    _gameObject.transform.Rotate(-Vector3.forward, 100 * elaseTime);
                }

            }

            public Transform transform;
            //public Lib.Core.ObjectPool<GameObject> objectPools=new Lib.Core.ObjectPool<GameObject>(10);
            //public List<GameObject> instancedObjects = new List<GameObject>();
            private List<PerformItem> _listPerfom = new List<PerformItem>();

            private GameObject assetObj;
            private Transform start;
            private Transform start_1;
            private Transform end;
            private Transform endPos_1;
            private Transform pool;
            private Vector3 _end;
            private float SpawnRateTime = 0.5f;
            private float SpawnRateTimer;
            private float PauseShowTime = 0.5f;
            //private float RotateSpeed=500;
            //private Timer timer;
            private float deltaTime;


            public void Init(Transform trans, float delTime)
            {
                transform = trans;
                deltaTime = delTime;

                start = transform.Find("startPos");
                start_1 = transform.Find("start_1Pos");
                end = transform.Find("endPos");
                endPos_1 = transform.Find("endPos_1");
                pool = transform.Find("pool");
                assetObj = GlobalAssets.GetAsset<GameObject>(GlobalAssets.sPrefab_FlightItem);
            }

            public void Update()
            {
                if (Sys_Bag.Instance.AddItemQueue.Count > 0)
                {
                    SpawnRateTimer -= deltaTime;
                    if (SpawnRateTimer <= 0)
                    {
                        uint id = Sys_Bag.Instance.AddItemQueue.Dequeue();
                        SpawnRateTimer = SpawnRateTime;
                        Spawn(id);
                    }
                }
            }

            public void OnClose()
            {
                //OnDispose();
            }

            public void OnHide()
            {
                OnDispose();
            }

            private void OnDispose()
            {
                if (_listPerfom != null)
                {
                    for (int i = 0; i < _listPerfom.Count; ++i)
                    {
                        if (_listPerfom[i] != null)
                            _listPerfom[i].Stop();
                    }
                }
            }

            private PerformItem GetPerformItem()
            {
                for (int i = 0; i < _listPerfom.Count; ++i)
                {
                    if (!_listPerfom[i].IsUsing)
                        return _listPerfom[i];
                }

                return null;
            }

            private void Spawn(uint itemId)
            {
                PerformItem performItem = GetPerformItem();
                if (performItem == null)
                {
                    performItem = new PerformItem();
                    _listPerfom.Add(performItem);
                    performItem.Init(pool, GameObject.Instantiate<GameObject>(assetObj));
                }

                Vector3 startPos = Vector3.zero;
                CSVItem.Data itemData = CSVItem.Instance.GetConfData(itemId);
                if (itemData != null)
                {
                    if (itemData.show_above == 0)
                    {
                        startPos = start.position;
                        PauseShowTime = 0.5f;
                    }
                    else
                    {
                        startPos = start_1.position;
                        PauseShowTime = 1f;
                    }

                    if (itemData.type_id == 5000)
                    {
                        _end = endPos_1.position;
                    }
                    else
                    {
                        _end = end.position;
                    }

                    performItem.Start(itemId, startPos, _end, PauseShowTime);
                }
            }
        }

        public class LifeSkillPerForm : UIComponent
        {
            public Lib.Core.ObjectPool<GameObject> objectPools = new Lib.Core.ObjectPool<GameObject>(10);
            public List<GameObject> instancedObjects = new List<GameObject>();
            public List<GameObject> fliter = new List<GameObject>();
            private GameObject assetObj;
            private Transform start;
            private Transform end;
            private Transform pool;
            public Vector3 _end;
            private float PauseShowTime = 2f;
            private float RotateSpeed = 500;
            private Timer timer;

            protected override void Loaded()
            {
                start = transform.Find("startPos");
                end = transform.Find("endPos");
                pool = transform.Find("pool");
                assetObj = GlobalAssets.GetAsset<GameObject>(GlobalAssets.sPrefab_FlightItem);
            }

            protected override void Update()
            {
                if (instancedObjects.Count > 0)
                {
                    for (int i = instancedObjects.Count - 1; i >= 0; i--)
                    {
                        instancedObjects[i].transform.Rotate(-Vector3.forward, RotateSpeed * Time.deltaTime);
                    }
                }
            }

            public void OnSpawn(uint skillId)
            {
                GameObject clone = objectPools.Get();
                if (clone == null)
                {
                    clone = GameObject.Instantiate<GameObject>(assetObj);
                }
                clone.SetActive(true);
                clone.transform.position = start.position;
                clone.transform.SetParent(pool);
                clone.transform.localScale = new Vector3(1, 1, 1);
                clone.transform.localRotation = Quaternion.identity;
                ImageHelper.SetIcon(clone.GetComponent<Image>(), CSVLifeSkill.Instance.GetConfData(skillId).icon_id_learn);
                timer = Timer.Register(PauseShowTime, () =>
                {
                    if (Sys_LivingSkill.Instance.refrenceButton)
                    {
                        _end = Sys_LivingSkill.Instance.refrenceButton.transform.position;
                    }
                    else
                    {
                        _end = end.position;
                    }
                    instancedObjects.Add(clone);
                    clone.transform.DOMove(_end, 1.2f).onComplete +=
                      () =>
                      {
                          clone.SetActive(false);
                          objectPools.Push(clone);
                          instancedObjects.Remove(clone);
                      };
                }
            );
            }


            public void OnClose()
            {
                timer?.Cancel();
            }

            public void OnHide()
            {
                timer?.Cancel();
            }
        }

        protected override void OnLoaded()
        {
            bagItemFlightPerFormCompmonent = new BagItemFlightPerForm();
            bagItemFlightPerFormCompmonent.Init(transform.Find("Animator/BagFlightItem_Root"), Sys_HUD.Instance.GetDeltaTime());
            //lifeSkillPerFormComponent = AddComponent<LifeSkillPerForm>(transform.Find("Animator/LifeSkill_Root"));
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_LivingSkill.Instance.eventEmitter.Handle<uint>(Sys_LivingSkill.EEvents.OnLearnedSkill, OnLearnedSkill, toRegister);
        }

        private void OnLearnedSkill(uint skillId)
        {
            //lifeSkillPerFormComponent.OnSpawn(skillId);
        }

        protected override void OnUpdate()
        {
            bagItemFlightPerFormCompmonent.Update();
            //lifeSkillPerFormComponent.ExecUpdate();
        }

        protected override void OnClose()
        {
            bagItemFlightPerFormCompmonent.OnClose();
            //lifeSkillPerFormComponent.OnClose();
        }

        protected override void OnHide()
        {
            //lifeSkillPerFormComponent.OnHide();
            bagItemFlightPerFormCompmonent.OnHide();
        }
    }
}


