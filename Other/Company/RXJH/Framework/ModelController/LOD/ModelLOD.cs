using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ModelLOD : MonoBehaviour
{
    public int nLOD;
    public IModel mModel;

    private int CalculateLOD()
    {
        return 0;
    }

    private void Update()
    {
        int newLOD = CalculateLOD();
        if (newLOD != nLOD)
        {
            mModel.OnLODChange(nLOD, newLOD);
            nLOD = newLOD;
        }
    }
}
