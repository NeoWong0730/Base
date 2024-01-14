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
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
using AutoList = System.Collections.Generic.List<object>;
#else
using AutoList = ILRuntime.Other.UncheckedList<object>;
#endif
namespace ILRuntime.Runtime.Generated
{
    unsafe class Rotater_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::Rotater);

            field = type.GetField("speed", flag);
            app.RegisterCLRFieldGetter(field, get_speed_0);
            app.RegisterCLRFieldSetter(field, set_speed_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_speed_0, AssignFromStack_speed_0);
            field = type.GetField("rotateType", flag);
            app.RegisterCLRFieldGetter(field, get_rotateType_1);
            app.RegisterCLRFieldSetter(field, set_rotateType_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_rotateType_1, AssignFromStack_rotateType_1);


        }



        static object get_speed_0(ref object o)
        {
            return ((global::Rotater)o).speed;
        }

        static StackObject* CopyToStack_speed_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::Rotater)o).speed;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_speed_0(ref object o, object v)
        {
            ((global::Rotater)o).speed = (System.Single)v;
        }

        static StackObject* AssignFromStack_speed_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @speed = *(float*)&ptr_of_this_method->Value;
            ((global::Rotater)o).speed = @speed;
            return ptr_of_this_method;
        }

        static object get_rotateType_1(ref object o)
        {
            return ((global::Rotater)o).rotateType;
        }

        static StackObject* CopyToStack_rotateType_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::Rotater)o).rotateType;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_rotateType_1(ref object o, object v)
        {
            ((global::Rotater)o).rotateType = (global::Rotater.ERotateType)v;
        }

        static StackObject* AssignFromStack_rotateType_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::Rotater.ERotateType @rotateType = (global::Rotater.ERotateType)typeof(global::Rotater.ERotateType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            ((global::Rotater)o).rotateType = @rotateType;
            return ptr_of_this_method;
        }



    }
}
