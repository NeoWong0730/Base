using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PosConvertUtil
{
    public static Vector3 Svr2Client(uint posX, uint posY)
    {
        Vector3 clientPos = Vector3.zero;
        Vector3 rayPos = Vector3.zero;

        clientPos.x = rayPos.x = posX / 100f;
        clientPos.z = rayPos.z = -posY / 100f;
        rayPos.y = 20f;

        Ray ray = new Ray();
        ray.origin = rayPos;
        ray.direction = -Vector3.up;
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, 40, (int)ELayerMask.Terrain))
        {
            clientPos = rayHit.point;
        }

        return clientPos;
    }

    public static Vector2 Client2Svr(Vector3 pos)
    {
        return new Vector2((uint)Mathf.RoundToInt(pos.x * 100f), (uint)Mathf.RoundToInt(-pos.z * 100f));
    }
}


