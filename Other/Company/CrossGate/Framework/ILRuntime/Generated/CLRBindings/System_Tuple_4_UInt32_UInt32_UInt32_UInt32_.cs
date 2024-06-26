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
    unsafe class System_Tuple_4_UInt32_UInt32_UInt32_UInt32_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32>);
            args = new Type[]{};
            method = type.GetMethod("get_Item1", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Item1_0);
            args = new Type[]{};
            method = type.GetMethod("get_Item2", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Item2_1);
            args = new Type[]{};
            method = type.GetMethod("get_Item3", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Item3_2);
            args = new Type[]{};
            method = type.GetMethod("get_Item4", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Item4_3);

            args = new Type[]{typeof(System.UInt32), typeof(System.UInt32), typeof(System.UInt32), typeof(System.UInt32)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* get_Item1_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32> instance_of_this_method = (System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32>)typeof(System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Item1;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_Item2_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32> instance_of_this_method = (System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32>)typeof(System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Item2;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_Item3_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32> instance_of_this_method = (System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32>)typeof(System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Item3;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_Item4_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32> instance_of_this_method = (System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32>)typeof(System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Item4;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 @item4 = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.UInt32 @item3 = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.UInt32 @item2 = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.UInt32 @item1 = (uint)ptr_of_this_method->Value;


            var result_of_this_method = new System.Tuple<System.UInt32, System.UInt32, System.UInt32, System.UInt32>(@item1, @item2, @item3, @item4);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
