using UnityEngine;

namespace Logic
{
    public class VirtualActorMoveToTargetPosition : VirtualActorMoveTo
    {
        public Vector3 targetPosition;

        public override string OnDrawNodeText()
        {
            NodeData.Comment = "虚拟角色移动到指定位置";
            return base.OnDrawNodeText();
        }

        protected override void SetTargetPosition()
        {
            _targetPosition = targetPosition;
        }
    }
}