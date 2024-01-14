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

public class KeyValuePair_2_UInt32_ILTypeInstance_Binder : ValueTypeBinder<KeyValuePair<System.UInt32, ILRuntime.Runtime.Intepreter.ILTypeInstance>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<uint, ILTypeInstance> ins, StackObject* ptr, IList<object> mStack)
    {
        var v1 = ILIntepreter.Minus(ptr, 1);
        uint key = *(uint*)&v1->Value;

        var v2 = ILIntepreter.Minus(ptr, 2);

        ins = new KeyValuePair<uint, ILTypeInstance>(key, (ILTypeInstance)mStack[v2->Value]);

        //UnityEngine.Debug.LogError("-----KeyValuePair_2_UInt32_ILTypeInstance_Binder"); 

    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<uint, ILTypeInstance> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(uint*)&v->Value = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        mStack[v->Value] = ins.Value;
    }
}
