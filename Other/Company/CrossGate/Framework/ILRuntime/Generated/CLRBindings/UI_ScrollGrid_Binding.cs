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
    unsafe class UI_ScrollGrid_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UI_ScrollGrid);
            args = new Type[]{typeof(System.Int32), typeof(System.Int32)};
            method = type.GetMethod("SetValusRange", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetValusRange_0);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("Focus", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Focus_1);

            field = type.GetField("FocusChangeEvent", flag);
            app.RegisterCLRFieldGetter(field, get_FocusChangeEvent_0);
            app.RegisterCLRFieldSetter(field, set_FocusChangeEvent_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_FocusChangeEvent_0, AssignFromStack_FocusChangeEvent_0);


        }


        static StackObject* SetValusRange_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @max = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @min = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UI_ScrollGrid instance_of_this_method = (global::UI_ScrollGrid)typeof(global::UI_ScrollGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetValusRange(@min, @max);

            return __ret;
        }

        static StackObject* Focus_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UI_ScrollGrid instance_of_this_method = (global::UI_ScrollGrid)typeof(global::UI_ScrollGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Focus(@index);

            return __ret;
        }


        static object get_FocusChangeEvent_0(ref object o)
        {
            return ((global::UI_ScrollGrid)o).FocusChangeEvent;
        }

        static StackObject* CopyToStack_FocusChangeEvent_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UI_ScrollGrid)o).FocusChangeEvent;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_FocusChangeEvent_0(ref object o, object v)
        {
            ((global::UI_ScrollGrid)o).FocusChangeEvent = (global::UI_ScrollGrid.ChangeEvent)v;
        }

        static StackObject* AssignFromStack_FocusChangeEvent_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UI_ScrollGrid.ChangeEvent @FocusChangeEvent = (global::UI_ScrollGrid.ChangeEvent)typeof(global::UI_ScrollGrid.ChangeEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::UI_ScrollGrid)o).FocusChangeEvent = @FocusChangeEvent;
            return ptr_of_this_method;
        }



    }
}
