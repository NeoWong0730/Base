using Logic.Core;
using Lib.Core;
using UnityEngine;
using Table;
using System.Collections.Generic;
using Packet;

namespace Logic
{
    /// <summary>
    /// 怪物的一部分///
    /// </summary>
    public class MonsterPart : Actor, ISceneActor, IClick, IDoubleClick, ILongPress
    {
        public SceneActorWrap sceneActorWrap
        {
            get;
            set;
        }

        public CSVMonster.Data cSVMonsterData
        {
            get;
            set;
        }

        public BattleUnit battleUnit
        {
            get;
            set;
        }

        public GameObject gameObject
        {
            get;
            set;
        }

        public ulong UID
        {
            get
            {
                return uID;
            }
        }

        public ClickComponent clickComponent = new ClickComponent();
        public DoubleClickComponent doubleClickComponent = new DoubleClickComponent();
        public LongPressComponent longpressComponent = new LongPressComponent();

        protected override void OnConstruct()
        {
            base.OnConstruct();

            clickComponent.actor = this;
            clickComponent.Construct();

            doubleClickComponent.actor = this;
            doubleClickComponent.Construct();

            longpressComponent.actor = this;
            longpressComponent.Construct();

        }

        protected override void OnDispose()
        {
            clickComponent.Dispose();
            doubleClickComponent.Dispose();
            longpressComponent.Dispose();

            sceneActorWrap = null;
            cSVMonsterData = null;
            gameObject = null;                        
            battleUnit = null;

            base.OnDispose();
        }

        public void OnClick()
        {
            clickComponent.OnClick();
        }

        public void OnDoubleClick()
        {
            doubleClickComponent.OnDouleClick();
        }

        public void OnLongPress()
        {
            longpressComponent.OnClick();
        }
    }

    /// <summary>
    /// 战斗场景中怪物///
    /// </summary>
    public class Monster : FightActor, IWeaponComponent
    {
        public AnimationComponent animationComponent = new AnimationComponent();
        public WeaponComponent weaponComponent = new WeaponComponent();
        public MonsterSkillComponent monsterSkillComponent = new MonsterSkillComponent();

        public CSVMonster.Data cSVMonsterData
        {
            get;
            set;
        }

        public uint suitID
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

            monsterSkillComponent.actor = this;
            monsterSkillComponent.Construct();            
        }

        protected override void OnDispose()
        {
            monsterSkillComponent.Dispose();
            animationComponent.Dispose();
            weaponComponent.Dispose();

            cSVMonsterData = null;
            //Sys_Fight.Instance.cacheMonsterGos[cSVMonsterData.id] = gameObject;
            suitID = 0;

            base.OnDispose();
        }

        //protected override void OnSetName()
        //{
        //    if (battleUnit != null)
        //        gameObject.name = $"Monster_{battleUnit.Pos.ToString()}";
        //    else
        //        DebugUtil.LogError("battleUnit is null");
        //}

        public override void SetLayer(Transform transform)
        {
            transform.Setlayer(ELayerMask.Monster);
            cacheELayerMask = ELayerMask.Monster;
        }

        protected override void OnOtherSet()
        {
            base.OnOtherSet();

            float scale = cSVMonsterData.zooming / 10000f;
            transform.localScale = new Vector3(scale, scale, scale);

            PetWearSet(Constants.PETWEAR_EQUIP, suitID, modelTransform);
        }

        protected override void LoadDepencyAssets()
        {
            assetsGroupLoader = new AssetsGroupLoader();
            List<string> animationPaths;
            AnimationComponent.GetAnimationPaths(cSVMonsterData.monster_id, cSVMonsterData.weapon_id, out animationPaths);
            if(animationPaths != null)
            {
                for (int index = 0, len = animationPaths.Count; index < len; index++)
                {
                    assetsGroupLoader.AddLoadTask(animationPaths[index]);
                }
            }
            else
            {
                DebugUtil.LogError($"animationPaths is null cSVMonsterData.id: {cSVMonsterData.id}");
            }
        }

        public WeaponComponent GetWeaponComponent()
        {
            return weaponComponent;
        }
    }
}
