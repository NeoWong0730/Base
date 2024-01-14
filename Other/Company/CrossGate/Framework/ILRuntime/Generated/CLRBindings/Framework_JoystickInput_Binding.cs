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
    unsafe class Framework_JoystickInput_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.JoystickInput);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_JoytickEnable", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_JoytickEnable_0);

            field = type.GetField("SetLeftJoystick", flag);
            app.RegisterCLRFieldGetter(field, get_SetLeftJoystick_0);
            app.RegisterCLRFieldSetter(field, set_SetLeftJoystick_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_SetLeftJoystick_0, AssignFromStack_SetLeftJoystick_0);
            field = type.GetField("SendTouchUp", flag);
            app.RegisterCLRFieldGetter(field, get_SendTouchUp_1);
            app.RegisterCLRFieldSetter(field, set_SendTouchUp_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_SendTouchUp_1, AssignFromStack_SendTouchUp_1);
            field = type.GetField("SendTouchRightUp", flag);
            app.RegisterCLRFieldGetter(field, get_SendTouchRightUp_2);
            app.RegisterCLRFieldSetter(field, set_SendTouchRightUp_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_SendTouchRightUp_2, AssignFromStack_SendTouchRightUp_2);
            field = type.GetField("SendMoveFollowMouse", flag);
            app.RegisterCLRFieldGetter(field, get_SendMoveFollowMouse_3);
            app.RegisterCLRFieldSetter(field, set_SendMoveFollowMouse_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_SendMoveFollowMouse_3, AssignFromStack_SendMoveFollowMouse_3);


        }


        static StackObject* set_JoytickEnable_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.JoystickInput instance_of_this_method = (Framework.JoystickInput)typeof(Framework.JoystickInput).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.JoytickEnable = value;

            return __ret;
        }


        static object get_SetLeftJoystick_0(ref object o)
        {
            return ((Framework.JoystickInput)o).SetLeftJoystick;
        }

        static StackObject* CopyToStack_SetLeftJoystick_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.JoystickInput)o).SetLeftJoystick;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SetLeftJoystick_0(ref object o, object v)
        {
            ((Framework.JoystickInput)o).SetLeftJoystick = (System.Action<Framework.JoystickData>)v;
        }

        static StackObject* AssignFromStack_SetLeftJoystick_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<Framework.JoystickData> @SetLeftJoystick = (System.Action<Framework.JoystickData>)typeof(System.Action<Framework.JoystickData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.JoystickInput)o).SetLeftJoystick = @SetLeftJoystick;
            return ptr_of_this_method;
        }

        static object get_SendTouchUp_1(ref object o)
        {
            return ((Framework.JoystickInput)o).SendTouchUp;
        }

        static StackObject* CopyToStack_SendTouchUp_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.JoystickInput)o).SendTouchUp;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SendTouchUp_1(ref object o, object v)
        {
            ((Framework.JoystickInput)o).SendTouchUp = (System.Action<UnityEngine.Vector3>)v;
        }

        static StackObject* AssignFromStack_SendTouchUp_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.Vector3> @SendTouchUp = (System.Action<UnityEngine.Vector3>)typeof(System.Action<UnityEngine.Vector3>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.JoystickInput)o).SendTouchUp = @SendTouchUp;
            return ptr_of_this_method;
        }

        static object get_SendTouchRightUp_2(ref object o)
        {
            return ((Framework.JoystickInput)o).SendTouchRightUp;
        }

        static StackObject* CopyToStack_SendTouchRightUp_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.JoystickInput)o).SendTouchRightUp;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SendTouchRightUp_2(ref object o, object v)
        {
            ((Framework.JoystickInput)o).SendTouchRightUp = (System.Action<UnityEngine.Vector3>)v;
        }

        static StackObject* AssignFromStack_SendTouchRightUp_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.Vector3> @SendTouchRightUp = (System.Action<UnityEngine.Vector3>)typeof(System.Action<UnityEngine.Vector3>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.JoystickInput)o).SendTouchRightUp = @SendTouchRightUp;
            return ptr_of_this_method;
        }

        static object get_SendMoveFollowMouse_3(ref object o)
        {
            return ((Framework.JoystickInput)o).SendMoveFollowMouse;
        }

        static StackObject* CopyToStack_SendMoveFollowMouse_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.JoystickInput)o).SendMoveFollowMouse;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SendMoveFollowMouse_3(ref object o, object v)
        {
            ((Framework.JoystickInput)o).SendMoveFollowMouse = (System.Action<UnityEngine.Vector3>)v;
        }

        static StackObject* AssignFromStack_SendMoveFollowMouse_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.Vector3> @SendMoveFollowMouse = (System.Action<UnityEngine.Vector3>)typeof(System.Action<UnityEngine.Vector3>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.JoystickInput)o).SendMoveFollowMouse = @SendMoveFollowMouse;
            return ptr_of_this_method;
        }



    }
}
