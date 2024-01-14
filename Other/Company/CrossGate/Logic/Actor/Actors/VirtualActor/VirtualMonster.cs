using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public class VirtualMonster : VirtualSceneActor
    {        
        public CSVMonster.Data cSVMonsterData
        {
            get;
            set;
        }

        protected override void OnDispose()
        {
            cSVMonsterData = null;

            base.OnDispose();
        }

        //protected override void OnSetName()
        //{
        //    gameObject.name = $"TaskVirtualMonster_{uID.ToString()}";
        //}

        protected override void LoadDepencyAssets()
        {
            assetsGroupLoader = new AssetsGroupLoader();
            List<string> animationPaths;
            AnimationComponent.GetAnimationPaths(cSVMonsterData.id, Constants.UMARMEDID, out animationPaths, null);
            if (animationPaths != null)
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

        public override uint ID
        {
            get
            {
                return cSVMonsterData.id;
            }
        }
    }
}