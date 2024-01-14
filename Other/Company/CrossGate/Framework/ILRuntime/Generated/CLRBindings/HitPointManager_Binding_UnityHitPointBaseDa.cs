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
    unsafe class HitPointManager_Binding_UnityHitPointBaseData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::HitPointManager.UnityHitPointBaseData);

            field = type.GetField("test_id", flag);
            app.RegisterCLRFieldGetter(field, get_test_id_0);
            app.RegisterCLRFieldSetter(field, set_test_id_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_test_id_0, AssignFromStack_test_id_0);
            field = type.GetField("network", flag);
            app.RegisterCLRFieldGetter(field, get_network_1);
            app.RegisterCLRFieldSetter(field, set_network_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_network_1, AssignFromStack_network_1);
            field = type.GetField("system_version", flag);
            app.RegisterCLRFieldGetter(field, get_system_version_2);
            app.RegisterCLRFieldSetter(field, set_system_version_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_system_version_2, AssignFromStack_system_version_2);
            field = type.GetField("phone_model", flag);
            app.RegisterCLRFieldGetter(field, get_phone_model_3);
            app.RegisterCLRFieldSetter(field, set_phone_model_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_phone_model_3, AssignFromStack_phone_model_3);
            field = type.GetField("screen_width", flag);
            app.RegisterCLRFieldGetter(field, get_screen_width_4);
            app.RegisterCLRFieldSetter(field, set_screen_width_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_screen_width_4, AssignFromStack_screen_width_4);
            field = type.GetField("pixel_density", flag);
            app.RegisterCLRFieldGetter(field, get_pixel_density_5);
            app.RegisterCLRFieldSetter(field, set_pixel_density_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_pixel_density_5, AssignFromStack_pixel_density_5);
            field = type.GetField("cpu", flag);
            app.RegisterCLRFieldGetter(field, get_cpu_6);
            app.RegisterCLRFieldSetter(field, set_cpu_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_cpu_6, AssignFromStack_cpu_6);
            field = type.GetField("memory_size", flag);
            app.RegisterCLRFieldGetter(field, get_memory_size_7);
            app.RegisterCLRFieldSetter(field, set_memory_size_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_memory_size_7, AssignFromStack_memory_size_7);
            field = type.GetField("account_type", flag);
            app.RegisterCLRFieldGetter(field, get_account_type_8);
            app.RegisterCLRFieldSetter(field, set_account_type_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_account_type_8, AssignFromStack_account_type_8);


        }



        static object get_test_id_0(ref object o)
        {
            return ((global::HitPointManager.UnityHitPointBaseData)o).test_id;
        }

        static StackObject* CopyToStack_test_id_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.UnityHitPointBaseData)o).test_id;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_test_id_0(ref object o, object v)
        {
            ((global::HitPointManager.UnityHitPointBaseData)o).test_id = (System.String)v;
        }

        static StackObject* AssignFromStack_test_id_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @test_id = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::HitPointManager.UnityHitPointBaseData)o).test_id = @test_id;
            return ptr_of_this_method;
        }

        static object get_network_1(ref object o)
        {
            return ((global::HitPointManager.UnityHitPointBaseData)o).network;
        }

        static StackObject* CopyToStack_network_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.UnityHitPointBaseData)o).network;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_network_1(ref object o, object v)
        {
            ((global::HitPointManager.UnityHitPointBaseData)o).network = (System.String)v;
        }

        static StackObject* AssignFromStack_network_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @network = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::HitPointManager.UnityHitPointBaseData)o).network = @network;
            return ptr_of_this_method;
        }

        static object get_system_version_2(ref object o)
        {
            return ((global::HitPointManager.UnityHitPointBaseData)o).system_version;
        }

        static StackObject* CopyToStack_system_version_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.UnityHitPointBaseData)o).system_version;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_system_version_2(ref object o, object v)
        {
            ((global::HitPointManager.UnityHitPointBaseData)o).system_version = (System.String)v;
        }

        static StackObject* AssignFromStack_system_version_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @system_version = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::HitPointManager.UnityHitPointBaseData)o).system_version = @system_version;
            return ptr_of_this_method;
        }

        static object get_phone_model_3(ref object o)
        {
            return ((global::HitPointManager.UnityHitPointBaseData)o).phone_model;
        }

        static StackObject* CopyToStack_phone_model_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.UnityHitPointBaseData)o).phone_model;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_phone_model_3(ref object o, object v)
        {
            ((global::HitPointManager.UnityHitPointBaseData)o).phone_model = (System.String)v;
        }

        static StackObject* AssignFromStack_phone_model_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @phone_model = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::HitPointManager.UnityHitPointBaseData)o).phone_model = @phone_model;
            return ptr_of_this_method;
        }

        static object get_screen_width_4(ref object o)
        {
            return ((global::HitPointManager.UnityHitPointBaseData)o).screen_width;
        }

        static StackObject* CopyToStack_screen_width_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.UnityHitPointBaseData)o).screen_width;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static void set_screen_width_4(ref object o, object v)
        {
            ((global::HitPointManager.UnityHitPointBaseData)o).screen_width = (System.UInt32)v;
        }

        static StackObject* AssignFromStack_screen_width_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.UInt32 @screen_width = (uint)ptr_of_this_method->Value;
            ((global::HitPointManager.UnityHitPointBaseData)o).screen_width = @screen_width;
            return ptr_of_this_method;
        }

        static object get_pixel_density_5(ref object o)
        {
            return ((global::HitPointManager.UnityHitPointBaseData)o).pixel_density;
        }

        static StackObject* CopyToStack_pixel_density_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.UnityHitPointBaseData)o).pixel_density;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static void set_pixel_density_5(ref object o, object v)
        {
            ((global::HitPointManager.UnityHitPointBaseData)o).pixel_density = (System.UInt32)v;
        }

        static StackObject* AssignFromStack_pixel_density_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.UInt32 @pixel_density = (uint)ptr_of_this_method->Value;
            ((global::HitPointManager.UnityHitPointBaseData)o).pixel_density = @pixel_density;
            return ptr_of_this_method;
        }

        static object get_cpu_6(ref object o)
        {
            return ((global::HitPointManager.UnityHitPointBaseData)o).cpu;
        }

        static StackObject* CopyToStack_cpu_6(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.UnityHitPointBaseData)o).cpu;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_cpu_6(ref object o, object v)
        {
            ((global::HitPointManager.UnityHitPointBaseData)o).cpu = (System.String)v;
        }

        static StackObject* AssignFromStack_cpu_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @cpu = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::HitPointManager.UnityHitPointBaseData)o).cpu = @cpu;
            return ptr_of_this_method;
        }

        static object get_memory_size_7(ref object o)
        {
            return ((global::HitPointManager.UnityHitPointBaseData)o).memory_size;
        }

        static StackObject* CopyToStack_memory_size_7(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.UnityHitPointBaseData)o).memory_size;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static void set_memory_size_7(ref object o, object v)
        {
            ((global::HitPointManager.UnityHitPointBaseData)o).memory_size = (System.UInt32)v;
        }

        static StackObject* AssignFromStack_memory_size_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.UInt32 @memory_size = (uint)ptr_of_this_method->Value;
            ((global::HitPointManager.UnityHitPointBaseData)o).memory_size = @memory_size;
            return ptr_of_this_method;
        }

        static object get_account_type_8(ref object o)
        {
            return ((global::HitPointManager.UnityHitPointBaseData)o).account_type;
        }

        static StackObject* CopyToStack_account_type_8(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HitPointManager.UnityHitPointBaseData)o).account_type;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_account_type_8(ref object o, object v)
        {
            ((global::HitPointManager.UnityHitPointBaseData)o).account_type = (System.String)v;
        }

        static StackObject* AssignFromStack_account_type_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @account_type = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::HitPointManager.UnityHitPointBaseData)o).account_type = @account_type;
            return ptr_of_this_method;
        }



    }
}
