using Logic.Core;
using Lib.Core;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Logic
{
    public class SuperHero : SceneActor, IAnimatorActor
    {
        public static Type Type = typeof(SuperHero);

        public GameObject AnimatorGameObject { get; set; }

        public uint SuperHeroID { get; set; }

        //protected override void OnSetName()
        //{
        //    gameObject.name = $"SuperHero_{uID.ToString()}";
        //}
        //
        //protected override void OnSetParent()
        //{
        //    SetParent(GameCenter.partnerRoot.transform);
        //}

        public override void SetLayer(Transform transform)
        {
            transform.Setlayer(ELayerMask.Partner);
            cacheELayerMask = ELayerMask.Partner;
        }

        protected override void LoadDepencyAssets()
        {
            assetsGroupLoader = new AssetsGroupLoader();
            List<string> animationPaths;
            AnimationComponent.GetAnimationPaths(SuperHeroID, Constants.UMARMEDID, out animationPaths, Constants.IdleAnimationClipHashSet);
            if (animationPaths != null)
            {
                for (int index = 0, len = animationPaths.Count; index < len; index++)
                {
                    assetsGroupLoader.AddLoadTask(animationPaths[index]);
                }
            }
            else
            {
                DebugUtil.LogError($"animationPaths is null PartnerID: {SuperHeroID}");
            }
        }

        protected override void OnOtherSet()
        {
            AnimatorGameObject = modelGameObject.transform.GetChild(0).gameObject;
        }
    }

    /// <summary>
    /// 大场景伙伴///
    /// </summary>
    public class MPartner : SceneActor, IAnimatorActor, IWeaponComponent, ICareerComponent
    {
        public GameObject AnimatorGameObject { get; set; }
        
        public WeaponComponent weaponComponent = new WeaponComponent();
        public CareerComponent careerComponent = new CareerComponent();        

        public uint PartnerID { get; set; }

        protected override void OnConstruct()
        {
            base.OnConstruct();

            weaponComponent.actor = this;
            weaponComponent.Construct();

            careerComponent.actor = this;
            careerComponent.Construct();
        }

        protected override void OnDispose()
        {
            weaponComponent.Dispose();
            careerComponent.Dispose();

            base.OnDispose();
        }

        //protected override void OnSetName()
        //{
        //    gameObject.name = $"partner_{uID.ToString()}";
        //}
        //
        //protected override void OnSetParent()
        //{
        //    SetParent(GameCenter.partnerRoot.transform);
        //}

        public override void SetLayer(Transform transform)
        {
            transform.Setlayer(ELayerMask.Partner);
            cacheELayerMask = ELayerMask.Partner;
        }

        protected override void LoadDepencyAssets()
        {
            assetsGroupLoader = new AssetsGroupLoader();
            List<string> animationPaths;
            AnimationComponent.GetAnimationPaths(PartnerID, weaponComponent.CurWeaponID, out animationPaths, Constants.IdleAnimationClipHashSet);
            if (animationPaths != null)
            {
                for (int index = 0, len = animationPaths.Count; index < len; index++)
                {
                    assetsGroupLoader.AddLoadTask(animationPaths[index]);
                }
            }
            else
            {
                DebugUtil.LogError($"animationPaths is null PartnerID: {PartnerID}");
            }
        }

        protected override void OnOtherSet()
        {
            AnimatorGameObject = modelGameObject.transform.GetChild(0).gameObject;
            weaponComponent?.LoadWeapon();
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