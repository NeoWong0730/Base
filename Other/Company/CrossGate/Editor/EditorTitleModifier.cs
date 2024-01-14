using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// https://github.com/XINCGer/UnityToolchainsTrick/tree/main/Assets/Editor/Examples/Example_20_TitleModifier
#if UNITY_EDITOR
[InitializeOnLoad]
public class TitleModifier {
    private static object[] args = new object[1];

    static TitleModifier() {
        Type editor = typeof(EditorApplication);

        Type titleDescriptor = editor.Assembly.GetTypes().First(x => x.FullName == "UnityEditor.ApplicationTitleDescriptor");
        Type delegateType = typeof(Action<>).MakeGenericType(titleDescriptor);
        MethodInfo methodInfo = ((Action<object>) UpdateWindowTitle).Method;
        Delegate del = Delegate.CreateDelegate(delegateType, null, methodInfo);

        args[0] = del;
        EventInfo updateTitle = editor.GetEvent("updateMainWindowTitle", BindingFlags.Static | BindingFlags.NonPublic);
        updateTitle?.GetRemoveMethod(true).Invoke(null, args);
        updateTitle?.GetAddMethod(true).Invoke(null, args);
    }

    static void UpdateWindowTitle(object desc) {
        var fieldInfo = typeof(EditorApplication).Assembly.GetTypes().First(x => x.FullName == "UnityEditor.ApplicationTitleDescriptor").GetField("title", BindingFlags.Instance | BindingFlags.Public);
        var str = fieldInfo.GetValue(desc) as string;
        fieldInfo.SetValue(desc, $"{str} --> {Application.dataPath}");
    }
}
#endif