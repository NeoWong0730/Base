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

public class keyValuePair_2_String_Boolean_Binder : ValueTypeBinder<KeyValuePair<System.String, System.Boolean>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<string, bool> ins, StackObject* ptr, IList<object> mStack)
    {
        var v1 = ILIntepreter.Minus(ptr, 1);
        
        var v2 = ILIntepreter.Minus(ptr, 2);
        bool value = *(bool*)&v1->Value;

        ins = new KeyValuePair<string, bool>((string)mStack[v1->Value], value);

      //  UnityEngine.Debug.LogError("-----keyValuePair_2_String_Boolean_Binder");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<string, bool> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        mStack[v->Value] = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        *(bool*)&v->Value = ins.Value;
    }
}
 

