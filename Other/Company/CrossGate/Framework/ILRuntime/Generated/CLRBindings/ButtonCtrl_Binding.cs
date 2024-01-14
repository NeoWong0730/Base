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
    unsafe class ButtonCtrl_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ButtonCtrl);

            field = type.GetField("button", flag);
            app.RegisterCLRFieldGetter(field, get_button_0);
            app.RegisterCLRFieldSetter(field, set_button_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_button_0, AssignFromStack_button_0);
            field = type.GetField("cd", flag);
            app.RegisterCLRFieldGetter(field, get_cd_1);
            app.RegisterCLRFieldSetter(field, set_cd_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_cd_1, AssignFromStack_cd_1);


        }



        static object get_button_0(ref object o)
        {
            return ((global::ButtonCtrl)o).button;
        }

        static StackObject* CopyToStack_button_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::ButtonCtrl)o).button;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_button_0(ref object o, object v)
        {
            ((global::ButtonCtrl)o).button = (UnityEngine.UI.Button)v;
        }

        static StackObject* AssignFromStack_button_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.UI.Button @button = (UnityEngine.UI.Button)typeof(UnityEngine.UI.Button).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::ButtonCtrl)o).button = @button;
            return ptr_of_this_method;
        }

        static object get_cd_1(ref object o)
        {
            return ((global::ButtonCtrl)o).cd;
        }

        static StackObject* CopyToStack_cd_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::ButtonCtrl)o).cd;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_cd_1(ref object o, object v)
        {
            ((global::ButtonCtrl)o).cd = (System.Single)v;
        }

        static StackObject* AssignFromStack_cd_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @cd = *(float*)&ptr_of_this_method->Value;
            ((global::ButtonCtrl)o).cd = @cd;
            return ptr_of_this_method;
        }



    }
}
