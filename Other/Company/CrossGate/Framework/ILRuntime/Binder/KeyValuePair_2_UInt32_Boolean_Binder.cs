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

public class KeyValuePair_2_UInt32_Boolean_Binder : ValueTypeBinder<KeyValuePair<System.UInt32, System.Boolean>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<uint, bool> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        uint key = *(uint*)&v->Value;

        v = ILIntepreter.Minus(ptr, 2);
        bool value = *(bool*)&v->Value;

        ins = new KeyValuePair<uint, bool>(key, value);

      // UnityEngine.Debug.LogError("-----KeyValuePair_2_UInt32_Boolean_Binder");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<uint, bool> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(uint*)&v->Value = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        *(bool*)&v->Value = ins.Value;
    }
}
