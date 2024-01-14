using Lib.Core;
using UnityEngine;
using Table;

namespace Logic
{
    /// <summary>
    /// 技能预览怪物///
    /// </summary>
    public class SkillPreViewMonster : Monster
    {
        //protected override void OnSetName()
        //{
        //    gameObject.name = "SkillPreViewMonster";
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
