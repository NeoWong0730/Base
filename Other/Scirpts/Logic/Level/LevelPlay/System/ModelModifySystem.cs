using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

namespace Logic
{
    public class ModelModifySystem : LvPlaySystemBase
    {
        public override void OnUpdate()
        {
            foreach (var item in mData.mActorList)
            {
                Excute(item);
            }
        }

        private void Excute(Actor actor)
        {
            if (actor.mFaceModifyData.hasBoneChange)
            {
                SkeletonModelController humanModel = actor.mModelBase as SkeletonModelController;
                ModelPartLoader humanModelPart = humanModel.GetPart((int)EHumanModelPart.Face);
                humanModelPart.mUserData = actor.mFaceModifyData;
                humanModelPart.ApplyUserDataChange();

                actor.mFaceModifyData.hasBoneChange = false;
            }
        }
    }
}
