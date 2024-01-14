using UnityEngine;

namespace Lib.Core
{
    public class EffectLookAt : MonoBehaviour
    {
        public Vector3 targetPos
        {
            get;
            set;
        }

        public Vector3 offset
        {
            get;
            set;
        }       

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(targetPos);
            transform.localEulerAngles += offset;
        }
    }
}
