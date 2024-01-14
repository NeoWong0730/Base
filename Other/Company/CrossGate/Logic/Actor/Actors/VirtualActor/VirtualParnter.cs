using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public class VirtualParnter : VirtualSceneActor
    {        
        public CSVPartner.Data cSVPartnerData
        {
            get;
            set;
        }

        protected override void OnDispose()
        {
            cSVPartnerData = null;

            base.OnDispose();
        }

        //protected override void OnSetName()
        //{
        //    gameObject.name = $"TaskVirtualParnter_{uID.ToString()}";
        //}

        protected override void LoadDepencyAssets()
        {
            assetsGroupLoader = new AssetsGroupLoader();
            //assetsGroupLoader = Logic.Core.ObjectPool<AssetsGroupLoader>.Fetch(typeof(AssetsGroupLoader));

            List<string> animationPaths;
            AnimationComponent.GetAnimationPaths(cSVPartnerData.id, Constants.UMARMEDID, out animationPaths, null);
            if (animationPaths != null)
            {
                for (int index = 0, len = animationPaths.Count; index < len; index++)
                {
                    assetsGroupLoader.AddLoadTask(animationPaths[index]);
                }
            }
            else
            {
                DebugUtil.LogError($"animationPaths is null cSVPartnerData.id: {cSVPartnerData.id}");
            }
        }

        public override uint ID
        {
            get
            {
                return cSVPartnerData.id;
            }
        }
    }  
}
