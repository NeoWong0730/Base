using UnityEngine;

namespace Framework
{
    public class ParentTransformBinder : MonoBehaviour
    {
        public Transform parent;
        public float scaleX;
        public float scaleY;
        public float scaleZ;

        void Update()
        {
            if (parent == null)
                return;

            gameObject.transform.localPosition = new Vector3(parent.localPosition.x * scaleX, parent.localPosition.y * scaleY, parent.localPosition.z * scaleZ);
            gameObject.transform.localEulerAngles = parent.localEulerAngles;
        }
    }
}
