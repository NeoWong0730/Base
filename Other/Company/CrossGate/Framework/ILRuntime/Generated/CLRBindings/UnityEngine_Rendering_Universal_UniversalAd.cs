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
    unsafe class UnityEngine_Rendering_Universal_UniversalAdditionalCameraData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData);
            args = new Type[]{};
            method = type.GetMethod("get_requiresDepthOption", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_requiresDepthOption_0);
            args = new Type[]{};
            method = type.GetMethod("get_requiresColorOption", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_requiresColorOption_1);
            args = new Type[]{typeof(UnityEngine.Rendering.Universal.CameraOverrideOption)};
            method = type.GetMethod("set_requiresDepthOption", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_requiresDepthOption_2);
            args = new Type[]{typeof(UnityEngine.Rendering.Universal.CameraOverrideOption)};
            method = type.GetMethod("set_requiresColorOption", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_requiresColorOption_3);
            args = new Type[]{};
            method = type.GetMethod("get_cameraStack", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_cameraStack_4);


        }


        static StackObject* get_requiresDepthOption_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData instance_of_this_method = (UnityEngine.Rendering.Universal.UniversalAdditionalCameraData)typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.requiresDepthOption;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_requiresColorOption_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData instance_of_this_method = (UnityEngine.Rendering.Universal.UniversalAdditionalCameraData)typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.requiresColorOption;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_requiresDepthOption_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Rendering.Universal.CameraOverrideOption @value = (UnityEngine.Rendering.Universal.CameraOverrideOption)typeof(UnityEngine.Rendering.Universal.CameraOverrideOption).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData instance_of_this_method = (UnityEngine.Rendering.Universal.UniversalAdditionalCameraData)typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.requiresDepthOption = value;

            return __ret;
        }

        static StackObject* set_requiresColorOption_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Rendering.Universal.CameraOverrideOption @value = (UnityEngine.Rendering.Universal.CameraOverrideOption)typeof(UnityEngine.Rendering.Universal.CameraOverrideOption).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData instance_of_this_method = (UnityEngine.Rendering.Universal.UniversalAdditionalCameraData)typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.requiresColorOption = value;

            return __ret;
        }

        static StackObject* get_cameraStack_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData instance_of_this_method = (UnityEngine.Rendering.Universal.UniversalAdditionalCameraData)typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.cameraStack;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
