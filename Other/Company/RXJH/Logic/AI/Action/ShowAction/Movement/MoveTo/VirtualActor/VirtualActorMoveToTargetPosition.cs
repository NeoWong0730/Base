using UnityEngine;

namespace Logic
{
    public class VirtualActorMoveToTargetPosition : VirtualActorMoveTo
    {
        public Vector3 targetPosition;

        public override string OnDrawNodeText()
        {
            NodeData.Comment = "�����ɫ�ƶ���ָ��λ��";
            return base.OnDrawNodeText();
        }

        protected override void SetTargetPosition()
        {
            _targetPosition = targetPosition;
        }
    }
}