using Framework;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using ILRuntime.CLR.Method;
public unsafe class RectBinder : ValueTypeBinder<Rect>
{
    Vector2Binder vector2Binder;
    bool vector2BinderGot;

    Vector2Binder Vector2Binder
    {
        get
        {
            if (!vector2BinderGot)
            {
                vector2BinderGot = true;
                var vector2Type = CLRType.AppDomain.GetType(typeof(Vector2)) as CLRType;
                vector2Binder = vector2Type.ValueTypeBinder as Vector2Binder;
            }

            return vector2Binder;
        }
    }
    public override unsafe void AssignFromStack(ref Rect ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins.x = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 2);
        ins.y = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 3);
        ins.width = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 4);
        ins.height = *(float*)&v->Value;
    }

    public override unsafe void CopyValueTypeToStack(ref Rect ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(float*)&v->Value = ins.x;
        v = ILIntepreter.Minus(ptr, 2);
        *(float*)&v->Value = ins.y;
        v = ILIntepreter.Minus(ptr, 3);
        *(float*)&v->Value = ins.width;
        v = ILIntepreter.Minus(ptr, 4);
        *(float*)&v->Value = ins.height;
    }

    public override void RegisterCLRRedirection(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
    {
        BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        MethodBase method;
        Type[] args;
        Type type = typeof(UnityEngine.Rect);
        args = new Type[] { };
        method = type.GetMethod("get_zero", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, get_zero_0);
        args = new Type[] { typeof(UnityEngine.Rect) };
        method = type.GetMethod("Equals", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Equals_1);
        args = new Type[] { typeof(UnityEngine.Vector2) };
        method = type.GetMethod("Contains", flag, null, args, null);
        appdomain.RegisterCLRMethodRedirection(method, Contains_2);
        //args = new Type[] { };
        //method = type.GetMethod("get_width", flag, null, args, null);
        //app.RegisterCLRMethodRedirection(method, get_width_2);
        //args = new Type[] { };
        //method = type.GetMethod("get_x", flag, null, args, null);
        //app.RegisterCLRMethodRedirection(method, get_x_3);
        //args = new Type[] { typeof(UnityEngine.Vector2) };
        //method = type.GetMethod("set_position", flag, null, args, null);
        //app.RegisterCLRMethodRedirection(method, set_position_4);
        //args = new Type[] { typeof(UnityEngine.Vector2) };
        //method = type.GetMethod("set_size", flag, null, args, null);
        //app.RegisterCLRMethodRedirection(method, set_size_5);
        //args = new Type[] { };
        //method = type.GetMethod("get_position", flag, null, args, null);
        //app.RegisterCLRMethodRedirection(method, get_position_6);
        //args = new Type[] { };
        //method = type.GetMethod("get_size", flag, null, args, null);
        //app.RegisterCLRMethodRedirection(method, get_size_7);
        //args = new Type[] { };
        //method = type.GetMethod("get_y", flag, null, args, null);
        //app.RegisterCLRMethodRedirection(method, get_y_8);
        //args = new Type[] { };
        //method = type.GetMethod("get_center", flag, null, args, null);
        //app.RegisterCLRMethodRedirection(method, get_center_9);
    }

    StackObject* get_zero_0(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = esp;
        var res = UnityEngine.Rect.zero;
        PushRect(ref res, intp, ret, mStack);
        return ret + 1;
    }

    StackObject* Equals_1(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 2);
        var ptr = ILIntepreter.Minus(esp, 1);

        float x1,y1,width1,height1,x2,y2,width2,height2;
        var b = ILIntepreter.GetObjectAndResolveReference(ptr);
        if (b->ObjectType == ObjectTypes.ValueTypeObjectReference)
        {
            var src = *(StackObject**)&b->Value;
            x2 = *(float*)&ILIntepreter.Minus(src, 1)->Value;
            y2 = *(float*)&ILIntepreter.Minus(src, 2)->Value;
            width2 = *(float*)&ILIntepreter.Minus(src, 3)->Value;
            height2 = *(float*)&ILIntepreter.Minus(src, 4)->Value;
            intp.FreeStackValueType(ptr);
        }
        else
        {
            var src = (Rect)StackObject.ToObject(b, intp.AppDomain, mStack);
            x2 = src.x;
            y2 = src.y;
            width2 = src.width;
            height2 = src.height;
            intp.Free(ptr);
        }

        ptr = ILIntepreter.Minus(esp, 2);
        var a = ILIntepreter.GetObjectAndResolveReference(ptr);
        if (a->ObjectType == ObjectTypes.ValueTypeObjectReference)
        {
            var src = *(StackObject**)&a->Value;
            x1 = *(float*)&ILIntepreter.Minus(src, 1)->Value;
            y1 = *(float*)&ILIntepreter.Minus(src, 2)->Value;
            width1 = *(float*)&ILIntepreter.Minus(src, 3)->Value;
            height1 = *(float*)&ILIntepreter.Minus(src, 4)->Value;
            intp.FreeStackValueType(ptr);
        }
        else
        {
            var src = (Rect)StackObject.ToObject(a, intp.AppDomain, mStack);
            x1 = src.x;
            y1 = src.y;
            width1 = src.width;
            height1 = src.height;
            intp.Free(ptr);
        }

        var res = (x1 == x2) && (y1 == y2) && (width1 == width2) && (height1 == height2);

        ret->ObjectType = ObjectTypes.Integer;
        ret->Value = res ? 1 : 0;
        return ret + 1;
    }


    StackObject* Contains_2(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
    {
        var ret = ILIntepreter.Minus(esp, 2);

        Vector2 vec;
        var ptr = ILIntepreter.Minus(esp, 1);
        Vector2Binder.ParseVector2(out vec, intp, ptr, mStack);

        ptr = ILIntepreter.Minus(esp, 2);
        Rect rect = new Rect();

        var a = ILIntepreter.GetObjectAndResolveReference(ptr);
        if (a->ObjectType == ObjectTypes.ValueTypeObjectReference)
        {
            var src = *(StackObject**)&a->Value;
            rect.x = *(float*)&ILIntepreter.Minus(src, 1)->Value;
            rect.y = *(float*)&ILIntepreter.Minus(src, 2)->Value;
            rect.width = *(float*)&ILIntepreter.Minus(src, 3)->Value;
            rect.height = *(float*)&ILIntepreter.Minus(src, 4)->Value;
            intp.FreeStackValueType(ptr);
        }
        else
        {
            rect = (Rect)StackObject.ToObject(a, intp.AppDomain, mStack);
            intp.Free(ptr);
        }

        var res = rect.Contains(vec);

       
        ret->ObjectType = ObjectTypes.Integer;
        ret->Value = res ? 1 : 0;
        return ret + 1;
    }


    public void PushRect(ref Rect rect, ILIntepreter intp, StackObject* ptr, IList<object>mStack)
    {
        intp.AllocValueType(ptr, CLRType);
        var dst = *((StackObject**)&ptr->Value);
        CopyValueTypeToStack(ref rect, dst, mStack);
    }
}
