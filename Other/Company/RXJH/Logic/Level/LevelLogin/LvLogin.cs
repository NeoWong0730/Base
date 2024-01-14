using Framework;
using Logic.Core;
using System;

namespace Logic
{
    public class LvLogin : LevelBase
    {        
        //private float TimeMax = 1.5f;
        //private float m_TimeCounter = 0f;

        public override void OnEnter(LevelParams param, Type fromLevelType)
        {
            //base.OnEnter(param, fromLevelType);

            //UIManager.PreloadUI(EUIID.UI_Login);

            // VideoManager.Play(Lib.AssetLoader.AssetPath.GetVideoFullUrl("Config/Video/cutscene_login_01.mp4"), true);

            //使用最低帧率
            OptionManager.Instance.SetInt(OptionManager.EOptionID.FrameRate, 0, true);
            //OptionManager.Instance.mQuality.SetOverride((int)EQuality.High);
            //OptionManager.Instance.mPostProcess.SetOverride(true);

            //mLevelPreload.Preload<GameObject>(UIConfig.GetConfData(EUIID.UI_Login)?.prefabPath);
            //mLevelPreload.Preload<GameObject>(UIConfig.GetConfData(EUIID.UI_Server)?.prefabPath);

            //mLevelPreload.PreOpenScene("Empty", UnityEngine.SceneManagement.LoadSceneMode.Single);//(EScene.Login.ToString());

            //mLevelPreload.StartLoad();

            //Sys_Login.Instance.ClearServerList();
            //Sys_Login.Instance.GetServerList();

            //进入登录界面, 关闭socket
            //Sys_Net.Instance.Disconnect();
        }

        public override void OnExit(Type toLevelType)
        {
            base.OnExit(toLevelType);

            OptionManager.Instance.CancelOverride(OptionManager.EOptionID.FrameRate);
            //OptionManager.Instance.mFrameRate.CancelOverride();
            //OptionManager.Instance.mQuality.CancelOverride();
            //OptionManager.Instance.mPostProcess.CancelOverride();

            SceneManager.UnLoadAllScene();

            VideoManager.Stop();
        }

        //public override void OnUpdate(float dt)
        //{
        //    base.OnUpdate(dt);
        //}
        public override float OnLoading()
        {
            //mLevelPreload.OnLoading();
            //m_TimeCounter += Time.deltaTime;
            //float progress = m_TimeCounter / TimeMax - 0.05f;
            //progress = progress < 0f ? 0f : progress;
            //if (progress >= 0.95f && mLevelPreload.CheckLoaded())
            //{
            //    return 1f;
            //}
            //else
            //{
            //    return progress >= 0.95f ? 0.95f : progress;
            //}

            return 1f;
        }

        public override void OnLoaded()
        {
            UIManager.OpenUI(EUIID.UI_Login, true);

            //UIManager.PreloadUI(EUIID.UI_Loading);//先预加载 loadingUI 直到进lvplay后删除

            //play bgm
            //AudioUtil.PlayAudio(3016);
        }
    }
}
