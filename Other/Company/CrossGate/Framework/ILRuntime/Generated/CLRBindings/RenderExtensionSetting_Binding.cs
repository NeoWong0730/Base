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
    unsafe class RenderExtensionSetting_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::RenderExtensionSetting);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_bUsageOcclusionTransparent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_bUsageOcclusionTransparent_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_bUsageGrass", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_bUsageGrass_1);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("SetGrassInteractive", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetGrassInteractive_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_nSceneMaxLOD", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_nSceneMaxLOD_3);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_UseDepthTexture", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_UseDepthTexture_4);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_bUsageOutLine", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_bUsageOutLine_5);


        }


        static StackObject* set_bUsageOcclusionTransparent_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;


            global::RenderExtensionSetting.bUsageOcclusionTransparent = value;

            return __ret;
        }

        static StackObject* set_bUsageGrass_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;


            global::RenderExtensionSetting.bUsageGrass = value;

            return __ret;
        }

        static StackObject* SetGrassInteractive_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @v = ptr_of_this_method->Value == 1;


            global::RenderExtensionSetting.SetGrassInteractive(@v);

            return __ret;
        }

        static StackObject* set_nSceneMaxLOD_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;


            global::RenderExtensionSetting.nSceneMaxLOD = value;

            return __ret;
        }

        static StackObject* set_UseDepthTexture_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;


            global::RenderExtensionSetting.UseDepthTexture = value;

            return __ret;
        }

        static StackObject* set_bUsageOutLine_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;


            global::RenderExtensionSetting.bUsageOutLine = value;

            return __ret;
        }



    }
}
