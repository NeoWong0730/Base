using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RawImage))]
public class RawTextureClip : MonoBehaviour
{
    private Vector2 _size;
    private Vector2 _normalPosition;

    public Vector2 Size
    {
        get { return _size; }
        set
        {
            if (_size != value)
            {
                _size = value;
                Refresh();
            }
        }
    }

    public Vector2 NormalPosition
    {
        get { return _normalPosition; }
        set
        {
            if (_normalPosition != value)
            {
                _normalPosition = value;
                Refresh();
            }
        }
    }

    public void Refresh()
    {
        Vector2 size = new Vector2(rawImage.texture.width, rawImage.texture.height);
        Vector2 normalSize = _size / size;

        rawImage.uvRect = new Rect(_normalPosition - normalSize * 0.5f, normalSize);
    }

    private RawImage rawImage;
    private void Start()
    {
        rawImage = GetComponent<RawImage>();
        RectTransform rectTransform = transform as RectTransform;
        if(rectTransform)
        {
            Size = rectTransform.sizeDelta;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RawTextureClip))]
public class UIMapInsprctor : Editor
{
    RawTextureClip _uIMap = null;
    RawTextureClip uIMap
    {
        get
        {
            if (_uIMap == null)
                _uIMap = target as RawTextureClip;
            return _uIMap;
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        uIMap.Size = EditorGUILayout.Vector2Field("Size", uIMap.Size);
        uIMap.NormalPosition = EditorGUILayout.Vector2Field("Normal Position", uIMap.NormalPosition);
    }
}
#endif