using Framework;
using Lib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Logic
{
    public enum EQuality
    {
        Low = 0,
        Middle = 1,
        High = 2,
        Custom,
    }

    public enum EPostProcessQuality
    {
        Close = 0,
        Low = 1,
        Middle = 2,
        High = 3,
    }

    public class OptionManager : TSingleton<OptionManager>
    {
        public enum EOptionID
        {
            LocalOptionStart = 0,
            Quality = 0,
        }

        public enum EEvents : int
        {
            OptionValueChange,  //玩家设置的值变更
            OptionFinalChange,  //设置的实际最终值变更
        }
    }
}