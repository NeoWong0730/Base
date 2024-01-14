using Framework;
using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace Logic
{
    public partial class Sys_LittleGame : SystemModuleBase<Sys_LittleGame>, ISystemModuleUpdate
    {
        private bool bstart = false;

        private float timer;
        private Action onOverTime;
        private Action<int> onCountDownTime;


        public bool TimeOut = false;

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            StartGame,
        }


        public void StartGame(float _totletime, Action<int> onCountDownTime = null, Action onOverTime = null)
        {
            this.timer = _totletime;
            this.bstart = true;
            this.onOverTime = onOverTime;
            this.onCountDownTime = onCountDownTime;
            this.TimeOut = false;
        }

        public void EndGame()
        {
            timer = 0;
            onOverTime = null;
            onCountDownTime = null;
            bstart = false;
            TimeOut = false;
            curTime = 0;
            lastTime = 0;
            //stringBuilder.Clear();
        }


        public void OnUpdate()
        {
            if (bstart)
            {
                timer -= GetDeltaTime();
                onCountDownTime?.Invoke((int)timer + 1);
                if (timer <= 0)
                {
                    timer = 0;
                    bstart = false;
                    TimeOut = true;

                    onOverTime?.Invoke();
                }
            }
        }

        private int curTime;
        private int lastTime;
        public string GetTimeFormat()
        {
            if (!bstart)
                return "0:00";

            StringBuilder stringBuilder = StringBuilderPool.GetTemporary();

            curTime = (int)timer + 1;

            int minute = curTime / 60;
            int seconds = curTime % 60;
            string sec = seconds >= 10 ? seconds.ToString() : "0" + seconds.ToString();
            stringBuilder.Append(minute.ToString()).Append(":").Append(sec.ToString());

            if (lastTime == curTime)
            {
                return StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder);
            }

            lastTime = curTime;
          
            
            return StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder);
        }
    }
}


