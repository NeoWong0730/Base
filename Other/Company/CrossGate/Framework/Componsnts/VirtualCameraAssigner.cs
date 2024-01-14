using UnityEngine;
using Cinemachine;

[System.Serializable]
public class VirtualCameraNode
{
    public CinemachineVirtualCamera camera;

    [Header("虚拟相机的lookat路径 对应的角色骨骼")]
    public string lookAtPath = "Neck";
    [Header("虚拟相机的Follow路径 对应的角色骨骼")]
    public string followPath = "Neck";
}

[DisallowMultipleComponent]
public class VirtualCameraAssigner : MonoBehaviour
{
    public VirtualCameraNode[] nodes = new VirtualCameraNode[0];

    public void Bind(Transform trans)
    {
        // 效率警告!!!
        foreach (var tr in trans.GetComponentsInChildren<Transform>())
        {
            for (int i = 0, length = nodes.Length; i < length; ++i)
            {
                if(!string.IsNullOrEmpty(nodes[i].lookAtPath) && tr.name.Contains(nodes[i].lookAtPath))
                {
                    nodes[i].camera.LookAt = tr;
                }
                if (!string.IsNullOrEmpty(nodes[i].followPath) && tr.name.Contains(nodes[i].followPath))
                {
                    nodes[i].camera.Follow = tr;
                }
            }
        }
    }
}
