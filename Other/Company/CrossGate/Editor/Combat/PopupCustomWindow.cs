using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PopupCustomWindow : EditorWindow   //PopupWindowContent
{
    private static PopupCustomWindow _window;
    public static PopupCustomWindow Window 
    {
        get 
        {
            if (_window == null)
                _window = new PopupCustomWindow();

            return _window;
        }
    }

    private int _selectedIndex;
    public int SelectedIndex 
    {
        get { return _selectedIndex; }
        set 
        {
            if (_selectedIndex != value) 
            {
                _selectedIndex = value;

                _changeSelectIndexAction?.Invoke(_selectedIndex);
            }
        }
    }

    private string[] _popupContents;

    private static GUIStyle _textStyle;
    private static Color _hoverItmeColor = new Color(0.45f, 0.71f, 1f, 1f);
    private static Color _windowBgColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    private float _itemHeight = 25f;

    private Action<int> _changeSelectIndexAction;

    private float _colWidth;
    private float _colHeight;

    private int _row;
    private int _col;

    public static void ShowPopupCustomWindow(Rect position, int selectedIndex, string[] displayedOptions, GUIStyle style, Action<int> action) 
    {
        if (GUI.Button(position, displayedOptions[selectedIndex], style)) 
        {
            Window.ShowPopup();
            Window._selectedIndex = selectedIndex;
            Window._popupContents = displayedOptions;
            Window._changeSelectIndexAction = action;
            Window.Init();
        }
    }

    public static void ShowPopupCustomWindow(int selectedIndex, string[] displayedOptions, GUIStyle style, Action<int> action)
    {
        if (GUILayout.Button(displayedOptions[selectedIndex], style))
        {
            Window.ShowPopup();
            Window._selectedIndex = selectedIndex;
            Window._popupContents = displayedOptions;
            Window._changeSelectIndexAction = action;
            Window.Init();
        }
    }

    public void Init()
    {
        float posX = mouseOverWindow != null ? mouseOverWindow.position.x + 250 : Event.current.mousePosition.x;

        float maxHeight = Screen.currentResolution.height - 80f;

        int length = _popupContents.Length;

        _row = Mathf.FloorToInt(maxHeight / _itemHeight);
        _col = Mathf.CeilToInt((float)length / _row);

        maxHeight = _row * _itemHeight + 5f;

        _colWidth = 300f;
        float heightSum = _itemHeight * length;
        if (heightSum < Screen.currentResolution.height)
        {
            _colHeight = heightSum;
            position = new Rect(posX, Event.current.mousePosition.y, _colWidth, _colHeight);
        }
        else 
        {
            _colHeight = maxHeight;
            position = new Rect(posX, 20f, _colWidth * _col, _colHeight);
        }
    }

    public void OnDestroy()
    {
        _changeSelectIndexAction = null;
    }

    public void Update()
    {
        Repaint();
    }

    void OnLostFocus()
    {
        Close();
    }
    
    private static void GenerateStyles()
    {
        if (_textStyle != null)
            return;

        _textStyle = new GUIStyle(EditorStyles.label);
        _textStyle.fontSize = 13;
        _textStyle.alignment = TextAnchor.MiddleLeft;
        _textStyle.normal.textColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        _textStyle.focused.textColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    }

    public void OnGUI()
    {
        if (_popupContents == null || _popupContents.Length <= 0)
        {
            Close();
            return;
        }

        GenerateStyles();

        EditorGUI.DrawRect(new Rect(0, 0, _colWidth * _col, _colHeight), _windowBgColor);
        bool isOver = false;
        for (int i = 0; i < _col; i++)
        {
            float y = 5f;
            float x = 2f + i * _colWidth;

            for (int j = 0; j < _row; j++)
            {
                int index = j + i * _row;
                if (index >= _popupContents.Length)
                {
                    isOver = true;
                    break;
                }

                string content = _popupContents[index];

                Vector2 mousePos = Event.current.mousePosition;
                if (mousePos.x >= x && mousePos.x <= x + _colWidth &&
                    mousePos.y >= y && mousePos.y <= y + _itemHeight)
                {
                    EditorGUI.DrawRect(new Rect(x, y, _colWidth, _itemHeight), _hoverItmeColor);
                    if (GUI.Button(new Rect(x, y, _colWidth, _itemHeight), string.Empty, EditorStyles.label))
                    {
                        SelectedIndex = index;
                        Close();
                        return;
                    }
                }

                if (index == SelectedIndex)
                {
                    EditorGUI.DrawRect(new Rect(x, y, 25f, _itemHeight), _hoverItmeColor);
                    GUI.Toggle(new Rect(x + 5f, y, 10f, _itemHeight), true, string.Empty);
                }

                EditorGUI.LabelField(new Rect(x + 25f, y, _colWidth - 25f, _itemHeight), content, _textStyle);

                y += _itemHeight;
            }

            if (isOver)
                break;
        }
    }
}