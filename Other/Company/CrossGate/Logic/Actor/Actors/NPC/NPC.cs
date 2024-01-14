using Logic.Core;
using Table;
using UnityEngine;
using Lib.Core;
using System;
using System.Collections.Generic;
using Framework;
using UnityEngine.AI;

namespace Logic
{
    public class Npc : SceneActor, IAnimatorActor, IMovementComponent, IClick
    {
        public CSVNpc.Data _csvData;
        public CSVNpc.Data cSVNpcData
        {
            get
            {
                return _csvData;
            }
            //set
            //{
            //    _csvData = value;
            //    if (_csvData != null)
            //        InitRigData();
            //}
        }

        public GameObject AnimatorGameObject
        {
            get;
            set;
        }

        //public NPCHUDComponent NPCHUDComponent;

        public VisualComponent VisualComponent = new VisualComponent();
        public AnimationComponent AnimationComponent = new AnimationComponent();
        public ClickComponent clickComponent = new ClickComponent();
        public NPCFunctionComponent NPCFunctionComponent = new NPCFunctionComponent();
        public NpcActiveListenerComponent ActiveListenerComponent = new NpcActiveListenerComponent();
        public MovementComponent movementComponent = new MovementComponent();

        //并非所有都有
        public NPCActionListenerComponent npcActionListenerComponent = new NPCActionListenerComponent();    //npc.cSVNpcData.TriggerPerformRange != 0
        public ActiveMonsterComponent activeMonsterComponent = new ActiveMonsterComponent();                //npc.cSVNpcData.type == (uint)ENPCType.ActiveMonster

        //TODO 新增
        //public CollectionComponent collectionComponent;
        //public WorldBossComponent worldBossComponent;
        //public NPCAreaCheckComponent nPCAreaCheckComponent;

        private WS_NPCManagerEntity _ws_npcManagerEntity;
        public WS_NPCManagerEntity wS_NPCManagerEntity
        {
            get { return _ws_npcManagerEntity; }
            set
            {
                if (_ws_npcManagerEntity != null && _ws_npcManagerEntity.Id != 0)
                    _ws_npcManagerEntity.Dispose();

                _ws_npcManagerEntity = value;
            }
        }

        public Sys_Map.RigNpcData rigNpcData;

        protected override void OnConstruct()
        {
            base.OnConstruct();

            AnimationComponent.actor = this;
            AnimationComponent.Construct();

            clickComponent.actor = this;
            clickComponent.Construct();

            VisualComponent.actor = this;
            VisualComponent.Construct();

            movementComponent.actor = this;
            movementComponent.Construct();

            NPCFunctionComponent.actor = this;
            NPCFunctionComponent.Construct();

            ActiveListenerComponent.actor = this;                                                            
            ActiveListenerComponent.Construct();

            npcActionListenerComponent.actor = this;
            npcActionListenerComponent.Construct();

            activeMonsterComponent.actor = this;
            activeMonsterComponent.Construct();
        }

        protected override void OnDispose()
        {
            Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnRemoveActorHUD, uID);

            AnimationComponent.Dispose();
            clickComponent.Dispose();
            VisualComponent.Dispose();
            movementComponent.Dispose();
            NPCFunctionComponent.Dispose();
            ActiveListenerComponent.Dispose();
            npcActionListenerComponent.Dispose();
            activeMonsterComponent.Dispose();
            
            AnimatorGameObject = null;

            //collectionComponent = null;
            //worldBossComponent = null;
            //nPCAreaCheckComponent = null;            
            //inquiryDistanceCheckComponent = null;            

            rigNpcData = null;

            //if (GameCenter.uniqueNpcs.ContainsKey(cSVNpcData.id))
            //{
            //    GameCenter.uniqueNpcs.Remove(cSVNpcData.id);
            //}

            wS_NPCManagerEntity?.Dispose();
            wS_NPCManagerEntity = null;

            _csvData = null;

            base.OnDispose();
        }

        //protected override void SetGameObject()
        //{
        //    //因为先创建Npc再添加的Movement组件 理论上这个肯定为空
        //    if (this.movementComponent != null)
        //        this.movementComponent.Stop(false);
        //
        //    base.SetGameObject();
        //}

