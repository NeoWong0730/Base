using Lib.Core;
using UnityEngine;

using EUIState = Framework.Core.UI.EUIState;
using UIBaseClass = Framework.Core.UI.FUIBase;
using UIConfigData = Framework.Core.UI.UIConfigData;

namespace Logic.Core
{
    public static class UIManager
    {
        private static UIStack mStack = null;
        public static Transform mRoot { get; private set; }
        public static Camera mUICamera { get; private set; }
        public static void Init()
        {
            GameObject root = new GameObject("UIRoot");
            mRoot = root.transform;
            GameObject.DontDestroyOnLoad(root);

            mUICamera = CameraManager.mUICamera;

            //GameObject camera = new GameObject("UICamera");
            //camera.tag = "UICamera";
            //GameObject.DontDestroyOnLoad(camera);
            //
            //camera.transform.position = new Vector3(100f, 100f, 100f);
            //mUICamera = camera.AddComponent<Camera>();
            //mUICamera.orthographic = true;
            ////TODO:mask
            //mUICamera.cullingMask = LayerMask.GetMask("UI", "UnimportantUIFX");
            //mUICamera.cameraType = CameraType.Game;
            //mUICamera.clearFlags = CameraClearFlags.Nothing;
            //mUICamera.backgroundColor = Color.black;
            //mUICamera.depthTextureMode = DepthTextureMode.None;
            //mUICamera.allowMSAA = false;
            //mUICamera.allowHDR = false;
            //
            //UniversalAdditionalCameraData cameraData = mUICamera.GetUniversalAdditionalCameraData();
            //cameraData.requiresColorTexture = false;
            //cameraData.requiresDepthTexture = false;
            //cameraData.renderShadows = false;
            //
            //camera.AddComponent<AutoCameraOverlay>();

            //mStack = new UIStack(16);
            mStack = new UIStack();            
            mStack.Init(mRoot, mUICamera);
            mStack.eventEmitter.Handle<uint, bool>(UIStack.EUIStackEvent.RealHideMainCameraChange, OnRealHideMainCameraChange, true);
            mStack.eventEmitter.Handle<uint, bool>(UIStack.EUIStackEvent.ReduceFrameRateChange, OnReduceFrameRateChange, true);
            mStack.eventEmitter.Handle<uint, bool>(UIStack.EUIStackEvent.ReduceMainCameraQualityChange, OnReduceMainCameraQualityChange, true);
            //CameraManager.SetUICamera(mUICamera);
        }

        private static void OnReduceMainCameraQualityChange(uint stack, bool obj)
        {
            if (obj)
            {
                CameraManager.ReduceMainCameraQuality();
            }
            else
            {
                CameraManager.CancelReduceMainCameraQuality();
            }
        }

        private static void OnReduceFrameRateChange(uint stack, bool obj)
        {
            Logic.Core.OptionManager.Instance.SwitchReduceFrameRate(obj);
        }

        private static void OnRealHideMainCameraChange(uint stack, bool obj)
        {
            if (obj)
            {
                CameraManager.Hide();
            }
            else
            {
                CameraManager.CancelHide();
            }
        }

        public static EventEmitter<UIStack.EUIStackEvent> GetStackEventEmitter()
        {
            return mStack.eventEmitter;
        }

        public static void PreloadUI(EUIID id)
        {
            UIConfigData configData = UIConfig.GetConfData(id);
            if (configData != null)
            {
                mStack.PreloadUI((int)id, configData);
            }
        }
        public static void OpenUI(int id, bool immediate = false, object arg = null, int parentID = 0)
        {
            UIConfigData configData = UIConfig.GetConfData(id);
            if (configData != null)
            {
                mStack.OpenUI(id, configData, immediate, arg, parentID);
            }
        }
        public static void OpenUI(EUIID id, bool immediate = false, object arg = null, EUIID parentID = EUIID.Invalid)
        {
            if (id == EUIID.UI_Chat)
            {
                if (UIManager.IsShowState(EUIID.UI_FamilyBoss))
                    parentID = EUIID.UI_FamilyBoss;
            }

            UIConfigData configData = UIConfig.GetConfData(id);
            if (configData != null)
            {
                mStack.OpenUI((int)id, configData, immediate, arg, (int)parentID);
            }
        }

        public static void CloseUI(EUIID id, bool immediate = false, bool needDestroy = true)
        {
            mStack.HideUI((int)id, needDestroy ? EUIState.Destroy : EUIState.Close, immediate);
        }

