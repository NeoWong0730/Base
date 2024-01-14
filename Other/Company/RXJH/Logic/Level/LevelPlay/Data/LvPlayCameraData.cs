using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

namespace Logic
{
    public class LvPlayCameraData : AxisState.IInputAxisProvider
    {
        public Transform mLookAtAim;
        private InputAction mInputLook;
        private InputAction mInputScale;

        public InputAction InputLook { get { return mInputLook; } set { mInputLook = value; } }
        public InputAction InputScale { get { return mInputScale; } set { mInputScale = value; } }

        private CinemachineFreeLook mCMFreeLook;
        public CinemachineFreeLook CMFreeLook
        {
            get
            {
                return mCMFreeLook; 
            }
            set
            {
                if (mCMFreeLook)
                {
                    mCMFreeLook.m_XAxis.SetInputAxisProvider(0, null);
                    mCMFreeLook.m_YAxis.SetInputAxisProvider(1, null);
                    mCMFreeLook.Follow = null;
                    mCMFreeLook.LookAt = null;
                }

                mCMFreeLook = value;

                if (mCMFreeLook)
                {
                    mCMFreeLook.m_XAxis.SetInputAxisProvider(0, this);
                    mCMFreeLook.m_YAxis.SetInputAxisProvider(1, this);
                    mCMFreeLook.Follow = mLookAtAim;
                    mCMFreeLook.LookAt = mLookAtAim;
                }                
            }
        }

        public Camera mainCamera
        {
            get => Camera.main;
        }

        public GameObject mainCameraObj
        {
            get => mainCamera.gameObject;
        }

        public Transform mainCameraTran
        {
            get => mainCamera.transform;
        }

        public void SetFollow(Transform follow)
        {
            mCMFreeLook.Follow = follow;
        }

        public void SetLookAt(Transform lookAt)
        {
            mCMFreeLook.LookAt = lookAt;
        }

        public float GetAxisValue(int axis)
        {
            switch (axis)
            {
                case 0: return mInputLook != null && mInputLook.WasPerformedThisFrame() ? mInputLook.ReadValue<Vector2>().x : 0;
                case 1: return mInputLook != null && mInputLook.WasPerformedThisFrame() ? mInputLook.ReadValue<Vector2>().y : 0;
                case 2: return mInputScale != null && mInputScale.WasPerformedThisFrame() ? mInputScale.ReadValue<float>() : 0;
            }
            return 0;
        }
    }
}
