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
    unsafe class CP_VerticalCenterOnChild_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CP_VerticalCenterOnChild);

            field = type.GetField("onBeginCenter", flag);
            app.RegisterCLRFieldGetter(field, get_onBeginCenter_0);
            app.RegisterCLRFieldSetter(field, set_onBeginCenter_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onBeginCenter_0, AssignFromStack_onBeginCenter_0);


        }



        static object get_onBeginCenter_0(ref object o)
        {
            return ((global::CP_VerticalCenterOnChild)o).onBeginCenter;
        }

        static StackObject* CopyToStack_onBeginCenter_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_VerticalCenterOnChild)o).onBeginCenter;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onBeginCenter_0(ref object o, object v)
        {
            ((global::CP_VerticalCenterOnChild)o).onBeginCenter = (System.Action)v;
        }

        static StackObject* AssignFromStack_onBeginCenter_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @onBeginCenter = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::CP_VerticalCenterOnChild)o).onBeginCenter = @onBeginCenter;
            return ptr_of_this_method;
        }



    }
}
