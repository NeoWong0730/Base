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
    unsafe class DG_Tweening_Tween_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(DG.Tweening.Tween);

            field = type.GetField("onComplete", flag);
            app.RegisterCLRFieldGetter(field, get_onComplete_0);
            app.RegisterCLRFieldSetter(field, set_onComplete_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onComplete_0, AssignFromStack_onComplete_0);


        }



        static object get_onComplete_0(ref object o)
        {
            return ((DG.Tweening.Tween)o).onComplete;
        }

        static StackObject* CopyToStack_onComplete_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((DG.Tweening.Tween)o).onComplete;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onComplete_0(ref object o, object v)
        {
            ((DG.Tweening.Tween)o).onComplete = (DG.Tweening.TweenCallback)v;
        }

        static StackObject* AssignFromStack_onComplete_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            DG.Tweening.TweenCallback @onComplete = (DG.Tweening.TweenCallback)typeof(DG.Tweening.TweenCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((DG.Tweening.Tween)o).onComplete = @onComplete;
            return ptr_of_this_method;
        }



    }
}
