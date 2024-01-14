#if UNITY_STANDALONE_WIN
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WinTitle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static readonly WinTitle Instance = new WinTitle();
    #region Win
    public Rect screenPosition;
    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();
    [DllImport("user32.dll")]
    public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string className, string windowName);
    //[DllImport("user32.dll")]
    //private static extern bool IsIconic(IntPtr hWnd);
    [DllImport("user32")]
    public static extern int GetSystemMetrics(int nIndex); //通过设置不同的标识符就可以获取系统分辨率、窗体显示区域的宽度和高度、滚动

    const int SM_CXSCREEN = 0x00000000;
    const int SM_CYSCREEN = 0x00000001;
    const uint SWP_SHOWWINDOW = 0x0040;
    const int GWL_STYLE = -16;
    const int WS_BORDER = 1;
    const int WS_POPUP = 0x800000;
    const int SW_SHOWMINIMIZED = 2; //最小化, 激活
    const int SW_SHOWMAXIMIZED = 3; //最大化, 激活 

    public const int WM_SYSCOMMAND = 0x0112;
    public const int SC_MOVE = 0xF010;
    public const int HTCAPTION = 0x0002;

    const int WS_MINIMIZEBOX = 0x00020000;

    #endregion

    IntPtr Handle;
    float xx;
    bool bx;
    bool canMove, inMove;

    #region unity ui
    public Button btn_MinWin, btn_MaxWin, btn_CloseWin;
    public Button btn_VolAdd, btn_VolMin;
    #endregion

    void Awake()
    {
        Debug.LogError("WinTitle");
        Screen.fullScreen = false;
        bx = false;
        xx = 0f;
#if UNITY_STANDALONE_WIN
        screenPosition = new Rect(((int)GetSystemMetrics(SM_CXSCREEN) - Screen.width) / 2, ((int)GetSystemMetrics(SM_CYSCREEN) - Screen.height) / 2, 1000, 600);
        IntPtr intPtr = FindWindow(null, Application.productName);
        Handle = intPtr; //Handle = GetForegroundWindow();
        SetWindowLong(Handle, GWL_STYLE, WS_POPUP | WS_MINIMIZEBOX);//WS_BORDER
        SetWindowPos(Handle, 0, (int)screenPosition.x, (int)screenPosition.y, 1000, 600, SWP_SHOWWINDOW);
        bool result = SetWindowPos(Handle, 0, (int)screenPosition.x, (int)screenPosition.y, 1000, 600, SWP_SHOWWINDOW);//居中显示
#endif
        Transform parent = this.transform.parent;
        btn_MinWin = parent.transform.Find("Btn_Small").GetComponent<Button>();
        btn_MaxWin = parent.transform.Find("Btn_Big").GetComponent<Button>();
        btn_CloseWin = parent.transform.Find("Btn_Close").GetComponent<Button>();
        btn_VolAdd = parent.transform.Find("Btn_VolAdd").GetComponent<Button>();
        btn_VolMin = parent.transform.Find("Btn_VolMin").GetComponent<Button>();
    }
    void Start()
    {
        btn_MinWin.onClick.AddListener(Btn_OnClickMin);
        btn_MaxWin.onClick.AddListener(Btn_OnClickMax);
        btn_CloseWin.onClick.AddListener(Btn_OnClickClose);
        btn_VolAdd.onClick.AddListener(Btn_OnClickVolAdd);
        btn_VolMin.onClick.AddListener(Btn_OnClickVolMin);
    }

    void Update()
    {
#if UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonDown(0) && canMove && inMove)
        {
            xx = 0f;
            bx = true;
        }
        if (bx && xx >= 0.02f)
        {
            ReleaseCapture();
            SendMessage(Handle, 0xA1, 0x02, 0);
            SendMessage(Handle, 0x0202, 0, 0);
        }
        if (bx)
            xx += Time.deltaTime;
        if (Input.GetMouseButtonUp(0))
        {
            xx = 0f;
            bx = false;
        }
#endif
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        inMove = false;
        canMove = false;
        Debug.Log("inMove" + inMove + " --- " + "canMove" + canMove);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        canMove = true;
        Debug.Log("canMove" + canMove);
    }
    public void OnDrag(PointerEventData eventData)
    {
        //inMove = true;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //ReleaseCapture();
        //SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        inMove = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        inMove = false;
    }

    public void Btn_OnClickMin()
    { 
        //IntPtr intPtr = FindWindow(null, Application.productName);
        //Handle = intPtr;
        ShowWindow(Handle, SW_SHOWMINIMIZED);
    }
    public void Btn_OnClickMax()
    {
        return;
        ShowWindow(GetForegroundWindow(), SW_SHOWMAXIMIZED);
    }

    public void Btn_OnClickClose()
    {
        Application.Quit();
    }

    #region 模拟键盘调节系统音量
    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, UInt32 dwFlags, UInt32 dwExtraInfo);

    [DllImport("user32.dll")]
    static extern Byte MapVirtualKey(UInt32 uCode, UInt32 uMapType);

    private const byte VK_VOLUME_MUTE = 0xAD;
    private const byte VK_VOLUME_DOWN = 0xAE;
    private const byte VK_VOLUME_UP = 0xAF;
    private const UInt32 KEYEVENTF_EXTENDEDKEY = 0x0001;
    private const UInt32 KEYEVENTF_KEYUP = 0x0002;

    /// <summary>
    /// 改变系统音量大小，增加
    /// </summary>
    public void SystemVolumeUp()
    {
        keybd_event(VK_VOLUME_UP, MapVirtualKey(VK_VOLUME_UP, 0), KEYEVENTF_EXTENDEDKEY, 0);
        keybd_event(VK_VOLUME_UP, MapVirtualKey(VK_VOLUME_UP, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
    }

    /// <summary>
    /// 改变系统音量大小，减小
    /// </summary>
    public void SystemVolumeDown()
    {
        keybd_event(VK_VOLUME_DOWN, MapVirtualKey(VK_VOLUME_DOWN, 0), KEYEVENTF_EXTENDEDKEY, 0);
        keybd_event(VK_VOLUME_DOWN, MapVirtualKey(VK_VOLUME_DOWN, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
    }
    private void Btn_OnClickVolMin()
    {
        SystemVolumeDown();
        Debug.LogError("SystemVolumeDown");
    }

    private void Btn_OnClickVolAdd()
    {
        SystemVolumeUp();
        Debug.LogError("SystemVolumeUp");
    }
    #endregion
}
#endif