        //protected override void OnSetName()
        //{
        //
        //    if (cSVNpcData.type == (uint)ENPCType.Collection)
        //    {
        //        gameObject.name = $"Collection_{cSVNpcData.id.ToString()}_{uID.ToString()}";
        //    }
        //    else if (cSVNpcData.type == (uint)ENPCType.Note)
        //    {
        //        gameObject.name = $"Note_{cSVNpcData.id.ToString()}_{uID.ToString()}";
        //    }
        //    else if (cSVNpcData.type == (uint)ENPCType.Transmit)
        //    {
        //        gameObject.name = $"Transmit_{cSVNpcData.id.ToString()}_{uID.ToString()}";
        //    }
        //    else if (cSVNpcData.type == (uint)ENPCType.ActiveMonster)
        //    {
        //        gameObject.name = $"ActiveMonster_{cSVNpcData.id.ToString()}_{uID.ToString()}";
        //    }
        //    else if (cSVNpcData.type == (uint)ENPCType.LittleKnow)
        //    {
        //        gameObject.name = $"LittleKnow_{cSVNpcData.id.ToString()}_{uID.ToString()}";
        //    }
        //    else if (cSVNpcData.type == (uint)ENPCType.WorldBoss)
        //    {
        //        gameObject.name = $"WorldBoss_{cSVNpcData.id.ToString()}_{uID.ToString()}";
        //    }
        //    else if (cSVNpcData.type == (uint)ENPCType.EventNPC)
        //    {
        //        gameObject.name = $"EventNPC_{cSVNpcData.id.ToString()}_{uID.ToString()}";
        //    }
        //    else
        //    {
        //        gameObject.name = $"Npc_{cSVNpcData.id.ToString()}_{uID.ToString()}";
        //    }
        //}

        //protected override void OnSetParent()
        //{
        //    SetParent(GameCenter.npcRoot.transform);
        //}

        public override void SetLayer(Transform transform)
        {
            transform.Setlayer(ELayerMask.NPC);
            cacheELayerMask = ELayerMask.NPC;
        }

        protected override void OnOtherSet()
        {
            base.OnOtherSet();

            AnimatorGameObject = modelGameObject.transform.GetChild(0).gameObject;

#if UNITY_EDITOR

            NPCButtonInspector nPCButtonInspector = gameObject.GetNeedComponent<NPCButtonInspector>();

            nPCButtonInspector.SetLookPointAction = () =>
            {
                GameCenter.mCameraController.SetFollowActor(this);
                GameCenter.mCameraController.virtualCamera.ForceRecalculation();

                Debug.Log($"x: {GameCenter.mainHero.transform.position.x - transform.position.x}   y: {GameCenter.mainHero.transform.position.y - transform.position.y}   z:{GameCenter.mainHero.transform.position.z - transform.position.z}");
                Debug.Log($"rx: {GameCenter.mainHero.transform.eulerAngles.x - transform.eulerAngles.x}   ry: {GameCenter.mainHero.transform.eulerAngles.y - transform.eulerAngles.y}   rz:{GameCenter.mainHero.transform.eulerAngles.z - transform.eulerAngles.z}");

                GameCenter.mCameraController.SetFollowActor(GameCenter.mainHero);
                GameCenter.mCameraController.virtualCamera.ForceRecalculation();
            };
#endif
        }

        protected override void LoadDepencyAssets()
        {
            assetsGroupLoader = new AssetsGroupLoader();

            List<string> animationPaths;
            if (cSVNpcData.type == (uint)ENPCType.Collection)
            {
                AnimationComponent.GetAnimationPaths(cSVNpcData.action_id, Constants.UMARMEDID, out animationPaths, Constants.CollectionAnimationClipHashSet);
            }
            else
            {
                AnimationComponent.GetAnimationPaths(cSVNpcData.action_id, Constants.UMARMEDID, out animationPaths, Constants.IdleAnimationClipHashSet);
            }
            if (animationPaths != null)
            {
                for (int index = 0, len = animationPaths.Count; index < len; index++)
                {
                    assetsGroupLoader.AddLoadTask(animationPaths[index]);
                }
            }
            else
            {
                DebugUtil.LogError($"animationPaths is null npcType: {cSVNpcData.id} action_id: {cSVNpcData.action_id}");
            }
        }

        public bool IsColliderEnable()
        {
            UnityEngine.Component collider;
            if (modelGameObject != null && modelGameObject.TryGetComponent(typeof(Collider), out collider))
            {
                if (((Collider)collider).enabled)
                {
                    return true;
                }
            }
            return false;
        }

        public void InitRigData()
        {
            Sys_Map.MapClientData mapClientData = Sys_Map.Instance.GetMapClientData(Sys_Map.Instance.CurMapId);
            if (mapClientData != null && mapClientData.rigNpcDict != null && _csvData != null)
            {
                mapClientData.rigNpcDict.TryGetValue(cSVNpcData.id, out rigNpcData);
            }
        }

        public bool Contains(Transform trans)
        {
            if (rigNpcData != null)
                return rigNpcData.scopeDetection.Contains(trans);
            else
                return false;
        }

        public void OnClick()
        {
            clickComponent.OnClick();
        }

        public MovementComponent GetMovementComponent()
        {
            return movementComponent;
        }
    }
}