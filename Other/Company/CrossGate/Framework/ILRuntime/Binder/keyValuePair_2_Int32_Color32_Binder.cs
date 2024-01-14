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

public class keyValuePair_2_Int32_Color32_Binder : ValueTypeBinder<KeyValuePair<System.Int32, UnityEngine.Color32>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<int, Color32> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        int key = v->Value;

        v = ILIntepreter.Minus(ptr, 2);
        byte r = *(byte*)&v->Value;

        v = ILIntepreter.Minus(ptr, 3);
        byte g = *(byte*)&v->Value;

        v = ILIntepreter.Minus(ptr, 4);
        byte b = *(byte*)&v->Value;

        v = ILIntepreter.Minus(ptr, 5);
        byte a= *(byte*)&v->Value;

        Color32 color = new Color32(r, g, b, a);

        ins = new KeyValuePair<int, Color32>(key, color);

        //UnityEngine.Debug.LogError("-----keyValuePair_2_Int32_Color32_Binder-----AssignFromStack ---这个真的很特殊呢！！！！");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<int, Color32> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        v->Value = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        *(byte*)&v->Value = ins.Value.r;

        v = ILIntepreter.Minus(ptr, 3);
        *(byte*)&v->Value = ins.Value.g;

        v = ILIntepreter.Minus(ptr, 4);
        *(byte*)&v->Value = ins.Value.b;

        v = ILIntepreter.Minus(ptr, 5);
        *(byte*)&v->Value = ins.Value.a;
        
        //UnityEngine.Debug.LogError("-----keyValuePair_2_Int32_Color32_Binder----CopyValueTypeToStack----这个真的很特殊呢！！！！");
    }
}
