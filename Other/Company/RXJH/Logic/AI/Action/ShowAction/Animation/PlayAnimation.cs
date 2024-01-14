using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Logic.Core;

namespace Logic
{
    public abstract class PlayAnimation : Action
    {
        public string stateName;
        public float fadeTime = 0.1f;

        protected Actor _actor;

        public override void OnStart()
        {
            SetActor();
            Play();
        }

        protected abstract void SetActor();

        void Play()
        {
            if (_actor != null)
            {
                _actor.mAnimationPlayable.PlayAnimatorControllerState(stateName, fadeTime);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_actor != null)
            {
                return TaskStatus.Success;
            }
            else
            {
                SetActor();
                Play();
            }

            return TaskStatus.Running;
        }
    }

    public class PlayerPlayAnimation : PlayAnimation
    {
        protected override void SetActor()
        {
            LvPlay lvPlay = LevelManager.mMainLevel as LvPlay;
            if (lvPlay != null)
            {
                _actor = lvPlay.mLvPlayData.mMainActor;
            }
        }
    }

    public class VirtualActorPlayAnimation : PlayAnimation
    {
        public uint virtualActorUID;

        protected override void SetActor()
        {
            LvPlay lvPlay = LevelManager.mMainLevel as LvPlay;
            if (lvPlay != null)
            {
                lvPlay.mLvPlayData.mActors.TryGetValue(virtualActorUID, out _actor);
            }
        }
    }
}