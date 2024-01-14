using UnityEngine;

public class SceneNodeSplit : MonoBehaviour
{
    public enum EProcessType
    {
        Export = 0,
        MoveToParent,
    }

    [System.Serializable]
    public struct SplitData
    {
        public Transform transform;
        public EProcessType pProcessType;
    }

    public SplitData[] splitDatas;
}
