using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using Framework;
using Lib.Core;

namespace Logic
{
    public class UI_CharacterHead : UIComponent
    {
        public uint characterId;
        private CSVCharacter.Data csvCharacter;

        public Button button;
        public Image image;

        protected override void Loaded()
        {
            image = transform.Find("Image_Icon").GetComponent<Image>();
            button = transform.GetComponent<Button>();

            if (button.GetComponent<ButtonCtrl>() == null)
            {
                button.gameObject.AddComponent<ButtonCtrl>();
            }
        }

        public void Refresh(uint characterId)
        {
            this.characterId = characterId;
            this.csvCharacter = CSVCharacter.Instance.GetConfData(characterId);

            if (csvCharacter != null)
            {
                image.enabled = false;
                ImageHelper.SetIcon(image, csvCharacter.headid);
            }
            else
            {
                // Execption
            }
        }
    }

    public class UI_CharacterHighlight : UIComponent
    {
        protected override void Loaded()
        {
        }
        public void Refresh(uint characterId, CP_ScrolCircleListItem parent)
        {
            CSVCharacter.Data csvCharacter = CSVCharacter.Instance.GetConfData(characterId);
            transform.SetParent(parent.binder, false);
            (transform as RectTransform).anchoredPosition3D = Vector3.zero;
            gameObject.SetActive(false);
            gameObject.SetActive(/*!parent.inCircle*/ true);
        }
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
    }

    public class UI_CreateCharacter : UIBase, UI_CreateCharacter_Layout.IListener
    {
        private UI_CreateCharacter_Layout layout = new UI_CreateCharacter_Layout();

        private uint characterId;
        private CSVCharacter.Data csvCharacter;

        private UI_CharacterHighlight selected;

        private GameObject timeline;
        private Camera timelineCamera;
        private PlayableDirector playableDirector;
        private TimelineDirector timelineDirector;
        private CP_BehaviourCollector behaviourCollector;
        private CP_TransformCollector transformCollector;
        private BoxCollider collider;

        //private World world;
        private Transform root;
        private PerformActor player;
        //private AnimationComponent animationComponent;

        private Dictionary<uint, UI_CharacterHead> heads;

        protected override void OnLoaded()
        {
            FindComponents();
        }
        protected override void OnClose() {
            timelineCamera = null;
            playableDirector = null;
            behaviourCollector = null;
            transformCollector = null;
            collider = null;
            heads = null;
            //animationComponent = null;

            audioEntry?.Stop();
        }

        protected override void OnOpened() {
            //world = new World("CreateCharacter");
            GameObject go = new GameObject("CreateCharacter");
            root = go.transform;

            List<uint> ls = new List<uint>();
            var csvDict = CSVCharacter.Instance.GetAll();
            foreach (var kvp in csvDict)
            {
                if (kvp.active > 0)
                {
                    ls.Add(kvp.id);
                }
            }
            heads = new Dictionary<uint, UI_CharacterHead>();

            layout.scrollList.SetData(ls, OnBtnClick, OnRefresh, OnStateChanged);
            var indexer = layout.scrollList.deQueue[0];

            OnBtnClick(indexer.id, indexer.cp);

            OnRandom_ButtonClicked();
        }

        protected override void OnDestroy() {
            UnloadLast();
            if (root != null)
            {
                GameObject.DestroyImmediate(root.gameObject);
                root = null;
            }

            //world?.Dispose();
            //world = null;
            characterId = 0;
            selected.SetParent(layout.highlightParent);
            if (layout.scrollList)
            {
                layout.scrollList.Clear();
            }
        }

        private void FindComponents()
        {
            layout.Parse(gameObject);
            layout.RegisterEvents(this);

            layout.scrollList.scrollRect.onBeginDrag += OnBeginDrag;
            layout.scrollList.scrollRect.onEndDrag += OnEndDrag;
            layout.scrollList.centerOnChild.onBeginCenter += OnBeginCenter;

            selected = new UI_CharacterHighlight();
            selected.Init(layout.highlight.gameObject.transform);
        }
        private void OnBeginDrag(PointerEventData data) { selected.Hide(); }
        private void OnEndDrag(PointerEventData data) { }
        private void OnFinished()
        {
            // OnBtnClick(layout.scrollList.inCircleItem.id, layout.scrollList.inCircleItem);
        }
        private void OnBeginCenter()
        {
            float diffY = layout.scrollList.GetToCenterOnDiffY();
            if (Mathf.Abs(diffY) < layout.scrollList.ySize / 2)
            {
                CP_ScrolCircleListItem first = layout.scrollList.GetFirst();
                if (first != null)
                {
                    OnBtnClick(first.id, first);
                }
            }
            else
            {
                CP_ScrolCircleListItem first = layout.scrollList.inCircleItem;
                if (first != null)
                {
                    OnBtnClick(first.id, first);
                }
            }
        }

