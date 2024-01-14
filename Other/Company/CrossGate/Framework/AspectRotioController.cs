#if UNITY_STANDALONE_WIN //&& !UNITY_EDITOR
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Events;
using System.IO;
using Framework;

public class AspectRotioController : MonoBehaviour
{
    public static readonly AspectRotioController Instance = new AspectRotioController();

    public static ResolutionChangedEvent resolutionChangedEvent;
    public class ResolutionChangedEvent : UnityEvent<int, int, bool> { }

    public static bool IsExpandState = false; //外置状态

    private static bool allowFullscreen = false;

    public float aspectRatioWidth = 4;
    public float aspectRatioHeight = 3;

    private float defaultWidth = 4;
    private float defaultHright = 3;

    private static int defaultWidthPixel = 1000;

    private static int minWidthPixel = 400;
    private static int minHeightPixel = 300;
    private static int maxWidthPixel = 2048;
    private static int maxHeightPixel = 2048;

    // 当前锁定长宽比。
    private static float aspect;

    private static int setWidth = -1;
    private static int setHeight = -1;

    // 最后一帧全屏状态。
    private bool wasFullscreenLastFrame;

    // 是否初始化AspectRatioController
    private bool started;

    // 显示器的宽度和高度
    public int pixelHeightOfCurrentScreen;
    public int pixelWidthOfCurrentScreen;

    /// <summary>
    /// 当前设置的分辨率（屏幕比）
    /// </summary>
    public Enum_Ratio curRatio = Enum_Ratio.Type_2;
    private string m_ResolutionConfig;

    /// <summary>
    /// 全局热键boss键
    /// </summary>
    HotKey.KeyModifiers modifier = HotKey.KeyModifiers.Shift;

    private string ProcessPath;
    private int ProcessID;

    private static uint WinMessage_HotFix;

    // WinAPI相关定义
    #region WINAPI

    // 当窗口调整时,WM_SIZING消息通过WindowProc回调发送到窗口
    private const int WM_SIZING = 0x214;
    private const int WM_HOTKEY = 0x0312;

    // WM大小调整消息的参数
    private const int WMSZ_LEFT = 1;
    private const int WMSZ_RIGHT = 2;
    private const int WMSZ_TOP = 3;
    private const int WMSZ_BOTTOM = 6;
    /// <summary>
    /// 窗口风格
    /// </summary>
    const int GWL_STYLE = -16;
    /// <summary>
    /// 最大化
    /// </summary>
    const long WS_MAXIMIZEBOX = 0x00010000L;
    /// <summary>
    /// 该窗口最初可见
    /// </summary>
    const long WS_VISIBLE = 0x10000000L;
    const long WM_CLOSE = 0x0010;
    const long WM_DESTROY = 0x0002;

    // 获取指向WindowProc函数的指针
    private const int GWLP_WNDPROC = -4;

    // 委托设置为新的WindowProc回调函数
    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    private static WndProcDelegate wndProcDelegate;

