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
    unsafe class UnityEngine_Profiling_Profiler_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.Profiling.Profiler);
            args = new Type[]{};
            method = type.GetMethod("GetMonoHeapSizeLong", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetMonoHeapSizeLong_0);
            args = new Type[]{};
            method = type.GetMethod("GetMonoUsedSizeLong", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetMonoUsedSizeLong_1);
            args = new Type[]{};
            method = type.GetMethod("GetTotalAllocatedMemoryLong", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetTotalAllocatedMemoryLong_2);
            args = new Type[]{};
            method = type.GetMethod("GetTotalReservedMemoryLong", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetTotalReservedMemoryLong_3);
            args = new Type[]{};
            method = type.GetMethod("GetTotalUnusedReservedMemoryLong", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetTotalUnusedReservedMemoryLong_4);
            args = new Type[]{};
            method = type.GetMethod("GetTempAllocatorSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetTempAllocatorSize_5);


        }


        static StackObject* GetMonoHeapSizeLong_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetMonoUsedSizeLong_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetTotalAllocatedMemoryLong_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetTotalReservedMemoryLong_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetTotalUnusedReservedMemoryLong_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong();

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetTempAllocatorSize_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.Profiling.Profiler.GetTempAllocatorSize();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }



    }
}
