using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using UnityEngine;
using Table;
using Lib.AssetLoader;
using System.Collections.Generic;
using Logic;
using System;
using Framework;

namespace Logic
{
    public class Sys_NumInput : SystemModuleBase<Sys_NumInput>
    {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnNumInput,
            OnNumInputEnd,
        }

        /// <summary>
        /// 数字输入框偏移方向
        /// </summary>
        public enum NumInputOffsetDir
        {
            None = 0,
            Left = 1,
            Right = 2,
            Top = 4,
            Bottom = 8,
        }

        /// <summary>
        /// 数字输入框参数(包含位置和偏移方向)
        /// </summary>
        public class NumInputPrama
        {
            public Vector3 Position;
            public NumInputOffsetDir Dir = NumInputOffsetDir.None;
            public NumInputPrama(Vector3 pos, NumInputOffsetDir dir = NumInputOffsetDir.None)
            {
                this.Position = pos;
                this.Dir = dir;
            }
        }

        /// <summary>
        /// 输入最大999,999,999
        /// </summary>
        /// <returns></returns>
        public uint GetInputValueMax()
        {
            return 999999999;
        }
    }
}
