using UnityEngine;

public enum ENetWorkCode
{

}
public enum ETimelineLifeCircle
{
    OnGraphStart,
    OnGraphStop,

    OnGraphPause,
    OnGraphResume,

    OnBehaviourPlay,
    OnBehaviourResume,
    OnBehaviourPause,
    
    SetUIAlpha,
    Fadeinout,
    PrepareFrame,
}

public enum EVideoPlayerCircle
{
    OnGraphStart,
    OnGraphStop,
}