using System;
using Lib.Core;
using Logic.Core;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 战斗场景中角色
    /// </summary>
    public class FightHero : FightActor, IWeaponComponent, ICareerComponent
    {
        public HeroLoader heroLoader;
        public AnimationComponent animationComponent = new AnimationComponent();
        public WeaponComponent weaponComponent = new WeaponComponent();
        public CareerComponent careerComponent = new CareerComponent();
        public HeroSkillComponent heroSkillComponent = new HeroSkillComponent();

        protected override void OnConstruct()
        {
            base.OnConstruct();
            heroLoader = HeroLoader.Create(false);

            animationComponent.actor = this;
            animationComponent.Construct();

            weaponComponent.actor = this;
            weaponComponent.Construct();

            careerComponent.actor = this;
            careerComponent.Construct();

            heroSkillComponent.actor = this;
            heroSkillComponent.Construct();            
        }        

        public override void LoadModel(Action<SceneActor> LoadOver)
        {
            base.LoadModel(LoadOver);

            //OnSetName();
            //OnSetParent();
            heroLoader.LoadHero(battleUnit.UnitInfoId, (uint)battleUnit.WeaponId, ELayerMask.Partner, Sys_Fashion.Instance.GetDressData(battleUnit.FashionInfo, battleUnit.UnitInfoId), (modelGameObject) =>
            {
                SetModelGameObject(modelGameObject);
            });
        }

        protected override void OnDispose()
        {
            heroSkillComponent.Dispose();
            weaponComponent.Dispose();
            careerComponent.Dispose();
            animationComponent.Dispose();
            
            heroLoader?.Dispose();
            heroLoader = null;

            base.OnDispose();
        }

        //protected override void OnSetName()
        //{
        //    if (battleUnit != null)
        //        gameObject.name = $"FightHero_{battleUnit.Pos.ToString()}";
        //    else
        //        DebugUtil.LogError("battleUnit is null");
        //}

        public override void SetLayer(Transform transform)
        {
            transform.Setlayer(ELayerMask.Partner);
            cacheELayerMask = ELayerMask.Partner;
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
