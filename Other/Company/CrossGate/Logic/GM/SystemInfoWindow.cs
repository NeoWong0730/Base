using Lib.Core;
using System;
using UnityEngine;

public class SystemInfoWindow : DebugWindowBase
{
    string systemInfo = null;
    Vector2 systemInfoPos = Vector2.zero;
    int mb = 1024 * 1024;

    string input1 = string.Empty;
    string input2 = string.Empty;
    string input3 = string.Empty;
    string input4 = string.Empty;

    public SystemInfoWindow(int id) : base(id) { }

    public override void WindowFunction(int id)
    {
        if (systemInfo == null)
        {
            Type type = typeof(SystemInfo);
            System.Reflection.PropertyInfo[] propertyInfos = type.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty);
            System.Text.StringBuilder sb = StringBuilderPool.GetTemporary();
            for (int i = 0; i < propertyInfos.Length; ++i)
            {
                sb.AppendFormat("{0} = {1}", propertyInfos[i].Name, type.InvokeMember(propertyInfos[i].Name, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty, null, null, null));
                sb.AppendLine();
            }
            sb.AppendLine();

            type = typeof(Environment);
            propertyInfos = type.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty);
            for (int i = 0; i < propertyInfos.Length; ++i)
            {
                if (propertyInfos[i].Name.Equals("TickCount"))
                    continue;

                if (propertyInfos[i].Name.Equals("NewLine"))
                    continue;

                if (propertyInfos[i].Name.Equals("CommandLine"))
                    continue;

                if (propertyInfos[i].Name.Equals("StackTrace"))
                    continue;

                sb.AppendFormat("{0} = {1}", propertyInfos[i].Name, type.InvokeMember(propertyInfos[i].Name, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty, null, null, null));
                sb.AppendLine();
            }

            systemInfo = StringBuilderPool.ReleaseTemporaryAndToString(sb);
            sb.Clear();
        }
        systemInfoPos = GUILayout.BeginScrollView(systemInfoPos);

        if (Logic.Core.OptionManager.Instance != null)
        {
            GUILayout.Label("fPerformance Score = " + Logic.Core.OptionManager.Instance.nPerformanceScore.ToString());
        }

        GUILayout.Label("TickCount = " + Environment.TickCount.ToString());
        GUILayout.Label("HeapSize = " + UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong() / mb);
        GUILayout.Label("UsedSize = " + UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() / mb);
        GUILayout.Label("TotalAllocatedSize = " + UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / mb);
        GUILayout.Label("TotalReservedSize = " + UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / mb);
        GUILayout.Label("TotalUnusedReserved = " + UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong() / mb);
        GUILayout.Label("TempAllocatorSize = " + UnityEngine.Profiling.Profiler.GetTempAllocatorSize() / mb);

        GUILayout.Label("Texture.currentTextureMemory = " + (Texture.currentTextureMemory / (uint)mb).ToString());
        GUILayout.Label("Texture.desiredTextureMemory （不使用串流纹理预算会使用的内存）= " + (Texture.desiredTextureMemory / (uint)mb).ToString());
        GUILayout.Label("Texture.totalTextureMemory (使用mipmap级别0将使用的内存)= " + (Texture.totalTextureMemory / (uint)mb).ToString());
        GUILayout.Label("Texture.targetTextureMemory (使用串流实际使用内存)= " + (Texture.targetTextureMemory / (uint)mb).ToString());
        GUILayout.Label("Texture.nonStreamingTextureMemory = " + (Texture.nonStreamingTextureMemory / (uint)mb).ToString());

        GUILayout.Label("Texture.streamingTextureLoadingCount = " + Texture.streamingTextureLoadingCount.ToString());
        GUILayout.Label("Texture.streamingTexturePendingLoadCount = " + Texture.streamingTexturePendingLoadCount.ToString());

        GUILayout.Label("Texture.streamingMipmapUploadCount = " + Texture.streamingMipmapUploadCount.ToString());
        GUILayout.Label("Texture.nonStreamingTextureCount = " + Texture.nonStreamingTextureCount.ToString());
        GUILayout.Label("Texture.streamingTextureCount = " + Texture.streamingTextureCount.ToString());
        GUILayout.Label("Texture.streamingRendererCount = " + Texture.streamingRendererCount.ToString());

        QualitySettings.streamingMipmapsActive = GUILayout.Toggle(QualitySettings.streamingMipmapsActive, "QualitySettings.streamingMipmapsActive");
        QualitySettings.streamingMipmapsMemoryBudget = GUI_FloatButton("QualitySettings.streamingMipmapsMemoryBudget = ", ref input1, QualitySettings.streamingMipmapsMemoryBudget);
        GUI_IntButton("QualitySettings.streamingMipmapsRenderersPerFrame = " , ref input2, QualitySettings.streamingMipmapsRenderersPerFrame);
        QualitySettings.streamingMipmapsMaxLevelReduction = GUI_IntButton("QualitySettings.streamingMipmapsMaxLevelReduction = " , ref input3, QualitySettings.streamingMipmapsMaxLevelReduction);
        QualitySettings.streamingMipmapsMaxFileIORequests = GUI_IntButton("QualitySettings.streamingMipmapsMaxFileIORequests = " , ref input4, QualitySettings.streamingMipmapsMaxFileIORequests);
        Texture.streamingTextureDiscardUnusedMips = GUILayout.Toggle(Texture.streamingTextureDiscardUnusedMips, "Texture.streamingTextureDiscardUnusedMips");

        GUILayout.Label(systemInfo);
        //GUILayout.Label(Environment.StackTrace);
        GUILayout.EndScrollView();
    }

    private float GUI_FloatButton(string name, ref string input, float f)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name + f.ToString());
        input = GUILayout.TextField(input);
        if (GUILayout.Button("apply"))
        {
            if (float.TryParse(input, out float v))
            {
                f = v;
            }
        }
        GUILayout.EndHorizontal();
        return f;
    }

    private int GUI_IntButton(string name, ref string input, int f)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name + f.ToString());
        input = GUILayout.TextField(input);
        if (GUILayout.Button("apply"))
        {
            if (int.TryParse(input, out int v))
            {
                f = v;
            }
        }
        GUILayout.EndHorizontal();
        return f;
    }
}