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

public class ResolutionBinder : ValueTypeBinder<Resolution>
{
    public override unsafe void AssignFromStack(ref Resolution ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins.width = v->Value;
        v = ILIntepreter.Minus(ptr, 2);
        ins.height = v->Value;
        v = ILIntepreter.Minus(ptr, 3);
        ins.refreshRate = v->Value;
  
    }

    public override unsafe void CopyValueTypeToStack(ref Resolution ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        v->Value = ins.width;
        v = ILIntepreter.Minus(ptr, 2);
        v->Value = ins.height;
        v = ILIntepreter.Minus(ptr, 3);
        v->Value = ins.refreshRate;
    }
}