        public static void SendMsg(EUIID id, object arg = null)
        {
            UIConfigData configData = UIConfig.GetConfData(id);
            if (configData != null)
            {
                mStack.SendMsg((int)id, arg);
            }
        }
        public static void ClearUI(bool destroy = true)
        {
            mStack.ClearStackUI(destroy);
            mStack.UpdateState();
        }

        public static void ClearUntilMain(bool destroy = true)
        {
            ClearUI(destroy);
            UpdateState();
            OpenUI(EUIID.UI_MainInterface);
        }

        /// <summary>
        /// 逻辑上打开包括隐藏的
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsOpen(EUIID id)
        {
            if (mStack.TryGetUI((int)id, out UIBaseClass ui))
            {
                return ui.isOpen;
            }
            return false;
        }

        /// <summary>
        /// 可见并且逻辑上打开的
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsVisibleAndOpen(EUIID id)
        {
            if (mStack.TryGetUI((int)id, out UIBaseClass ui))
            {
                return ui.isVisibleAndOpen;
            }
            return false;
        }

        /// <summary>
        /// 视觉上可见的(包括正在进行关闭动画的)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsVisible(EUIID id)
        {
            if (mStack.TryGetUI((int)id, out UIBaseClass ui))
            {
                return ui.isVisible;
            }
            return false;
        }

        /// <summary>
        /// 只处于显示状态(引导在战斗中特殊处理，有其他界面优于目标界面时处于不显示状态)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsShowState(EUIID id)
        {
            if (!mStack.TryGetUI((int)id, out UIBaseClass ui))
            {
                return false;
            }
            switch (id)
            {
                case EUIID.UI_MainBattle:
                    {
                        if (mStack.TryGetUI((int)EUIID.UI_MainInterface, out UIBaseClass uiMainInterface))
                        {
                            return uiMainInterface.eState == EUIState.Show;
                        }
                    }
                    break;
            }
            return ui.eState == EUIState.Show;
        }
        public static bool IsOpenState(EUIID id)
        {
            if (!mStack.TryGetUI((int)id, out UIBaseClass ui))
            {
                return false;
            }
            switch (id)
            {
                case EUIID.UI_MainBattle:
                    {
                        if (mStack.TryGetUI((int)EUIID.UI_MainInterface, out Framework.Core.UI.FUIBase uiMainInterface))
                        {
                            return uiMainInterface.isVisibleAndOpen;
                        }
                    }
                    break;
            }
            return ui.isVisibleAndOpen;
        }

        public static int TopUIID()
        {
            return mStack.TopUIID();
        }
        public static UIBase GetUI(int id)
        {
            return mStack.GetUI(id) as UIBase;
        }
        public static bool IsTop(int id)
        {
            return TopUIID() == id;
        }

        public static void UpdateState()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Sys_PCExpandChatUI.UpdateState();
#endif
            mStack.UpdateState();
        }
        public static void Update()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Sys_PCExpandChatUI.UpdateState();            
#endif
            mStack.UpdateState();

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Sys_PCExpandChatUI.Update();
#endif
            mStack.Update();
        }
        public static void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Sys_PCExpandChatUI.LateUpdate(deltaTime, unscaledDeltaTime);
