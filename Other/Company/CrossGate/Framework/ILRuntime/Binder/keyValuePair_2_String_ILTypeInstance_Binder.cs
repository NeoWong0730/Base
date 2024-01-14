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
using System.Collections.Generic;

public class keyValuePair_2_String_ILTypeInstance_Binder : ValueTypeBinder<KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<string, ILTypeInstance> ins, StackObject* ptr, IList<object> mStack)
    {
        var v1 = ILIntepreter.Minus(ptr, 1);

        var v2 = ILIntepreter.Minus(ptr, 2);

        ins = new KeyValuePair<string, ILTypeInstance>((string)mStack[v1->Value], (ILTypeInstance)mStack[v2->Value]);

        //UnityEngine.Debug.LogError("-----keyValuePair_2_String_ILTypeInstance_Binder");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<string, ILTypeInstance> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        mStack[v->Value] = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        mStack[v->Value] = ins.Value;
    }
}
