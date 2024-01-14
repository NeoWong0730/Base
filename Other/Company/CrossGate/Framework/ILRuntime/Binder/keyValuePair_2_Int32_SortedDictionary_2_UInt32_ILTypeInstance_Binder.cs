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
public class keyValuePair_2_Int32_SortedDictionary_2_UInt32_ILTypeInstance_Binder : ValueTypeBinder<KeyValuePair<System.Int32, System.Collections.Generic.SortedDictionary<System.UInt32, ILRuntime.Runtime.Intepreter.ILTypeInstance>>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<int, SortedDictionary<uint, ILTypeInstance>> ins, StackObject* ptr, IList<object> mStack)
    {
        var v1 = ILIntepreter.Minus(ptr, 1);
        int key = v1->Value;

        var v2 = ILIntepreter.Minus(ptr, 2);

        ins = new KeyValuePair<int, SortedDictionary<uint, ILTypeInstance>>(key, (SortedDictionary<uint, ILTypeInstance>)mStack[v2->Value]);

        //UnityEngine.Debug.LogError("-----keyValuePair_2_Int32_SortedDictionary_2_UInt32_ILTypeInstance_Binder");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<int, SortedDictionary<uint, ILTypeInstance>> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        v->Value = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        mStack[v->Value] = ins.Value;
    }
}
