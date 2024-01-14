using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Reflection;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Stack;
using UnityEngine.AI;

public class NavMeshHitBinder : ValueTypeBinder<NavMeshHit>
{
    public override unsafe void AssignFromStack(ref NavMeshHit ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        float x1 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 2);
        float y1 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 3);
        float z1 = *(float*)&v->Value;
        ins.position = new Vector3(x1, y1, z1);
        
        v = ILIntepreter.Minus(ptr, 5);
        float x2 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 6);
        float y2 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 7);
        float z2 = *(float*)&v->Value;
        ins.normal = new Vector3(x2,y2,z2);
        
        v = ILIntepreter.Minus(ptr, 8);
        ins.distance = *(float*)&v->Value;

        v = ILIntepreter.Minus(ptr, 9);
        ins.mask = v->Value;

        v = ILIntepreter.Minus(ptr, 10);
        ins.hit =  *(bool*)&v->Value;

       // Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eTestILruntime, "yd----NavMeshHitBinder------AssignFromStack------------");
    }

    public override unsafe void CopyValueTypeToStack(ref NavMeshHit ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(float*)&v->Value = ins.position.x;
        v = ILIntepreter.Minus(ptr, 2);
        *(float*)&v->Value = ins.position.y;
        v = ILIntepreter.Minus(ptr, 3);
        *(float*)&v->Value = ins.position.z;

        v = ILIntepreter.Minus(ptr, 5);
        *(float*)&v->Value = ins.normal.x;
        v = ILIntepreter.Minus(ptr, 6);
        *(float*)&v->Value = ins.normal.y;
        v = ILIntepreter.Minus(ptr, 7);
        *(float*)&v->Value = ins.normal.z;

        v = ILIntepreter.Minus(ptr, 8);
        *(float*)&v->Value = ins.distance;


        v = ILIntepreter.Minus(ptr, 9);
        v->Value = ins.mask;

        v = ILIntepreter.Minus(ptr, 10);
        *(bool*)&(v->Value) = ins.hit;

       // Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eTestILruntime, "yd----NavMeshHitBinder------CopyValueTypeToStack------------");
    }
}
