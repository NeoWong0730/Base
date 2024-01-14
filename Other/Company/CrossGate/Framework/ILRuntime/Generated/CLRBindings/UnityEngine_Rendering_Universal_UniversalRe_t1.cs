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
    unsafe class UnityEngine_Rendering_Universal_UniversalRenderPipelineAsset_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_msaaSampleCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_msaaSampleCount_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_supportsHDR", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_supportsHDR_1);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_maxAdditionalLightsCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_maxAdditionalLightsCount_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_mainLightShadowmapResolution", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_mainLightShadowmapResolution_3);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_renderScale", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_renderScale_4);


        }


        static StackObject* set_msaaSampleCount_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset instance_of_this_method = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)typeof(UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.msaaSampleCount = value;

            return __ret;
        }

        static StackObject* set_supportsHDR_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset instance_of_this_method = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)typeof(UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.supportsHDR = value;

            return __ret;
        }

        static StackObject* set_maxAdditionalLightsCount_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset instance_of_this_method = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)typeof(UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.maxAdditionalLightsCount = value;

            return __ret;
        }

        static StackObject* set_mainLightShadowmapResolution_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset instance_of_this_method = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)typeof(UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.mainLightShadowmapResolution = value;

            return __ret;
        }

        static StackObject* set_renderScale_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset instance_of_this_method = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)typeof(UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.renderScale = value;

            return __ret;
        }



    }
}
