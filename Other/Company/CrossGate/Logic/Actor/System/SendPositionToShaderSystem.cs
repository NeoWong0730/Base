using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;

namespace Logic
{
    public class SendPositionToShaderSystem : LevelSystemBase
    {
        public override void OnUpdate()
        {
            if (GameCenter.mainHero != null)
            {
                SceneInstanceRender.CollidePosition = GameCenter.mainHero.transform.position;
            }
            else
            {
                SceneInstanceRender.CollidePosition = Vector3.zero;
            }
        }
    }
}