        private void OnBtnClick(uint id, CP_ScrolCircleListItem cp)
        {
            // 切换角色
            ChangeCharacter(id, cp);
            MoveTargetPosition(id, cp);
        }
        private void MoveTargetPosition(uint id, CP_ScrolCircleListItem cp)
        {
            float diffY = 0f;
            if (!cp.inCircle) // 逆转
            {
                diffY = layout.scrollList.GetToCenterOnDiffY(cp);
            }
            else // 顺转
            {
                diffY = layout.scrollList.ySize - layout.scrollList.GetToCenterOnDiffY();
                diffY = -diffY;
            }

            // 移动到特定的位置
            Vector3 scrollLocalPosition = layout.scrollList.transform.localPosition;
            Vector3 targetLocalPosition = new Vector3(scrollLocalPosition.x, scrollLocalPosition.y + diffY, scrollLocalPosition.z);
            CP_SpringPanel.Begin(layout.scrollList.gameObject, layout.scrollList.transform.parent.TransformPoint(targetLocalPosition)/*, OnFinished*/);
        }
        private void OnRefresh(uint id, GameObject go)
        {
            if (!heads.ContainsKey(id))
            {
                UI_CharacterHead uc = new UI_CharacterHead();
                uc.Init(go.transform);
                heads.Add(id, uc);
            }
            heads[id].Refresh(id);
        }
        private void OnStateChanged(CP_ScrolCircleListItem cp, bool oldState, bool newState)
        {
            //cp.binder.gameObject.SetActive(!newState);
        }

