using NWFramework;
using Unity.VisualScripting;
using YooAsset;
using ProcedureOwner = NWFramework.IFsm<NWFramework.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// ���� => ������
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        public override bool UseNativeDialog => true;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            //�ȸ���UI��ʼ��
            UILoadMgr.Initialize();

            //�������ã����õ�ǰʹ�õ����ԣ���������ã���Ĭ��ʹ�ò���ϵͳ����
            InitLanguageSettings();

            //�������ã������û��������ݣ����ü���ʹ�õ�����ѡ��
            InitSoundSettings();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            //����һ֡���л��� Splash չʾ����
            ChangeState<ProcedureSplash>(procedureOwner);
        }

        private void InitLanguageSettings()
        {
            if (GameModule.Resource.PlayMode == EPlayMode.EditorSimulateMode && GameModule.Base.EditorLanguage == Language.Unspecified)
            {
                //�༭����Դģʽֱ��ʹ�� Inspector �����õ�����
                return;
            }

            Language language = GameModule.Localization.Language;
            if (GameModule.Setting.HasSetting(Constant.Setting.Language))
            {
                try
                {
                    string languageString = GameModule.Setting.GetString(Constant.Setting.Language);
                    language = (Language)System.Enum.Parse(typeof(Language), languageString);
                }
                catch (System.Exception exception)
                {
                    Log.Error("Init language error, reason {0}", exception.ToString());
                }
            }

            if (language != Language.English
                && language != Language.ChineseSimplified
                && language != Language.ChineseTraditional)
            {
                //�����ݲ�֧�ֵ����ԣ���ʹ��Ӣ��
                language = Language.English;

                GameModule.Setting.SetString(Constant.Setting.Language, language.ToString());
                GameModule.Setting.Save();
            }

            GameModule.Localization.Language = language;
            Log.Info("Init language settings complete, current language is '{0}'.", language.ToString());
        }

        private void InitSoundSettings()
        {
            GameModule.Audio.MusicEnable = !GameModule.Setting.GetBool(Constant.Setting.MusicMuted, false);
            GameModule.Audio.MusicVolume = GameModule.Setting.GetFloat(Constant.Setting.MusicVolume, 1f);
            GameModule.Audio.SoundEnable = !GameModule.Setting.GetBool(Constant.Setting.SoundMuted, false);
            GameModule.Audio.SoundVolume = GameModule.Setting.GetFloat(Constant.Setting.SoundVolume, 1f);
            GameModule.Audio.UISoundEnable = !GameModule.Setting.GetBool(Constant.Setting.UISoundMuted, false);
            GameModule.Audio.UISoundVolume = GameModule.Setting.GetFloat(Constant.Setting.UISoundVolume, 1f);
            Log.Info("Init sound settings complete.");
        }
    }
}