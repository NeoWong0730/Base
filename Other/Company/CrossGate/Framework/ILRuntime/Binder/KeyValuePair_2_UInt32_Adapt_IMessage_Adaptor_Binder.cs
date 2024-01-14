﻿using System;
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
public class KeyValuePair_2_UInt32_Adapt_IMessage_Adaptor_Binder : ValueTypeBinder<KeyValuePair<System.UInt32, global::Adapt_IMessage.Adaptor>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<uint, Adapt_IMessage.Adaptor> ins, StackObject* ptr, IList<object> mStack)
    {
        var v1 = ILIntepreter.Minus(ptr, 1);
        uint key = *(uint*)&v1->Value;

        var v2 = ILIntepreter.Minus(ptr, 2);

        ins = new KeyValuePair<uint, Adapt_IMessage.Adaptor>(key, (Adapt_IMessage.Adaptor)mStack[v2->Value]);

        //UnityEngine.Debug.LogError("-----KeyValuePair_2_UInt32_Adapt_IMessage_Adaptor_Binder");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<uint, Adapt_IMessage.Adaptor> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(uint*)&v->Value = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        mStack[v->Value] = ins.Value;
    }
}
