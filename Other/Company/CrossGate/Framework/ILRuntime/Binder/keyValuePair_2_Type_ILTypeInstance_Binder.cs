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
using System.Json;




public class keyValuePair_2_Type_ILTypeInstance_Binder : ValueTypeBinder<KeyValuePair<System.Type, ILRuntime.Runtime.Intepreter.ILTypeInstance>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<Type, ILTypeInstance> ins, StackObject* ptr, IList<object> mStack)
    {
        var v1 = ILIntepreter.Minus(ptr, 1);

        var v2 = ILIntepreter.Minus(ptr, 2);

        ins = new KeyValuePair<Type, ILTypeInstance>((Type)mStack[v1->Value], (ILTypeInstance)mStack[v2->Value]);

        //UnityEngine.Debug.LogError("-----keyValuePair_2_Type_ILTypeInstance_Binder");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<Type, ILTypeInstance> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        mStack[v->Value] = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        mStack[v->Value] = ins.Value;
    }
}
