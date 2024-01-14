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

public class RaycastHitBinder : ValueTypeBinder<RaycastHit>
{
    public override unsafe void AssignFromStack(ref RaycastHit ins, StackObject* ptr, IList<object> mStack)
    {
        //var v = ILIntepreter.Minus(ptr, 1);
        //ins.collider = *(float*)&v->Value;
        //v = ILIntepreter.Minus(ptr, 2);
        //ins.y = *(float*)&v->Value;
    }

    
    public override unsafe void CopyValueTypeToStack(ref RaycastHit ins, StackObject* ptr, IList<object> mStack)
    {
   
    }
}
