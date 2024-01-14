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
    unsafe class HollowOutBg_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::HollowOutBg);
            args = new Type[]{};
            method = type.GetMethod("ClearTargets", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ClearTargets_0);
            args = new Type[]{typeof(UnityEngine.Vector2)};
            method = type.GetMethod("SetMaskPivot", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetMaskPivot_1);
            args = new Type[]{typeof(UnityEngine.EventSystems.PointerEventData)};
            method = type.GetMethod("OnClick_Target", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnClick_Target_2);
            args = new Type[]{typeof(System.Collections.Generic.List<UnityEngine.Transform>), typeof(UnityEngine.Vector2)};
            method = type.GetMethod("SetTargets", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetTargets_3);

            field = type.GetField("action_ClickTarget", flag);
            app.RegisterCLRFieldGetter(field, get_action_ClickTarget_0);
            app.RegisterCLRFieldSetter(field, set_action_ClickTarget_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_action_ClickTarget_0, AssignFromStack_action_ClickTarget_0);
            field = type.GetField("action_ClickBg", flag);
            app.RegisterCLRFieldGetter(field, get_action_ClickBg_1);
            app.RegisterCLRFieldSetter(field, set_action_ClickBg_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_action_ClickBg_1, AssignFromStack_action_ClickBg_1);


        }


        static StackObject* ClearTargets_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::HollowOutBg instance_of_this_method = (global::HollowOutBg)typeof(global::HollowOutBg).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ClearTargets();

            return __ret;
        }

        static StackObject* SetMaskPivot_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector2 @pivot = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @pivot, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @pivot = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::HollowOutBg instance_of_this_method = (global::HollowOutBg)typeof(global::HollowOutBg).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetMaskPivot(@pivot);

            return __ret;
        }

        static StackObject* OnClick_Target_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.EventSystems.PointerEventData @eventData = (UnityEngine.EventSystems.PointerEventData)typeof(UnityEngine.EventSystems.PointerEventData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::HollowOutBg instance_of_this_method = (global::HollowOutBg)typeof(global::HollowOutBg).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnClick_Target(@eventData);

            return __ret;
        }

        static StackObject* SetTargets_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector2 @size = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @size, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @size = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Generic.List<UnityEngine.Transform> @targets = (System.Collections.Generic.List<UnityEngine.Transform>)typeof(System.Collections.Generic.List<UnityEngine.Transform>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::HollowOutBg instance_of_this_method = (global::HollowOutBg)typeof(global::HollowOutBg).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetTargets(@targets, @size);

            return __ret;
        }


        static object get_action_ClickTarget_0(ref object o)
        {
            return ((global::HollowOutBg)o).action_ClickTarget;
        }

        static StackObject* CopyToStack_action_ClickTarget_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HollowOutBg)o).action_ClickTarget;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_action_ClickTarget_0(ref object o, object v)
        {
            ((global::HollowOutBg)o).action_ClickTarget = (System.Action)v;
        }

        static StackObject* AssignFromStack_action_ClickTarget_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @action_ClickTarget = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::HollowOutBg)o).action_ClickTarget = @action_ClickTarget;
            return ptr_of_this_method;
        }

        static object get_action_ClickBg_1(ref object o)
        {
            return ((global::HollowOutBg)o).action_ClickBg;
        }

        static StackObject* CopyToStack_action_ClickBg_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HollowOutBg)o).action_ClickBg;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_action_ClickBg_1(ref object o, object v)
        {
            ((global::HollowOutBg)o).action_ClickBg = (System.Action<UnityEngine.EventSystems.PointerEventData>)v;
        }

        static StackObject* AssignFromStack_action_ClickBg_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.EventSystems.PointerEventData> @action_ClickBg = (System.Action<UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::HollowOutBg)o).action_ClickBg = @action_ClickBg;
            return ptr_of_this_method;
        }



    }
}
