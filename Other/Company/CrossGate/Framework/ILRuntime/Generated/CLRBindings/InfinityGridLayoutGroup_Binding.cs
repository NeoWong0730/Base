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
    unsafe class InfinityGridLayoutGroup_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::InfinityGridLayoutGroup);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetAmount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetAmount_0);
            args = new Type[]{typeof(System.Int32), typeof(System.Single)};
            method = type.GetMethod("MoveToCellIndex", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MoveToCellIndex_1);

            field = type.GetField("updateChildrenCallback", flag);
            app.RegisterCLRFieldGetter(field, get_updateChildrenCallback_0);
            app.RegisterCLRFieldSetter(field, set_updateChildrenCallback_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_updateChildrenCallback_0, AssignFromStack_updateChildrenCallback_0);
            field = type.GetField("minAmount", flag);
            app.RegisterCLRFieldGetter(field, get_minAmount_1);
            app.RegisterCLRFieldSetter(field, set_minAmount_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_minAmount_1, AssignFromStack_minAmount_1);


        }


        static StackObject* SetAmount_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @count = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::InfinityGridLayoutGroup instance_of_this_method = (global::InfinityGridLayoutGroup)typeof(global::InfinityGridLayoutGroup).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetAmount(@count);

            return __ret;
        }

        static StackObject* MoveToCellIndex_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @duration = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::InfinityGridLayoutGroup instance_of_this_method = (global::InfinityGridLayoutGroup)typeof(global::InfinityGridLayoutGroup).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.MoveToCellIndex(@index, @duration);

            return __ret;
        }


        static object get_updateChildrenCallback_0(ref object o)
        {
            return ((global::InfinityGridLayoutGroup)o).updateChildrenCallback;
        }

        static StackObject* CopyToStack_updateChildrenCallback_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::InfinityGridLayoutGroup)o).updateChildrenCallback;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_updateChildrenCallback_0(ref object o, object v)
        {
            ((global::InfinityGridLayoutGroup)o).updateChildrenCallback = (global::InfinityGridLayoutGroup.UpdateChildrenCallbackDelegate)v;
        }

        static StackObject* AssignFromStack_updateChildrenCallback_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::InfinityGridLayoutGroup.UpdateChildrenCallbackDelegate @updateChildrenCallback = (global::InfinityGridLayoutGroup.UpdateChildrenCallbackDelegate)typeof(global::InfinityGridLayoutGroup.UpdateChildrenCallbackDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::InfinityGridLayoutGroup)o).updateChildrenCallback = @updateChildrenCallback;
            return ptr_of_this_method;
        }

        static object get_minAmount_1(ref object o)
        {
            return ((global::InfinityGridLayoutGroup)o).minAmount;
        }

        static StackObject* CopyToStack_minAmount_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::InfinityGridLayoutGroup)o).minAmount;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_minAmount_1(ref object o, object v)
        {
            ((global::InfinityGridLayoutGroup)o).minAmount = (System.Int32)v;
        }

        static StackObject* AssignFromStack_minAmount_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @minAmount = ptr_of_this_method->Value;
            ((global::InfinityGridLayoutGroup)o).minAmount = @minAmount;
            return ptr_of_this_method;
        }



    }
}
