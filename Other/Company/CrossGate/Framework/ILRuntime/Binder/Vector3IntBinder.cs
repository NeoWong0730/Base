﻿using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Reflection;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Stack;

public class Vector3IntBinder : ValueTypeBinder<Vector3Int>
{
    public override unsafe void AssignFromStack(ref Vector3Int ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins.x = v->Value;
        v = ILIntepreter.Minus(ptr, 2);
        ins.y = v->Value;
        v = ILIntepreter.Minus(ptr, 3);
        ins.z = v->Value;
    }

    public override unsafe void CopyValueTypeToStack(ref Vector3Int ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        v->Value = ins.x;
        v = ILIntepreter.Minus(ptr, 2);
        v->Value = ins.y;
        v = ILIntepreter.Minus(ptr, 3);
        v->Value = ins.z;

    }
}
