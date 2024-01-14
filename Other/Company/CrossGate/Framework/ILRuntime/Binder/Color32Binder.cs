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


public unsafe class Color32Binder : ValueTypeBinder<Color32>
{
    public override unsafe void AssignFromStack(ref Color32 ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins.r = (byte)v->Value;
        v = ILIntepreter.Minus(ptr, 2);
        ins.g = (byte)v->Value;
        v = ILIntepreter.Minus(ptr, 3);
        ins.b = (byte)v->Value;
        v = ILIntepreter.Minus(ptr, 4);
        ins.a = (byte)v->Value;
    }

    public override unsafe void CopyValueTypeToStack(ref Color32 ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        v->Value = ins.r;
        v = ILIntepreter.Minus(ptr, 2);
        v->Value = ins.g;
        v = ILIntepreter.Minus(ptr, 3);
        v->Value = ins.b;
        v = ILIntepreter.Minus(ptr, 4);
        v->Value = ins.a;
    }
}
