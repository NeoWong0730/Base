using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class CameraControlSystem : LvPlaySystemBase
    {
        public override void OnCreate()
        {
            mData.mCameraData.InputLook = mData.mInputDatas.Player.Look;
            mData.mCameraData.InputScale = mData.mInputDatas.Player.Scale;
        }

        public override void OnDestroy()
        {         
            mData.mCameraData.InputLook = null;
            mData.mCameraData.InputScale = null;
        }

        public override void OnUpdate()
        {
            Transform mainActorTransform = mData.mMainCharacter.mTransform;
            if (mData.mMainCharacter != null)
            {
                mainActorTransform = mData.mMainCharacter.mTransform;
            }
            if (mainActorTransform)
            {
                mData.mCameraData.mLookAtAim.SetPositionAndRotation(mainActorTransform.position, mainActorTransform.rotation);
            }
        }
    }
}
