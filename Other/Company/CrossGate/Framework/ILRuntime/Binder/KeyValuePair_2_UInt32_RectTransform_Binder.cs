using System;
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

public class KeyValuePair_2_UInt32_RectTransform_Binder : ValueTypeBinder<KeyValuePair<System.UInt32, UnityEngine.RectTransform>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<uint, RectTransform> ins, StackObject* ptr, IList<object> mStack)
    {
        var v1 = ILIntepreter.Minus(ptr, 1);
        System.UInt32 key = *(uint*)&v1->Value;

        var v2 = ILIntepreter.Minus(ptr, 2);

        ins = new KeyValuePair<uint, RectTransform>(key, (RectTransform)mStack[v2->Value]);
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<uint, RectTransform> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(uint*)&v->Value = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        mStack[v->Value] = ins.Value;
    }
}
