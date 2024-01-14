using Framework;
using Lib.AssetLoader;
using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UIHotFix : MonoBehaviour
{
    //public static readonly string sTip_Title = "提示";
    //public Text Tex_Version; //检测版本号 暂时用不到
    //public Text Tex_Res;

    private static GameObject uiHotFix;
    public static void Create()
    {
        if (uiHotFix)
        {
            uiHotFix.gameObject.SetActive(true);
            return;
        }
          
        uiHotFix = GameObject.Instantiate(Resources.Load("UI_HotFix") as GameObject);
        uiHotFix.transform.localPosition = Vector3.zero;
        uiHotFix.transform.localScale = Vector3.one;
        uiHotFix.gameObject.SetActive(true);        
        GameObject.DontDestroyOnLoad(uiHotFix);
    }

    public static void Destroy()
    {
        if(uiHotFix)
        {
            GameObject.DestroyImmediate(uiHotFix);
            uiHotFix = null;
        }        
    }

    public static void Close()
    {
        if (uiHotFix)
        {
            uiHotFix.gameObject.SetActive(false);
        }
    }

    public GameObject TipRoot;//二级弹框
    public Text Tex_Des;//描述 下载尺寸大小  错误描述
    public Button Btn_no;
    public Button Btn_ok;
    public Button Btn_hotfix;
    public Text Tex_no;
    public Text Tex_ok;
    public Text Tex_State;//资源状态（更新检测中）
    public Slider Slider_Jindu;
    public Text Tex_Tips;//仅显示 此过程不消耗流量
    public Text Tex_Progress;//更新进度 %50
    public GameObject ProgressRoot;//只有进度条

    private EAppError eAppError = EAppError.None;
    private EAppState eAppState = EAppState.Invalid;
    //private EHotFixPipeline eHotFixPipeline = EHotFixPipeline.None;
    private EHotFixState eHotFixState = EHotFixState.Invalid;


    private bool bShowTipsView = false;
    private bool bShowStateView = false;

    private bool bNextShowTipsView = false;
    private bool bNextShowStateView = false;

    private HotFixTipWord hotFixTipWord;

    private bool[] arrHotFixRecord= new bool[4] {false,false,false,false};

    //public GameObject hotfixBox;
    void Start()
    {
        hotFixTipWord = HotFixTipWordManager.Instance.hotFixTipWord;

        Btn_ok.onClick.AddListener(OnBtnOk);
        Btn_no.onClick.AddListener(OnBtnNo);
        Btn_hotfix.onClick.AddListener(OnBtnHotFix);
        Tex_State.text = string.Empty;
        Tex_Tips.text = string.Empty;

        Tex_State.gameObject.SetActive(true);

        TipRoot.SetActive(bShowTipsView);
        ProgressRoot.SetActive(bShowStateView);

        Tex_no.text = hotFixTipWord.btn_quit;
    }

    public void OnBtnHotFix()
    {
        //弹框
        UI_Box.Create(UseUIBOxType.HotFixStart);
    }

    public void OnBtnOk()
    {
        if (eAppState == EAppState.HotFix)
        {
            if(eHotFixState == EHotFixState.Wait_FixAsset)
                HotFixStateManager.Instance.SetUserConfirm(EUserConfirm.Yes);
            
            switch (AppManager.eAppError)
            {
                case EAppError.RemoteAssetListError:
                    AppManager.Quit();
                    break;

                case EAppError.NetworkError://断网
                case EAppError.HttpError:
                case EAppError.HotFixAssetListMD5Error://HotFixList文件md5出错
                case EAppError.AssetVerifyError://资产验证出错 (再次比对资源md5，新增)
                    eAppError = AppManager.eAppError = EAppError.None;
                    bShowTipsView = bNextShowTipsView = false;
                    TipRoot.SetActive(bShowTipsView);
                    bShowStateView  = bNextShowStateView = true;
                    ProgressRoot.SetActive(bShowStateView);
                    isWaitSecond = true;
                    HotFixStateManager.Instance.OnExit();
                    HotFixStateManager.Instance.OnEnter();
                    break;
                case EAppError.HotFixSeriesError:
                    AppManager.Quit();
                    //eAppError = AppManager.eAppError = EAppError.None;
                    //bShowTipsView = bNextShowTipsView = false;
                    //TipRoot.SetActive(bShowTipsView);
                    //bShowStateView = bNextShowStateView = true;
                    //ProgressRoot.SetActive(bShowStateView);
                    //isWaitSecond = true;
                    //HotFixStateManager.Instance.HotFixSeriesErrorRetry = true;
                    //HotFixStateManager.Instance.OnExit();
                    //HotFixStateManager.Instance.OnEnter();
                    break;
                default:
                    break;
            }

        }
        else
        {
            switch (AppManager.eAppError)
            {
                case EAppError.RemoteVersionNetError:
                    UIHotFix.Close();
                    AppManager.eAppError = EAppError.None;
                    CoroutineManager.Instance.StartHandler(AppState_CheckVersion.CheckVersion());
                    break;
                case EAppError.RemoteVersionInfoError:
                default:
                    AppManager.Quit();
                    break;
            }
        }
    }

    public void OnBtnNo()
    {
        if (eAppState == EAppState.HotFix)
        {
            if (eHotFixState == EHotFixState.Wait_FixAsset)
                HotFixStateManager.Instance.SetUserConfirm(EUserConfirm.No);

            switch (AppManager.eAppError)
            {
                case EAppError.RemoteAssetCompleteFailure:
                case EAppError.NetworkError:
                case EAppError.HttpError:
                case EAppError.HotFixSeriesError:
                case EAppError.HotFixAssetListMD5Error:
                case EAppError.AssetVerifyError://资产验证出错 (再次比对资源md5，新增)
                case EAppError.MemoryNotEnoughError://内存不足
                    //AppManager.Quit();
                    break;
            }
            AppManager.Quit();
        }
        else
        {
            AppManager.Quit();
        }
    }

    float totalTimes = 0.0f;
    bool isWaitSecond = false;
    void Update()
    {
        if (eAppState != AppManager.eAppState)
        {
            eAppState = AppManager.eAppState;
        }

        switch (eAppState)
        {
            case EAppState.Invalid:
                break;
            case EAppState.Game:
                bNextShowStateView = false;
                break;
            case EAppState.HotFix:
                //bNextShowStateView = true;
                Update_HotFix();
                break;
            case EAppState.CheckVersion:
                bNextShowStateView = false;
                break;
            default:
                break;
        }

        //由于太快看不到进度条，这边做了个优化延迟1s显示
        if (isWaitSecond == true)
        {
            totalTimes += Time.deltaTime;
            if (totalTimes >= 1f)
            {
                totalTimes = 0;
                isWaitSecond = false;
                Update_Error();
            }
        }
        else
        {
            Update_Error();
        }
        
        if (bShowStateView != bNextShowStateView)
        {
            bShowStateView = bNextShowStateView;
            ProgressRoot.SetActive(bShowStateView);
        }

        if (bShowTipsView != bNextShowTipsView)
        {
            bShowTipsView = bNextShowTipsView;
            TipRoot.SetActive(bShowTipsView);
        }
    }

    void Update_Error()
    {
        //bNextShowTipsView = false;
        if (eAppError != AppManager.eAppError)
        {
            eAppError = AppManager.eAppError;
      
            //弹窗
            bNextShowTipsView = true;
            bNextShowStateView = false;

            if (eAppState == EAppState.HotFix)
            {
                for (int i = 0; i < arrHotFixRecord.Length; i++)
                {
                    arrHotFixRecord[i] = false;
                }
                switch (AppManager.eAppError)
                {
                    case EAppError.RemoteAssetCompleteFailure:
                        Btn_ok.gameObject.SetActive(false);
                        Btn_no.gameObject.SetActive(true);
                        Tex_no.text = hotFixTipWord.btn_quit;
                        Tex_Des.text = string.Format(hotFixTipWord.remote_asset_complete_failure);//网络环境异常
                        break;
                    case EAppError.MemoryNotEnoughError:
                        Btn_ok.gameObject.SetActive(false);
                        Btn_no.gameObject.SetActive(true);
                        Tex_no.text = hotFixTipWord.btn_quit;
                        Tex_Des.text = string.Format(hotFixTipWord.local_memory_not_enough);//网络环境异常
                        break;
                    case EAppError.NetworkError:
                        Btn_ok.gameObject.SetActive(true);
                        Btn_no.gameObject.SetActive(false);
                        Tex_ok.text = hotFixTipWord.btn_try;
                        Tex_Des.text = string.Format(hotFixTipWord.network_error, AppManager.ResponseCode);//网络环境异常
                        break;
                    case EAppError.HttpError://Http请求出错(1.请求的资源HotFixList.txt不存在 ErrorCode = 404，2..)
                        Btn_ok.gameObject.SetActive(true);
                        Btn_no.gameObject.SetActive(false);
                        Tex_ok.text = hotFixTipWord.btn_try;
                        Tex_Des.text = string.Format(hotFixTipWord.http_error, AppManager.ResponseCode);
                        break;
                    case EAppError.RemoteAssetListError://服务器资源列表解析错误 --退出游戏
                        Btn_ok.gameObject.SetActive(true);
                        Btn_no.gameObject.SetActive(false);
                        Tex_ok.text = hotFixTipWord.btn_quit;
                        Tex_Des.text = hotFixTipWord.remote_assetlist_error;
                        break;
                    case EAppError.StreamingVersionError://本地大版本填写有误
                        Btn_ok.gameObject.SetActive(true);
                        Btn_no.gameObject.SetActive(false);
                        Tex_Des.text = hotFixTipWord.streaming_maxversion_error;
                        Tex_ok.text = hotFixTipWord.btn_quit;
                        break;
                    case EAppError.AssetVerifyError://资产验证出错 (再次比对资源md5，新增)
                        Btn_ok.gameObject.SetActive(true);
                        Btn_no.gameObject.SetActive(true);
                        Tex_ok.text = hotFixTipWord.btn_try;
                        Tex_no.text = hotFixTipWord.btn_quit;
                        Tex_Des.text = hotFixTipWord.verify_error;
                        break;
                    case EAppError.HotFixAssetListMD5Error:
                        Btn_ok.gameObject.SetActive(true);
                        Btn_no.gameObject.SetActive(true);
                        Tex_ok.text = hotFixTipWord.btn_try;
                        Tex_no.text = hotFixTipWord.btn_quit;
                        Tex_Des.text = hotFixTipWord.remote_hotfixlist_md5_error;
                        break;
                    case EAppError.HotFixSeriesError:
                        Btn_ok.gameObject.SetActive(true);
                        Btn_no.gameObject.SetActive(false);
                        Tex_ok.text = hotFixTipWord.btn_quit;
                        Tex_no.text = hotFixTipWord.btn_quit;
                        Tex_Des.text = hotFixTipWord.verify_hotfixSeries_error;
                        break;
                }
            }
            else
            {
                switch (AppManager.eAppError)
                {
                    case EAppError.RemoteVersionNetError:
                        Btn_ok.gameObject.SetActive(true);
                        Btn_no.gameObject.SetActive(false);
                        Tex_Des.text = hotFixTipWord.remote_version_net_error;
                        Tex_ok.text = hotFixTipWord.btn_try;
                        break;
                    case EAppError.RemoteVersionInfoError://1.远端版本号出错 2.热更地址为空
                        Btn_ok.gameObject.SetActive(true);
                        Btn_no.gameObject.SetActive(false);
                        Tex_ok.text = hotFixTipWord.btn_quit;
                        Tex_Des.text = hotFixTipWord.remote_version_info_error;
                        break;
                    case EAppError.VersionVerifyError://强更
                        Btn_ok.gameObject.SetActive(true);
                        Btn_no.gameObject.SetActive(false);
                        Tex_Des.text = hotFixTipWord.maximal_version;
                        Tex_ok.text = hotFixTipWord.btn_quit;
                        break;
                    case EAppError.HttpError:
                        Btn_ok.gameObject.SetActive(true);
                        Btn_no.gameObject.SetActive(false);
                        Tex_ok.text = hotFixTipWord.btn_quit;
                        Tex_Des.text = string.Format(hotFixTipWord.http_error, AppManager.ResponseCode);
                        break;
                    case EAppError.NetworkError:
                        Btn_ok.gameObject.SetActive(true);
                        Btn_no.gameObject.SetActive(false);
                        Tex_ok.text = hotFixTipWord.btn_quit;
                        Tex_Des.text = string.Format(hotFixTipWord.network_error, AppManager.ResponseCode);//网络环境异常
                        break;
                }
            }
        }
    }
    
    
    void Update_HotFix()
    {
        //更新 热更期间的几个状态

        if (eHotFixState != HotFixStateManager.Instance.CurrentHotFixState)
        {
            eHotFixState = HotFixStateManager.Instance.CurrentHotFixState;
            
            bNextShowTipsView = HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.Wait_FixAsset;
            bNextShowStateView = !bNextShowTipsView;

            if (HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.Destroy_SpareAssets)
            {
                Tex_State.text = hotFixTipWord.destroy_asset;
                Tex_Tips.text = hotFixTipWord.no_need_net;
                Tex_Tips.gameObject.SetActive(false);
            }
            else if (HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.Success)
            {
                Slider_Jindu.value = 1f;
                Tex_Progress.text = string.Format("{0:P0}", 1f);
                Tex_State.text = string.Format(hotFixTipWord.success);
                Tex_Tips.gameObject.SetActive(false);
            }
            else if (HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.Wait_FixAsset)
            {
                //弹窗
                Slider_Jindu.value = 1f;
                Tex_Progress.text = string.Format("{0:P0}", 1f);
                Tex_ok.text = hotFixTipWord.btn_fix;
                Tex_Des.text = string.Format(hotFixTipWord.asset_download_tip, HotFixStateManager.CountSize(HotFixStateManager.Instance.HotFixTotalSize));
            }
            else if (HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.Check_HotFixList)
            {
                Tex_State.text = hotFixTipWord.asset_check;
                Tex_Tips.gameObject.SetActive(false);
            }
            else if (HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.ReCheck_HotFixList)
            {
                Tex_State.text = hotFixTipWord.asset_verify;
                Tex_Tips.text = hotFixTipWord.no_need_net;
                Tex_Tips.gameObject.SetActive(true);
            }
            else if (HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.DownLoad_HotFixList)
            {
                Tex_State.text = hotFixTipWord.asset_check;
                Tex_Tips.gameObject.SetActive(false);
            }
            else if (HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.DownLoad_ConfigAsset || HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.DownLoad_AddressAsset)
            {
                Tex_State.text = hotFixTipWord.asset_fix;
                Tex_Tips.gameObject.SetActive(false);
            }
        }


        if (eAppError == EAppError.None)
        {
            if (HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.DownLoad_ConfigAsset || HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.DownLoad_AddressAsset)
            {
                //下载资源，更新进度
                float unzipP = 1f;
                if (HotFixStateManager.Instance.HotFixTotalSize > 0)
                {
                    unzipP = (float)((double)HotFixStateManager.Instance.HotFixSize / (double)HotFixStateManager.Instance.HotFixTotalSize);
                    Slider_Jindu.value = unzipP;
                    Tex_Progress.text = string.Format("{0:P0}", unzipP);
                    Tex_State.text = string.Format(hotFixTipWord.asset_download, HotFixStateManager.CountSize(HotFixStateManager.Instance.HotFixSize), HotFixStateManager.CountSize(HotFixStateManager.Instance.HotFixTotalSize), HotFixStateManager.CountSize(HotFixStateManager.Instance.DownloadSpeed));

                    //热更下载资源分别在各个阶段打点
                    if (unzipP >= 0.1f && arrHotFixRecord[0] == false)
                    {
                        arrHotFixRecord[0] = true;
                        HitPointManager.HitPoint("game_update_progress_10");
                    }
                    else if (unzipP >= 0.3f && arrHotFixRecord[1] == false)
                    {
                        arrHotFixRecord[1] = true;
                        HitPointManager.HitPoint("game_update_progress_30");
                    }
                    else if (unzipP >= 0.5f && arrHotFixRecord[2] == false)
                    {
                        arrHotFixRecord[2] = true;
                        HitPointManager.HitPoint("game_update_progress_50");
                    }
                    else if (unzipP >= 0.7f && arrHotFixRecord[3] == false)
                    {
                        arrHotFixRecord[3] = true;
                        HitPointManager.HitPoint("game_update_progress_70");
                    }
                }
            }
            else if(HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.DownLoad_HotFixList 
                || HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.Check_HotFixList
                || HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.ReCheck_HotFixList
                || HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.Destroy_SpareAssets)
            {
                Slider_Jindu.value = HotFixStateManager.Instance.CheckVersionProgress;
                Tex_Progress.text = string.Format("{0:P0}", HotFixStateManager.Instance.CheckVersionProgress);
            }
        }
    }
}
