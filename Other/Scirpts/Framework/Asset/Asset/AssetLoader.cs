using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework
{
    public class AssetLoader : MonoBehaviour
    {
        public static int gCullLevelLimit = 32;

        [SerializeField]
        private int nCullLevel = 0;
        [SerializeField]
        private string sAssetName;
        [SerializeField]
        private bool usedInActived = true;
        [SerializeField]
        private Transform m_Parent;
        [SerializeField]
        private Vector3 m_Position;
        [SerializeField]
        private Quaternion m_Rotation;

        private AsyncOperationHandle<GameObject> handle;

        private void OnDestroy()
        {
            AddressablesUtil.ReleaseInstance(ref handle, null);
        }

        private void OnEnable()
        {
            if (nCullLevel <= gCullLevelLimit && !handle.IsValid())
            {
                //InstantiationParameters instantiationParameters = new InstantiationParameters(m_Position, m_Rotation, m_Parent);
                AddressablesUtil.InstantiateAsync(ref handle, sAssetName, OnLoaded, true, m_Parent);
            }
        }

        private void OnLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                Transform trans = handle.Result.transform;
                trans.localPosition = m_Position;
                trans.rotation = m_Rotation;
            }
        }

        private void OnDisable()
        {
            if (usedInActived)
            {
                AddressablesUtil.ReleaseInstance(ref handle, OnLoaded);
            }
        }
    }
}