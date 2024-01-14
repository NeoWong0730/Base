using System;
using UnityEngine;

namespace Framework
{
    public class AppHooker : MonoBehaviour
    {
        public System.Action<Vector2Int> onScreenResolutionChanged;
        private int lastWidth = Screen.width;
        private int lastHeight = Screen.height;

        public System.Action<Rect> onScreenSafeAreaChanged;
        [SerializeField] private Rect lastScreenSafeArea;

        public System.Action<NetworkReachability, NetworkReachability> onNetworkReachabilityChanged;
        [SerializeField] private NetworkReachability lastNetworkReachability;

        public System.Action<bool, bool> onNetworkStatusChanged;
        [SerializeField] private bool lastNetworkStatus;


        float timer;

        private void Awake()
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;

            lastScreenSafeArea = Screen.safeArea;

            lastNetworkReachability = Application.internetReachability;
            lastNetworkStatus = NetworkHelper.IsWanOrLanOpen();
        }

        private void Update()
        {
            timer++;
            if (timer >= 0.5f)
            {
                timer = 0;
                JudgeNetworkReachability();
            }

            JudgeScreenSafeArea();
            JudgeScreenResolution();
        }

        #region ¸¨Öú¼¶
        private void JudgeNetworkReachability()
        {
            //NetworkReachability currentNetworkReachability = Application.internetReachability;
            //if (lastNetworkReachability != currentNetworkReachability)
            //{
            //    onNetworkReachabilityChanged?.Invoke(lastNetworkReachability, currentNetworkReachability);
            //    lastNetworkReachability = currentNetworkReachability;
            //}

            bool current = NetworkHelper.IsWanOrLanOpen();
            if (lastNetworkStatus != current)
            {
                onNetworkStatusChanged?.Invoke(lastNetworkStatus, current);
                lastNetworkStatus = current;
            }
        }
        private void JudgeScreenSafeArea()
        {
            Rect currentScreenSafeArea = Screen.safeArea;
            if (lastScreenSafeArea != currentScreenSafeArea)
            {
                onScreenSafeAreaChanged?.Invoke(currentScreenSafeArea);
                lastScreenSafeArea = currentScreenSafeArea;
            }
        }
        private void JudgeScreenResolution()
        {
            if (Screen.width != lastWidth || Screen.height != lastHeight)
            {
                onScreenResolutionChanged?.Invoke(new Vector2Int(Screen.width, Screen.height));
                lastWidth = Screen.width;
                lastHeight = Screen.height;
            }
        }
        #endregion
    }
}