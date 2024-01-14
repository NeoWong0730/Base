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
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class keyValuePair_2_String_AsyncOperationHandle_AnimationClip_Binder : ValueTypeBinder<KeyValuePair<System.String, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.AnimationClip>>>
{
    public override unsafe void AssignFromStack(ref KeyValuePair<string, AsyncOperationHandle<AnimationClip>> ins, StackObject* ptr, IList<object> mStack)
    {
        var v1 = ILIntepreter.Minus(ptr, 1);

        var v2 = ILIntepreter.Minus(ptr, 2);

        ins = new KeyValuePair<string, AsyncOperationHandle<AnimationClip>>((string)mStack[v1->Value], (AsyncOperationHandle<AnimationClip>)mStack[v2->Value]);

        //UnityEngine.Debug.LogError("-----keyValuePair_2_String_AsyncOperationHandle_AnimationClip_Binder");
    }

    public override unsafe void CopyValueTypeToStack(ref KeyValuePair<string, AsyncOperationHandle<AnimationClip>> ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        mStack[v->Value] = ins.Key;

        v = ILIntepreter.Minus(ptr, 2);
        mStack[v->Value] = ins.Value;
    }
}
