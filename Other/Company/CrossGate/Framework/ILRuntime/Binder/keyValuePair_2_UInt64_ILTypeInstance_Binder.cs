﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

public class keyValuePair_2_UInt64_ILTypeInstance_Binder : ValueTypeBinder<KeyValuePair<System.UInt64, ILRuntime.Runtime.Intepreter.ILTypeInstance>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<ulong, ILTypeInstance> ins, StackObject* ptr, IList<object> mStack)
    {
        var v1 = ILIntepreter.Minus(ptr, 1);
        ulong key = *(ulong*)&v1->Value;

        var v2 = ILIntepreter.Minus(ptr, 2);

        ins = new KeyValuePair<ulong, ILTypeInstance>(key, (ILTypeInstance)mStack[v2->Value]);

        //UnityEngine.Debug.LogError("-----keyValuePair_2_UInt64_ILTypeInstance_Binder");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<ulong, ILTypeInstance> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(ulong*)&v->Value = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        mStack[v->Value] = ins.Value;
    }
}
