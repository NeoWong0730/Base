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
    unsafe class ScrollOrDragControl_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ScrollOrDragControl);

            field = type.GetField("onCopyDataCallback", flag);
            app.RegisterCLRFieldGetter(field, get_onCopyDataCallback_0);
            app.RegisterCLRFieldSetter(field, set_onCopyDataCallback_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onCopyDataCallback_0, AssignFromStack_onCopyDataCallback_0);
            field = type.GetField("onChangeDataCallback", flag);
            app.RegisterCLRFieldGetter(field, get_onChangeDataCallback_1);
            app.RegisterCLRFieldSetter(field, set_onChangeDataCallback_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onChangeDataCallback_1, AssignFromStack_onChangeDataCallback_1);


        }



        static object get_onCopyDataCallback_0(ref object o)
        {
            return global::ScrollOrDragControl.onCopyDataCallback;
        }

        static StackObject* CopyToStack_onCopyDataCallback_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::ScrollOrDragControl.onCopyDataCallback;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onCopyDataCallback_0(ref object o, object v)
        {
            global::ScrollOrDragControl.onCopyDataCallback = (global::ScrollOrDragControl.OnCopyDataCallback)v;
        }

        static StackObject* AssignFromStack_onCopyDataCallback_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::ScrollOrDragControl.OnCopyDataCallback @onCopyDataCallback = (global::ScrollOrDragControl.OnCopyDataCallback)typeof(global::ScrollOrDragControl.OnCopyDataCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            global::ScrollOrDragControl.onCopyDataCallback = @onCopyDataCallback;
            return ptr_of_this_method;
        }

        static object get_onChangeDataCallback_1(ref object o)
        {
            return global::ScrollOrDragControl.onChangeDataCallback;
        }

        static StackObject* CopyToStack_onChangeDataCallback_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::ScrollOrDragControl.onChangeDataCallback;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onChangeDataCallback_1(ref object o, object v)
        {
            global::ScrollOrDragControl.onChangeDataCallback = (global::ScrollOrDragControl.OnChangeDataCallback)v;
        }

        static StackObject* AssignFromStack_onChangeDataCallback_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::ScrollOrDragControl.OnChangeDataCallback @onChangeDataCallback = (global::ScrollOrDragControl.OnChangeDataCallback)typeof(global::ScrollOrDragControl.OnChangeDataCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            global::ScrollOrDragControl.onChangeDataCallback = @onChangeDataCallback;
            return ptr_of_this_method;
        }



    }
}
