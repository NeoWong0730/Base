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
    unsafe class ToggleCD_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ToggleCD);

            field = type.GetField("onValueTrue", flag);
            app.RegisterCLRFieldGetter(field, get_onValueTrue_0);
            app.RegisterCLRFieldSetter(field, set_onValueTrue_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onValueTrue_0, AssignFromStack_onValueTrue_0);
            field = type.GetField("toggle", flag);
            app.RegisterCLRFieldGetter(field, get_toggle_1);
            app.RegisterCLRFieldSetter(field, set_toggle_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_toggle_1, AssignFromStack_toggle_1);


        }



        static object get_onValueTrue_0(ref object o)
        {
            return ((global::ToggleCD)o).onValueTrue;
        }

        static StackObject* CopyToStack_onValueTrue_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::ToggleCD)o).onValueTrue;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onValueTrue_0(ref object o, object v)
        {
            ((global::ToggleCD)o).onValueTrue = (System.Action)v;
        }

        static StackObject* AssignFromStack_onValueTrue_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @onValueTrue = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::ToggleCD)o).onValueTrue = @onValueTrue;
            return ptr_of_this_method;
        }

        static object get_toggle_1(ref object o)
        {
            return ((global::ToggleCD)o).toggle;
        }

        static StackObject* CopyToStack_toggle_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::ToggleCD)o).toggle;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_toggle_1(ref object o, object v)
        {
            ((global::ToggleCD)o).toggle = (global::CP_Toggle)v;
        }

        static StackObject* AssignFromStack_toggle_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::CP_Toggle @toggle = (global::CP_Toggle)typeof(global::CP_Toggle).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::ToggleCD)o).toggle = @toggle;
            return ptr_of_this_method;
        }



    }
}
