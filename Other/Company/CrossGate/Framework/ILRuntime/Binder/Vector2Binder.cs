﻿//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
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

public unsafe class Vector2Binder : ValueTypeBinder<Vector2>
{
    Vector3Binder vector3Binder;
    bool vector3BinderGot;

    Vector3Binder Vector3Binder
    {
        get
        {
            if (!vector3BinderGot)
            {
                vector3BinderGot = true;
                var vector3Type = CLRType.AppDomain.GetType(typeof(Vector3)) as CLRType;
                vector3Binder = vector3Type.ValueTypeBinder as Vector3Binder;
            }

            return vector3Binder;
        }
    }

    public override unsafe void AssignFromStack(ref Vector2 ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins.x = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 2);
        ins.y = *(float*)&v->Value;
    }

    public override unsafe void CopyValueTypeToStack(ref Vector2 ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(float*)&v->Value = ins.x;
        v = ILIntepreter.Minus(ptr, 2);
        *(float*)&v->Value = ins.y;
    }
    public override void RegisterCLRRedirection(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
    {
        BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        MethodBase method;
        Type[] args;
        Type type = typeof(Vector2);
        args = new Type[] { typeof(float), typeof(float) };
        method = type.GetConstructor(flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, NewVector2);

        args = new Type[] { typeof(Vector2), typeof(Vector2) };
        method = type.GetMethod("op_Addition", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Vector2_Add);

        args = new Type[] { typeof(Vector2), typeof(Vector2) };
        method = type.GetMethod("op_Subtraction", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Vector2_Subtraction);

        args = new Type[] { typeof(Vector2), typeof(float) };
        method = type.GetMethod("op_Multiply", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Vector2_Multiply);

        args = new Type[] { typeof(float), typeof(Vector2) };
        method = type.GetMethod("op_Multiply", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Vector2_Multiply2);

        args = new Type[] { typeof(Vector2), typeof(float) };
        method = type.GetMethod("op_Division", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Vector2_Division);

        args = new Type[] { typeof(Vector2) };
        method = type.GetMethod("op_UnaryNegation", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Vector2_Negate);

        args = new Type[] { typeof(Vector2), typeof(Vector2) };
        method = type.GetMethod("op_Equality", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Vector2_Equality);

        args = new Type[] { typeof(Vector2), typeof(Vector2) };
        method = type.GetMethod("op_Inequality", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Vector2_Inequality);

        args = new Type[] { typeof(Vector2) };
        method = type.GetMethod("op_Implicit", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Vector2_Implicit);

        args = new Type[] { typeof(Vector3) };
        method = type.GetMethod("op_Implicit", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Vector2_Implicit2);

        args = new Type[] { typeof(Vector2), typeof(Vector2) };
        method = type.GetMethod("Dot", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Vector2_Dot);

        args = new Type[] { typeof(Vector2), typeof(Vector2) };
        method = type.GetMethod("Distance", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Vector2_Distance);

        args = new Type[] { };
        method = type.GetMethod("get_magnitude", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Get_Magnitude);

        args = new Type[] { };
        method = type.GetMethod("get_sqrMagnitude", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Get_SqrMagnitude);

        args = new Type[] { };
        method = type.GetMethod("get_normalized", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Get_Normalized);

        args = new Type[] { };
        method = type.GetMethod("get_one", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Get_One);

        args = new Type[] { };
        method = type.GetMethod("get_zero", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Get_Zero);

        //args = new Type[] { typeof(float), typeof(float) };
        //method = type.GetMethod("Set", flag, null, args, null);
        //appdomain.RegisterCLRMethodRedirection(method, Set);
    }

    StackObject* Vector2_Add(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 2);
        var ptr = ILIntepreter.Minus(esp, 1);

        Vector2 left, right;
        ParseVector2(out right, intp, ptr, mStack);

        ptr = ILIntepreter.Minus(esp, 2);
        ParseVector2(out left, intp, ptr, mStack);

        var res = left + right;
        PushVector2(ref res, intp, ret, mStack);

        return ret + 1;
    }

    StackObject* Vector2_Subtraction(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 2);
        var ptr = ILIntepreter.Minus(esp, 1);

        Vector2 left, right;
        ParseVector2(out right, intp, ptr, mStack);

        ptr = ILIntepreter.Minus(esp, 2);
        ParseVector2(out left, intp, ptr, mStack);

        var res = left - right;
        PushVector2(ref res, intp, ret, mStack);

        return ret + 1;
    }

    StackObject* Vector2_Multiply(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 2);

        var ptr = ILIntepreter.Minus(esp, 1);
        var b = ILIntepreter.GetObjectAndResolveReference(ptr);

        float val = *(float*)&b->Value;

        Vector2 vec;

        ptr = ILIntepreter.Minus(esp, 2);
        ParseVector2(out vec, intp, ptr, mStack);

        vec = vec * val;
        PushVector2(ref vec, intp, ret, mStack);

        return ret + 1;
    }

    StackObject* Vector2_Multiply2(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 2);
        Vector2 vec;

        var ptr = ILIntepreter.Minus(esp, 1);
        ParseVector2(out vec, intp, ptr, mStack);

        ptr = ILIntepreter.Minus(esp, 2);
        var b = ILIntepreter.GetObjectAndResolveReference(ptr);

        float val = *(float*)&b->Value;

        vec = val * vec;
        PushVector2(ref vec, intp, ret, mStack);

        return ret + 1;
    }

    StackObject* Vector2_Division(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 2);

        var ptr = ILIntepreter.Minus(esp, 1);
        var b = ILIntepreter.GetObjectAndResolveReference(ptr);

        float val = *(float*)&b->Value;

        Vector2 vec;

        ptr = ILIntepreter.Minus(esp, 2);
        ParseVector2(out vec, intp, ptr, mStack);

        vec = vec / val;
        PushVector2(ref vec, intp, ret, mStack);

        return ret + 1;
    }

    StackObject* Vector2_Negate(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 1);

        var ptr = ILIntepreter.Minus(esp, 1);
        Vector2 vec;

        ptr = ILIntepreter.Minus(esp, 1);
        ParseVector2(out vec, intp, ptr, mStack);

        vec = -vec;
        PushVector2(ref vec, intp, ret, mStack);

        return ret + 1;
    }

    StackObject* Vector2_Implicit(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 1);

        var ptr = ILIntepreter.Minus(esp, 1);
        Vector2 vec;

        ptr = ILIntepreter.Minus(esp, 1);
        ParseVector2(out vec, intp, ptr, mStack);

        Vector3 res = vec;
        PushVector3(ref res, intp, ret, mStack);

        return ret + 1;
    }

    StackObject* Vector2_Implicit2(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 1);

        var ptr = ILIntepreter.Minus(esp, 1);
        Vector3 vec;

        ptr = ILIntepreter.Minus(esp, 1);
        Vector3Binder.ParseVector3(out vec, intp, ptr, mStack);

        Vector2 res = vec;
        PushVector2(ref res, intp, ret, mStack);

        return ret + 1;
    }

    StackObject* Vector2_Equality(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 2);
        var ptr = ILIntepreter.Minus(esp, 1);

        Vector2 left, right;
        ParseVector2(out right, intp, ptr, mStack);

        ptr = ILIntepreter.Minus(esp, 2);
        ParseVector2(out left, intp, ptr, mStack);

        var res = left == right;

        ret->ObjectType = ObjectTypes.Integer;
        ret->Value = res ? 1 : 0;
        return ret + 1;
    }

    StackObject* Vector2_Inequality(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 2);
        var ptr = ILIntepreter.Minus(esp, 1);

        Vector2 left, right;
        ParseVector2(out right, intp, ptr, mStack);

        ptr = ILIntepreter.Minus(esp, 2);
        ParseVector2(out left, intp, ptr, mStack);

        var res = left != right;

        ret->ObjectType = ObjectTypes.Integer;
        ret->Value = res ? 1 : 0;
        return ret + 1;
    }

    StackObject* Vector2_Dot(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 2);
        var ptr = ILIntepreter.Minus(esp, 1);

        Vector2 left, right;
        ParseVector2(out right, intp, ptr, mStack);

        ptr = ILIntepreter.Minus(esp, 2);
        ParseVector2(out left, intp, ptr, mStack);

        var res = Vector3.Dot(left, right);

        ret->ObjectType = ObjectTypes.Float;
        *(float*)&ret->Value = res;
        return ret + 1;
    }

    StackObject* Vector2_Distance(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 2);
        var ptr = ILIntepreter.Minus(esp, 1);

        Vector2 left, right;
        ParseVector2(out right, intp, ptr, mStack);

        ptr = ILIntepreter.Minus(esp, 2);
        ParseVector2(out left, intp, ptr, mStack);

        var res = Vector3.Distance(left, right);

        ret->ObjectType = ObjectTypes.Float;
        *(float*)&ret->Value = res;
        return ret + 1;
    }        

    StackObject* NewVector2(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        StackObject* ret;
        if (isNewObj)
        {
            ret = ILIntepreter.Minus(esp, 1);
            Vector2 vec;
            var ptr = ILIntepreter.Minus(esp, 1);
            vec.y = *(float*)&ptr->Value;
            ptr = ILIntepreter.Minus(esp, 2);
            vec.x = *(float*)&ptr->Value;

            PushVector2(ref vec, intp, ptr, mStack);
        }
        else
        {
            ret = ILIntepreter.Minus(esp, 3);
            var instance = ILIntepreter.GetObjectAndResolveReference(ret);
            var dst = *(StackObject**)&instance->Value;
            var f = ILIntepreter.Minus(dst, 1);
            var v = ILIntepreter.Minus(esp, 2);
            *f = *v;

            f = ILIntepreter.Minus(dst, 2);
            v = ILIntepreter.Minus(esp, 1);
            *f = *v;
        }
        return ret;
    }

    StackObject* Get_Magnitude(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 1);

        var ptr = ILIntepreter.Minus(esp, 1);
        Vector2 vec;
        ParseVector2(out vec, intp, ptr, mStack);

        float res = vec.magnitude;

        ret->ObjectType = ObjectTypes.Float;
        *(float*)&ret->Value = res;
        return ret + 1;
    }

    StackObject* Get_SqrMagnitude(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 1);

        var ptr = ILIntepreter.Minus(esp, 1);
        Vector2 vec;
        ParseVector2(out vec, intp, ptr, mStack);

        float res = vec.sqrMagnitude;

        ret->ObjectType = ObjectTypes.Float;
        *(float*)&ret->Value = res;
        return ret + 1;
    }

    StackObject* Get_Normalized(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 1);
        var ptr = ILIntepreter.Minus(esp, 1);
        Vector2 vec;
        ParseVector2(out vec, intp, ptr, mStack);

        var res = vec.normalized;

        PushVector2(ref res, intp, ret, mStack);
        return ret + 1;
    }

    StackObject* Get_One(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = esp;
        var res = Vector2.one;
        PushVector2(ref res, intp, ret, mStack);
        return ret + 1;
    }

    StackObject* Get_Zero(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = esp;
        var res = Vector2.zero;
        PushVector2(ref res, intp, ret, mStack);
        return ret + 1;
    }

    //StackObject* Set(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    //{
    //    //var ret = ILIntepreter.Minus(esp, 3);

    //    //var ptr = ILIntepreter.Minus(esp, 1);
    //    //float y = *(float*)&ptr->Value;

    //    //ptr = ILIntepreter.Minus(esp, 2);
    //    //float x = *(float*)&ptr->Value;

    //    //Vector2 vec = new Vector2();
    //    ////var a = ILIntepreter.GetObjectAndResolveReference(ret);
    //    ////if (a->ObjectType == ObjectTypes.ValueTypeObjectReference)
    //    ////{
    //    ////    var src = *(StackObject**)&a->Value;
    //    ////    vec.x = *(float*)&ILIntepreter.Minus(src, 1)->Value;
    //    ////    vec.y = *(float*)&ILIntepreter.Minus(src, 2)->Value;
    //    ////    intp.FreeStackValueType(ret);
    //    ////}
    //    ////else
    //    ////{
    //    ////    vec = (Vector2)StackObject.ToObject(a, intp.AppDomain, mStack);
    //    ////    intp.Free(ret);
    //    ////}

    //    ////vec.Set(x, y);
    //    //ret = ILIntepreter.Minus(esp, 3);
    //    //var instance = ILIntepreter.GetObjectAndResolveReference(ret);
    //    //if(instance->ObjectType == ObjectTypes.ValueTypeObjectReference)
    //    //{
    //    //    var dst = ILIntepreter.ResolveReference(instance);
    //    //    var f = ILIntepreter.Minus(dst, 1);
    //    //    var v = ILIntepreter.Minus(esp, 2);
    //    //    *f = *v;
    //    //    //*(float*)&f->Value = vec.x;

    //    //    f = ILIntepreter.Minus(dst, 2);
    //    //    v = ILIntepreter.Minus(esp, 1);
    //    //    *f = *v;
    //    //    //*(float*)&f->Value = vec.y;
    //    //}

    //    ////ParseVector2(out vec, intp, ret, mStack);
    //    //return ret;



    //    ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
    //    StackObject* ptr_of_this_method;
    //    StackObject* ret = ILIntepreter.Minus(esp, 3);

    //    ptr_of_this_method = ILIntepreter.Minus(esp, 1);
    //    System.Single @newY = *(float*)&ptr_of_this_method->Value;

    //    ptr_of_this_method = ILIntepreter.Minus(esp, 2);
    //    System.Single @newX = *(float*)&ptr_of_this_method->Value;

    //    ptr_of_this_method = ILIntepreter.Minus(esp, 3);
    //    UnityEngine.Vector2 instance_of_this_method = new UnityEngine.Vector2();
    //    if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null)
    //    {
    //        ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref instance_of_this_method, intp, ptr_of_this_method, mStack, false);
    //    }

    //    instance_of_this_method.Set(@newX, @newY);

    //    ptr_of_this_method = ILIntepreter.Minus(esp, 3);
    //    if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null)
    //    {
    //        ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.WriteBackValue(__domain, ptr_of_this_method, mStack, ref instance_of_this_method);
    //    }



    //    //ptr_of_this_method = ILIntepreter.Minus(esp, 3);
    //    //var instance = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
    //    //if (instance->ObjectType == ObjectTypes.ValueTypeObjectReference)
    //    //{
    //    //    var dst = ILIntepreter.ResolveReference(instance);
    //    //    var f = ILIntepreter.Minus(dst, 1);
    //    //    var v = ILIntepreter.Minus(esp, 2);
    //    //    *f = *v;

    //    //    f = ILIntepreter.Minus(dst, 2);
    //    //    v = ILIntepreter.Minus(esp, 1);
    //    //    *f = *v;
    //    //    Debug.LogError("ValueTypeObjectReference");
    //    //}
    //    //else
    //    //{
    //    //    //*(float*)&(mStack[instance->Value]) = instance_of_this_method.x;
    //    //    //*((float*)&(mStack[instance->Value]) + 1) = instance_of_this_method.y;
    //    //}

    //    intp.Free(ptr_of_this_method);
    //    return ret;
    //}

    public static void ParseVector2(out Vector2 vec, ILIntepreter intp, StackObject* ptr, IList<object> mStack)
    {
        var a = ILIntepreter.GetObjectAndResolveReference(ptr);
        if (a->ObjectType == ObjectTypes.ValueTypeObjectReference)
        {
            var src = *(StackObject**)&a->Value;
            vec.x = *(float*)&ILIntepreter.Minus(src, 1)->Value;
            vec.y = *(float*)&ILIntepreter.Minus(src, 2)->Value;
            intp.FreeStackValueType(ptr);
        }
        else
        {
            vec = (Vector2)StackObject.ToObject(a, intp.AppDomain, mStack);
            intp.Free(ptr);
        }
    }

    public void PushVector2(ref Vector2 vec, ILIntepreter intp, StackObject* ptr, IList<object> mStack)
    {
        intp.AllocValueType(ptr, CLRType);
        var dst = *((StackObject**)&ptr->Value);
        CopyValueTypeToStack(ref vec, dst, mStack);
    }

    void PushVector3(ref Vector3 vec, ILIntepreter intp, StackObject* ptr, IList<object> mStack)
    {
        var binder = Vector3Binder;
        if (binder != null)
            binder.PushVector3(ref vec, intp, ptr, mStack);
        else
            ILIntepreter.PushObject(ptr, mStack, vec, true);
    }
}
