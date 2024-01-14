using System;
using Cysharp.Threading.Tasks;
using NWFramework;
using YooAsset;
using ProcedureOwner = NWFramework.IFsm<NWFramework.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程 => 下载资源
    /// </summary>
    public class ProcedureDownloadFile : ProcedureBase
    {
        public override bool UseNativeDialog { get; }

        private ProcedureOwner _procedureOwner;

        private float _lastUpdateDownloadedSize;
        private float CurrentSpeed
        {
            get
            {
                float interval = GameTime.deltaTime;
                var sizeDiff = GameModule.Resource.Downloader.CurrentDownloadBytes - _lastUpdateDownloadedSize;
                _lastUpdateDownloadedSize = GameModule.Resource.Downloader.CurrentDownloadBytes;
                var speed = (float)Math.Floor(sizeDiff / interval);
                return speed;
            }
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            _procedureOwner = procedureOwner;

            Log.Info("开始下载更新文件！");

            UILoadMgr.Show(UIDefine.UILoadUpdate, $"开始下载更新文件...");

            BeginDownload().Forget();
        }

        private async UniTaskVoid BeginDownload()
        {
            var downloader = GameModule.Resource.Downloader;

            //注册下载回调
            downloader.OnDownloadErrorCallback = OnDownloadErrorCallback;
            downloader.OnDownloadProgressCallback = OnDownloadProgressCallback;
            downloader.BeginDownload();
            await downloader;

            // 检测下载结果
            if (downloader.Status != EOperationStatus.Succeed)
                return;

            ChangeState<ProcedureDownloadOver>(_procedureOwner);
        }

        private void OnDownloadErrorCallback(string fileName, string error)
        {
            UILoadTip.ShowMessageBox($"Failed to download file : {fileName}", MessageShowType.TwoButton,
                LoadStyle.StyleEnum.Style_Default
                , () => { ChangeState<ProcedureCreateDownloader>(_procedureOwner); }, UnityEngine.Application.Quit);
        }

        private void OnDownloadProgressCallback(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            string currentSizeMb = (currentDownloadBytes / 1048576f).ToString("f1");
            string totalSizeMb = (totalDownloadBytes / 1048576f).ToString("f1");

            string descriptionText = Utility.Text.Format("正在更新，已更新{0}，总更新{1}，已更新大小{2}，总更新大小{3}，更新进度{4}，当前网速{5}/s",
                currentDownloadCount.ToString(),
                totalDownloadCount.ToString(),
                Utility.File.GetByteLengthString(currentDownloadBytes),
                Utility.File.GetByteLengthString(totalDownloadBytes),
                GameModule.Resource.Downloader.Progress,
                Utility.File.GetLengthString((int)CurrentSpeed));

            LoadUpdateLogic.Instance.DownProgressAction?.Invoke(GameModule.Resource.Downloader.Progress);
            UILoadMgr.Show(UIDefine.UILoadUpdate, descriptionText);

            int needTime = 0;
            if (CurrentSpeed > 0)
            {
                needTime = (int)((totalDownloadBytes - currentDownloadBytes) / CurrentSpeed);
            }

            TimeSpan ts = new TimeSpan(0, 0, needTime);
            string timeStr = ts.ToString(@"mm\:ss");
            string updateProgress = Utility.Text.Format("剩余时间 {0}({1}/s)", timeStr, Utility.File.GetLengthString((int)CurrentSpeed));
            Log.Info(updateProgress);
        }
    }
}