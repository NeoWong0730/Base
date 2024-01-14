using UnityEngine;

namespace Framework
{
    public class CP_TransformAdjuster : MonoBehaviour
    {
        [System.Serializable]
        public struct Vector3Bool
        {
            public bool use;
            public Vector3 v3;
        }

        public Vector3Bool localPosition;
        public Vector3Bool localAngle;
        public Vector3Bool localScale;

        public int frameRate = 1;

        private Vector3 origPosition;
        private Vector3 origAngle;
        private Vector3 origScale;

        private void Awake()
        {
            origPosition = transform.localPosition;
            origAngle = transform.localEulerAngles;
            origScale = transform.localScale;
        }

        private void LateUpdate()
        {
            if (frameRate > 0 && Time.frameCount % frameRate == 0)
            {
                if (localPosition.use)
                {
                    transform.localPosition = localPosition.v3;
                }
                else
                {
                    transform.localPosition = origPosition;
                }

                if (localAngle.use)
                {
                    transform.localEulerAngles = localAngle.v3;
                }
                else
                {
                    transform.localEulerAngles = origAngle;
                }

                if (localScale.use)
                {
                    transform.localScale = localScale.v3;
                }
                else
                {
                    transform.localScale = origScale;
                }
            }
        }
    }
}