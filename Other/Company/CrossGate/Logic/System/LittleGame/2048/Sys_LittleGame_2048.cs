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


namespace Logic
{

    public enum GameState
    {
        None,
        Playing,
        Pause,
        End
    }
    public enum TouchDir
    {
        None,
        Left,
        Right,
        Top,
        Bottom,
    }

    public class Sys_LittleGame_2048 : SystemModuleBase<Sys_LittleGame_2048>
    {
        private Timer timer;

        public Num[][] numComponentArray = new Num[4][];

        public bool success = false;
        public override void Init()
        {
            numComponentArray[0] = new Num[] { null, null, null, null };
            numComponentArray[1] = new Num[] { null, null, null, null };
            numComponentArray[2] = new Num[] { null, null, null, null };
            numComponentArray[3] = new Num[] { null, null, null, null };
        }


        public void PlayAgain()
        {
            Clear();
            timer = Timer.Register(0.6f, () =>Sys_LittleGame.Instance.eventEmitter.Trigger(Sys_LittleGame.EEvents.StartGame));
        }

        public void Clear()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Num num = numComponentArray[i][j];
                    if (num != null)
                    {
                        num.Dispose();
                        numComponentArray[i][j] = null;
                    }
                }
            }
        }
       

        public void Log()
        {
            for (int i = 0; i < 4; i++)
            {
                Debug.Log((numComponentArray[0][i] != null) + " " + (numComponentArray[1][i] != null) + " " + (numComponentArray[2][i] != null) + " " + (numComponentArray[3][i] != null));
            }
        }
    }
}

