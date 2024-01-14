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


public class keyValuePair_2_UInt64_CoroutineAdapter_Adaptor_Binder : ValueTypeBinder<KeyValuePair<System.UInt64, global::CoroutineAdapter.Adaptor>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<ulong, CoroutineAdapter.Adaptor> ins, StackObject* ptr, IList<object> mStack)
    {
        var v1 = ILIntepreter.Minus(ptr, 1);
        ulong key = *(ulong*)&v1->Value;

        var v2 = ILIntepreter.Minus(ptr, 2);

        ins = new KeyValuePair<ulong, CoroutineAdapter.Adaptor>(key, (CoroutineAdapter.Adaptor)mStack[v2->Value]);

        //UnityEngine.Debug.LogError("-----keyValuePair_2_UInt64_CoroutineAdapter_Adaptor_Binder");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<ulong, CoroutineAdapter.Adaptor> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(ulong*)&v->Value = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        mStack[v->Value] = ins.Value;
    }
}
