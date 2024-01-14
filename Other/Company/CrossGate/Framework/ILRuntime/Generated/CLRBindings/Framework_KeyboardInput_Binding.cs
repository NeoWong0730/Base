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
    unsafe class Framework_KeyboardInput_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.KeyboardInput);
            args = new Type[]{typeof(System.Single), typeof(System.Single)};
            method = type.GetMethod("KeyMove", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, KeyMove_0);

            field = type.GetField("SetLeftJoystick", flag);
            app.RegisterCLRFieldGetter(field, get_SetLeftJoystick_0);
            app.RegisterCLRFieldSetter(field, set_SetLeftJoystick_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_SetLeftJoystick_0, AssignFromStack_SetLeftJoystick_0);
            field = type.GetField("SetRightJoystick", flag);
            app.RegisterCLRFieldGetter(field, get_SetRightJoystick_1);
            app.RegisterCLRFieldSetter(field, set_SetRightJoystick_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_SetRightJoystick_1, AssignFromStack_SetRightJoystick_1);
            field = type.GetField("SendInput", flag);
            app.RegisterCLRFieldGetter(field, get_SendInput_2);
            app.RegisterCLRFieldSetter(field, set_SendInput_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_SendInput_2, AssignFromStack_SendInput_2);
            field = type.GetField("SendScale", flag);
            app.RegisterCLRFieldGetter(field, get_SendScale_3);
            app.RegisterCLRFieldSetter(field, set_SendScale_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_SendScale_3, AssignFromStack_SendScale_3);
            field = type.GetField("SendHotKey", flag);
            app.RegisterCLRFieldGetter(field, get_SendHotKey_4);
            app.RegisterCLRFieldSetter(field, set_SendHotKey_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_SendHotKey_4, AssignFromStack_SendHotKey_4);


        }


        static StackObject* KeyMove_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @lv = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @lh = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Framework.KeyboardInput instance_of_this_method = (Framework.KeyboardInput)typeof(Framework.KeyboardInput).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.KeyMove(@lh, @lv);

            return __ret;
        }


        static object get_SetLeftJoystick_0(ref object o)
        {
            return ((Framework.KeyboardInput)o).SetLeftJoystick;
        }

        static StackObject* CopyToStack_SetLeftJoystick_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.KeyboardInput)o).SetLeftJoystick;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SetLeftJoystick_0(ref object o, object v)
        {
            ((Framework.KeyboardInput)o).SetLeftJoystick = (System.Action<Framework.JoystickData>)v;
        }

        static StackObject* AssignFromStack_SetLeftJoystick_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<Framework.JoystickData> @SetLeftJoystick = (System.Action<Framework.JoystickData>)typeof(System.Action<Framework.JoystickData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.KeyboardInput)o).SetLeftJoystick = @SetLeftJoystick;
            return ptr_of_this_method;
        }

        static object get_SetRightJoystick_1(ref object o)
        {
            return ((Framework.KeyboardInput)o).SetRightJoystick;
        }

        static StackObject* CopyToStack_SetRightJoystick_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.KeyboardInput)o).SetRightJoystick;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SetRightJoystick_1(ref object o, object v)
        {
            ((Framework.KeyboardInput)o).SetRightJoystick = (System.Action<Framework.JoystickData>)v;
        }

        static StackObject* AssignFromStack_SetRightJoystick_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<Framework.JoystickData> @SetRightJoystick = (System.Action<Framework.JoystickData>)typeof(System.Action<Framework.JoystickData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.KeyboardInput)o).SetRightJoystick = @SetRightJoystick;
            return ptr_of_this_method;
        }

        static object get_SendInput_2(ref object o)
        {
            return ((Framework.KeyboardInput)o).SendInput;
        }

        static StackObject* CopyToStack_SendInput_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.KeyboardInput)o).SendInput;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SendInput_2(ref object o, object v)
        {
            ((Framework.KeyboardInput)o).SendInput = (System.Action<UnityEngine.KeyCode>)v;
        }

        static StackObject* AssignFromStack_SendInput_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.KeyCode> @SendInput = (System.Action<UnityEngine.KeyCode>)typeof(System.Action<UnityEngine.KeyCode>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.KeyboardInput)o).SendInput = @SendInput;
            return ptr_of_this_method;
        }

        static object get_SendScale_3(ref object o)
        {
            return ((Framework.KeyboardInput)o).SendScale;
        }

        static StackObject* CopyToStack_SendScale_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.KeyboardInput)o).SendScale;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SendScale_3(ref object o, object v)
        {
            ((Framework.KeyboardInput)o).SendScale = (System.Action<System.Single>)v;
        }

        static StackObject* AssignFromStack_SendScale_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.Single> @SendScale = (System.Action<System.Single>)typeof(System.Action<System.Single>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.KeyboardInput)o).SendScale = @SendScale;
            return ptr_of_this_method;
        }

        static object get_SendHotKey_4(ref object o)
        {
            return ((Framework.KeyboardInput)o).SendHotKey;
        }

        static StackObject* CopyToStack_SendHotKey_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.KeyboardInput)o).SendHotKey;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SendHotKey_4(ref object o, object v)
        {
            ((Framework.KeyboardInput)o).SendHotKey = (System.Action)v;
        }

        static StackObject* AssignFromStack_SendHotKey_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @SendHotKey = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.KeyboardInput)o).SendHotKey = @SendHotKey;
            return ptr_of_this_method;
        }



    }
}
