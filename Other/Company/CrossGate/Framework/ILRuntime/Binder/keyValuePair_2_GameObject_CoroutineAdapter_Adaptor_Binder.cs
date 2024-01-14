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

public class keyValuePair_2_GameObject_CoroutineAdapter_Adaptor_Binder : ValueTypeBinder<KeyValuePair<UnityEngine.GameObject, global::CoroutineAdapter.Adaptor>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<GameObject, CoroutineAdapter.Adaptor> ins, StackObject* ptr, IList<object> mStack)
    {
        var v1 = ILIntepreter.Minus(ptr, 1);

        var v2 = ILIntepreter.Minus(ptr, 2);

        ins = new KeyValuePair<GameObject, CoroutineAdapter.Adaptor>((GameObject)mStack[v1->Value], (CoroutineAdapter.Adaptor)mStack[v2->Value]);

        //UnityEngine.Debug.LogError("-----keyValuePair_2_GameObject_CoroutineAdapter_Adaptor_Binder");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<GameObject, CoroutineAdapter.Adaptor> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        mStack[v->Value] = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        mStack[v->Value] = ins.Value;
    }
}
