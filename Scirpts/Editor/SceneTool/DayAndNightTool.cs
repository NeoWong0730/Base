using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.Rendering;
using UnityEditor.SceneManagement;
using System.IO;
using Framework;

public class DayAndNightTool : EditorWindow
{
    [MenuItem("__Tools__/天气系统")]
    public static void OpenWindow()
    {
        DayAndNightTool dayAndNightTool = EditorWindow.GetWindow<DayAndNightTool>("天气系统");
    }

    Vector3Int count = new Vector3Int(2, 2, 2);
    Vector3 constOffset = new Vector3(0, 1.5f, 0);
    Vector3 rangeSize = new Vector3(2, 2, 2);
    float dayTime = 0;
    float windRotation;

    private string[] weathers = new string[]
{
        "晴天",
        "下雨",
        "下雪",
        "雷雨",
};

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            rangeSize = EditorGUILayout.Vector3Field("范围尺寸", rangeSize);
            count = EditorGUILayout.Vector3IntField("点数量", count);
            constOffset = EditorGUILayout.Vector3Field("偏移", constOffset);

            if (GUILayout.Button("生成光照探针"))
            {
                GenLightProbePositions2();
            }

            if (GUILayout.Button("设置昼夜参数"))
            {
                CollectLightMapMesh();
            }

