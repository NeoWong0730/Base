using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EditorAssembliesEvent<T> where T : new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = new T();

            return _instance;
        }
    }

    //static EditorAssembliesEvent()
    //{
    //    //Types.GetType("UnityEditor.EditorAssemblies", "UnityEditor.dll");
    //    Type type = Assembly.Load("UnityEditor.dll").GetType("UnityEditor.EditorAssemblies");
    //    var method = type.GetMethod("SubclassesOf", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Type) }, null);
    //    var e = method.Invoke(null, new object[] { typeof(EditorAssembliesEvent) }) as IEnumerable;
    //    foreach (Type editorMonoBehaviourClass in e)
    //    {
    //        method = editorMonoBehaviourClass.BaseType.GetMethod("OnEditorMonoBehaviour", BindingFlags.NonPublic | BindingFlags.Instance);
    //        if (method != null)
    //        {
    //            method.Invoke(System.Activator.CreateInstance(editorMonoBehaviourClass), new object[0]);
    //        }
    //    }
    //}

    public void OnEnableEditorMonoBehaviour()
    {
        //EditorApplication.update += Update;
        //EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        //EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
        //EditorApplication.modifierKeysChanged += OnModifierKeysChanged;

        // globalEventHandler
        EditorApplication.CallbackFunction function = () => OnGlobalEventHandler(Event.current);
        FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
        EditorApplication.CallbackFunction functions = (EditorApplication.CallbackFunction)info.GetValue(null);
        functions += function;
        info.SetValue(null, (object)functions);

        //EditorApplication.searchChanged += OnSearchChanged;

        //EditorApplication.playModeStateChanged += (PlayModeStateChange playModeStateChange) => {
        //    if (EditorApplication.isPaused)
        //    {
        //        OnPlaymodeStateChanged(PlayModeState.Paused);
        //    }
        //    if (EditorApplication.isPlaying)
        //    {
        //        OnPlaymodeStateChanged(PlayModeState.Playing);
        //    }
        //    if (EditorApplication.isPlayingOrWillChangePlaymode)
        //    {
        //        OnPlaymodeStateChanged(PlayModeState.PlayingOrWillChangePlaymode);
        //    }
        //};
    }

    public void OnDisableEditorMonoBehaviour() 
    {
        // globalEventHandler
        FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
        info.SetValue(null, null);
    }

    public virtual void Update()
    {

    }

    public virtual void OnHierarchyWindowChanged()
    {

    }

    public virtual void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {

    }

    public virtual void OnProjectWindowChanged()
    {

    }

    public virtual void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
    {

    }

    public virtual void OnModifierKeysChanged()
    {

    }

    public virtual void OnGlobalEventHandler(Event e)
    {

    }

    public virtual void OnSearchChanged()
    {
    }

    public virtual void OnPlaymodeStateChanged(PlayModeState playModeState)
    {

    }

    public enum PlayModeState
    {
        Playing,
        Paused,
        Stop,
        PlayingOrWillChangePlaymode
    }
}