#endif
            mStack.LateUpdate(deltaTime, unscaledDeltaTime);
            //UIBase top = mStack.TopUI();

            //Logic.Core.OptionManager.Instance.SwitchReduceFrameRate(mStack.nReduceFrameRateRef > 0);
            //             if (top != null && top.ContainsOptions(EUIOption.eReduceFrameRate))
            //             {
            //                 Logic.Core.OptionManager.Instance.SwitchReduceFrameRate(true);
            //             }
            //             else
            //             {
            //                 Logic.Core.OptionManager.Instance.SwitchReduceFrameRate(false);
            //             }
        }

        public static bool ReadyHideMainCamera()
        {
            return mStack.nReadyHideMainCameraRef > 0;
        }

        public static bool RealHideMainCamera()
        {
            return mStack.nRealHideMainCameraRef > 0;
        }

        public static bool ReduceFrameRate()
        {
            return mStack.nReduceFrameRateRef > 0;
        }

        public static bool ReduceMainCameraQuality()
        {
            return mStack.nReduceMainCameraQualityRef > 0;
        }

        public static void UnInit()
        {
            ClearUI();
        }

        public static void OnGUI_UIStack()
        {
            if (DebugUtil.IsOpenLogType((int)ELogType.eUIState))
            {
                mStack.OnGUI();
            }
        }

        #region 页面展示埋点
        private static int GetLastUIID(EUIID uiId)
        {
            return mStack.LastUIID();
        }

        static HitPointShowEvent showEvent = new HitPointShowEvent();
        static string gString_Zero = "0";
        static string gString_One = "1";
        static string gString_Three = "3";
        public static void HitPointShow(EUIID uiId, string subPage = "base")
        {
            return;
            if (!Sys_Role.Instance.hasSyncFinished)
                return;

            if (uiId == EUIID.UI_Menu
                || uiId == EUIID.UI_Joystick
                || uiId == EUIID.UI_UseItem
                || uiId == EUIID.UI_HornHUD
                || uiId == EUIID.UI_ChatSimplify
                || uiId == EUIID.UI_Buff
                || uiId == EUIID.HUD
                || uiId == EUIID.UI_Loading
                || uiId == EUIID.UI_Loading2
                || uiId == EUIID.UI_MapTips)
            {
                return;
            }

            showEvent.AppendBaseData();
            showEvent.page_id = subPage;
            showEvent.page_type = uiId.ToString();
            showEvent.type = gString_One;
            showEvent.ending = string.Empty;
            showEvent.duration = 0;
            showEvent.option = 0;
            showEvent.button_id = GetLastUIID(uiId);

            HitPointManager.HitPoint(HitPointShowEvent.Key, showEvent);
        }

        public static void HitPointHide(EUIID uiId, uint showTime, string subPage = "base", bool normal = true)
        {
            return;
            if (!Sys_Role.Instance.hasSyncFinished)
                return;

            if (uiId == EUIID.UI_Menu
                || uiId == EUIID.UI_Joystick
                || uiId == EUIID.UI_UseItem
                || uiId == EUIID.UI_HornHUD
                || uiId == EUIID.UI_ChatSimplify
                || uiId == EUIID.UI_Buff
                || uiId == EUIID.HUD
                || uiId == EUIID.UI_Loading
                || uiId == EUIID.UI_Loading2
                || uiId == EUIID.UI_MapTips)
            {
                return;
            }

            showEvent.AppendBaseData();
            showEvent.page_id = subPage;
            showEvent.page_type = uiId.ToString();
            showEvent.type = gString_Zero;
            showEvent.ending = normal ? gString_Zero : gString_Three;
            showEvent.duration = (Sys_Time.Instance.GetServerTime() - showTime);
            showEvent.option = 0;
            showEvent.button_id = 0;

            HitPointManager.HitPoint(HitPointShowEvent.Key, showEvent);
        }

        public static void HitPartition(EUIID uiId, uint typeId)
        {
            return;
            showEvent.AppendBaseData();
            showEvent.page_id = string.Empty;
            showEvent.page_type = uiId.ToString();
            showEvent.type = string.Empty;
            showEvent.ending = string.Empty;
            showEvent.duration = 0;
            showEvent.option = typeId;
            showEvent.button_id = 0;

            HitPointManager.HitPoint(HitPointShowEvent.Key, showEvent);
        }


        static HitPointClickEvent clickEvent = new HitPointClickEvent();
        /// <summary>
        /// 按钮事件
        /// subPageId 子界面名称(希望和页签对应的名字一一对应)
        /// </summary>
        public static void HitButton(EUIID UIId, string buttonId, string subPageId = "base") 
        {
            clickEvent.AppendBaseData();
            clickEvent.page_type = UIId.ToString();
            clickEvent.page_id = subPageId;
            clickEvent.button_id = buttonId;

            HitPointManager.HitPoint(HitPointClickEvent.Key, clickEvent);
        }

        /// <summary>
        /// GameKey埋点
        /// tag 事件名称 param 事件参数
        /// </summary>
        public static void HitGameKeyPoint(string tag, string param = "")
        {
            HitPointGameKey gameKeyEvent = new HitPointGameKey();

            gameKeyEvent.gameKey = SDKManager.GetUID();
            gameKeyEvent.eventTag = tag;
            gameKeyEvent.param = param;

            HitPointManager.HitPoint(HitPointGameKey.Key, gameKeyEvent);
        }
        #endregion
    }
}