﻿using Framework;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector4Binder : ValueTypeBinder<Vector4>
{
    public override unsafe void AssignFromStack(ref Vector4 ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins.x = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 2);
        ins.y = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 3);
        ins.z = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 4);
        ins.w = *(float*)&v->Value;
    }

    public override unsafe void CopyValueTypeToStack(ref Vector4 ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(float*)&v->Value = ins.x;
        v = ILIntepreter.Minus(ptr, 2);
        *(float*)&v->Value = ins.y;
        v = ILIntepreter.Minus(ptr, 3);
        *(float*)&v->Value = ins.z;
        v = ILIntepreter.Minus(ptr, 4);
        *(float*)&v->Value = ins.w;

    }
}
