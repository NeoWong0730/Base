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
    unsafe class System_Action_3_CoroutineAdapter_Binding_Adaptor_CoroutineAdapter_Binding_Adaptor_CoroutineAdapter_Binding_Adaptor_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(System.Action<global::CoroutineAdapter.Adaptor, global::CoroutineAdapter.Adaptor, global::CoroutineAdapter.Adaptor>);
            args = new Type[]{typeof(global::CoroutineAdapter.Adaptor), typeof(global::CoroutineAdapter.Adaptor), typeof(global::CoroutineAdapter.Adaptor)};
            method = type.GetMethod("Invoke", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Invoke_0);


        }


        static StackObject* Invoke_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CoroutineAdapter.Adaptor @arg3 = (global::CoroutineAdapter.Adaptor)typeof(global::CoroutineAdapter.Adaptor).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CoroutineAdapter.Adaptor @arg2 = (global::CoroutineAdapter.Adaptor)typeof(global::CoroutineAdapter.Adaptor).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::CoroutineAdapter.Adaptor @arg1 = (global::CoroutineAdapter.Adaptor)typeof(global::CoroutineAdapter.Adaptor).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Action<global::CoroutineAdapter.Adaptor, global::CoroutineAdapter.Adaptor, global::CoroutineAdapter.Adaptor> instance_of_this_method = (System.Action<global::CoroutineAdapter.Adaptor, global::CoroutineAdapter.Adaptor, global::CoroutineAdapter.Adaptor>)typeof(System.Action<global::CoroutineAdapter.Adaptor, global::CoroutineAdapter.Adaptor, global::CoroutineAdapter.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Invoke(@arg1, @arg2, @arg3);

            return __ret;
        }



    }
}
