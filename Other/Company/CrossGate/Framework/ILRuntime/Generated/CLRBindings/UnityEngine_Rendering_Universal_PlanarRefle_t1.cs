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
    unsafe class UnityEngine_Rendering_Universal_PlanarReflections_Binding_PlanarReflectionSettings_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.Rendering.Universal.PlanarReflections.PlanarReflectionSettings);

            field = type.GetField("m_ClipPlaneOffset", flag);
            app.RegisterCLRFieldGetter(field, get_m_ClipPlaneOffset_0);
            app.RegisterCLRFieldSetter(field, set_m_ClipPlaneOffset_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_ClipPlaneOffset_0, AssignFromStack_m_ClipPlaneOffset_0);


        }



        static object get_m_ClipPlaneOffset_0(ref object o)
        {
            return ((UnityEngine.Rendering.Universal.PlanarReflections.PlanarReflectionSettings)o).m_ClipPlaneOffset;
        }

        static StackObject* CopyToStack_m_ClipPlaneOffset_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Rendering.Universal.PlanarReflections.PlanarReflectionSettings)o).m_ClipPlaneOffset;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m_ClipPlaneOffset_0(ref object o, object v)
        {
            ((UnityEngine.Rendering.Universal.PlanarReflections.PlanarReflectionSettings)o).m_ClipPlaneOffset = (System.Single)v;
        }

        static StackObject* AssignFromStack_m_ClipPlaneOffset_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m_ClipPlaneOffset = *(float*)&ptr_of_this_method->Value;
            ((UnityEngine.Rendering.Universal.PlanarReflections.PlanarReflectionSettings)o).m_ClipPlaneOffset = @m_ClipPlaneOffset;
            return ptr_of_this_method;
        }



    }
}
