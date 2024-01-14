using UnityEngine.UI;

namespace Framework
{
    public static class LogicStaticMethodDispatcher
    {
        private static IStaticMethod _AudioUtil_PlayAudioMethod;
        private static IStaticMethod _TextHelper_SetTextByLanguageIDMethod;
        private static bool _IsValid;

        public static void Init()
        {
            _TextHelper_SetTextByLanguageIDMethod = AssemblyManager.Instance.CreateStaticMethod("Logic.TextHelper", "SetTextByLanguageID", 2);
            _AudioUtil_PlayAudioMethod = AssemblyManager.Instance.CreateStaticMethod("AudioUtil", "PlayAudio", 1);
            _IsValid = true;
        }

        public static void UnInit()
        {
            _IsValid = false;
            _TextHelper_SetTextByLanguageIDMethod = null;
            _AudioUtil_PlayAudioMethod = null;
        }

        internal static AudioEntry AudioUtil_PlayAudio(uint audioId)
        {
            if (!_IsValid)
                return null;

            AudioEntry audioEntry = null;
#if ILRUNTIME_MODE
            ILStaticMethod _AudioUtil_PlayAudioMethod_IL = _AudioUtil_PlayAudioMethod as ILStaticMethod;
            using (var ctx = _AudioUtil_PlayAudioMethod_IL.appDomain.BeginInvoke(_AudioUtil_PlayAudioMethod_IL.method))
            {
                ctx.PushInteger(audioId);
                ctx.Invoke();
                audioEntry = ctx.ReadObject<AudioEntry>();
            }
#else
            audioEntry = _AudioUtil_PlayAudioMethod.Run(audioId) as AudioEntry;
#endif
            return audioEntry;
        }

        internal static void TextHelper_SetText(Text text, uint languageID)
        {
#if ILRUNTIME_MODE
            ILStaticMethod _TextHelper_SetText_IL = _TextHelper_SetTextByLanguageIDMethod as ILStaticMethod;
            using (var ctx = _TextHelper_SetText_IL.appDomain.BeginInvoke(_TextHelper_SetText_IL.method))
            {
                ctx.PushObject(text);
                ctx.PushInteger(languageID);
                ctx.Invoke();
            }
#else
            _TextHelper_SetTextByLanguageIDMethod.Run(text, languageID);
#endif
        }
    }
}