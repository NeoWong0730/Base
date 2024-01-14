#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using Logic.Core;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Framework.Core.UI;

namespace Logic
{
    public class Sys_PCExpandChatUI : SystemModuleBase<Sys_PCExpandChatUI>
    {
        public int WinScreenX, WinScreenY; //win屏幕分辨率

        private bool mExpandResolution; //扩展
        private bool mReturnResolution; //还原
        public Camera mExpandCamera;
        private Rect mCameraRect;

        private int mExpandwidth = 700;
        private float ratio; //原屏宽度占比

        private AsyncOperationHandle<GameObject> mHandle;

#region 系统函数
        public override void OnLogin()
        {
            base.OnLogin();
        }
        public override void OnLogout()
        {
            base.OnLogout();
            if (AspectRotioController.IsExpandState)
                ChangeScreenResolution(!AspectRotioController.IsExpandState);
        }
#endregion

        public void WinInitData()
        {
            mCameraRect = new Rect(0, 0, 1, 1);
            CameraManager.mCamera.rect = mCameraRect;
            CameraManager.mUICamera.rect = mCameraRect;
            InitExpand();
        }

        private void TurnResolution()
        {
            if (mExpandResolution)
            {
                int expandwidth = GetExpandWidth();
                ratio = (float)Screen.width / (Screen.width + expandwidth);
                mCameraRect.width = ratio;
                AspectRotioController.Instance.SetNewResolution(Screen.width + expandwidth, Screen.height, true);
                CameraManager.mCamera.rect = mCameraRect;
                CameraManager.mUICamera.rect = mCameraRect;
                mExpandCamera.rect = new Rect(ratio, 0, 1 - ratio, 1);
                foreach (Camera camera in Camera.allCameras)
                {
                    AutoCameraStack cameraStack;
                    if (camera.TryGetComponent<AutoCameraStack>(out cameraStack))
                    {
                        if (camera != mExpandCamera && camera.rect != CameraManager.mCamera.rect)
                            camera.rect = CameraManager.mCamera.rect;
                    }
                }

                mExpandResolution = false;
            }
            if(mReturnResolution)
            {
                AspectRotioController.Instance.SetNewResolution(ratio * Screen.width, Screen.height, true);
                mReturnResolution = false;
            }
        }

        private void ReturnAllCameraRect()
        {
            mCameraRect.x = 0;
            mCameraRect.y = 0;
            mCameraRect.width = 1;
            mCameraRect.height = 1;
            foreach (Camera camera in Camera.allCameras)
            {
                if (camera != mExpandCamera)
                    camera.rect = mCameraRect;
            }
        }

        public void ChangeScreenResolution(bool isExpand = false)
        {
            if (isExpand)
            {
                if (!IsCanExpamdUI())
                    return;
                UIManager.CloseUI(EUIID.UI_Chat, true);
                AspectRotioController.IsExpandState = true;
                AddressablesUtil.InstantiateAsync(ref mHandle, "UI/UICameraExpandUI.prefab", OnAssetLoaded);
            }
            else
            {
                CloseUI(EUIID.UI_ChatPCExpand, true);
                mReturnResolution = true;
                mExpandResolution = false;
                CameraManager.mCamera.rect = new Rect(0, 0, 1, 1);
                //CameraManager.mUICamera.rect = new Rect(0, 0, 1, 1);
                ReturnAllCameraRect();
                AddressablesUtil.ReleaseInstance(ref mHandle, OnAssetLoaded);
                AspectRotioController.IsExpandState = false;
            }
        }

        private void OnAssetLoaded(AsyncOperationHandle<GameObject> obj)
        {
            if (obj.Result == null)
                return;
            GameObject gameObject = obj.Result;
            gameObject.name = "UICameraExpand";
            mExpandCamera = gameObject.transform.GetComponent<Camera>();
            mStack.Init(mRoot, mExpandCamera);
            OpenUI(EUIID.UI_ChatPCExpand);
            Object.DontDestroyOnLoad(mExpandCamera);
            mExpandResolution = true;
        }

        private int GetExpandWidth()
        {
            if (AspectRotioController.Instance.curRatio == Enum_Ratio.Type_2)
                mExpandwidth = Mathf.RoundToInt(Screen.width * 0.482f);
            else if (AspectRotioController.Instance.curRatio == Enum_Ratio.Type_1)
                mExpandwidth = Mathf.RoundToInt(Screen.width * 0.64f);
            else
                mExpandwidth = Mathf.RoundToInt(Screen.width * 0.370f);
            return mExpandwidth;
        }

        public bool IsCanExpamdUI()
        {
            WinScreenX = AspectRotioController.Instance.pixelWidthOfCurrentScreen;
            int expandWidth = GetExpandWidth() + Screen.width;
            if (expandWidth > WinScreenX)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1012021));
                return false;
            }
            return true;
        }


#region ExpandUIManager
        private static UIStack mStack = null;
        private Transform mRoot;
        public void InitExpand()
        {
            GameObject root = new GameObject("UIExpandRoot");
            mRoot = root.transform;
            GameObject.DontDestroyOnLoad(root);
            mStack = new UIStack();
        }

        public static void OpenUI(EUIID id, bool immediate = false, object arg = null, EUIID parentID = EUIID.Invalid)
        {
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

        public static void UpdateState()
        {
            if (mStack != null)
                mStack.UpdateState();
        }

        public static void Update()
        {
            if (mStack != null)
                mStack.Update();
        }
        public static void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (mStack != null)
            {
                mStack.LateUpdate(deltaTime, unscaledDeltaTime);
                Instance.TurnResolution();
            }
        }
#endregion
    }
}
#endif


