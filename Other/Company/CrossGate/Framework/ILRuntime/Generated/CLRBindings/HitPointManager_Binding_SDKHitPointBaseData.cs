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
    unsafe class HitPointManager_Binding_SDKHitPointBaseData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::HitPointManager.SDKHitPointBaseData);

            field = type.GetField("markert_channel", flag);
            app.RegisterCLRFieldGetter(field, get_markert_channel_0);
            app.RegisterCLRFieldSetter(field, set_markert_channel_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_markert_channel_0, AssignFromStack_markert_channel_0);
            field = type.GetField("device_id", flag);
            app.RegisterCLRFieldGetter(field, get_device_id_1);
            app.RegisterCLRFieldSetter(field, set_device_id_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_device_id_1, AssignFromStack_device_id_1);
            field = type.GetField("app_version", flag);
            app.RegisterCLRFieldGetter(field, get_app_version_2);
            app.RegisterCLRFieldSetter(field, set_app_version_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_app_version_2, AssignFromStack_app_version_2);
            field = type.GetField("platform", flag);
            app.RegisterCLRFieldGetter(field, get_platform_3);
            app.RegisterCLRFieldSetter(field, set_platform_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_platform_3, AssignFromStack_platform_3);
            field = type.GetField("operator_type", flag);
            app.RegisterCLRFieldGetter(field, get_operator_type_4);
            app.RegisterCLRFieldSetter(field, set_operator_type_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_operator_type_4, AssignFromStack_operator_type_4);
            field = type.GetField("channel", flag);
            app.RegisterCLRFieldGetter(field, get_channel_5);
            app.RegisterCLRFieldSetter(field, set_channel_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_channel_5, AssignFromStack_channel_5);


        }



        static object get_markert_channel_0(ref object o)
        {
            return ((global::HitPointManager.SDKHitPointBaseData)o).markert_channel;
        }

        static StackObject* CopyToStack_markert_channel_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.SDKHitPointBaseData)o).markert_channel;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_markert_channel_0(ref object o, object v)
        {
            ((global::HitPointManager.SDKHitPointBaseData)o).markert_channel = (System.String)v;
        }

        static StackObject* AssignFromStack_markert_channel_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @markert_channel = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::HitPointManager.SDKHitPointBaseData)o).markert_channel = @markert_channel;
            return ptr_of_this_method;
        }

        static object get_device_id_1(ref object o)
        {
            return ((global::HitPointManager.SDKHitPointBaseData)o).device_id;
        }

        static StackObject* CopyToStack_device_id_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.SDKHitPointBaseData)o).device_id;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_device_id_1(ref object o, object v)
        {
            ((global::HitPointManager.SDKHitPointBaseData)o).device_id = (System.String)v;
        }

        static StackObject* AssignFromStack_device_id_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @device_id = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::HitPointManager.SDKHitPointBaseData)o).device_id = @device_id;
            return ptr_of_this_method;
        }

        static object get_app_version_2(ref object o)
        {
            return ((global::HitPointManager.SDKHitPointBaseData)o).app_version;
        }

        static StackObject* CopyToStack_app_version_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.SDKHitPointBaseData)o).app_version;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_app_version_2(ref object o, object v)
        {
            ((global::HitPointManager.SDKHitPointBaseData)o).app_version = (System.String)v;
        }

        static StackObject* AssignFromStack_app_version_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @app_version = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::HitPointManager.SDKHitPointBaseData)o).app_version = @app_version;
            return ptr_of_this_method;
        }

        static object get_platform_3(ref object o)
        {
            return ((global::HitPointManager.SDKHitPointBaseData)o).platform;
        }

        static StackObject* CopyToStack_platform_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.SDKHitPointBaseData)o).platform;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_platform_3(ref object o, object v)
        {
            ((global::HitPointManager.SDKHitPointBaseData)o).platform = (System.String)v;
        }

        static StackObject* AssignFromStack_platform_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @platform = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::HitPointManager.SDKHitPointBaseData)o).platform = @platform;
            return ptr_of_this_method;
        }

        static object get_operator_type_4(ref object o)
        {
            return ((global::HitPointManager.SDKHitPointBaseData)o).operator_type;
        }

        static StackObject* CopyToStack_operator_type_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.SDKHitPointBaseData)o).operator_type;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_operator_type_4(ref object o, object v)
        {
            ((global::HitPointManager.SDKHitPointBaseData)o).operator_type = (System.String)v;
        }

        static StackObject* AssignFromStack_operator_type_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @operator_type = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::HitPointManager.SDKHitPointBaseData)o).operator_type = @operator_type;
            return ptr_of_this_method;
        }

        static object get_channel_5(ref object o)
        {
            return ((global::HitPointManager.SDKHitPointBaseData)o).channel;
        }

        static StackObject* CopyToStack_channel_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.SDKHitPointBaseData)o).channel;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_channel_5(ref object o, object v)
        {
            ((global::HitPointManager.SDKHitPointBaseData)o).channel = (System.String)v;
        }

        static StackObject* AssignFromStack_channel_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @channel = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::HitPointManager.SDKHitPointBaseData)o).channel = @channel;
            return ptr_of_this_method;
        }



    }
}
