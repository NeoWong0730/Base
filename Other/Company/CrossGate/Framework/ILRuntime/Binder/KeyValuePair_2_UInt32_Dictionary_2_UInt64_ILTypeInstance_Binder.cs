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
using UnityEngine;
public class KeyValuePair_2_UInt32_Dictionary_2_UInt64_ILTypeInstance_Binder : ValueTypeBinder<KeyValuePair<uint, Dictionary<ulong, ILTypeInstance>>>
{

    public override unsafe void AssignFromStack(ref KeyValuePair<uint, Dictionary<ulong, ILTypeInstance>> ins, StackObject* ptr, IList<object> mStack)
    {
        var v1 = ILIntepreter.Minus(ptr, 1);
        uint key = *(uint*)&v1->Value;

        var v2 = ILIntepreter.Minus(ptr, 2);
        object value = mStack[v2->Value];

        ins = new KeyValuePair<uint, Dictionary<ulong, ILTypeInstance>>(key, (Dictionary<ulong, ILTypeInstance>)value);

        //UnityEngine.Debug.LogError("-----KeyValuePair_2_UInt32_Dictionary_2_UInt64_ILTypeInstance_Binder----------AssignFromStack--------------");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<uint, Dictionary<ulong, ILTypeInstance>> ins, StackObject* ptr, IList<object> mStack)
    {
        //UnityEngine.Debug.LogError("-----KeyValuePair_2_UInt32_Dictionary_2_UInt64_ILTypeInstance_Binder----------CopyValueTypeToStack--------------");
        var v = ILIntepreter.Minus(ptr, 1);
        *(uint*)&v->Value = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        mStack[v->Value] = ins.Value;
    }
}
