using UnityEngine;
using UnityEngine.UI;
using NWFramework;

namespace GameMain
{
    [Window(UILayer.UI, fromResources:true, location:"UI/UILoadUpdate", fullScreen:true)]
    public class UILoadUpdate : UIWindow
    {
        private Scrollbar m_scrollbarProgress;
        private Image m_imgBackGround;
        private Text m_textDesc;
        private Button m_btnClear;
        private Text m_textAppid;
        private Text m_textResid;

        public override void ScriptGenerator()
        {
            m_imgBackGround = FindChildComponent<Image>("m_imgBackGround");
            m_textDesc = FindChildComponent<Text>("m_textDesc");
            m_btnClear = FindChildComponent<Button>("TopNode/m_btnClear");
            m_textAppid = FindChildComponent<Text>("TopNode/m_textAppid");
            m_textResid = FindChildComponent<Text>("TopNode/m_textResid");
            m_scrollbarProgress = FindChildComponent<Scrollbar>("m_scrollbarProgress");
            m_btnClear.onClick.AddListener(OnClickClearBtn);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            LoadUpdateLogic.Instance.DownloadCompleteAction += DownLoad_Complete_Action;
            LoadUpdateLogic.Instance.DownProgressAction += DownLoad_Progress_Action;
            LoadUpdateLogic.Instance.UnpackedCompleteAction += Unpacked_Complete_Action;
            LoadUpdateLogic.Instance.UnpackedProgressAction += Unpacked_Progress_Action;
            m_btnClear.gameObject.SetActive(true);
        }

        public override void RegisterEvent()
        {
            base.RegisterEvent();
            AddUIEvent(RuntimeId.ToRuntimeId("RefreshVersion"), RefreshVersion);
        }

        public override void OnRefresh()
        {
            base.OnRefresh();
        }

        private void OnClickClearBtn()
        {
            OnStop(null);
            UILoadTip.ShowMessageBox(LoadText.Instance.Label_Clear_Comfirm, MessageShowType.TwoButton,
                LoadStyle.StyleEnum.Style_Clear,
                () =>
                {
                    GameModule.Resource.ClearSandBox();
                    Application.Quit();
                }, () => { OnContinue(null); });
        }

        private void RefreshVersion()
        {
            m_textAppid.text = string.Format(LoadText.Instance.Label_App_id, Version.GameVersion);
            m_textResid.text = string.Format(LoadText.Instance.Label_Res_id, GameModule.Resource.GetPackageVersion());
        }

        public virtual void OnContinue(GameObject obj)
        {
        }

        public virtual void OnStop(GameObject obj)
        {
        }

        protected virtual void DownLoad_Complete_Action(int type)
        {
            Log.Info("DownLoad_Complete");
        }

        protected virtual void DownLoad_Progress_Action(float progress)
        {
            m_scrollbarProgress.gameObject.SetActive(true);

            m_scrollbarProgress.size = progress;
        }

        protected virtual void Unpacked_Complete_Action(bool type)
        {
            m_scrollbarProgress.gameObject.SetActive(true);
            m_textDesc.text = LoadText.Instance.Label_Load_UnpackComplete;
        }

        protected virtual void Unpacked_Progress_Action(float progress)
        {
            m_scrollbarProgress.gameObject.SetActive(true);
            m_textDesc.text = LoadText.Instance.Label_Load_Unpacking;

            m_scrollbarProgress.size = progress;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnStop(null);
            LoadUpdateLogic.Instance.DownloadCompleteAction -= DownLoad_Complete_Action;
            LoadUpdateLogic.Instance.DownProgressAction -= DownLoad_Progress_Action;
        }
    }
}