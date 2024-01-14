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
    unsafe class UnityEngine_Rendering_Universal_PlanarReflections_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.Rendering.Universal.PlanarReflections);

            field = type.GetField("m_settings", flag);
            app.RegisterCLRFieldGetter(field, get_m_settings_0);
            app.RegisterCLRFieldSetter(field, set_m_settings_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_settings_0, AssignFromStack_m_settings_0);


        }



        static object get_m_settings_0(ref object o)
        {
            return ((UnityEngine.Rendering.Universal.PlanarReflections)o).m_settings;
        }

        static StackObject* CopyToStack_m_settings_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Rendering.Universal.PlanarReflections)o).m_settings;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_settings_0(ref object o, object v)
        {
            ((UnityEngine.Rendering.Universal.PlanarReflections)o).m_settings = (UnityEngine.Rendering.Universal.PlanarReflections.PlanarReflectionSettings)v;
        }

        static StackObject* AssignFromStack_m_settings_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Rendering.Universal.PlanarReflections.PlanarReflectionSettings @m_settings = (UnityEngine.Rendering.Universal.PlanarReflections.PlanarReflectionSettings)typeof(UnityEngine.Rendering.Universal.PlanarReflections.PlanarReflectionSettings).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((UnityEngine.Rendering.Universal.PlanarReflections)o).m_settings = @m_settings;
            return ptr_of_this_method;
        }



    }
}
