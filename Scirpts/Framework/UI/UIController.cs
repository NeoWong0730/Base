using UnityEngine;

namespace Framework
{
    public class UIController : MonoBehaviour
    {
        public uint openId;
        public uint closeId;

        private AudioEntry audioEntry_open;
        private AudioEntry audioEntry_close;

        public void PlayOpen()
        {
            audioEntry_open = LogicStaticMethodDispatcher.AudioUtil_PlayAudio(0);
        }

        public void PlayClose()
        {
            audioEntry_close = LogicStaticMethodDispatcher.AudioUtil_PlayAudio(0);
        }
    }
}