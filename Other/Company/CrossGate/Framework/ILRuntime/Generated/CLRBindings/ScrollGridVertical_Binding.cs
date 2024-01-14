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
    unsafe class ScrollGridVertical_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::ScrollGridVertical);
            args = new Type[]{typeof(System.Action<global::ScrollGridCell>)};
            method = type.GetMethod("AddCellListener", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddCellListener_0);
            args = new Type[]{typeof(System.Action<global::ScrollGridCell>)};
            method = type.GetMethod("AddCreateCellListener", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddCreateCellListener_1);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetCellCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetCellCount_2);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("FixedPosition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, FixedPosition_3);
            args = new Type[]{};
            method = type.GetMethod("RefreshAllCells", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RefreshAllCells_4);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("MoveToTopOrBotton", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MoveToTopOrBotton_5);
            args = new Type[]{typeof(UnityEngine.GameObject)};
            method = type.GetMethod("RefreshOneCell", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RefreshOneCell_6);


        }


        static StackObject* AddCellListener_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<global::ScrollGridCell> @call = (System.Action<global::ScrollGridCell>)typeof(System.Action<global::ScrollGridCell>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::ScrollGridVertical instance_of_this_method = (global::ScrollGridVertical)typeof(global::ScrollGridVertical).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddCellListener(@call);

            return __ret;
        }

        static StackObject* AddCreateCellListener_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<global::ScrollGridCell> @call = (System.Action<global::ScrollGridCell>)typeof(System.Action<global::ScrollGridCell>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::ScrollGridVertical instance_of_this_method = (global::ScrollGridVertical)typeof(global::ScrollGridVertical).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddCreateCellListener(@call);

            return __ret;
        }

        static StackObject* SetCellCount_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @count = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::ScrollGridVertical instance_of_this_method = (global::ScrollGridVertical)typeof(global::ScrollGridVertical).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetCellCount(@count);

            return __ret;
        }

        static StackObject* FixedPosition_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::ScrollGridVertical instance_of_this_method = (global::ScrollGridVertical)typeof(global::ScrollGridVertical).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.FixedPosition(@value);

            return __ret;
        }

        static StackObject* RefreshAllCells_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::ScrollGridVertical instance_of_this_method = (global::ScrollGridVertical)typeof(global::ScrollGridVertical).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RefreshAllCells();

            return __ret;
        }

        static StackObject* MoveToTopOrBotton_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isBotton = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::ScrollGridVertical instance_of_this_method = (global::ScrollGridVertical)typeof(global::ScrollGridVertical).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.MoveToTopOrBotton(@isBotton);

            return __ret;
        }

        static StackObject* RefreshOneCell_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject @target = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::ScrollGridVertical instance_of_this_method = (global::ScrollGridVertical)typeof(global::ScrollGridVertical).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RefreshOneCell(@target);

            return __ret;
        }



    }
}
