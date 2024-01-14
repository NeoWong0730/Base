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

public class KeyValuePair_2_UInt32_Int64_Binder : ValueTypeBinder<KeyValuePair<System.UInt32, System.Int64>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<uint, long> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        uint key = *(uint*)&v->Value;

        v = ILIntepreter.Minus(ptr, 2);
        long value = *(long*)&v->Value;

        ins = new KeyValuePair<uint, long>(key, value);

       // UnityEngine.Debug.LogError("-----KeyValuePair_2_UInt32_Int64_Binder");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<uint, long> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(uint*)&v->Value = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        *(long*)&v->Value = ins.Value;
    }
}
