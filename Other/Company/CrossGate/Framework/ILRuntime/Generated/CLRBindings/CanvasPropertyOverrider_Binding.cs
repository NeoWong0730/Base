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
    unsafe class CanvasPropertyOverrider_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CanvasPropertyOverrider);
            args = new Type[]{};
            method = type.GetMethod("Clear", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Clear_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("HandleRight", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HandleRight_1);
            args = new Type[]{typeof(System.String), typeof(System.Single), typeof(System.Single), typeof(System.Single), typeof(System.Single)};
            method = type.GetMethod("AddSpecialAreas", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddSpecialAreas_2);

            field = type.GetField("isSafeCanvas", flag);
            app.RegisterCLRFieldGetter(field, get_isSafeCanvas_0);
            app.RegisterCLRFieldSetter(field, set_isSafeCanvas_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_isSafeCanvas_0, AssignFromStack_isSafeCanvas_0);


        }


        static StackObject* Clear_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::CanvasPropertyOverrider.Clear();

            return __ret;
        }

        static StackObject* HandleRight_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @target = ptr_of_this_method->Value == 1;


            global::CanvasPropertyOverrider.HandleRight(@target);

            return __ret;
        }

        static StackObject* AddSpecialAreas_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @height = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @width = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @y = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Single @x = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.String @name = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            global::CanvasPropertyOverrider.AddSpecialAreas(@name, @x, @y, @width, @height);

            return __ret;
        }


        static object get_isSafeCanvas_0(ref object o)
        {
            return ((global::CanvasPropertyOverrider)o).isSafeCanvas;
        }

        static StackObject* CopyToStack_isSafeCanvas_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CanvasPropertyOverrider)o).isSafeCanvas;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_isSafeCanvas_0(ref object o, object v)
        {
            ((global::CanvasPropertyOverrider)o).isSafeCanvas = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_isSafeCanvas_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @isSafeCanvas = ptr_of_this_method->Value == 1;
            ((global::CanvasPropertyOverrider)o).isSafeCanvas = @isSafeCanvas;
            return ptr_of_this_method;
        }



    }
}
