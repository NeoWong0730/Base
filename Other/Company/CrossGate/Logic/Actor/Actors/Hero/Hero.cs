using Logic.Core;
using Lib.Core;
using UnityEngine;
using System.Collections.Generic;
using System;
using Framework;
using Table;
using UnityEngine.AI;

namespace Logic
{
    /// <summary>
    /// 大场景玩家角色///
    /// </summary>
    public class Hero : SceneActor, IAnimatorActor, IMovementComponent, IWeaponComponent, ICareerComponent
    {
        public enum EHeroType
        {
            None,
            Self,     //玩家自己
            Other,      //其它
        }

        private EHeroType _heroType;
        public EHeroType eHeroType
        {
            get
            {
                return _heroType;
            }
            set
            {
                _heroType = value;
                if (_heroType == EHeroType.Self)
                {
                    Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateAttr, OnUpdateAttr, true);
                    Sys_Pet.Instance.eventEmitter.Handle<int>(Sys_Pet.EEvents.OnPerFectFxChange, OnPerFectFxChange, true);
                }
            }
        }

        public bool SetLayerCompleted
        {
            get;
            set;
        } = false;

        public GameObject AnimatorGameObject { get; set; }

        //必有组件
        public HeroBaseComponent heroBaseComponent = new HeroBaseComponent();
        public SyncTransformComponent syncTransformComponent = new SyncTransformComponent();
        public AnimationComponent animationComponent = new AnimationComponent();
        public HeroFxComponent heroFxComponent = new HeroFxComponent();
        public StateComponent stateComponent = new StateComponent();
        public MovementComponent movementComponent = new MovementComponent();
        public WeaponComponent weaponComponent = new WeaponComponent();
        public CareerComponent careerComponent = new CareerComponent();

        //必有组件 有生命周期
        //public MovementComponent movementComponent;
        public RoleActionComponent roleActionComponent = new RoleActionComponent();

        //非必有组件 有生命周期
        public FollowComponent followComponent = new FollowComponent();

        //仅主角有
        //public PathComponent pathComponent;
        //public UploadTransformComponent uploadTransformComponent;
        //public HeroSkillComponent skillComponent;
        public AudioListenerComponent audioListenerComponent = new AudioListenerComponent();
        
        // 组件常驻，但是进入家族资源战才会有数值
        public FamilyResBattleComponent familyResBattleComponent = new FamilyResBattleComponent();

        public HeroLoader heroLoader;
        public Mount Mount;
        public Pet Pet;

        protected override void OnConstruct()
        {
            base.OnConstruct();

            //TODO:如果仅仅是为了编辑器下查看方便 可以在出包的时候 去掉这行代码
            SetParent(GameCenter.heroRoot.transform);

            heroLoader = HeroLoader.Create(false);
            heroLoader.SetParent(transform);

            heroLoader.heroDisplay.onLoaded += onModelLoaded;

            heroBaseComponent.actor = this;
            heroBaseComponent.Construct();

            careerComponent.actor = this;
            careerComponent.Construct();

            weaponComponent.actor = this;
            weaponComponent.Construct();

            heroFxComponent.actor = this;
            heroFxComponent.Construct();

            familyResBattleComponent.actor = this;
            familyResBattleComponent.Construct();

            syncTransformComponent.actor = this;
            syncTransformComponent.Construct();

            stateComponent.actor = this;
            stateComponent.Construct();

            animationComponent.stateComponent = stateComponent;
            animationComponent.actor = this;
            animationComponent.Construct();

            roleActionComponent.actor = this;
            roleActionComponent.Construct();

            movementComponent.stateComponent = stateComponent;
            movementComponent.actor = this;
            movementComponent.Construct();

            followComponent.actor = this;
            followComponent.Construct();

            audioListenerComponent.actor = this;
            audioListenerComponent.Construct();
        }

