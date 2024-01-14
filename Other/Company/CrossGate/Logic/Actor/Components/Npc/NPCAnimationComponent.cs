using System.Collections.Generic;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine;

namespace Logic
{
    //public class NPCAnimationComponent : Logic.Core.Component
    //{
    //    public AnimationComponent AnimationComponent
    //    {
    //        get;
    //        private set;
    //    }

    //    public CSVNpc.Data CSVNpcData
    //    {
    //        get;
    //        set;
    //    }

    //    bool CDFlag = true;
    //    Timer cdTimer;

    //    protected override void OnConstruct()
    //    {
    //        base.OnConstruct();

    //        AnimationComponent = World.GetComponent<AnimationComponent>(actor);

    //        CDFlag = true;
    //    }

    //    protected override void OnDispose()
    //    {
    //        AnimationComponent = null;
    //        CDFlag = true;
    //        cdTimer?.Cancel();
    //        cdTimer = null;
    //        CSVNpc.Data = null;

    //        base.OnDispose();
    //    }

    //    public void ActiveAction(bool force = false)
    //    {           
    //        if (CDFlag || force)
    //        {
    //            CDFlag = false;

    //            List<string> actions = CSVNpcData.PerformAction;
    //            float animLength = CSVNpcData.PerformCooling / 1000f;
    //            if (actions != null && actions.Count > 0)
    //            {
    //                int index = Random.Range(0, actions.Count);
    //                animLength += AnimationComponent.GetLength(actions[index]);
    //                AnimationComponent?.Play(actions[index], () =>
    //                {
    //                    AnimationComponent?.Play("action_idle");
    //                });                   
    //            }

    //            cdTimer?.Cancel();
    //            cdTimer = Timer.Register(animLength, () =>
    //            {
    //                CDFlag = true;
    //            }, null, false, true);
    //        }
    //    }    
    //}
}
