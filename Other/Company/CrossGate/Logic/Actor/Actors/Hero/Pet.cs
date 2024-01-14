using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class Pet : SceneActor, IAnimatorActor, IMovementComponent
    {        
        public CSVPetNew.Data csvPetData
        {
            get;
            set;
        }

        public Hero HandlerHero
        {
            get;
            set;
        }

        public bool Max
        {
            get;
            set;
        }

        public uint suitID
        {
            get;
            set;
        }

        public uint Build
        {
            get;
            set;
        }

        public bool MagicSoul
        {
            get;
            set;
        }

        public GameObject AnimatorGameObject { get; set; }

        public StateComponent stateComponent = new StateComponent();
        public AnimationComponent animationComponent = new AnimationComponent();
        public MovementComponent movementComponent = new MovementComponent();
        public FollowComponent followComponent = new FollowComponent();
        public PetShowInMainWorldComponent petShowInMainWorldComponent;

        protected override void OnConstruct()
        {
            base.OnConstruct();
            stateComponent.actor = this;
            stateComponent.Construct();

            movementComponent.stateComponent = stateComponent;
            movementComponent.actor = this;
            movementComponent.Construct();

            animationComponent.stateComponent = stateComponent;
            animationComponent.actor = this;
            animationComponent.Construct();

            followComponent.actor = this;
            followComponent.Construct();
        }

        protected override void OnDispose()
        {            
            movementComponent.Dispose();
            animationComponent.Dispose();
            stateComponent.Dispose();
            followComponent.Dispose();
            petShowInMainWorldComponent.Dispose();
            petShowInMainWorldComponent = null;

            csvPetData = null;
            HandlerHero = null;

            if (Max)
            {
                EffectUtil.Instance.UnloadEffectByTag(uID, EffectUtil.EEffectTag.FullPet);
            }
            suitID = 0;

            if (Build != 0)
            {
                EffectUtil.Instance.UnloadEffectByTag(uID, EffectUtil.EEffectTag.Build);
            }
            Build = 0;

            if (MagicSoul)
            {
                EffectUtil.Instance.UnloadEffectByTag(uID, EffectUtil.EEffectTag.MagicSoul);
            }
            MagicSoul = false;

            base.OnDispose();
        }

        //protected override void OnSetName()
        //{
        //    if (HandlerHero.eHeroType == Hero.EHeroType.Self)
        //    {
        //        gameObject.name = $"PetSelf_{csvPetData.id.ToString()}_{uID.ToString()}";
        //    }
        //    else if (HandlerHero.eHeroType == Hero.EHeroType.Other)
        //    {
        //        gameObject.name = $"PetOther_{csvPetData.id.ToString()}_{uID.ToString()}";
        //    }
        //}

        //protected override void OnSetParent()
        //{
        //    SetParent(GameCenter.heroRoot.transform);
        //}

        public override void SetLayer(Transform transform)
        {
            if (HandlerHero.eHeroType == Hero.EHeroType.Self)
            {
                transform.Setlayer(ELayerMask.Player);
                cacheELayerMask = ELayerMask.Player;
            }
            else if (HandlerHero.eHeroType == Hero.EHeroType.Other)
            {
                transform.Setlayer(ELayerMask.OtherActor);
                cacheELayerMask = ELayerMask.OtherActor;
            }

            if (HandlerHero != null && HandlerHero.heroBaseComponent.TeamID != 0)
            {
                SetLayerHide();
                fxRoot.SetActive(false);
            }
        }

        protected override void OnOtherSet()
        {
            AnimatorGameObject = modelGameObject.transform.GetChild(0).gameObject;

            if (Max)
            {
                string path = Sys_Pet.Instance.GetPetGearFxPath(csvPetData, true);

                if (path != null)
                {
                    if (HandlerHero.eHeroType == Hero.EHeroType.Self)
                    {
                        EffectUtil.Instance.LoadEffect(uID, path, fxRoot.transform, EffectUtil.EEffectTag.FullPet, 0, 1, 1, ELayerMask.Player);
                    }
                    else if (HandlerHero.eHeroType == Hero.EHeroType.Other)
                    {
                        EffectUtil.Instance.LoadEffect(uID, path, fxRoot.transform, EffectUtil.EEffectTag.FullPet, 0, 1, 1, ELayerMask.OtherActor);
                    }
                }
            }

            if (Build != 0)
            {
                string path = Sys_Pet.Instance.GetPetRemakePerfectFxPath((int)Build);
                if (HandlerHero.eHeroType == Hero.EHeroType.Self)
                {
                    EffectUtil.Instance.LoadEffect(uID, path, fxRoot.transform, EffectUtil.EEffectTag.Build, 0, 1, 1, ELayerMask.Player);
                }
                else if (HandlerHero.eHeroType == Hero.EHeroType.Other)
                {
                    EffectUtil.Instance.LoadEffect(uID, path, fxRoot.transform, EffectUtil.EEffectTag.Build, 0, 1, 1, ELayerMask.OtherActor);
                }
            }

            if (MagicSoul)
            {
                if (HandlerHero.eHeroType == Hero.EHeroType.Self)
                {
                    EffectUtil.Instance.LoadEffect(uID, CSVParam.Instance.GetConfData(1575).str_value, fxRoot.transform, EffectUtil.EEffectTag.MagicSoul, 0, 1, 1, ELayerMask.Player);
                }
                else if (HandlerHero.eHeroType == Hero.EHeroType.Other)
                {
                    EffectUtil.Instance.LoadEffect(uID, CSVParam.Instance.GetConfData(1575).str_value, fxRoot.transform, EffectUtil.EEffectTag.MagicSoul, 0, 1, 1, ELayerMask.OtherActor);
                }
            }

            PetWearSet(Constants.PETWEAR_EQUIP, suitID, modelTransform);
        }

        public void ChangeBuildEffect()
        {
            EffectUtil.Instance.UnloadEffectByTag(uID, EffectUtil.EEffectTag.Build);
            string path = Sys_Pet.Instance.GetPetRemakePerfectFxPath((int)Build);
            if (HandlerHero.eHeroType == Hero.EHeroType.Self)
            {
                EffectUtil.Instance.LoadEffect(uID, path, fxRoot.transform, EffectUtil.EEffectTag.Build, 0, 1, 1, ELayerMask.Player);
            }
            else if (HandlerHero.eHeroType == Hero.EHeroType.Other)
            {
                EffectUtil.Instance.LoadEffect(uID, path, fxRoot.transform, EffectUtil.EEffectTag.Build, 0, 1, 1, ELayerMask.OtherActor);
            }
        }

        protected override void LoadDepencyAssets()
        {
            assetsGroupLoader = new AssetsGroupLoader();
            List<string> animationPaths;
            AnimationComponent.GetAnimationPaths(csvPetData.action_id, Constants.UMARMEDID, out animationPaths, Constants.IdleAnimationClipHashSet);
            if (animationPaths != null)
            {
                for (int index = 0, len = animationPaths.Count; index < len; index++)
                {
                    assetsGroupLoader.AddLoadTask(animationPaths[index]);
                }
            }
            else
            {
                DebugUtil.LogError($"animationPaths is null cSVPetData.id: {csvPetData.id}");
            }
        }

        public MovementComponent GetMovementComponent()
        {
            return movementComponent;
        }
    }
}