        protected override void OnDispose()
        {
            Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnRemoveActorHUD, uID);
            if (eHeroType == EHeroType.Self)
            {
                Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateAttr, OnUpdateAttr, false);
                Sys_Pet.Instance.eventEmitter.Handle<int>(Sys_Pet.EEvents.OnPerFectFxChange, OnPerFectFxChange, false);
            }

            animationComponent.Dispose();
            stateComponent.Dispose();
            syncTransformComponent.Dispose();
            heroFxComponent.Dispose();
            familyResBattleComponent.Dispose();
            weaponComponent.Dispose();
            careerComponent.Dispose();
            heroBaseComponent.Dispose();
            roleActionComponent.Dispose();
            movementComponent.Dispose();
            followComponent.Dispose();
            audioListenerComponent.Dispose();

            //skillComponent = null;
            //followComponent = null;
            //uploadTransformComponent = null;

            eHeroType = EHeroType.None;

            heroLoader.heroDisplay.onLoaded -= onModelLoaded;
            heroLoader.Dispose();
            heroLoader = null;

            SetLayerCompleted = false;

            if (Mount != null)
            {
                //GameCenter.mainWorld.DestroyActor(Mount);
                //Mount = null;                
                World.CollecActor(ref Mount);
            }

            RemovePet();

            base.OnDispose();
        }

        private void onModelLoaded(int part)
        {
            if (part == (int)EHeroModelParts.Main)
            {
                GameObject go = heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).gameObject;
                SetModelGameObject(go);
                AnimatorGameObject = modelTransform.GetChild(0).gameObject;
                animationComponent.SetSimpleAnimation(AnimatorGameObject.GetNeedComponent<SimpleAnimation>());
            }
        }

        /// <summary>
        /// 加载模型///
        /// </summary>
        /// <param name="LoadOver"></param>
        public override void LoadModel(Action<SceneActor> LoadOver)
        {
            base.LoadModel(LoadOver);

            ELayerMask maskLayer = (uID == Sys_Role.Instance.Role.RoleId) ? ELayerMask.Player : ELayerMask.OtherActor;
            heroLoader.LoadHero(heroBaseComponent.HeroID, weaponComponent.CurWeaponID, maskLayer, heroBaseComponent.fashData, null);
        }

        /// <summary>
        /// 换装切换模型///
        /// </summary>
        public void ChangeModel(bool UpdateAnim = true)
        {
            ELayerMask maskLayer = (uID == Sys_Role.Instance.Role.RoleId) ? ELayerMask.Player : ELayerMask.OtherActor;
            if (UpdateAnim)
            {
                heroLoader.LoadHero(heroBaseComponent.HeroID, weaponComponent.CurWeaponID, maskLayer, heroBaseComponent.fashData, onModelChanged);
            }
            else
            {
                heroLoader.LoadHero(heroBaseComponent.HeroID, weaponComponent.CurWeaponID, maskLayer, heroBaseComponent.fashData, null);
            }
        }

        public void UnloadModelPart(EHeroModelParts eHeroModelParts)
        {
            heroLoader.UnloadModelParts(eHeroModelParts);
        }


        private void onModelChanged(GameObject go)
        {
            animationComponent?.UpdateHoldingAnimations(heroBaseComponent.HeroID, weaponComponent.CurWeaponID, CSVActionState.Instance.GetHeroPreLoadActions(), stateComponent.CurrentState);
        }

        //protected override void OnSetParent()
        //{
        //    SetParent(GameCenter.heroRoot.transform);
        //}

        public override void SetLayer(Transform transform)
        {
            if (eHeroType == EHeroType.Self)
            {
                transform.Setlayer(ELayerMask.Player);
                cacheELayerMask = ELayerMask.Player;
            }
            else if (eHeroType == EHeroType.Other)
            {
                transform.Setlayer(ELayerMask.OtherActor);
                cacheELayerMask = ELayerMask.OtherActor;
            }

            SetLayerCompleted = true;
        }

        protected override void LoadDepencyAssets()
        {
            assetsGroupLoader = new AssetsGroupLoader();
            List<string> animationPaths;
            AnimationComponent.GetAnimationPaths(heroBaseComponent.HeroID, weaponComponent.CurWeaponID, out animationPaths, CSVActionState.Instance.GetHeroPreLoadActions());
            if (animationPaths != null)
            {
                for (int index = 0, len = animationPaths.Count; index < len; index++)
                {
                    assetsGroupLoader.AddLoadTask(animationPaths[index]);
                }
            }
            else
            {
                DebugUtil.LogError($"animationPaths is null heroID: {heroBaseComponent.HeroID}");
            }
        }

        protected override void OnOtherSet()
        {
            AnimatorGameObject = modelGameObject.transform.GetChild(0).gameObject;
        }

        private void OnUpdateAttr()
        {
            if (movementComponent != null)
                movementComponent.fMoveSpeed = Sys_Attr.Instance.pkAttrs[101] / 10000f;
        }

        public void OnMount(uint mountId, ulong uid, uint suitID, uint build, bool magicSoul, bool createEffect = true)
        {
            uint id = mountId / 10;
            uint flag = mountId % 10;
            if (Mount != null)
            {
                OffMount();
            }

            Mount mount = World.AllocActor<Mount>(uid);
            mount.csvPetData = CSVPetNew.Instance.GetConfData(id);
            mount.Max = (flag == 1);
            mount.SetParent(this.transform, false);
            mount.SetName($"Mount_{id.ToString()}_{uID.ToString()}");
            mount.suitID = suitID;
            mount.Build = build;
            mount.MagicSoul = magicSoul;

            mount.HandlerHero = this;
            Mount = mount;
            mount.LoadModel(mount.csvPetData.model, (actor) =>
            {
                mount.animationComponent.SetSimpleAnimation(mount.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                mount.animationComponent.UpdateHoldingAnimations(mount.csvPetData.action_id, Constants.UMARMEDID, null, mount.stateComponent.CurrentState);
                mount.MountRoot = actor.modelGameObject.FindChildByName("mount").transform;
                mount.Scale = new Vector3(1f / mount.AnimatorGameObject.transform.localScale.x, 1f / mount.AnimatorGameObject.transform.localScale.y, 1f / mount.AnimatorGameObject.transform.localScale.z);

                //modelTransform.SetParent(mount.MountRoot, false);
                //modelGameObject.transform.SetAsFirstSibling();
                var binder = modelGameObject.GetNeedComponent<ParentTransformBinder>();
                binder.parent = mount.MountRoot;
                binder.scaleX = mount.AnimatorGameObject.transform.localScale.x;
                binder.scaleY = mount.AnimatorGameObject.transform.localScale.y;
                binder.scaleZ = mount.AnimatorGameObject.transform.localScale.z;
                modelGameObject.transform.localPosition = Vector3.zero;
                modelGameObject.transform.localRotation = Quaternion.identity;
                //modelGameObject.transform.localScale = new Vector3(mount.Scale.x * heroBaseComponent.Scale.x, mount.Scale.y * heroBaseComponent.Scale.y, mount.Scale.z * heroBaseComponent.Scale.z);
                Mount.modelTransform.localScale = new Vector3(heroBaseComponent.Scale.x, heroBaseComponent.Scale.y, heroBaseComponent.Scale.z);

                if (stateComponent.CurrentState == EStateType.Idle)
                {
                    mount.stateComponent.ChangeState(EStateType.Stand);
                }
                else if (stateComponent.CurrentState == EStateType.Walk || stateComponent.CurrentState == EStateType.Run)
                {
                    mount.stateComponent.ChangeState(EStateType.Sprint);
                }
                else
                {
                    mount.stateComponent.ChangeState(stateComponent.CurrentState);
                }

                if (stateComponent.CurrentState == EStateType.Idle)
                {
                    if (mount.csvPetData.action_id_mount == 0)
                        animationComponent?.CrossFade((uint)EStateType.mount_1_idle, Constants.CORSSFADETIME);
                    else
                        animationComponent?.CrossFade((uint)EStateType.mount_2_idle, Constants.CORSSFADETIME);
                }
                else if (stateComponent.CurrentState == EStateType.Run)
                {
                    if (mount.csvPetData.action_id_mount == 0)
                        animationComponent?.CrossFade((uint)EStateType.mount_1_run, Constants.CORSSFADETIME);
                    else
                        animationComponent?.CrossFade((uint)EStateType.mount_2_run, Constants.CORSSFADETIME);
                }
                else if (stateComponent.CurrentState == EStateType.Walk)
                {
                    if (mount.csvPetData.action_id_mount == 0)
                        animationComponent?.CrossFade((uint)EStateType.mount_1_walk, Constants.CORSSFADETIME);
                    else
                        animationComponent?.CrossFade((uint)EStateType.mount_2_walk, Constants.CORSSFADETIME);
                }

                UpMountEvt upMountEvt = new UpMountEvt();
                upMountEvt.actorId = uID;
                upMountEvt.mountId = mount.csvPetData.id;
                Sys_HUD.Instance.eventEmitter.Trigger<UpMountEvt>(Sys_HUD.EEvents.OnUpMount, upMountEvt);
            });

            if (createEffect)
                EffectUtil.Instance.LoadEffect(uID, CSVEffect.Instance.GetConfData(999800001).effects_path, fxRoot.transform, EffectUtil.EEffectTag.OnMount, 1f);
        }

        public void OffMount()
        {
            if (Mount == null)
                return;

            if (modelTransform != null)
            {
                modelTransform.SetParent(gameObject.transform, false);
                modelTransform.SetAsFirstSibling();
                modelTransform.localPosition = Vector3.zero;
                modelTransform.localRotation = Quaternion.identity;
                modelTransform.localScale = Vector3.one;
            }

            World.CollecActor(ref Mount);
            Mount = null;
            animationComponent.UpdateHoldingAnimations(heroBaseComponent.HeroID, weaponComponent.CurWeaponID, CSVActionState.Instance.GetHeroPreLoadActions(), stateComponent.CurrentState, modelGameObject);

            ChangeModelScale(heroBaseComponent.Scale.x, heroBaseComponent.Scale.y, heroBaseComponent.Scale.z);

            Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnDownMount, uID);

            EffectUtil.Instance.LoadEffect(uID, CSVEffect.Instance.GetConfData(999800002).effects_path, fxRoot.transform, EffectUtil.EEffectTag.OffMount, 1f);
        }

        public void AddPet(uint infoID, ulong uid, uint suitID, uint build, bool magicSoul = false)
        {
            if (Pet != null)
            {
                RemovePet();
            }

            if (Sys_FamilyResBattle.Instance.InFamilyBattle)
                return;

            uint id = infoID / 10;
            uint flag = infoID % 10;
            //Pet pet = GameCenter.mainWorld.CreateActor<Pet>(uid);
            Pet pet = World.AllocActor<Pet>(uid, null);
            Pet = pet;

            pet.SetParent(GameCenter.heroRoot.transform);
            if (eHeroType == Hero.EHeroType.Self)
            {
                pet.SetName($"PetSelf_{id.ToString()}_{uID.ToString()}");
            }
            else if (eHeroType == Hero.EHeroType.Other)
            {
                pet.SetName($"PetOther_{id.ToString()}_{uID.ToString()}");
            }

            pet.Max = (flag == 1);
            pet.csvPetData = CSVPetNew.Instance.GetConfData(id);
            pet.HandlerHero = this;
            pet.suitID = suitID;
            pet.Build = build;
            pet.MagicSoul = magicSoul;

            //pet.followComponent = World.AddComponent<FollowComponent>(pet);
            pet.followComponent.Target = this;
            pet.followComponent.Follow = true;
            pet.followComponent.PetFollow = true;
            pet.followComponent.KeepDistance = float.Parse(CSVParam.Instance.GetConfData(1054).str_value) / 1000f;
            //World.AddComponent<PetShowInMainWorldComponent>(pet);
            //World.AddComponent<PetFollowInMainWorldComponent>(pet);
            pet.petShowInMainWorldComponent = new PetShowInMainWorldComponent();
            pet.petShowInMainWorldComponent.Pet = pet;

            //pet.stateComponent = World.AddComponent<StateComponent>(pet);
            //pet.movementComponent = World.AddComponent<MovementComponent>(pet);
            //pet.movementComponent.TransformToPosImmediately(transform.position);

            NavMeshHit navMeshHit;
            Vector3 hitPos = transform.position;
            MovementComponent.GetNavMeshHit(hitPos, out navMeshHit);
            if (navMeshHit.hit)
                pet.transform.position = navMeshHit.position;
            else
                pet.transform.position = hitPos;

            pet.movementComponent.InitNavMeshAgent();

            pet.followComponent.movementComponent.mNavMeshAgent.stoppingDistance = pet.csvPetData.follow_distance;

            pet.LoadModel(pet.csvPetData.model, (actor) =>
            {
                // pet.animationComponent = World.AddComponent<AnimationComponent>(pet);
                pet.animationComponent.SetSimpleAnimation(pet.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                pet.animationComponent.UpdateHoldingAnimations(pet.csvPetData.action_id);
            });
        }

        public void RemovePet()
        {
            if (Pet != null)
            {
                //GameCenter.mainWorld.DestroyActor(Pet);
                //Pet = null;

                World.CollecActor(ref Pet);
            }
        }

        public void ResetPetPos()
        {
            if (Pet != null)
            {
                Pet.transform.position = transform.position;
            }
        }

        void OnPerFectFxChange(int type)
        {
            if (eHeroType == EHeroType.Self)
            {
                uint mountCount = (uint)Sys_Pet.Instance.GetMountPerfectRemakeCount();
                uint followCount = (uint)Sys_Pet.Instance.GetFollowPerfectRemakeCount();

                if (Mount != null && Mount.Build != mountCount)
                {
                    Mount.Build = mountCount;
                    Mount.ChangeBuildEffect();
                }

                if (Pet != null && Pet.Build != followCount)
                {
                    Pet.Build = followCount;
                    Pet.ChangeBuildEffect();
                }
            }
        }

        public override void ChangeModelScale(float x, float y, float z)
        {
            heroBaseComponent.Scale = new Vector3(x, y, z);
            fxRoot.transform.localScale = new Vector3(fxRoot.transform.localScale.x, y, fxRoot.transform.localScale.z);
            //if (Mount == null || Mount.MountRoot == null)
            //{
            //    base.ChangeModelScale(x, y, z);
            //}
            //else
            //{
            //    Mount.MountRoot.localScale = new Vector3(Mount.Scale.x * x, Mount.Scale.y * y, Mount.Scale.z * z);
            //}
            if (Mount != null && Mount.MountRoot != null)
            {
                Mount.modelTransform.localScale = new Vector3(x, y, z);
            }
            base.ChangeModelScale(x, y, z);
        }

        public override void ResetModelScale()
        {
            //if (Mount != null && Mount.MountRoot != null)
            //{
            //    Mount.MountRoot.localScale = Mount.Scale;
            //}
            //else
            //{
            //    base.ResetModelScale();
            //}
            if (Mount != null && Mount.MountRoot != null)
            {
                Mount.modelTransform.localScale = Vector3.one;
            }
            base.ResetModelScale();
        }

        public static uint GetHighModelID(uint id)
        {
            return (id - 1000) * 100 + 3000;
        }

        public static uint GetMainHeroHighModelAnimationID()
        {
            uint id = Sys_Fashion.Instance.GetDressedId(EHeroModelParts.Main);
            id = (uint)(id * 10000 + Sys_Role.Instance.HeroId);
            CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(id);
            if (cSVFashionModelData != null)
            {
                return cSVFashionModelData.action_show_id;
            }
            return 0;
        }

        public static uint GetOtherHeroHighModelAnimationID(uint heroId, Dictionary<uint, List<dressData>> fashoinDic)
        {
            uint id = Sys_Fashion.Instance.GetClothId(fashoinDic);
            id = (uint)(id * 10000 + heroId);
            CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(id);
            if (cSVFashionModelData != null)
            {
                return cSVFashionModelData.action_show_id;
            }
            return 0;
        }

        public MovementComponent GetMovementComponent()
        {
            return movementComponent;
        }

        public WeaponComponent GetWeaponComponent()
        {
            return weaponComponent;
        }

        public CareerComponent GetCareerComponent()
        {
            return careerComponent;
        }
    }
}