        private AudioEntry audioEntry;
        private void ChangeCharacter(uint id, CP_ScrolCircleListItem cp)
        {
            selected.Show();
            if (characterId == id) { return; }

            characterId = id;
            csvCharacter = CSVCharacter.Instance.GetConfData(id);

            audioEntry?.Stop();
            audioEntry = AudioUtil.PlayDubbing(csvCharacter.create_char_audio, AudioUtil.EAudioType.NPCSound, true, false);

            // 切换模型
            UnloadLast();

            selected.Refresh(id, cp);
            ImageHelper.SetIcon(layout.charName, csvCharacter.name_icon);

            GameObject go = GlobalAssets.GetAsset<GameObject>(csvCharacter?.create_char_timeline);
            if (go != null)
            {
                timeline = GameObject.Instantiate<GameObject>(go);
            }
            if (timeline != null)
            {
                timelineCamera = CameraManager.mCamera;
                if (timelineCamera != null && timelineCamera.GetComponent<PhysicsRaycaster>() == null)
                {
                    timelineCamera.gameObject.AddComponent<PhysicsRaycaster>();
                }

                Transform modelParent = timeline.transform.Find("model");
                GameObject showGo = GlobalAssets.GetAsset<GameObject>(csvCharacter?.model_show);
                if (showGo != null)
                {
                    //player = world.CreateActor<PerformActor>(csvCharacter.id);
                    //player = PerformActor.Create(csvCharacter.id, world);
                    player = World.AllocActor<PerformActor>(csvCharacter.id);
                    player.SetParent(root);

                    player.LoadModel(showGo, (actor) =>
                    {
                        actor.gameObject.transform.Setlayer(ELayerMask.Default);
                        actor.gameObject.transform.localPosition = Vector3.zero;
                        actor.gameObject.transform.localEulerAngles = Vector3.zero;
                        actor.gameObject.transform.localScale = Vector3.one;
                        actor.gameObject.AddComponent<DragGameObject>().onDrag += OnDragModel;
                        collider = actor.gameObject.AddComponent<BoxCollider>();
                        collider.center = new Vector3(0, 1, 0);
                        collider.size = new Vector3(1, 2, 1);

                        collider.enabled = false;

                        var binders = timeline.GetComponentsInChildren<CP_TransformBinder>(true);
                        foreach (var binder in binders)
                        {
                            binder.Init(actor.gameObject.transform.GetChild(0).transform.GetChild(0)).Bind();
                        }
                        behaviourCollector = actor.gameObject.GetComponentInChildren<CP_BehaviourCollector>();
                        if (behaviourCollector != null)
                        {
                            behaviourCollector.Enable(false);
                        }
                        transformCollector = timeline.GetComponentInChildren<CP_TransformCollector>();
                        if (transformCollector != null)
                        {
                            transformCollector.Enable(false);
                        }

                        playableDirector = timeline.transform.Find("timeline").GetComponent<PlayableDirector>();
                        if (playableDirector != null)
                        {
                            playableDirector.stopped += OnGraphStoped;
                            timelineDirector = playableDirector.gameObject.AddComponent<TimelineDirector>();
                            timelineDirector?.Collect();
                            Transform mesh = actor.modelTransform.GetChild(0);
                            timelineDirector.SetBinding("Model", mesh);
                            Animator animator = mesh.GetComponent<Animator>();
                            if (animator == null)
                            {
                                animator = mesh.gameObject.AddComponent<Animator>();
                            }
                            timelineDirector.SetBinding("ModelAnimator", animator);
                            timelineDirector.SetBinding("Camera", timelineCamera);
                            timelineDirector.SetBinding("CameraAnimator", timelineCamera.GetComponent<Animator>());

                            playableDirector.Play();
                        }
                    });

                    //player.animationComponent = World.AddComponent<AnimationComponent>(player);
                    player.animationComponent.SetSimpleAnimation(player.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                    player.SetParent(modelParent);
                }
            }
            timeline.transform.SetParent(TimelineManager.root);
        }
        private void OnDragModel()
        {
            if(behaviourCollector != null)
            {
                behaviourCollector.Enable(true);
            }
        }
        private void UnloadLast()
        {
            //world?.DestroyActor(player);
            //player = null;
            if(player != null)
            {
                World.CollecActor(ref player);
            }

            // 切换模型
            if (timeline != null)
            {
                Object.Destroy(timeline);
            }
            timeline = null;
        }

        HashSet<uint> npcShow1Set = new HashSet<uint>() { (uint)EStateType.NPCShow1 };
        private void OnGraphStoped(PlayableDirector playableDirector)
        {
            if (csvCharacter != null && UIManager.IsOpen(EUIID.UI_CreateCharacter))
            {
                if (player.animationComponent != null && this.playableDirector == playableDirector)
                {
                    // +100为高模
                    uint highId = Hero.GetHighModelID(csvCharacter.id);
                    if(transformCollector != null)
                    {
                        transformCollector.Enable(true);
                    }
                    if(collider != null)
                    {
                        collider.enabled = true;
                    }

                    player.animationComponent.UpdateHoldingAnimations(highId, csvCharacter.show_weapon_id, npcShow1Set, EStateType.NPCShow1);
                }
            }
        }
        public void OnEnter_ButtonClicked()
        {
            if (csvCharacter != null && csvCharacter.create <= 0)
            {
                // Todo:
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000300));
                return;
            }
            CSVParam.Data csv = CSVParam.Instance.GetConfData(1);
            uint nameLenLimit = csv == null ? 10 : System.Convert.ToUInt32(csv.str_value);
            string name = layout.Input.text.Trim();
            if(name == "")
            {
                // Todo:
                Sys_Hint.Instance.PushContent_Normal("名字太短");
            }
            else if (name.Length > nameLenLimit)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101001));
            }
            else if (Sys_RoleName.Instance.HasBadNames(name))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101011));
            }
            else
            {
                //检测网络断开
                if (Net.NetClient.Instance.IsConnected)
                {
                    //创建角色登录 打点
                    HitPointManager.HitPoint("game_btn_createrole_entergame");
                    Sys_Role.Instance.ReqCreateCharacter(name, characterId);
                }
                else
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(10014).words;
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        Sys_Role.Instance.ExitGameReq();
                    }, 10012);
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
            }
        }
        public void OnReturn_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_CreateCharacter);
            LevelManager.EnterLevel(typeof(LvLogin));
        }

        public void OnRandom_ButtonClicked()
        {
            layout.Input.text = Sys_RoleName.Instance.RandomName();
        }
        public void OnStart_ButtonClicked()
        {

        }
    }
}
