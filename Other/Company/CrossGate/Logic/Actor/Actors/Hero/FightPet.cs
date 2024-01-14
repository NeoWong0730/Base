using Logic.Core;
using Lib.Core;
using UnityEngine;
using Table;
using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 战斗场景中宠物///
    /// </summary>
    public class FightPet : FightActor, IWeaponComponent
    {
        public AnimationComponent animationComponent = new AnimationComponent();
        public WeaponComponent weaponComponent = new WeaponComponent();
        public FightPetSkillComponent fightPetSkillComponent = new FightPetSkillComponent();

        public CSVPetNew.Data cSVPetData
        {
            get;
            set;
        }

        public uint IsFullPet
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

        protected override void OnConstruct()
        {
            base.OnConstruct();

            weaponComponent.actor = this;
            weaponComponent.Construct();

            animationComponent.actor = this;
            animationComponent.Construct();

            fightPetSkillComponent.actor = this;
            fightPetSkillComponent.Construct();            
        }

        protected override void OnDispose()
        {
            fightPetSkillComponent.Dispose();
            animationComponent.Dispose();
            weaponComponent.Dispose();

            cSVPetData = null;
            IsFullPet = 0u;
            if (IsFullPet == 1)
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
        //    if (battleUnit != null)
        //        gameObject.name = $"FightPet_{battleUnit.Pos.ToString()}";
        //    else
        //        DebugUtil.LogError("battleUnit is null");
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
            AnimationComponent.GetAnimationPaths(cSVPetData.action_id, Constants.UMARMEDID, out animationPaths, Constants.IdleAnimationClipHashSet);
            if (animationPaths != null)
            {
                for (int index = 0, len = animationPaths.Count; index < len; index++)
                {
                    assetsGroupLoader.AddLoadTask(animationPaths[index]);
                }
            }
            else
            {
                DebugUtil.LogError($"animationPaths is null cSVPetData.id: {cSVPetData.id}");
            }
        }

        protected override void OnOtherSet()
        {
            base.OnOtherSet();

            string path = Sys_Pet.Instance.GetPetGearFxPath(cSVPetData, IsFullPet == 1);            if (path != null)                EffectUtil.Instance.LoadEffect(uID, path, transform, EffectUtil.EEffectTag.FullPet);

            if (Build != 0)
            {
                string buildPath = Sys_Pet.Instance.GetPetRemakePerfectFxPath((int)Build);
                EffectUtil.Instance.LoadEffect(uID, buildPath, transform, EffectUtil.EEffectTag.Build);
            }

            if (MagicSoul)
            {
                EffectUtil.Instance.LoadEffect(uID, CSVParam.Instance.GetConfData(1575).str_value, fxRoot.transform, EffectUtil.EEffectTag.MagicSoul);              
            }

            float scale = cSVPetData.zooming / 10000f;
            transform.localScale = new Vector3(scale, scale, scale);

            PetWearSet(Constants.PETWEAR_EQUIP, suitID, modelTransform);
        }

        public void ChangeBuildEffect()
        {
            EffectUtil.Instance.UnloadEffectByTag(uID, EffectUtil.EEffectTag.Build);
            string path = Sys_Pet.Instance.GetPetRemakePerfectFxPath((int)Build);
            EffectUtil.Instance.LoadEffect(uID, path, transform, EffectUtil.EEffectTag.Build);
        }

        public WeaponComponent GetWeaponComponent()
        {
            return weaponComponent;
        }       
    }
}
