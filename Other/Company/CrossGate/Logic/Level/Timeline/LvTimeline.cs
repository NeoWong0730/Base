using Lib.AssetLoader;
using Logic.Core;
using UnityEngine;
using Framework;
using Table;
using System;

namespace Logic
{
    public class LvTimeline : LevelBase
    {        
        public override void OnEnter(LevelParams param, Type fromLevelType)
        {
            base.OnEnter(param, fromLevelType);
        }
        public override void OnLoaded()
        {
            base.OnLoaded();
        }

        public override void OnExit(Type toLevelType)
        {
            base.OnExit(toLevelType);
        }
    }
}
