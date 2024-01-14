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
    unsafe class UI_LongPressButton_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UI_LongPressButton);

            field = type.GetField("onStartPress", flag);
            app.RegisterCLRFieldGetter(field, get_onStartPress_0);
            app.RegisterCLRFieldSetter(field, set_onStartPress_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onStartPress_0, AssignFromStack_onStartPress_0);
            field = type.GetField("interval", flag);
            app.RegisterCLRFieldGetter(field, get_interval_1);
            app.RegisterCLRFieldSetter(field, set_interval_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_interval_1, AssignFromStack_interval_1);
            field = type.GetField("bPressAcc", flag);
            app.RegisterCLRFieldGetter(field, get_bPressAcc_2);
            app.RegisterCLRFieldSetter(field, set_bPressAcc_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_bPressAcc_2, AssignFromStack_bPressAcc_2);
            field = type.GetField("onRelease", flag);
            app.RegisterCLRFieldGetter(field, get_onRelease_3);
            app.RegisterCLRFieldSetter(field, set_onRelease_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_onRelease_3, AssignFromStack_onRelease_3);
            field = type.GetField("OnPressAcc", flag);
            app.RegisterCLRFieldGetter(field, get_OnPressAcc_4);
            app.RegisterCLRFieldSetter(field, set_OnPressAcc_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnPressAcc_4, AssignFromStack_OnPressAcc_4);
            field = type.GetField("bLongPressed", flag);
            app.RegisterCLRFieldGetter(field, get_bLongPressed_5);
            app.RegisterCLRFieldSetter(field, set_bLongPressed_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_bLongPressed_5, AssignFromStack_bLongPressed_5);
            field = type.GetField("onClick", flag);
            app.RegisterCLRFieldGetter(field, get_onClick_6);
            app.RegisterCLRFieldSetter(field, set_onClick_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_onClick_6, AssignFromStack_onClick_6);
            field = type.GetField("onLongPress", flag);
            app.RegisterCLRFieldGetter(field, get_onLongPress_7);
            app.RegisterCLRFieldSetter(field, set_onLongPress_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_onLongPress_7, AssignFromStack_onLongPress_7);
            field = type.GetField("onClickDown", flag);
            app.RegisterCLRFieldGetter(field, get_onClickDown_8);
            app.RegisterCLRFieldSetter(field, set_onClickDown_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_onClickDown_8, AssignFromStack_onClickDown_8);
            field = type.GetField("onPress", flag);
            app.RegisterCLRFieldGetter(field, get_onPress_9);
            app.RegisterCLRFieldSetter(field, set_onPress_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_onPress_9, AssignFromStack_onPress_9);


        }



        static object get_onStartPress_0(ref object o)
        {
            return ((global::UI_LongPressButton)o).onStartPress;
        }

        static StackObject* CopyToStack_onStartPress_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UI_LongPressButton)o).onStartPress;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onStartPress_0(ref object o, object v)
        {
            ((global::UI_LongPressButton)o).onStartPress = (UnityEngine.Events.UnityEvent)v;
        }

        static StackObject* AssignFromStack_onStartPress_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Events.UnityEvent @onStartPress = (UnityEngine.Events.UnityEvent)typeof(UnityEngine.Events.UnityEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::UI_LongPressButton)o).onStartPress = @onStartPress;
            return ptr_of_this_method;
        }

        static object get_interval_1(ref object o)
        {
            return ((global::UI_LongPressButton)o).interval;
        }

        static StackObject* CopyToStack_interval_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UI_LongPressButton)o).interval;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_interval_1(ref object o, object v)
        {
            ((global::UI_LongPressButton)o).interval = (System.Single)v;
        }

        static StackObject* AssignFromStack_interval_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @interval = *(float*)&ptr_of_this_method->Value;
            ((global::UI_LongPressButton)o).interval = @interval;
            return ptr_of_this_method;
        }

        static object get_bPressAcc_2(ref object o)
        {
            return ((global::UI_LongPressButton)o).bPressAcc;
        }

        static StackObject* CopyToStack_bPressAcc_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UI_LongPressButton)o).bPressAcc;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_bPressAcc_2(ref object o, object v)
        {
            ((global::UI_LongPressButton)o).bPressAcc = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_bPressAcc_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @bPressAcc = ptr_of_this_method->Value == 1;
            ((global::UI_LongPressButton)o).bPressAcc = @bPressAcc;
            return ptr_of_this_method;
        }

        static object get_onRelease_3(ref object o)
        {
            return ((global::UI_LongPressButton)o).onRelease;
        }

        static StackObject* CopyToStack_onRelease_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UI_LongPressButton)o).onRelease;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onRelease_3(ref object o, object v)
        {
            ((global::UI_LongPressButton)o).onRelease = (UnityEngine.Events.UnityEvent)v;
        }

        static StackObject* AssignFromStack_onRelease_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Events.UnityEvent @onRelease = (UnityEngine.Events.UnityEvent)typeof(UnityEngine.Events.UnityEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::UI_LongPressButton)o).onRelease = @onRelease;
            return ptr_of_this_method;
        }

        static object get_OnPressAcc_4(ref object o)
        {
            return ((global::UI_LongPressButton)o).OnPressAcc;
        }

        static StackObject* CopyToStack_OnPressAcc_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UI_LongPressButton)o).OnPressAcc;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnPressAcc_4(ref object o, object v)
        {
            ((global::UI_LongPressButton)o).OnPressAcc = (UnityEngine.Events.UnityEvent)v;
        }

        static StackObject* AssignFromStack_OnPressAcc_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Events.UnityEvent @OnPressAcc = (UnityEngine.Events.UnityEvent)typeof(UnityEngine.Events.UnityEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::UI_LongPressButton)o).OnPressAcc = @OnPressAcc;
            return ptr_of_this_method;
        }

        static object get_bLongPressed_5(ref object o)
        {
            return ((global::UI_LongPressButton)o).bLongPressed;
        }

        static StackObject* CopyToStack_bLongPressed_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UI_LongPressButton)o).bLongPressed;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_bLongPressed_5(ref object o, object v)
        {
            ((global::UI_LongPressButton)o).bLongPressed = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_bLongPressed_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @bLongPressed = ptr_of_this_method->Value == 1;
            ((global::UI_LongPressButton)o).bLongPressed = @bLongPressed;
            return ptr_of_this_method;
        }

        static object get_onClick_6(ref object o)
        {
            return ((global::UI_LongPressButton)o).onClick;
        }

        static StackObject* CopyToStack_onClick_6(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UI_LongPressButton)o).onClick;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onClick_6(ref object o, object v)
        {
            ((global::UI_LongPressButton)o).onClick = (UnityEngine.Events.UnityEvent)v;
        }

        static StackObject* AssignFromStack_onClick_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Events.UnityEvent @onClick = (UnityEngine.Events.UnityEvent)typeof(UnityEngine.Events.UnityEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::UI_LongPressButton)o).onClick = @onClick;
            return ptr_of_this_method;
        }

        static object get_onLongPress_7(ref object o)
        {
            return ((global::UI_LongPressButton)o).onLongPress;
        }

        static StackObject* CopyToStack_onLongPress_7(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UI_LongPressButton)o).onLongPress;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onLongPress_7(ref object o, object v)
        {
            ((global::UI_LongPressButton)o).onLongPress = (global::UI_LongPressButton.LongPress)v;
        }

        static StackObject* AssignFromStack_onLongPress_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UI_LongPressButton.LongPress @onLongPress = (global::UI_LongPressButton.LongPress)typeof(global::UI_LongPressButton.LongPress).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::UI_LongPressButton)o).onLongPress = @onLongPress;
            return ptr_of_this_method;
        }

        static object get_onClickDown_8(ref object o)
        {
            return ((global::UI_LongPressButton)o).onClickDown;
        }

        static StackObject* CopyToStack_onClickDown_8(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UI_LongPressButton)o).onClickDown;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onClickDown_8(ref object o, object v)
        {
            ((global::UI_LongPressButton)o).onClickDown = (UnityEngine.Events.UnityEvent)v;
        }

        static StackObject* AssignFromStack_onClickDown_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Events.UnityEvent @onClickDown = (UnityEngine.Events.UnityEvent)typeof(UnityEngine.Events.UnityEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::UI_LongPressButton)o).onClickDown = @onClickDown;
            return ptr_of_this_method;
        }

        static object get_onPress_9(ref object o)
        {
            return ((global::UI_LongPressButton)o).onPress;
        }

        static StackObject* CopyToStack_onPress_9(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UI_LongPressButton)o).onPress;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onPress_9(ref object o, object v)
        {
            ((global::UI_LongPressButton)o).onPress = (UnityEngine.Events.UnityEvent)v;
        }

        static StackObject* AssignFromStack_onPress_9(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Events.UnityEvent @onPress = (UnityEngine.Events.UnityEvent)typeof(UnityEngine.Events.UnityEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::UI_LongPressButton)o).onPress = @onPress;
            return ptr_of_this_method;
        }



    }
}
