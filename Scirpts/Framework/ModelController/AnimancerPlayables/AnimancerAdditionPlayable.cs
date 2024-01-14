using Animancer;
using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public interface IDisposableAnimationJob : IAnimationJob, IDisposable { }

public abstract class AnimancerAdditionPlayable : MonoBehaviour
{
    protected AnimancerComponent _Animancer;
    protected AnimationScriptPlayable _AnimationScriptPlayable;
    protected IDisposableAnimationJob _DisposableAnimationJob;

    public virtual void Play(AnimancerComponent animancer, ModelPart modelPart, Skeleton skeleton) { }
    public virtual void Stop() 
    {
        if (_AnimationScriptPlayable.IsNull())
            return;

        _AnimationScriptPlayable.SetDone(true);

        Playable input = _AnimationScriptPlayable.GetInput(0);
        Playable output = _AnimationScriptPlayable.GetOutput(0);

        _AnimationScriptPlayable.DisconnectInput(0);
        if (output.IsNull())
        {
            _Animancer.Playable.Graph.GetOutput(0).SetSourcePlayable(input);
        }
        else
        {
            output.DisconnectInput(0);
            output.ConnectInput(0, input, 0);
        }

        _DisposableAnimationJob.Dispose();

        _Animancer.Playable.Graph.DestroyPlayable(_AnimationScriptPlayable);
    }
}
