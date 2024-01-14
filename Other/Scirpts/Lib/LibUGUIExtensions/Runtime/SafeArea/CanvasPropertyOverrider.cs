using System.Collections.Generic;
using UnityEngine;

/*
 * 该anchor调整计算方式依赖于当前脚本所在的recttransform的anchor为min(0, 0), max(1, 1)，最好position为(0, 0, 0)
 * 同时，拼UI必须在参考分辨率下拼，否则必然存在问题
 */
[RequireComponent(typeof(RectTransform))]
public class CanvasPropertyOverrider : MonoBehaviour {
    public bool isSafeCanvas = true;
    private static bool handleRight = true;

    #region
    // Landscape Left based
    public bool ignoreLX = false;
    public bool ignoreRX = false;

    public bool ignoreY = false;
    #endregion

    // Update Method
    public void UpdateCanvasProperty(int rootSortingOrder, Rect offset) {
        // 0. Get Value
        RectTransform myTransform = this.GetComponent<RectTransform>();
        Rect safeArea = this.GetSafeArea();
        Vector2 screen = new Vector2(Screen.width, Screen.height);

        Vector2 _saAnchorMin;
        Vector2 _saAnchorMax;

        var offset_left = offset.x;
        var offset_right = offset.y;
        var offset_top = offset.width;
        var offset_bottom = offset.height;

        // 1. Setup and apply safe area
        if (this.isSafeCanvas) {
            if (ScreenOrientation.LandscapeLeft == Screen.orientation) {
                if (this.ignoreRX) {
                    safeArea.width = Screen.width - safeArea.x;
                }
                if (this.ignoreLX) {
                    safeArea.width += safeArea.x;
                    safeArea.x = 0;
                }
            }
            else if (ScreenOrientation.LandscapeRight == Screen.orientation) {
                if (this.ignoreLX) {
                    safeArea.width = Screen.width - safeArea.x;
                }
                if (this.ignoreRX) {
                    safeArea.width += safeArea.x;
                    safeArea.x = 0;
                }
            }

            if (this.ignoreY) {
                safeArea.y = 0;
                safeArea.height = Screen.height;
            }

            _saAnchorMin.x = (safeArea.x + offset_left) / screen.x;
            _saAnchorMin.y = (safeArea.y + offset_bottom) / screen.y;

            _saAnchorMax.x = (safeArea.x + safeArea.width - offset_right) / screen.x;
            _saAnchorMax.y = (safeArea.y + safeArea.height - offset_top) / screen.y;

            myTransform.anchorMin = _saAnchorMin;
            myTransform.anchorMax = _saAnchorMax;
        }
        else {
            myTransform.anchorMin = Vector2.zero;
            myTransform.anchorMax = Vector2.one;
        }
    }

    #region 模拟
    public class DeviceSafeArea {
        public string name;
        public Rect rect;

        public DeviceSafeArea(string name, ref Rect rect) {
            this.name = name;
            this.rect = rect;
        }
        public DeviceSafeArea(string name, float x, float y, float width, float height) {
            this.name = name;
            this.rect = new Rect(x, y, width, height);
        }
    }
    private static readonly Dictionary<string, DeviceSafeArea> specialAreas = new Dictionary<string, DeviceSafeArea>() {
        /*
        // oppo a5
        {"OPPO PBAM00", new DeviceSafeArea("OPPO PBAM00", 53f, 0f, 1467f, 720f) },
        // oppo a7
        {"OPPO PBFM00", new DeviceSafeArea("OPPO PBFM00", 53f, 0f, 1467f, 720f) },
        {"OPPO PBFT00", new DeviceSafeArea("OPPO PBFT00", 53f, 0f, 1467f, 720f) },

        {"HUAWEI DUB-AL00", new DeviceSafeArea("HUAWEI DUB-AL00", 80f, 0f, 1440f, 720f) },
        */
    };

    // 用于热更外部传入
    public static void AddSpecialAreas(string name, float x, float y, float width, float height) {
        if (!specialAreas.TryGetValue(name, out DeviceSafeArea device)) {
            device = new DeviceSafeArea(name, x * 0.001f, y * 0.001f, width * 0.001f, height * 0.001f);
            specialAreas.Add(name, device);
        }
    }

    public static void HandleRight(bool target) {
        handleRight = target;
    }

    public static void Clear() {
        specialAreas.Clear();
    }

    private Rect GetSafeArea() {
        Rect safeArea = Screen.safeArea;
        // 横屏游戏，只考虑left/right即可
        if (specialAreas.TryGetValue(SystemInfo.deviceModel, out DeviceSafeArea device)) {
            if (handleRight) {
                if (ScreenOrientation.LandscapeRight == Screen.orientation) {
                    Rect GetR() {
                        float x = Screen.width - device.rect.x - device.rect.width;
                        float y = Screen.height - device.rect.y - device.rect.height;
                        float width = device.rect.width;
                        float height = device.rect.height;
                        return new Rect(x, y, width, height);
                    }
                    safeArea = GetR();
                }
                else if(ScreenOrientation.LandscapeLeft == Screen.orientation) {
                    safeArea = device.rect;
                }
            }
            else {
                safeArea = device.rect;
            }
        }

        return safeArea;
    }

    #endregion
}
