using UnityEngine.UI;

namespace Framework
{
    public static class LogicStaticMethodDispatcher
    {
        private static IStaticMethod _AudioUtil_PlayAudioMethod;
        private static IStaticMethod _TextHelper_SetTextByLanguageIDMethod;
        private static bool _IsVaild;

        public static void Init()
        {
            _TextHelper_SetTextByLanguageIDMethod = AssemblyManager.Instance.CreateStaticMethod("Logic.TextHelper", "SetTextByLanguageID", 2);
            _AudioUtil_PlayAudioMethod = AssemblyManager.Instance.CreateStaticMethod("AudioUtil", "PlayAudio", 1);
            _IsVaild = true;
        }

        public static void UnInit()
        {
            _IsVaild = false;
            _TextHelper_SetTextByLanguageIDMethod = null;
            _AudioUtil_PlayAudioMethod = null;
        }

        internal static AudioEntry AudioUtil_PlayAudio(uint audioId)
        {
            if (!_IsVaild)
                return null;

            AudioEntry audioEntry = null;

            audioEntry = _AudioUtil_PlayAudioMethod.Run(audioId) as AudioEntry;
            return audioEntry;
        }

        internal static void TextHelper_SetText(Text text, uint languageID)
        {
            _TextHelper_SetTextByLanguageIDMethod.Run(text, languageID);
        }
    }
}
