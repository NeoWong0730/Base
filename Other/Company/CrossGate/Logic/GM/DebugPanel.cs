using Framework;
using Logic;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DebugPanel : MonoBehaviour
{
    public Dictionary<string, DebugWindowBase> mWindows;    

    Vector2 toolBarPos;
    public bool debugIsOn = false;
    public float fScale;
    public bool bDisableInput = true;

    private string sDisableEvent;
    private string sMenu;
    string sCurrentKey;
    public static Vector2 buttonSize;

    private Rect mEnterWindowRect;
    private string sEnterWindowTip = "点此拖拽";

    void Awake()
    {
        DontDestroyOnLoad(this);
        buttonSize = new Vector2(128, 64);
        sDisableEvent = "禁用输入";
        sMenu = "Menu";
        sCurrentKey = "GM";
        mWindows = new Dictionary<string, DebugWindowBase>() { };
        //skin = Resources.Load<GUISkin>("DebugGUISkin");

#if DEBUG_MODE
        mWindows.Add("GM", new GMWindow(1));
#endif
        mWindows.Add("SystemInfo", new SystemInfoWindow(2));
        mWindows.Add("Scene", new SceneWindow(3));
        mWindows.Add("Level", new LevelWindow(4));
        mWindows.Add("Log", new LogWindow(5));
        mWindows.Add("Weather", new WeatherWindow(6));
        mWindows.Add("Pool", new PoolWindow(7));        
        mWindows.Add("Audio", new AudioWindow(8));
        mWindows.Add("Profiler", new ProfilerWindow(9));

        foreach (var v in mWindows.Values)
        {
            v.OnAwake();
        }        
    }

    private void OnDestroy()
    {
        foreach (var v in mWindows.Values)
        {
            v.OnDestroy();
        }
    }


    void Start()
    {
        //Rect safeArea = Screen.safeArea;
        //fScale = Screen.height / 720f;
    }


    private void ToolBarWindow(int id)
    {
        toolBarPos = GUILayout.BeginScrollView(toolBarPos);
        var enumerator = mWindows.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (GUILayout.Button(enumerator.Current.Key, GUILayout.Width(buttonSize.x * fScale), GUILayout.Height(buttonSize.y * fScale)))
            {
                sCurrentKey = enumerator.Current.Key;
            }
        }
        GUILayout.EndScrollView();
    }

    void CalScaleAndSetSkin()
    {
        //GUI.skin = skin;
        float newScale = 1;
        if ((float)Screen.width / Screen.height < 16f / 9f)
        {
            newScale = Screen.width / 1280f;
        }
        else
        {
            newScale = Screen.height / 720f;
        }

        if (newScale != fScale)
        {
            fScale = newScale;
            GUI.skin.button.fontSize = (int)(22 * fScale);
            GUI.skin.toggle.fontSize = (int)(22 * fScale);
            GUI.skin.label.fontSize = (int)(22 * fScale);
            GUI.skin.textField.fontSize = (int)(32 * fScale);
            GUI.skin.textArea.fontSize = (int)(32 * fScale);

            GUI.skin.toggle = GUI.skin.button;
            GUI.skin.toggle.fontStyle = FontStyle.Normal;
            GUI.skin.toggle.onNormal.textColor = Color.green;
            GUI.skin.toggle.onActive.textColor = Color.green;
            GUI.skin.toggle.onFocused.textColor = Color.green;
            GUI.skin.toggle.onHover.textColor = Color.green;

            GUI.skin.toggle.fixedHeight = (int)(64 * fScale);
            GUI.skin.button.fixedHeight = (int)(64 * fScale);

            GUI.skin.horizontalSlider.fixedHeight = (int)(64 * fScale);
            GUI.skin.horizontalSliderThumb.fixedWidth = (int)(64 * fScale);
            GUI.skin.horizontalSliderThumb.fixedHeight = (int)(64 * fScale);

            GUI.skin.verticalSlider.fixedWidth = (int)(64 * fScale);
            GUI.skin.verticalSliderThumb.fixedWidth = (int)(64 * fScale);
            GUI.skin.verticalSliderThumb.fixedHeight = (int)(64 * fScale);

            GUI.skin.horizontalScrollbar.fixedHeight = (int)(64 * fScale);
            GUI.skin.horizontalScrollbarThumb.fixedHeight = (int)(64 * fScale);
            GUI.skin.horizontalScrollbarLeftButton.fixedHeight = (int)(64 * fScale);
            GUI.skin.horizontalScrollbarRightButton.fixedHeight = (int)(64 * fScale);

            GUI.skin.verticalScrollbar.fixedWidth = (int)(64 * fScale);
            GUI.skin.verticalScrollbarThumb.fixedWidth = (int)(64 * fScale);
            GUI.skin.verticalScrollbarDownButton.fixedWidth = (int)(64 * fScale);
            GUI.skin.verticalScrollbarUpButton.fixedWidth = (int)(64 * fScale);
        }
    }

    void OnGUIEnterButton(Rect rect)
    {
        if (GUI.Button(rect, $"{Framework.TimeManager.FPS.ToString()}/{Application.targetFrameRate.ToString()}"))
        {
            debugIsOn = !debugIsOn;
            if (AppManager.mEventSystem != null)
            {
                AppManager.mEventSystem.enabled = !(debugIsOn && bDisableInput);
            }

            if (GameCenter.mainHero != null)
            {
                if (debugIsOn)
                {
                    //var component = World.GetComponent<MovementComponent>(GameCenter.mainHero);
                    if(GameCenter.mainHero != null)
                    {
                        var component = GameCenter.mainHero.movementComponent;
                        component.enableflag = false;
                    }                    
                }
                else
                {
                    //var component = World.GetComponent<MovementComponent>(GameCenter.mainHero);
                    if (GameCenter.mainHero != null)
                    {
                        var component = GameCenter.mainHero.movementComponent;
                        component.enableflag = true;
                    }                        
                }
            }
        }
    }

    void OnEnterWindow(int id)
    {
        GUI.DragWindow();
    }

    void OnGUI()
    {
        CalScaleAndSetSkin();        

        if (debugIsOn)
        {            
            OnGUIEnterButton(new Rect(Screen.width / 3f - buttonSize.x * fScale * 0.5f, 0, buttonSize.x * fScale * 0.5f, buttonSize.y * fScale));

            float menuWindowWidth = (GUI.skin.window.padding.right + GUI.skin.window.padding.left + buttonSize.x + 10) * fScale;
            bool disableInput = GUI.Toggle(new Rect(Screen.width / 2f - buttonSize.x * fScale * 1.5f, 0, buttonSize.x * fScale, buttonSize.y * fScale), bDisableInput, sDisableEvent);
            if (disableInput != bDisableInput)
            {
                bDisableInput = disableInput;
                if (AppManager.mEventSystem != null)
                {
                    AppManager.mEventSystem.enabled = !(debugIsOn && bDisableInput);
                }
            }

            GUI.Window(0, new Rect(0, buttonSize.y * fScale, menuWindowWidth, Screen.height - buttonSize.y * fScale), ToolBarWindow, sMenu, GUI.skin.box);

            if (mWindows.TryGetValue(sCurrentKey, out DebugWindowBase debugWindow))
            {
                debugWindow.fScale = fScale;
                debugWindow.vSize = new Vector2(Screen.width - menuWindowWidth, Screen.height - buttonSize.y * fScale);
                GUI.Window(debugWindow.windowID, new Rect(menuWindowWidth, buttonSize.y * fScale, debugWindow.vSize.x, debugWindow.vSize.y), debugWindow.WindowFunction, sCurrentKey, GUI.skin.box);
            }
        }
        else
        {
            if (mEnterWindowRect.size == Vector2.zero)
            {
                mEnterWindowRect = new Rect(Screen.width / 3f - buttonSize.x * fScale * 0.7f, buttonSize.y * fScale, buttonSize.x * fScale * 0.7f, buttonSize.y * fScale * 0.5f);
            }
            
            mEnterWindowRect = GUI.Window(1000, mEnterWindowRect, OnEnterWindow, sEnterWindowTip, GUI.skin.box);
            OnGUIEnterButton(new Rect(mEnterWindowRect.x, mEnterWindowRect.y + mEnterWindowRect.height, mEnterWindowRect.width, buttonSize.y * fScale));
        }
        //GUI.skin.button.fontSize = 0;
        //GUI.skin.label.fontSize = 0;
        //GUI.skin.toggle.fontSize = 0;
        //GUI.skin.textField.fontSize = 0;
        //GUI.skin.textArea.fontSize = 0;
    }
}