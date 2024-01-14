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
    unsafe class UIScrollCenterEx_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIScrollCenterEx);
            args = new Type[]{typeof(UnityEngine.GameObject), typeof(System.Int32)};
            method = type.GetMethod("SetParam", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetParam_0);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_scrollOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_scrollOffset_1);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("Init", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Init_2);
            args = new Type[]{typeof(System.Int32), typeof(System.Boolean), typeof(System.Single)};
            method = type.GetMethod("SwitchIndex", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SwitchIndex_3);

            field = type.GetField("m_itemSetHandler", flag);
            app.RegisterCLRFieldGetter(field, get_m_itemSetHandler_0);
            app.RegisterCLRFieldSetter(field, set_m_itemSetHandler_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_itemSetHandler_0, AssignFromStack_m_itemSetHandler_0);
            field = type.GetField("cb_CenterChildSettle", flag);
            app.RegisterCLRFieldGetter(field, get_cb_CenterChildSettle_1);
            app.RegisterCLRFieldSetter(field, set_cb_CenterChildSettle_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_cb_CenterChildSettle_1, AssignFromStack_cb_CenterChildSettle_1);
            field = type.GetField("canDrag", flag);
            app.RegisterCLRFieldGetter(field, get_canDrag_2);
            app.RegisterCLRFieldSetter(field, set_canDrag_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_canDrag_2, AssignFromStack_canDrag_2);


        }


        static StackObject* SetParam_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @_poolCount = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.GameObject @_prefab = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIScrollCenterEx instance_of_this_method = (global::UIScrollCenterEx)typeof(global::UIScrollCenterEx).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetParam(@_prefab, @_poolCount);

            return __ret;
        }

        static StackObject* set_scrollOffset_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIScrollCenterEx instance_of_this_method = (global::UIScrollCenterEx)typeof(global::UIScrollCenterEx).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.scrollOffset = value;

            return __ret;
        }

        static StackObject* Init_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @totalCount = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIScrollCenterEx instance_of_this_method = (global::UIScrollCenterEx)typeof(global::UIScrollCenterEx).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Init(@totalCount);

            return __ret;
        }

        static StackObject* SwitchIndex_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @speed = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @quickly = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            global::UIScrollCenterEx instance_of_this_method = (global::UIScrollCenterEx)typeof(global::UIScrollCenterEx).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SwitchIndex(@index, @quickly, @speed);

            return __ret;
        }


        static object get_m_itemSetHandler_0(ref object o)
        {
            return ((global::UIScrollCenterEx)o).m_itemSetHandler;
        }

        static StackObject* CopyToStack_m_itemSetHandler_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UIScrollCenterEx)o).m_itemSetHandler;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_itemSetHandler_0(ref object o, object v)
        {
            ((global::UIScrollCenterEx)o).m_itemSetHandler = (global::UIScrollCenterEx.UpdateChildrenCallbackDelegate)v;
        }

        static StackObject* AssignFromStack_m_itemSetHandler_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIScrollCenterEx.UpdateChildrenCallbackDelegate @m_itemSetHandler = (global::UIScrollCenterEx.UpdateChildrenCallbackDelegate)typeof(global::UIScrollCenterEx.UpdateChildrenCallbackDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::UIScrollCenterEx)o).m_itemSetHandler = @m_itemSetHandler;
            return ptr_of_this_method;
        }

        static object get_cb_CenterChildSettle_1(ref object o)
        {
            return ((global::UIScrollCenterEx)o).cb_CenterChildSettle;
        }

        static StackObject* CopyToStack_cb_CenterChildSettle_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UIScrollCenterEx)o).cb_CenterChildSettle;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_cb_CenterChildSettle_1(ref object o, object v)
        {
            ((global::UIScrollCenterEx)o).cb_CenterChildSettle = (global::UIScrollCenterEx.OnCenterDelegate)v;
        }

        static StackObject* AssignFromStack_cb_CenterChildSettle_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIScrollCenterEx.OnCenterDelegate @cb_CenterChildSettle = (global::UIScrollCenterEx.OnCenterDelegate)typeof(global::UIScrollCenterEx.OnCenterDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::UIScrollCenterEx)o).cb_CenterChildSettle = @cb_CenterChildSettle;
            return ptr_of_this_method;
        }

        static object get_canDrag_2(ref object o)
        {
            return ((global::UIScrollCenterEx)o).canDrag;
        }

        static StackObject* CopyToStack_canDrag_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UIScrollCenterEx)o).canDrag;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_canDrag_2(ref object o, object v)
        {
            ((global::UIScrollCenterEx)o).canDrag = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_canDrag_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @canDrag = ptr_of_this_method->Value == 1;
            ((global::UIScrollCenterEx)o).canDrag = @canDrag;
            return ptr_of_this_method;
        }



    }
}
