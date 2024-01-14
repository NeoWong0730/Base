using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherWindow : DebugWindowBase
{
    private string[] weathers = new string[]
    {
        "晴天",
        "下雨",
        "下雪",
        "雷雨",
    };

    public WeatherWindow(int id) : base(id) { }

    public override void WindowFunction(int id)
    {
#if DEBUG_MODE
        GUILayout.BeginHorizontal();
        WeatherSystem.gWeatherDebug = GUILayout.Toggle(WeatherSystem.gWeatherDebug, "本地效果调试");
        WeatherSystem.gOneSeasonWeatherDebug = GUILayout.Toggle(WeatherSystem.gOneSeasonWeatherDebug, "服务器数据调试");
        GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("季节");
        //WeatherSystem.gSeasonProgress = GUILayout.HorizontalSlider(WeatherSystem.gSeasonProgress, 0, 1);
        //GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("日夜", GUILayout.Width(fScale * 200));
        WeatherSystem.gDayProgress = GUILayout.HorizontalSlider(WeatherSystem.gDayProgress, 0, 1);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("天气", GUILayout.Width(fScale * 200));
        WeatherSystem.gWeatherProgress = GUILayout.HorizontalSlider(WeatherSystem.gWeatherProgress, 0, 1);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("上个天气", GUILayout.Width(fScale * 200));
        WeatherSystem.gWeatherTypeBefore = (EWeatherType)GUILayout.SelectionGrid((int)WeatherSystem.gWeatherTypeBefore - 1, weathers, weathers.Length) + 1;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("当前天气", GUILayout.Width(fScale * 200));        
        int rlt = GUILayout.SelectionGrid((int)WeatherSystem.gWeatherType - 1, weathers, weathers.Length) + 1;
        if (rlt != (int)WeatherSystem.gWeatherType)
        {
            WeatherSystem.gWeatherTypeBefore = WeatherSystem.gWeatherType;
            WeatherSystem.gWeatherType = (EWeatherType)rlt;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("季节时间", GUILayout.Width(fScale * 200));
        WeatherSystem.gCurSeasonProgress = GUILayout.HorizontalSlider(WeatherSystem.gCurSeasonProgress, 0, 1);
        GUILayout.EndHorizontal();
#endif
    }
}