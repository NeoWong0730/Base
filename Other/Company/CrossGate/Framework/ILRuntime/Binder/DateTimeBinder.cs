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
public class DateTimeBinder : ValueTypeBinder<DateTime>
{
    //这个也不知道咋写
    public override unsafe void AssignFromStack(ref DateTime ins, StackObject* ptr, IList<object> mStack)
    {
     
    }

    public override unsafe void CopyValueTypeToStack(ref DateTime ins, StackObject* ptr, IList<object> mStack)
    {
      
    }
}
