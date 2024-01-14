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
    unsafe class Framework_TouchInput_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.TouchInput);

            field = type.GetField("SendTouchUp", flag);
            app.RegisterCLRFieldGetter(field, get_SendTouchUp_0);
            app.RegisterCLRFieldSetter(field, set_SendTouchUp_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_SendTouchUp_0, AssignFromStack_SendTouchUp_0);
            field = type.GetField("SendTouchLongPress", flag);
            app.RegisterCLRFieldGetter(field, get_SendTouchLongPress_1);
            app.RegisterCLRFieldSetter(field, set_SendTouchLongPress_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_SendTouchLongPress_1, AssignFromStack_SendTouchLongPress_1);
            field = type.GetField("interval", flag);
            app.RegisterCLRFieldGetter(field, get_interval_2);
            app.RegisterCLRFieldSetter(field, set_interval_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_interval_2, AssignFromStack_interval_2);


        }



        static object get_SendTouchUp_0(ref object o)
        {
            return ((Framework.TouchInput)o).SendTouchUp;
        }

        static StackObject* CopyToStack_SendTouchUp_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.TouchInput)o).SendTouchUp;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SendTouchUp_0(ref object o, object v)
        {
            ((Framework.TouchInput)o).SendTouchUp = (System.Action<UnityEngine.Vector3>)v;
        }

        static StackObject* AssignFromStack_SendTouchUp_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.Vector3> @SendTouchUp = (System.Action<UnityEngine.Vector3>)typeof(System.Action<UnityEngine.Vector3>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.TouchInput)o).SendTouchUp = @SendTouchUp;
            return ptr_of_this_method;
        }

        static object get_SendTouchLongPress_1(ref object o)
        {
            return ((Framework.TouchInput)o).SendTouchLongPress;
        }

        static StackObject* CopyToStack_SendTouchLongPress_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.TouchInput)o).SendTouchLongPress;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SendTouchLongPress_1(ref object o, object v)
        {
            ((Framework.TouchInput)o).SendTouchLongPress = (System.Action<UnityEngine.Vector3>)v;
        }

        static StackObject* AssignFromStack_SendTouchLongPress_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.Vector3> @SendTouchLongPress = (System.Action<UnityEngine.Vector3>)typeof(System.Action<UnityEngine.Vector3>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.TouchInput)o).SendTouchLongPress = @SendTouchLongPress;
            return ptr_of_this_method;
        }

        static object get_interval_2(ref object o)
        {
            return ((Framework.TouchInput)o).interval;
        }

        static StackObject* CopyToStack_interval_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.TouchInput)o).interval;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_interval_2(ref object o, object v)
        {
            ((Framework.TouchInput)o).interval = (System.Single)v;
        }

        static StackObject* AssignFromStack_interval_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @interval = *(float*)&ptr_of_this_method->Value;
            ((Framework.TouchInput)o).interval = @interval;
            return ptr_of_this_method;
        }



    }
}
