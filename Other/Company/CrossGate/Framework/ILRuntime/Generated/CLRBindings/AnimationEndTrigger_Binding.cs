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
    unsafe class AnimationEndTrigger_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::AnimationEndTrigger);

            field = type.GetField("onAnimationEnd", flag);
            app.RegisterCLRFieldGetter(field, get_onAnimationEnd_0);
            app.RegisterCLRFieldSetter(field, set_onAnimationEnd_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onAnimationEnd_0, AssignFromStack_onAnimationEnd_0);


        }



        static object get_onAnimationEnd_0(ref object o)
        {
            return ((global::AnimationEndTrigger)o).onAnimationEnd;
        }

        static StackObject* CopyToStack_onAnimationEnd_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::AnimationEndTrigger)o).onAnimationEnd;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onAnimationEnd_0(ref object o, object v)
        {
            ((global::AnimationEndTrigger)o).onAnimationEnd = (System.Action<System.String>)v;
        }

        static StackObject* AssignFromStack_onAnimationEnd_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.String> @onAnimationEnd = (System.Action<System.String>)typeof(System.Action<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::AnimationEndTrigger)o).onAnimationEnd = @onAnimationEnd;
            return ptr_of_this_method;
        }



    }
}
