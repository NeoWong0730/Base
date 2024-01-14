using System.Collections;
using UnityEngine;

namespace Logic
{
    public class PlayerMoveToTargetPosition : PlayerMoveTo
    {
        public Vector3 targetPosition;

        public override string OnDrawNodeText()
        {
            NodeData.Comment = "����ƶ���ָ��λ��";
            return base.OnDrawNodeText();
        }

        protected override void SetTargetPosition()
        {
            _targetPosition = targetPosition;
        }
    }
}
