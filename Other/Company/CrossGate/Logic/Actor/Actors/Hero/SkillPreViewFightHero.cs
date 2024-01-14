using Lib.Core;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 技能预览角色///
    /// </summary>
    public class SkillPreViewFightHero : FightHero
    {
        //protected override void OnSetName()
        //{
        //    gameObject.name = "SkillPreViewFightHero";
        //}
        
        public override void SetLayer(Transform transform)
        {
            transform.Setlayer(ELayerMask.ModelShow);
            cacheELayerMask = ELayerMask.ModelShow;
        }

        //protected override void OnSetParent()
        //{
        //    SetParent(GameCenter.SkillPreViewWorld.RootTransform);
        //}
    }
}
