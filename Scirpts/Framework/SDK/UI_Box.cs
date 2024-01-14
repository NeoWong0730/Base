using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public enum UseUIBOxType
    {
        None,
        ClickBackToExitGame,
        ServerListMaintaing,
        NotPlayByLevel,
        //PayCancel,
        //PayFailure,
        InitFailure,
        HotFixStart,
        HotFixEnd,
        PushNotify,
        PC_WaitOtherAppHotFixError,//PC平台 等待其他端热更
        PC_RemoteVersionUpdateError,//PC平台 在进程中的端版本低于当前要热更的版本
        ServerBlackUser,//服务器封禁账号
    }
    public class UI_Box : MonoBehaviour
    {
        private static UseUIBOxType lastUIBoxType = UseUIBOxType.None;
        public static UseUIBOxType _useUIBoxType = UseUIBOxType.None;
        private static GameObject uibox;

        public static string content;
        public static bool cancelShow = true;
        public static bool btn_okshow = true;

        public static void Create(UseUIBOxType useUIBOxType)
        {
            if (uibox)
            {
                _useUIBoxType = useUIBOxType;
                SetUIByType();
                uibox.gameObject.SetActive(true);
                return;
            }

            uibox = GameObject.Instantiate(Resources.Load("UI_Box") as GameObject);
            uibox.transform.localPosition = Vector3.zero;
            uibox.transform.localScale = Vector3.one;
            uibox.gameObject.SetActive(true);
            GameObject.DontDestroyOnLoad(uibox);

            _useUIBoxType = useUIBOxType;
            SetUIByType();
        }
        public static void Destroy()
        {
            if (uibox)
            {
                GameObject.DestroyImmediate(uibox);
            }
        }

        public static void SetUIByType()
        {
            btn_okshow = true;
            switch (_useUIBoxType)
            {
                case UseUIBOxType.ServerListMaintaing:
                    content = "服务器维护中，请稍后再试!";
                    cancelShow = false;
                    break;
                case UseUIBOxType.ClickBackToExitGame:
                    content = "是否结束游戏，确认后退出游戏";
                    cancelShow = true;
                    break;
                case UseUIBOxType.NotPlayByLevel:
                    cancelShow = false;
                    content = "当前支持移动设备；iPhone6s,iPhone6sPlus,iPadmini4,iPadPro(12.9),iPhoneSE,iPhone7,iPhone7Plus,iPad Pro(9.7),iPhone8,iPhone8Plus ,iPhoneX,iPad (5rd),iPad Pro (10.5),iPad Pro (12.9-2)及以上机型";
                    break;
                //case UseUIBOxType.PayCancel:
                //    cancelShow = false;
                //    content = "取消支付";
                //    break;
                //case UseUIBOxType.PayFailure:
                //    cancelShow = false;
                //    break;
                case UseUIBOxType.InitFailure:
                    content = "初始化失败,请退出游戏重进";
                    cancelShow = false;
                    break;
                case UseUIBOxType.HotFixStart:
                    content = "修复过程可能耗时过长，确定修复？";
                    cancelShow = true;
                    break;
                case UseUIBOxType.HotFixEnd:
                    content = "热更修复已完成，请退出重新启动";
                    cancelShow = false;
                    break;
                case UseUIBOxType.PushNotify:
                    content = "推送通知暂未打开，需要打开系统设置推送通知吗？";
                    cancelShow = true;
                    break;
                case UseUIBOxType.PC_WaitOtherAppHotFixError:
                    content = "其他客户端正在进行更新，请稍等...";
                    cancelShow = false;
                    btn_okshow = false;
                    break;
                case UseUIBOxType.PC_RemoteVersionUpdateError:
                    content = "当前有新版本需要更新，请先退出游戏，关闭其他客户端!";
                    cancelShow = false;
                    break;
                case UseUIBOxType.ServerBlackUser:
                    content = "此角色已被封号，如有问题请联系客服";
                    cancelShow = false;
                    btn_okshow = true;
                    break;
            }
        }


        public Button Btn_no;
        public Button Btn_ok;
        public Text Tex_no;
        public Text Tex_ok;
        public Text Tex_Tips;



        // Start is called before the first frame update
        void Start()
        {
            //Tex_Tips.text = string.Empty;
            Btn_ok.onClick.AddListener(OnBtnOk);
            Btn_no.onClick.AddListener(OnBtnNo);

        }
        void Update()
        {
            if (_useUIBoxType == UseUIBOxType.None)
                return;

            if (_useUIBoxType != lastUIBoxType)
            {
                Tex_Tips.text = content;
                Btn_no.gameObject.SetActive(cancelShow);
                Btn_ok.gameObject.SetActive(btn_okshow);
                lastUIBoxType = _useUIBoxType;
            }
        }

        public void OnBtnOk()
        {
            if (_useUIBoxType == UseUIBOxType.HotFixStart)
            {
                uibox.gameObject.SetActive(false);
                if (AppManager.eAppState == EAppState.HotFix)
                {
                    HotFixStateManager.Instance.OnExit();
                    HotFixStateManager.Instance.OnEnter();
                }
                else
                {
                    HotFixStateManager.Instance.CheckPersistentAssetMd5(CheckPersistentCallBack);
                }
            }
            else if (_useUIBoxType == UseUIBOxType.PushNotify)
            {
                uibox.gameObject.SetActive(false);
                SDKManager.SDKPushOpenSystemSetting();
            }
            else
            {
                AppManager.Quit();
            }
        }

        public void CheckPersistentCallBack()
        {
            Debug.Log("热更检查结束回调");
            //弹框
            UI_Box.Create(UseUIBOxType.HotFixEnd);
        }


        public void OnBtnNo()
        {
            uibox.gameObject.SetActive(false);
        }

    }
}
