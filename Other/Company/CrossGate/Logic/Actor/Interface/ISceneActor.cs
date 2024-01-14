using Logic.Core;
using UnityEngine;

namespace Logic
{
    public interface ISceneActor
    {
        SceneActorWrap sceneActorWrap { get; }

        GameObject gameObject { get; }

        ulong UID { get; }
    }
}
