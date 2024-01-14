using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Reflection;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Stack;
using UnityEngine.SceneManagement;

public class SceneBinder : ValueTypeBinder<Scene>
{
    public override unsafe void AssignFromStack(ref Scene ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins = *(Scene*)&v->Value;
    }

    public override unsafe void CopyValueTypeToStack(ref Scene ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(Scene*)&v->Value = ins;
    }
}
