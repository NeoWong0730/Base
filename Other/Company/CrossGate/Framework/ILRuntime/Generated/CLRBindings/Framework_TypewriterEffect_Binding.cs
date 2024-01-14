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
    unsafe class Framework_TypewriterEffect_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.TypewriterEffect);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("WordByWord", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WordByWord_0);
            args = new Type[]{};
            method = type.GetMethod("QuickShow", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, QuickShow_1);

            field = type.GetField("charsPerSecond", flag);
            app.RegisterCLRFieldGetter(field, get_charsPerSecond_0);
            app.RegisterCLRFieldSetter(field, set_charsPerSecond_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_charsPerSecond_0, AssignFromStack_charsPerSecond_0);
            field = type.GetField("onFinished", flag);
            app.RegisterCLRFieldGetter(field, get_onFinished_1);
            app.RegisterCLRFieldSetter(field, set_onFinished_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onFinished_1, AssignFromStack_onFinished_1);


        }


        static StackObject* WordByWord_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @content = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.TypewriterEffect instance_of_this_method = (Framework.TypewriterEffect)typeof(Framework.TypewriterEffect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WordByWord(@content);

            return __ret;
        }

        static StackObject* QuickShow_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.TypewriterEffect instance_of_this_method = (Framework.TypewriterEffect)typeof(Framework.TypewriterEffect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.QuickShow();

            return __ret;
        }


        static object get_charsPerSecond_0(ref object o)
        {
            return ((Framework.TypewriterEffect)o).charsPerSecond;
        }

        static StackObject* CopyToStack_charsPerSecond_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.TypewriterEffect)o).charsPerSecond;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_charsPerSecond_0(ref object o, object v)
        {
            ((Framework.TypewriterEffect)o).charsPerSecond = (System.Int32)v;
        }

        static StackObject* AssignFromStack_charsPerSecond_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @charsPerSecond = ptr_of_this_method->Value;
            ((Framework.TypewriterEffect)o).charsPerSecond = @charsPerSecond;
            return ptr_of_this_method;
        }

        static object get_onFinished_1(ref object o)
        {
            return ((Framework.TypewriterEffect)o).onFinished;
        }

        static StackObject* CopyToStack_onFinished_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.TypewriterEffect)o).onFinished;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onFinished_1(ref object o, object v)
        {
            ((Framework.TypewriterEffect)o).onFinished = (UnityEngine.Events.UnityEvent)v;
        }

        static StackObject* AssignFromStack_onFinished_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Events.UnityEvent @onFinished = (UnityEngine.Events.UnityEvent)typeof(UnityEngine.Events.UnityEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((Framework.TypewriterEffect)o).onFinished = @onFinished;
            return ptr_of_this_method;
        }



    }
}
