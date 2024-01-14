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
public unsafe class Matrix4x4Binder : ValueTypeBinder<Matrix4x4>
{
    public override unsafe void AssignFromStack(ref Matrix4x4 ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins.m00 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 2);
        ins.m33 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 3);
        ins.m23 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 4);
        ins.m13 = *(float*)&v->Value;

        v = ILIntepreter.Minus(ptr, 5);
        ins.m03 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 6);
        ins.m32 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 7);
        ins.m22 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 8);
        ins.m02 = *(float*)&v->Value;

        v = ILIntepreter.Minus(ptr, 9);
        ins.m12 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 10);
        ins.m21 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 11);
        ins.m11 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 12);
        ins.m01 = *(float*)&v->Value;

        v = ILIntepreter.Minus(ptr, 13);
        ins.m30 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 14);
        ins.m20 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 15);
        ins.m10 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 16);
        ins.m31 = *(float*)&v->Value;
        
}

    public override unsafe void CopyValueTypeToStack(ref Matrix4x4 ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(float*)&v->Value = ins.m00;
        v = ILIntepreter.Minus(ptr, 2);
        *(float*)&v->Value = ins.m33;
        v = ILIntepreter.Minus(ptr, 3);
        *(float*)&v->Value = ins.m23;
        v = ILIntepreter.Minus(ptr, 4);
        *(float*)&v->Value = ins.m13;

        v = ILIntepreter.Minus(ptr, 5);
        *(float*)&v->Value = ins.m03;
        v = ILIntepreter.Minus(ptr, 6);
        *(float*)&v->Value = ins.m32;
        v = ILIntepreter.Minus(ptr, 7);
        *(float*)&v->Value = ins.m22;
        v = ILIntepreter.Minus(ptr, 8);
        *(float*)&v->Value = ins.m02;

        v = ILIntepreter.Minus(ptr, 9);
        *(float*)&v->Value = ins.m12;
        v = ILIntepreter.Minus(ptr, 10);
        *(float*)&v->Value = ins.m21;
        v = ILIntepreter.Minus(ptr, 11);
        *(float*)&v->Value = ins.m11;
        v = ILIntepreter.Minus(ptr, 12);
        *(float*)&v->Value = ins.m01;

        v = ILIntepreter.Minus(ptr, 13);
        *(float*)&v->Value = ins.m30;
        v = ILIntepreter.Minus(ptr, 14);
        *(float*)&v->Value = ins.m20;
        v = ILIntepreter.Minus(ptr, 15);
        *(float*)&v->Value = ins.m10;
        v = ILIntepreter.Minus(ptr, 16);
        *(float*)&v->Value = ins.m31;
    }



    //public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
    //{
    //    BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
    //    MethodBase method;
    //    FieldInfo field;
    //    Type[] args;
    //    Type type = typeof(UnityEngine.Matrix4x4);

    //    args = new Type[] { typeof(UnityEngine.Vector4), typeof(UnityEngine.Vector4), typeof(UnityEngine.Vector4), typeof(UnityEngine.Vector4) };
    //    method = type.GetConstructor(flag, null, args, null);
    //   // app.RegisterCLRMethodRedirection(method, NewMatrix4x4);
    //}

    //StackObject* NewMatrix4x4(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    //{
     //   StackObject* ret;
        

        //if (isNewObj)
        //{
        //    ret = ILIntepreter.Minus(esp, 1);
        //    var ptr = ILIntepreter.Minus(esp, 1);

        //    Vector2 vec;
        //    int value = ptr->Value;
        //    int* haha = &ptr->Value;
        //    float* huhu = (float*)haha;
        //    float cc = *huhu;


        //    vec.y = *(float*)&ptr->Value;
        //    ptr = ILIntepreter.Minus(esp, 2);
        //    vec.x = *(float*)&ptr->Value;

        //    PushVector2(ref vec, intp, ptr, mStack);
        //}
        //else
        //{
        //    ret = ILIntepreter.Minus(esp, 3);
        //    var instance = ILIntepreter.GetObjectAndResolveReference(ret);
        //    Debug.LogError("instance->Value:" + instance->Value);


        //    var dst = *(StackObject**)&instance->Value;


        //    var f = ILIntepreter.Minus(dst, 1);
        //    var v = ILIntepreter.Minus(esp, 2);
        //    *f = *v;

        //    f = ILIntepreter.Minus(dst, 2);
        //    v = ILIntepreter.Minus(esp, 1);
        //    *f = *v;
        //}
        //return ret;





   // }
}
