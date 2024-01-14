using System.Collections;
using UnityEngine;

namespace Logic
{
    public class PlayerMoveToTargetPosition : PlayerMoveTo
    {
        public Vector3 targetPosition;

        public override string OnDrawNodeText()
        {
            NodeData.Comment = "玩家移动到指定位置";
            return base.OnDrawNodeText();
        }

        protected override void SetTargetPosition()
        {
            _targetPosition = targetPosition;
        }
    }
}