            if (GUILayout.Button("创建天气配置"))
            {
                DayAndNightProfile profile = ScriptableObject.CreateInstance<DayAndNightProfile>();

                string matPath = Application.dataPath + "/ResourcesAB/Settings/";
                string matNameFormat = "DayAndNightProfile{0}.asset";

                int i = 0;
                string matFinalName = "DayAndNightProfile.asset";
                string matFinalFullPath = matPath + matFinalName;
                while (File.Exists(matFinalFullPath))
                {
                    ++i;
                    if (i > 20)
                    {
                        Debug.LogErrorFormat("请腾个名字出来");
                        break;
                    }
                    matFinalName = string.Format(matNameFormat, i.ToString("D2"));
                    matFinalFullPath = matPath + matFinalName;
                }

                if (i < 20)
                {
                    AssetDatabase.CreateAsset(profile, "Assets/ResourcesAB/Settings/" + matFinalName);
                }
            }
        }
        else
        {
#if DEBUG_MODE
            WeatherSystem.gWeatherDebug = EditorGUILayout.Toggle("Debug", WeatherSystem.gWeatherDebug);
            WeatherSystem.gSeasonProgress = EditorGUILayout.Slider("季节进度", WeatherSystem.gSeasonProgress, 0, 1);
            WeatherSystem.gDayProgress = EditorGUILayout.Slider("天进度", WeatherSystem.gDayProgress, 0, 1);

            float day = WeatherSystem.gDayProgress * 24 + 6;
            if (day > 24)
            {
                day -= 24;
            }

            day = Mathf.Clamp(EditorGUILayout.FloatField("天进度（小时）", day), 0, 24);
            if (day < 6)
            {
                day += 24;
            }

            WeatherSystem.gDayProgress = (day - 6) / 24f;

            dayTime = EditorGUILayout.FloatField("天时长", dayTime);
            dayTime = Mathf.Max(0, dayTime);
            if (dayTime == 0)
            {
                WeatherSystem.fSpeed = 0;
            }
            else
            {
                WeatherSystem.fSpeed = 1 / dayTime;
            }

            WeatherSystem.gWeatherProgress = EditorGUILayout.Slider("天气进度", WeatherSystem.gWeatherProgress, 0, 1);
            WeatherSystem.gWeatherTypeBefore = (EWeatherType)EditorGUILayout.EnumPopup("上个天气", WeatherSystem.gWeatherTypeBefore);
            EWeatherType rlt = (EWeatherType)EditorGUILayout.EnumPopup("当前天气", WeatherSystem.gWeatherType);
            if (rlt != WeatherSystem.gWeatherType)
            {
                WeatherSystem.gWeatherTypeBefore = WeatherSystem.gWeatherType;
                WeatherSystem.gWeatherType = rlt;
            }

            windRotation = EditorGUILayout.Slider("风向", windRotation, 0, 360);
            Vector3 dir = Quaternion.AngleAxis(windRotation, Vector3.up) * Vector3.right;
            WeatherSystem.gWindParam.x = dir.x;
            WeatherSystem.gWindParam.z = dir.z;

            WeatherSystem.gWindParam.y = EditorGUILayout.FloatField("风速", WeatherSystem.gWindParam.y);
            WeatherSystem.gWindParam.w = EditorGUILayout.Slider("强度", WeatherSystem.gWindParam.w, 0, 1);

            WeatherSystem.gWindDensity = EditorGUILayout.FloatField("风(波)的密度", WeatherSystem.gWindDensity);
#endif
        }
    }

    private void GenLightProbePositions2()
    {
        LightProbeGroup lightProbeGroup = GameObject.FindObjectOfType<LightProbeGroup>();
        if (lightProbeGroup == null)
        {
            Debug.LogError("未找到LightProbeGroup");
            return;
        }

        Light[] lights = GameObject.FindObjectsOfType<Light>();
        if (lights == null)
        {
            Debug.LogError("未找到Light");
            return;
        }

        List<Vector3> positions = new List<Vector3>(lights.Length * 10);

        for (int i = 0; i < lights.Length; ++i)
        {
            Light light = lights[i];

            if (light.lightmapBakeType == LightmapBakeType.Realtime)
            {
                continue;
            }

            Vector3 pos = light.transform.position;

            Vector3 dir1 = Vector3.forward;
            Vector3 dir2 = Vector3.right; //Quaternion.AngleAxis(120, Vector3.up) * dir1;
            Vector3 dir3 = Vector3.left;//Quaternion.AngleAxis(120, Vector3.up) * dir2;
            Vector3 dir33 = Vector3.back;

            Vector3 dir4 = -dir1 * 3;
            Vector3 dir5 = -dir2 * 3;
            Vector3 dir6 = -dir3 * 3;
            Vector3 dir66 = -dir33 * 3;

            //             float hight = (pos.y + constOffset.y) / 3f;
            // 
            //             positions.Add(pos + constOffset.y * Vector3.up);
            //             positions.Add(pos + dir1 + (constOffset.y - hight * 2) * Vector3.up);
            //             positions.Add(pos + dir2 + (constOffset.y - hight * 2) * Vector3.up);
            //             positions.Add(pos + dir3 + (constOffset.y - hight * 2) * Vector3.up);
            // 
            //             positions.Add(pos + dir1 * 2f + (constOffset.y) * Vector3.up);
            //             positions.Add(pos + dir2 * 2f + (constOffset.y) * Vector3.up);
            //             positions.Add(pos + dir3 * 2f + (constOffset.y) * Vector3.up);
            // 
            //             Vector3 pos4 = pos + dir4;
            //             pos4.y = 0.1f;
            //             positions.Add(pos4);
            // 
            //             Vector3 pos5 = pos + dir5;
            //             pos5.y = 0.1f;
            //             positions.Add(pos5);
            //             Vector3 pos6 = pos + dir6;
            //             pos6.y = 0.1f;
            //             positions.Add(pos6);
            pos.y = constOffset.y;
            positions.Add(pos);
            positions.Add(pos + dir1 * 2.5f + Vector3.up * 0.2f);
            positions.Add(pos + dir2 * 2.5f + Vector3.up * 0.2f);
            positions.Add(pos + dir3 * 2.5f + Vector3.up * 0.2f);
            positions.Add(pos + dir33 * 2.5f + Vector3.up * 0.2f);

            pos.y = 0.2f;
            positions.Add(pos + dir4);
            positions.Add(pos + dir5);
            positions.Add(pos + dir6);
            positions.Add(pos + dir66);
        }
        lightProbeGroup.probePositions = positions.ToArray();
    }

    private void GenLightProbePositions()
    {
        LightProbeGroup lightProbeGroup = GameObject.FindObjectOfType<LightProbeGroup>();
        if (lightProbeGroup == null)
        {
            Debug.LogError("未找到LightProbeGroup");
            return;
        }

        Light[] lights = GameObject.FindObjectsOfType<Light>();
        if (lights == null)
        {
            Debug.LogError("未找到Light");
            return;
        }


        Light light;
        int xCount = count.x < 2 ? 2 : count.x;
        int yCount = count.y < 2 ? 2 : count.y;
        int zCount = count.z < 2 ? 2 : count.z;

        List<Vector3> positions = new List<Vector3>(lights.Length * xCount * yCount * zCount + lights.Length);

        for (int i = 0; i < lights.Length; ++i)
        {
            light = lights[i];

            if (light.lightmapBakeType == LightmapBakeType.Realtime)
            {
                continue;
            }

            Vector3 pos = light.transform.position;

            Vector3 range = rangeSize;
            range.y = pos.y;

            positions.Add(new Vector3(pos.x, range.y * 0.5f, pos.z) + constOffset);

            Vector3 offset = -new Vector3(range.x * 0.5f, range.y, range.z * 0.5f);
            Vector3 layerSize = new Vector3(range.x / (xCount - 1), range.y / (yCount - 1), range.z / (zCount - 1));

            for (int y = 0; y < yCount; ++y)
            {
                for (int x = 0; x < xCount; ++x)
                {
                    for (int z = 0; z < zCount; ++z)
                    {
                        Vector3 point = pos + new Vector3(x * layerSize.x, y * layerSize.y, z * layerSize.z) + offset + constOffset;
                        positions.Add(point);
                    }
                }
            }
        }

        lightProbeGroup.probePositions = positions.ToArray();
    }

    public static void CollectLightMapMesh()
    {
        DayAndNightDynamic dayAndNightDynamic = GameObject.FindObjectOfType<DayAndNightDynamic>();
        if (dayAndNightDynamic == null)
        {
            Debug.LogWarning("未找到 DayAndNightDynamic");
            return;
        }

        List<Light> sceneLights = new List<Light>();
        GameObject[] sceneLightGameObjects = GameObject.FindGameObjectsWithTag(Tags.MainLight);
        if (sceneLightGameObjects != null)
        {
            for (int i = 0; i < sceneLightGameObjects.Length; ++i)
            {
                Light light = null;
                if (sceneLightGameObjects[i].TryGetComponent<Light>(out light) && light.type == LightType.Directional && light.lightmapBakeType == LightmapBakeType.Realtime)
                {
                    sceneLights.Add(light);
                }
            }
        }

        Light sceneLight = null;

        if (sceneLights.Count <= 0)
        {
            Debug.LogError("未找到场景主光源");
        }
        else
        {
            sceneLight = sceneLights[0];
            if (sceneLights.Count > 1)
            {
                Debug.LogError("有多个场景主光源, 将关闭其他的");
            }
        }

        dayAndNightDynamic.SetMainLights(sceneLight);

        GameObject[] nightLights = GameObject.FindGameObjectsWithTag(Tags.NightLight);
        dayAndNightDynamic.SetNightLights(nightLights);
        
        EditorUtility.SetDirty(dayAndNightDynamic);

        Debug.Log("设置昼夜参数 完成");        
    }
}
