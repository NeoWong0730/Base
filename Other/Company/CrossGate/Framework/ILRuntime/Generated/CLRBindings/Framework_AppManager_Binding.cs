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
    unsafe class Framework_AppManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.AppManager);
            args = new Type[]{};
            method = type.GetMethod("Quit", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Quit_0);
            args = new Type[]{};
            method = type.GetMethod("DestroyUIProgressBar", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DestroyUIProgressBar_1);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_InitGameProgress", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_InitGameProgress_2);

            field = type.GetField("mEventSystem", flag);
            app.RegisterCLRFieldGetter(field, get_mEventSystem_0);
            app.RegisterCLRFieldSetter(field, set_mEventSystem_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_mEventSystem_0, AssignFromStack_mEventSystem_0);
            field = type.GetField("nPerformanceScore", flag);
            app.RegisterCLRFieldGetter(field, get_nPerformanceScore_1);
            app.RegisterCLRFieldSetter(field, set_nPerformanceScore_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_nPerformanceScore_1, AssignFromStack_nPerformanceScore_1);


        }


        static StackObject* Quit_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            Framework.AppManager.Quit();

            return __ret;
        }

        static StackObject* DestroyUIProgressBar_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            Framework.AppManager.DestroyUIProgressBar();

            return __ret;
        }

        static StackObject* set_InitGameProgress_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;


            Framework.AppManager.InitGameProgress = value;

            return __ret;
        }


        static object get_mEventSystem_0(ref object o)
        {
            return Framework.AppManager.mEventSystem;
        }

        static StackObject* CopyToStack_mEventSystem_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = Framework.AppManager.mEventSystem;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_mEventSystem_0(ref object o, object v)
        {
            Framework.AppManager.mEventSystem = (UnityEngine.EventSystems.EventSystem)v;
        }

        static StackObject* AssignFromStack_mEventSystem_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.EventSystems.EventSystem @mEventSystem = (UnityEngine.EventSystems.EventSystem)typeof(UnityEngine.EventSystems.EventSystem).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            Framework.AppManager.mEventSystem = @mEventSystem;
            return ptr_of_this_method;
        }

        static object get_nPerformanceScore_1(ref object o)
        {
            return Framework.AppManager.nPerformanceScore;
        }

        static StackObject* CopyToStack_nPerformanceScore_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = Framework.AppManager.nPerformanceScore;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_nPerformanceScore_1(ref object o, object v)
        {
            Framework.AppManager.nPerformanceScore = (System.Int32)v;
        }

        static StackObject* AssignFromStack_nPerformanceScore_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @nPerformanceScore = ptr_of_this_method->Value;
            Framework.AppManager.nPerformanceScore = @nPerformanceScore;
            return ptr_of_this_method;
        }



    }
}
