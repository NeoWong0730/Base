using System;
using UnityEngine;
using Framework;
using System.Collections.Generic;

namespace Logic
{
    public class Actor
    {
        public uint uid;
        public uint modelConfigID;
        public FaceModifyData mFaceModifyData = new FaceModifyData((int)EFaceModifyType.Count);

        public Transform mTransform;
        public IModel mModelBase;
        public IAnimationPlayable mAnimationPlayable;
        public MovementController mMovementController;

        public Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();

        protected virtual void Init()
        {
            mModelBase = mTransform.GetComponent<SkeletonModelController>();
            mAnimationPlayable = mTransform.GetComponent<SkeletonModelController>();

            mMovementController = mTransform.GetOrAddComponent<MovementController>();
            mMovementController.Init(this);
        }

        public bool HasComponent(Type type)
        {
            return components.ContainsKey(type);
        }
    }

    public interface IComponent
    { }

    public enum EHumanModelPart
    {
        Invalid = -1,
        Cloth = 0,
        Hair = 1,
        Face = 2,        
        Count = 3,
    }
}