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

public class keyValuePair_2_UInt32_Single_Binder : ValueTypeBinder<KeyValuePair<System.UInt32, System.Single>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<uint, float> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        uint key = *(uint*)&v->Value;
        v = ILIntepreter.Minus(ptr, 2);
        float value = *(float*)&v->Value;

        ins = new KeyValuePair<uint, float>(key, value);

       // UnityEngine.Debug.LogError("-----keyValuePair_2_UInt32_Single_Binding");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<uint, float> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
       *(uint*)&v->Value = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        *(float*)&v->Value = ins.Value;
    }
}
