using Unity.Mathematics;
using UnityEngine;

public class SendPositionToShader : MonoBehaviour
{    
    void Update()
    {
        //Shader.SetGlobalVector("_CollideInfo", new float4(transform.position, 0.6f));
        SceneInstanceRender.CollidePosition = transform.position;
    }
}
