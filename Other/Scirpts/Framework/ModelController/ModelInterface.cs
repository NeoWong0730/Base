using Animancer;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using static SkeletonModelController;

public interface IModel
{
    void OnLODChange(int from, int to);
}

public interface IMaterialOverride
{
    void SetOverrideMaterial(string materialPath);
    void SetOverrideMaterial(Material material, bool autoDestroy);
}

public interface IAnimationPlayable
{
    void LoadAnimationClips(uint id);
    void PlayAnimation(string stateName, EAnimancerLayer layer = EAnimancerLayer.FullBody);
    void PlayAnimation(AnimationClip clip, EAnimancerLayer layer = EAnimancerLayer.FullBody);
    void StopLayer(EAnimancerLayer layer = EAnimancerLayer.FullBody);
    void PlayAnimatorControllerState(string stateName, float fadeTime = 0.1f);
    void SetBool(int id, bool value);
    void SetBool(string name, bool value);
    void SetFloat(int id, float value);
    void SetFloat(string name, float value);
    float SetFloat(string name, float value, float dampTime, float deltaTime, float maxSpeed = float.PositiveInfinity);
    float SetFloat(int id, float value, float dampTime, float deltaTime, float maxSpeed = float.PositiveInfinity);
    void SetInteger(int id, int value);
    void SetInteger(string name, int value);
    void SetTrigger(int id);
    void SetTrigger(string name);
    void ResetTrigger(int id);
    void ResetTrigger(string name);
    bool LoadOverrideController(string assetPath);
}