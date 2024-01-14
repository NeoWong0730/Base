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

namespace ILRuntime.Runtime.Generated
{
    unsafe class Lib_Core_StringBuilderPool_Binder
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Lib.Core.StringBuilderPool);
            args = new Type[]{};
            method = type.GetMethod("GetTemporary", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetTemporary_0);
            args = new Type[]{typeof(System.Text.StringBuilder)};
            method = type.GetMethod("ReleaseTemporary", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReleaseTemporary_1);
            args = new Type[]{typeof(System.Text.StringBuilder)};
            method = type.GetMethod("ReleaseTemporaryAndToString", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReleaseTemporaryAndToString_2);


        }


        static StackObject* GetTemporary_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

#if DEBUG_MODE && RECODE_STACK
            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            Lib.Core.StringBuilderPool.fromILRuntime = true;
            Lib.Core.StringBuilderPool.requestStack = stackTrace;
#endif
            var result_of_this_method = Lib.Core.StringBuilderPool.GetTemporary();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ReleaseTemporary_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Text.StringBuilder @target = (System.Text.StringBuilder)typeof(System.Text.StringBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Lib.Core.StringBuilderPool.ReleaseTemporary(@target);

            return __ret;
        }

        static StackObject* ReleaseTemporaryAndToString_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Text.StringBuilder @target = (System.Text.StringBuilder)typeof(System.Text.StringBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Lib.Core.StringBuilderPool.ReleaseTemporaryAndToString(@target);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
