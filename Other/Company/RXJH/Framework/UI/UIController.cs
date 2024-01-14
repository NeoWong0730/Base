using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    public class UIController : MonoBehaviour
    {
        public uint openId;
        public uint closeId;

        private AudioEntry audioEntry_open;
        private AudioEntry audioEntry_close;

        public void PlayOpen()
        {
            /*if (this.enabled && this.openId != 0)*/
            {
                this.audioEntry_open = LogicStaticMethodDispatcher.AudioUtil_PlayAudio(12001);
            }
        }
        public void PlayClose()
        {
            /* if (this.enabled && this.closeId != 0)*/
            {
                this.audioEntry_close = LogicStaticMethodDispatcher.AudioUtil_PlayAudio(12002);
            }
        }
    }
}

