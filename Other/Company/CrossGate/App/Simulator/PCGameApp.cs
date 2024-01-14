#if UNITY_STANDALONE_WIN
using Framework;
using UnityEngine;
using UnityEngine.EventSystems;

public class PCGameApp : MonoBehaviour
{
    public GameObject mSimulator = null;
    public GameObject mWinTitle = null;
    public Camera mUICamera;
    public EventSystem mEventSystem;

    private void Awake()
    {
        //WinTitle.Instance.Awake();
        Debug.LogError("PCGameApp");
        if (mWinTitle != null)
            mWinTitle.AddComponent<WinTitle>();
        if (mSimulator != null)
            mSimulator.AddComponent<UISimulator>();
        //WinTitle.Instance.Awake();
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR

#endif

        CameraManager.SetUICamera(mUICamera);
        AppManager.mEventSystem = mEventSystem;
    }
}
#endif