    // 检索调用线程的线程标识符
    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    // 检索指定窗口所属类的名称
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);


    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    // 通过将句柄传递给每个窗口，依次传递给应用程序定义的回调函数，枚举与线程关联的所有非子窗口
    [DllImport("user32.dll")]
    private static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    // 将消息信息传递给指定的窗口过程
    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    // 检索指定窗口的边框的尺寸
    // 尺寸是在屏幕坐标中给出的，它是相对于屏幕左上角的
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);

    //检索窗口客户区域的坐标。客户端坐标指定左上角
    //以及客户区的右下角。因为客户机坐标是相对于左上角的
    //在窗口的客户区域的角落，左上角的坐标是(0,0)
    [DllImport("user32.dll")]
    private static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

    // 更改指定窗口的属性。该函数还将指定偏移量的32位(长)值设置到额外的窗口内存中
    [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
    private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    //更改指定窗口的属性。该函数还在额外的窗口内存中指定的偏移量处设置一个值
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    //用于查找窗口句柄的Unity窗口类的名称
    private const string UNITY_WND_CLASSNAME = "UnityWndClass";

    // Unity窗口的窗口句柄
    private static IntPtr unityHWnd;

    // 指向旧WindowProc回调函数的指针
    private static IntPtr oldWndProcPtr;

    // 指向我们自己的窗口回调函数的指针
    private static IntPtr newWndProcPtr;

    [DllImport("user32.dll")]
    public static extern long GetWindowLong(IntPtr hwd, int nIndex);
    [DllImport("user32.dll")]
    public static extern void SetWindowLong(IntPtr hwd, int nIndex, long dwNewLong);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    public static extern IntPtr GetParent(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern long GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

    [DllImport("user32.dll")]
    public static extern uint RegisterWindowMessage(string sMessageName);

    [DllImport("user32.dll", EntryPoint = "SetWindowTextW", CharSet = CharSet.Unicode)]
    private static extern bool SetWindowTextW(IntPtr hwnd, string lPstring);

    /// <summary>
    /// WinAPI矩形定义。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    #endregion

    /// <summary>
    /// PC分辨率默认比例读取本地设置
    /// </summary>
    public void InitData()
    {
        if (!ReadLocalConfig("Shared/Options"))
        {
            curRatio = Enum_Ratio.Type_2;
        }
        ReadResolutionConfig();
        if (!string.IsNullOrEmpty(m_ResolutionConfig))
        {
            string[] sttr = m_ResolutionConfig.Split('|');
            if (sttr.Length != 5)
                return;
            //TODO:初始化数据
            float.TryParse(sttr[1].Split(':')[0], out aspectRatioWidth);
            float.TryParse(sttr[1].Split(':')[1], out aspectRatioHeight);
            defaultWidth = aspectRatioWidth;
            defaultHright = aspectRatioHeight;
            int.TryParse(sttr[2], out defaultWidthPixel);
            int.TryParse(sttr[3], out minWidthPixel);
            int.TryParse(sttr[4], out minHeightPixel);
            maxWidthPixel = 2048;
            maxHeightPixel = 2048;
        }

    }

    [AOT.MonoPInvokeCallback(typeof(EnumWindowsProc))]
    private static bool GetEnumWindowsProc(IntPtr hWnd, IntPtr lParam)
    {
        var classText = new StringBuilder(UNITY_WND_CLASSNAME.Length + 1);
        GetClassName(hWnd, classText, classText.Capacity);

        if (classText.ToString() == UNITY_WND_CLASSNAME)
        {
            unityHWnd = hWnd;
            SetWindowTextW(unityHWnd, "魔力宝贝");
            return false;
        }
        return true;
    }

    public void SetNotFullScreen()
    {
        if (!started)
            InitStart();
        var wl = GetWindowLong(unityHWnd, GWL_STYLE);
        wl &= ~WS_MAXIMIZEBOX;
        SetWindowLong(unityHWnd, GWL_STYLE, wl);
    }

    public void InitStart()
    {
        if (!started)
        {
            WinMessage_HotFix = RegisterWindowMessage("CrossGateHotFix");
            Screen.fullScreen = false;
            EnumThreadWindows(GetCurrentThreadId(), GetEnumWindowsProc, new IntPtr(10101));

            SetAspectRatio(aspectRatioWidth, aspectRatioHeight, true);

            wasFullscreenLastFrame = Screen.fullScreen;

            wndProcDelegate = wndProc;
            newWndProcPtr = Marshal.GetFunctionPointerForDelegate(wndProcDelegate);
            oldWndProcPtr = SetWindowLong(unityHWnd, GWLP_WNDPROC, newWndProcPtr);

            started = true;
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            modifier |= HotKey.KeyModifiers.Ctrl;
            RegisterHotKey(unityHWnd.ToInt32(), modifier);
#endif
            SetProcessInfo();
        }

    }

    /// <summary>
    /// 记录多开状态下的进程
    /// </summary>
    private void SetProcessInfo()
    {
        ProcessID = AppManager.ProcessID = System.Diagnostics.Process.GetCurrentProcess().Id;   
        string dir = Lib.AssetLoader.AssetPath.GetPersistentFullPath(Lib.AssetLoader.AssetPath.sProcessDir);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        ProcessPath = string.Format("{0}/{1}_pid.txt", dir, ProcessID.ToString());
        File.WriteAllText(ProcessPath, ProcessID.ToString());
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        OnExit();
    }

    public void OnExit()
    {
        if (string.IsNullOrEmpty(ProcessPath))
            return;
        if (File.Exists(ProcessPath))
            File.Delete(ProcessPath);
    }

    /// <summary>
    ///将目标长宽比设置为给定的长宽比。(以宽不变为基准)
    /// </summary>
    private void SetAspectRatio(float newAspectWidth, float newAspectHeight, bool apply)
    {
        aspectRatioWidth = newAspectWidth;
        aspectRatioHeight = newAspectHeight;
        aspect = aspectRatioWidth / aspectRatioHeight;

        if (apply)
        {
            if (started)
                Screen.SetResolution(Screen.width, Mathf.RoundToInt(Screen.width / aspect), false);
            else
                Screen.SetResolution(defaultWidthPixel, Mathf.RoundToInt(defaultWidthPixel / aspect), false);
        }
    }

    /// <summary>
    /// 设置新的宽高比，以高不变为基准
    /// </summary>
    public void SetNewResolution(float newAspectWidth, float newAspectHeight, bool apply)
    {
        aspectRatioWidth = newAspectWidth;
        aspectRatioHeight = newAspectHeight;
        aspect = aspectRatioWidth / aspectRatioHeight;

        if (apply)
        {
            Screen.SetResolution(Mathf.RoundToInt(aspect * Screen.height), Screen.height, false);
        }
    }

    /// <summary>
    /// 还原初始宽高比
    /// </summary>
    public void ReturnAspectRatio()
    {
        SetAspectRatio(defaultWidth, defaultHright, true);
    }

    /// <summary>
    /// WindowProc回调。应用程序定义的函数，用来处理发送到窗口的消息 
    /// </summary>
    /// <param name="msg">用于标识事件的消息</param>
    [AOT.MonoPInvokeCallback(typeof(WndProcDelegate))]
    static IntPtr wndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_HOTKEY)
        {
            int id = wParam.ToInt32();
            if (id == unityHWnd.ToInt32())
            {
                if ((GetWindowLong(unityHWnd, GWL_STYLE) & WS_VISIBLE) != 0)
                    isShow = false;
                else
                    isShow = true;
                Enum_Window();
            }
        }
        if (msg == WM_SIZING)
        {
            // 获取窗口大小结构体
            RECT rc = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));

            // 计算窗口边框的宽度和高度
            RECT windowRect = new RECT();
            GetWindowRect(unityHWnd, ref windowRect);

            RECT clientRect = new RECT();
            GetClientRect(unityHWnd, ref clientRect);

            int borderWidth = windowRect.Right - windowRect.Left - (clientRect.Right - clientRect.Left);
            int borderHeight = windowRect.Bottom - windowRect.Top - (clientRect.Bottom - clientRect.Top);

            // 在应用宽高比之前删除边框(包括窗口标题栏)
            rc.Right -= borderWidth;
            rc.Bottom -= borderHeight;

            // 限制窗口大小
            int newWidth = Mathf.Clamp(rc.Right - rc.Left, minWidthPixel, maxWidthPixel);
            int newHeight = Mathf.Clamp(rc.Bottom - rc.Top, minHeightPixel, maxHeightPixel);

            // 根据纵横比和方向调整大小
            switch (wParam.ToInt32())
            {
                case WMSZ_LEFT:
                    rc.Left = rc.Right - newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_RIGHT:
                    rc.Right = rc.Left + newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_TOP:
                    rc.Top = rc.Bottom - newHeight;
                    rc.Right = rc.Left + Mathf.RoundToInt(newHeight * aspect);
                    break;
                case WMSZ_BOTTOM:
                    rc.Bottom = rc.Top + newHeight;
                    rc.Right = rc.Left + Mathf.RoundToInt(newHeight * aspect);
                    break;
                case WMSZ_RIGHT + WMSZ_BOTTOM:
                    rc.Right = rc.Left + newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_RIGHT + WMSZ_TOP:
                    rc.Right = rc.Left + newWidth;
                    rc.Top = rc.Bottom - Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_LEFT + WMSZ_BOTTOM:
                    rc.Left = rc.Right - newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_LEFT + WMSZ_TOP:
                    rc.Left = rc.Right - newWidth;
                    rc.Top = rc.Bottom - Mathf.RoundToInt(newWidth / aspect);
                    break;
            }

            setWidth = rc.Right - rc.Left;
            setHeight = rc.Bottom - rc.Top;

            // 添加边界
            rc.Right += borderWidth;
            rc.Bottom += borderHeight;

            resolutionChangedEvent?.Invoke(setWidth, setHeight, Screen.fullScreen);

            Marshal.StructureToPtr(rc, lParam, true);
        }
        if (msg == WinMessage_HotFix)
        {
            Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eNone, "checkversion:{0}--{1}", wParam, unityHWnd);
            if (unityHWnd == wParam)
            {
                WaitAppHotFixManager.Instance.RecMessageByOtherProcess(lParam.ToInt32() == 1 ? false : true);
            }
        }
        //if (msg == WM_CLOSE || msg == WM_DESTROY)
        //{
        //    Instance.OnExit();
        //}
        // 调用原始的WindowProc函数
        return CallWindowProc(oldWndProcPtr, hWnd, msg, wParam, lParam);
    }
    public void Update()
    {
        if (!allowFullscreen && Screen.fullScreen)
        {
            Screen.fullScreen = false;
        }

        if (Screen.fullScreen && !wasFullscreenLastFrame)
        {
            int height;
            int width;

            bool blackBarsLeftRight = aspect < (float)pixelWidthOfCurrentScreen / pixelHeightOfCurrentScreen;

            if (blackBarsLeftRight)
            {
                height = pixelHeightOfCurrentScreen;
                width = Mathf.RoundToInt(pixelHeightOfCurrentScreen * aspect);
            }
            else
            {
                width = pixelWidthOfCurrentScreen;
                height = Mathf.RoundToInt(pixelWidthOfCurrentScreen / aspect);
            }

            Screen.SetResolution(width, height, true);
            resolutionChangedEvent?.Invoke(width, height, true);
        }
        else if (!Screen.fullScreen && wasFullscreenLastFrame)
        {
            Screen.SetResolution(setWidth, setHeight, false);
            resolutionChangedEvent?.Invoke(setWidth, setHeight, false);
        }
        else if (!Screen.fullScreen && setWidth != -1 && setHeight != -1 && (Screen.width != setWidth || Screen.height != setHeight))
        {
            setHeight = Screen.height;
            setWidth = Mathf.RoundToInt(Screen.height * aspect);

            Screen.SetResolution(setWidth, setHeight, Screen.fullScreen);
            resolutionChangedEvent?.Invoke(setWidth, setHeight, Screen.fullScreen);
        }
        else if (!Screen.fullScreen)
        {
            pixelHeightOfCurrentScreen = Screen.currentResolution.height;
            pixelWidthOfCurrentScreen = Screen.currentResolution.width;
        }

        wasFullscreenLastFrame = Screen.fullScreen;

        //unity编辑器下调试
#if UNITY_EDITOR
        if (Screen.width != setWidth || Screen.height != setHeight)
        {
            setWidth = Screen.width;
            setHeight = Screen.height;
            resolutionChangedEvent?.Invoke(setWidth, setHeight, Screen.fullScreen);
        }
#endif
        SetNotFullScreen();
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if(!isRegisterWindow)
            RegisterHotKey(unityHWnd.ToInt32(), modifier);
#endif
    }

    private static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        if (IntPtr.Size == 4)
        {
            return SetWindowLong32(hWnd, nIndex, dwNewLong);
        }
        return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }

    #region 系统热键

    private static bool isShow = true;
    private static bool isRegisterWindow = false;
    public void RegisterHotKey(int id, HotKey.KeyModifiers fsModifiers)
    {
        isRegisterWindow = HotKey.RegisterHotKey(unityHWnd, unityHWnd.ToInt32(), fsModifiers, 121);
    }
    public void UnregisterHotKey(int id = 100)
    {
        OnExit();
        HotKey.UnregisterHotKey(unityHWnd, unityHWnd.ToInt32());
    }

    public static void Enum_Window()
    {
        EnumWindows(Enum_WindowsProc, new IntPtr(10101));
    }
    [AOT.MonoPInvokeCallback(typeof(EnumWindowsProc))]
    private static bool Enum_WindowsProc(IntPtr hWnd, IntPtr lParam)
    {

        var classText = new StringBuilder(UNITY_WND_CLASSNAME.Length + 1);
        GetClassName(hWnd, classText, classText.Capacity);

        if (classText.ToString() == UNITY_WND_CLASSNAME && lParam == new IntPtr(10101))
        {
            if(!isShow) //((GetWindowLong(unityHWnd, GWL_STYLE) & WS_VISIBLE) != 0)
            {
                //Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eNone, "Hide:{0}", unityHWnd.ToInt32());
                ShowWindow(hWnd, 0);//隐藏窗体
            }
            else
            {
                //Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eNone, "Show:{0}", unityHWnd.ToInt32());
                ShowWindow(hWnd, 5);//显示窗体
            }
        }
        return true;
    }
    #endregion

    #region 分辨率切换（设置）

    /// <summary>
    /// 读取本地存储的分辨率值
    /// </summary>
    private bool ReadLocalConfig(string relativePath)
    {
        string path = Lib.AssetLoader.AssetPath.GetPersistentFullPath(relativePath);
        if (File.Exists(path))
        {
            FileStream fs = File.OpenRead(path);
            BinaryReader br = new BinaryReader(fs);
            while (br.BaseStream.Position + 8 <= br.BaseStream.Length)
            {
                int k = br.ReadInt32();
                int v = br.ReadInt32();
                if (k == 11)
                {
                    curRatio = (Enum_Ratio)(v + 1);
                    break;
                }
            }
            br.Dispose(); br.Close();
            fs.Dispose(); fs.Close();
            return true;
        }
        return false;
    }
    /// <summary>
    /// 读取分辨率的配置信息
    /// </summary>
    private void ReadResolutionConfig()
    {
        Stream stream = Lib.AssetLoader.AssetMananger.Instance.LoadStream("Config/ResolutionInfo.txt");
        StreamReader sr = new StreamReader(stream);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(line))
                continue;
            string[] sttr = line.Split('|');
            int index = -1;
            int.TryParse(sttr[0], out index);
            if (index == (int)curRatio)
            {
                m_ResolutionConfig = line;
                break;
            }
        }
        sr.Close();
        sr.Dispose();
        stream.Close();
        stream.Dispose();
    }
    #endregion

    #region 通过进程ID获取窗口，发消息
    public void SendMessageToProcess(int processID,int param)
    {
        IntPtr handle = GetCurrentWindowHandle(processID);
        if (handle == IntPtr.Zero)
            return;
        Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eNone, "SendMessageToProcess:{0}", processID);
        PostMessage(handle, WinMessage_HotFix, handle, new IntPtr(param));
        Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eNone, "PostMessage:{0}", WinMessage_HotFix);
    }

    static IntPtr ptrWnd;
    public static IntPtr GetCurrentWindowHandle(int proid)
    {
        ptrWnd = IntPtr.Zero;
        int uiPid = proid;

        bool bResult = EnumWindows(EnumWindowsProc1, new IntPtr(uiPid));
        if (!bResult)
        {
            return ptrWnd;
        }
        Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eNone, "GetCurrentWindowHandle:{0}", ptrWnd);
        return ptrWnd;
    }

    [AOT.MonoPInvokeCallback(typeof(EnumWindowsProc))]
    private static bool EnumWindowsProc1(IntPtr hwnd, IntPtr lParam)
    {
        uint uiPid = 0;
        if (GetParent(hwnd) == IntPtr.Zero)
        {
            GetWindowThreadProcessId(hwnd, ref uiPid);
            var classText = new StringBuilder(UNITY_WND_CLASSNAME.Length + 1);
            GetClassName(hwnd, classText, classText.Capacity);

            if (uiPid == lParam.ToInt64() && classText.ToString() == UNITY_WND_CLASSNAME)
            {
                ptrWnd = hwnd;
                Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eNone, "EnumWindowsProc1:{0}", ptrWnd);
                return false;
            }
        }
        return true;
    }

    public bool IsHaveProcessById(int pid)
    {
        IntPtr handle = GetCurrentWindowHandle(pid);
        if (handle == IntPtr.Zero)
            return false;
        return true;
    }
    #endregion
}

/// <summary>
/// 热键
/// </summary>
public class HotKey
{
    [DllImport("user32.dll")]
    public static extern bool RegisterHotKey(
      IntPtr hWnd,
      int id,           //定义热键ID（不能与其它ID重复）
      KeyModifiers fsModifiers,  //标识热键是否在按Alt、Ctrl、Shift等键时才会生效
      uint vk
      );
    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Ctrl = 2,
        Shift = 4,
    }
}


public enum Enum_Ratio
{
    None,
    /// <summary>4:3</summary>
    Type_1,
    /// <summary>16:9</summary>
    Type_2,
    /// <summary>21:9</summary>
    Type_3,
}
#endif
