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
public class keyValuePair_2_Int32_UInt64_Binder : ValueTypeBinder<KeyValuePair<System.Int32, System.UInt64>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<int, ulong> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        int key = v->Value;

        v = ILIntepreter.Minus(ptr, 2);
        ulong value = *(ulong*)&v->Value;

        ins = new KeyValuePair<int, ulong>(key, value);

        //UnityEngine.Debug.LogError("-----keyValuePair_2_Int32_UInt64_Binding");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<int, ulong> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(int*)&v->Value = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        *(ulong*)&v->Value = ins.Value;
    }
}
