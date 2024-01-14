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
    unsafe class MonoLife_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::MonoLife);

            field = type.GetField("onDestroy", flag);
            app.RegisterCLRFieldGetter(field, get_onDestroy_0);
            app.RegisterCLRFieldSetter(field, set_onDestroy_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDestroy_0, AssignFromStack_onDestroy_0);
            field = type.GetField("onUpdate", flag);
            app.RegisterCLRFieldGetter(field, get_onUpdate_1);
            app.RegisterCLRFieldSetter(field, set_onUpdate_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onUpdate_1, AssignFromStack_onUpdate_1);


        }



        static object get_onDestroy_0(ref object o)
        {
            return ((global::MonoLife)o).onDestroy;
        }

        static StackObject* CopyToStack_onDestroy_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::MonoLife)o).onDestroy;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDestroy_0(ref object o, object v)
        {
            ((global::MonoLife)o).onDestroy = (System.Action)v;
        }

        static StackObject* AssignFromStack_onDestroy_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @onDestroy = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::MonoLife)o).onDestroy = @onDestroy;
            return ptr_of_this_method;
        }

        static object get_onUpdate_1(ref object o)
        {
            return ((global::MonoLife)o).onUpdate;
        }

        static StackObject* CopyToStack_onUpdate_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::MonoLife)o).onUpdate;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onUpdate_1(ref object o, object v)
        {
            ((global::MonoLife)o).onUpdate = (System.Action)v;
        }

        static StackObject* AssignFromStack_onUpdate_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @onUpdate = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::MonoLife)o).onUpdate = @onUpdate;
            return ptr_of_this_method;
        }



    }
}
