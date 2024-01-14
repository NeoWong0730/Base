using DG.Tweening;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 姿态角变换 用于相机位置计算
    /// </summary>    
    [DisallowMultipleComponent]    
    public class AttitudeAngleTransform : MonoBehaviour
    {
        public bool autoFollowTarget;
        public Transform target;
        public Vector3 fixedLookPoint;

        /// <summary>观察点</summary>
        private Vector3 _lookPoint = Vector3.zero;
        public Vector3 lookPoint
        {
            get
            {
                return _lookPoint;
            }

            private set
            {
                if (!_lookPoint.Equals(value))
                {
                    _lookPoint = value;
                    bDirty = true;
                }
            }
        }

        /// <summary>观察点偏移</summary>
        private Vector3 _lookPointOffset = Vector3.zero;
        public Vector3 lookPointOffset
        {
            get
            {
                return _lookPointOffset;
            }

            set
            {
                if (!_lookPointOffset.Equals(value))
                {
                    _lookPointOffset = value;
                    bDirty = true;
                }
            }
        }

        /// <summary>当前距离</summary>
        private float _distance;
        public float distance
        {
            get
            {
                return _distance;
            }

            set
            {
                if (!_distance.Equals(value))
                {
                    _distance = value;
                    bDirty = true;
                }
            }
        }

        /// <summary>俯仰x</summary>
        private float _pith;
        public float pith
        {
            get
            {
                return _pith;
            }

            set
            {
                if (!_pith.Equals(value))
                {
                    _pith = ClampAngle(value, 10, 80);
                    bDirty = true;
                }
            }
        }

        /// <summary>偏航y</summary>
        private float _yaw;
        public float yaw
        {
            get
            {
                return _yaw;
            }

            set
            {
                if (!_yaw.Equals(value))
                {
                    _yaw = value;
                    bDirty = true;
                }
            }
        }

        /// <summary>翻滚z</summary>
        private float _roll;
        public float roll
        {
            get
            {
                return _roll;
            }
            set
            {
                if (!_roll.Equals(value))
                {
                    _roll = value;
                    bDirty = true;
                }
            }
        }

        private float _fov = 10.0f;
        public float fov
        {
            get
            {
                return _fov;
            }
            set
            {
                if (!_fov.Equals(value))
                {
                    _fov = value;
                    bDirty = true;
                }
            }
        }

        private bool bDirty;

        private float _clipFar = 80.0f;

        public float clipFar
        {
            get
            {
                return _clipFar;
            }
            set
            {
                if (!_clipFar.Equals(value))
                {
                    _clipFar = value;
                    bDirty = true;
                }
            }
        }

        public Vector3 eulerAngles
        {
            get
            {
                return new Vector3(_pith, _yaw, _roll);
            }
        }

        private Camera _targetCamera;
        private Transform _targetCameraTransform;

        public Camera TargetCamera
        {
            get
            {
                return _targetCamera;
            }
        }

        private void Awake()
        {
            _targetCamera = GetComponent<Camera>();
            _targetCameraTransform = _targetCamera.transform;
            bDirty = true;
        }

        public void Recalculation()
        {
            if (!bDirty || _targetCamera == null)
            {
                return;
            }
            _targetCameraTransform.transform.eulerAngles = new Vector3(_pith, _yaw, _roll);
            Vector3 pos = _lookPoint + _lookPointOffset - _targetCameraTransform.forward * _distance;
            _targetCameraTransform.position = pos;
            _targetCamera.fieldOfView = _fov;
            _targetCamera.farClipPlane = _clipFar;

            bDirty = false;
        }

        public void ForceRecalculation(bool immediately = false)
        {            
            bDirty = true;
            if(immediately)
            {
                Recalculation();
            }
        }

        private float ClampAngle(float angle,float min,float max)
        {
            if (angle<-360)
            {
                angle += 360;
            }
            if (angle>360)
            {
                angle -= 360;
            }
            return Mathf.Clamp(angle, min, max);
        }

        #region CameraShake
        private Tweener shakeTweener;
        private Vector3 shakeOffset;
        private Vector3 GetShakeOffset()
        {
            return shakeOffset;
        }

        private void SetShakeOffset(Vector3 vector)
        {
            shakeOffset = vector;
        }

        public void DoShake(float duration, Vector3 strength, int vibrato = 10, float randomness = 90, bool fadeOut = true)
        {
            if (shakeTweener != null)
            {
                shakeTweener.Kill(true);
            }

            shakeTweener = null;
            if (duration == 0 || Vector3.Equals(strength, Vector3.zero))
                return;

            shakeTweener = DOTween.Shake(GetShakeOffset, SetShakeOffset, duration, strength, vibrato, randomness, fadeOut);
        }
        public void StopShark() {
            if (shakeTweener != null) {
                shakeTweener.Kill(true);
            }
            shakeTweener = null;
        }
        #endregion      

        public void SetLookPoint(Vector3 pos)
        {
            lookPoint = pos;
        }

        private void LateUpdate()
        {
            if (autoFollowTarget && target)
            {
                lookPoint = target.position + shakeOffset;
            }
            else
            {
                lookPoint = fixedLookPoint + shakeOffset;
            }
            Recalculation();
        }
    }
}