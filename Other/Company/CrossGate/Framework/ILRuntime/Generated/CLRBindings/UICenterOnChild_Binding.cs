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
    unsafe class UICenterOnChild_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UICenterOnChild);
            args = new Type[]{};
            method = type.GetMethod("InitPageArray", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InitPageArray_0);
            args = new Type[]{typeof(System.Int32), typeof(System.Boolean)};
            method = type.GetMethod("SetCurrentPageIndex", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetCurrentPageIndex_1);

            field = type.GetField("onCenter", flag);
            app.RegisterCLRFieldGetter(field, get_onCenter_0);
            app.RegisterCLRFieldSetter(field, set_onCenter_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onCenter_0, AssignFromStack_onCenter_0);


        }


        static StackObject* InitPageArray_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UICenterOnChild instance_of_this_method = (global::UICenterOnChild)typeof(global::UICenterOnChild).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.InitPageArray();

            return __ret;
        }

        static StackObject* SetCurrentPageIndex_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @needTween = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UICenterOnChild instance_of_this_method = (global::UICenterOnChild)typeof(global::UICenterOnChild).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetCurrentPageIndex(@index, @needTween);

            return __ret;
        }


        static object get_onCenter_0(ref object o)
        {
            return ((global::UICenterOnChild)o).onCenter;
        }

        static StackObject* CopyToStack_onCenter_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UICenterOnChild)o).onCenter;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onCenter_0(ref object o, object v)
        {
            ((global::UICenterOnChild)o).onCenter = (global::UICenterOnChild.OnCenterHandler)v;
        }

        static StackObject* AssignFromStack_onCenter_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UICenterOnChild.OnCenterHandler @onCenter = (global::UICenterOnChild.OnCenterHandler)typeof(global::UICenterOnChild.OnCenterHandler).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::UICenterOnChild)o).onCenter = @onCenter;
            return ptr_of_this_method;
        }



    }
}
