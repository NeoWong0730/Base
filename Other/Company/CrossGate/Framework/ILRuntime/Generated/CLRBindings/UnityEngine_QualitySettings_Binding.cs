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
    unsafe class UnityEngine_QualitySettings_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.QualitySettings);
            args = new Type[]{};
            method = type.GetMethod("get_streamingMipmapsActive", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_streamingMipmapsActive_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_streamingMipmapsActive", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_streamingMipmapsActive_1);
            args = new Type[]{};
            method = type.GetMethod("get_streamingMipmapsMemoryBudget", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_streamingMipmapsMemoryBudget_2);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_streamingMipmapsMemoryBudget", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_streamingMipmapsMemoryBudget_3);
            args = new Type[]{};
            method = type.GetMethod("get_streamingMipmapsRenderersPerFrame", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_streamingMipmapsRenderersPerFrame_4);
            args = new Type[]{};
            method = type.GetMethod("get_streamingMipmapsMaxLevelReduction", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_streamingMipmapsMaxLevelReduction_5);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_streamingMipmapsMaxLevelReduction", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_streamingMipmapsMaxLevelReduction_6);
            args = new Type[]{};
            method = type.GetMethod("get_streamingMipmapsMaxFileIORequests", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_streamingMipmapsMaxFileIORequests_7);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_streamingMipmapsMaxFileIORequests", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_streamingMipmapsMaxFileIORequests_8);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetQualityLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetQualityLevel_9);


        }


        static StackObject* get_streamingMipmapsActive_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.QualitySettings.streamingMipmapsActive;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* set_streamingMipmapsActive_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;


            UnityEngine.QualitySettings.streamingMipmapsActive = value;

            return __ret;
        }

        static StackObject* get_streamingMipmapsMemoryBudget_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.QualitySettings.streamingMipmapsMemoryBudget;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_streamingMipmapsMemoryBudget_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;


            UnityEngine.QualitySettings.streamingMipmapsMemoryBudget = value;

            return __ret;
        }

        static StackObject* get_streamingMipmapsRenderersPerFrame_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.QualitySettings.streamingMipmapsRenderersPerFrame;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_streamingMipmapsMaxLevelReduction_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.QualitySettings.streamingMipmapsMaxLevelReduction;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_streamingMipmapsMaxLevelReduction_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;


            UnityEngine.QualitySettings.streamingMipmapsMaxLevelReduction = value;

            return __ret;
        }

        static StackObject* get_streamingMipmapsMaxFileIORequests_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.QualitySettings.streamingMipmapsMaxFileIORequests;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_streamingMipmapsMaxFileIORequests_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;


            UnityEngine.QualitySettings.streamingMipmapsMaxFileIORequests = value;

            return __ret;
        }

        static StackObject* SetQualityLevel_9(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @index = ptr_of_this_method->Value;


            UnityEngine.QualitySettings.SetQualityLevel(@index);

            return __ret;
        }



    }
}
