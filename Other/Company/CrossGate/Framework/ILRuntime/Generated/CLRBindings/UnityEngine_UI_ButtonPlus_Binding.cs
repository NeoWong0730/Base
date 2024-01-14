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
    unsafe class UnityEngine_UI_ButtonPlus_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.ButtonPlus);

            field = type.GetField("HighlightedAction", flag);
            app.RegisterCLRFieldGetter(field, get_HighlightedAction_0);
            app.RegisterCLRFieldSetter(field, set_HighlightedAction_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_HighlightedAction_0, AssignFromStack_HighlightedAction_0);
            field = type.GetField("NormalAction", flag);
            app.RegisterCLRFieldGetter(field, get_NormalAction_1);
            app.RegisterCLRFieldSetter(field, set_NormalAction_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_NormalAction_1, AssignFromStack_NormalAction_1);


        }



        static object get_HighlightedAction_0(ref object o)
        {
            return ((UnityEngine.UI.ButtonPlus)o).HighlightedAction;
        }

        static StackObject* CopyToStack_HighlightedAction_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.UI.ButtonPlus)o).HighlightedAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_HighlightedAction_0(ref object o, object v)
        {
            ((UnityEngine.UI.ButtonPlus)o).HighlightedAction = (System.Action)v;
        }

        static StackObject* AssignFromStack_HighlightedAction_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @HighlightedAction = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((UnityEngine.UI.ButtonPlus)o).HighlightedAction = @HighlightedAction;
            return ptr_of_this_method;
        }

        static object get_NormalAction_1(ref object o)
        {
            return ((UnityEngine.UI.ButtonPlus)o).NormalAction;
        }

        static StackObject* CopyToStack_NormalAction_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.UI.ButtonPlus)o).NormalAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_NormalAction_1(ref object o, object v)
        {
            ((UnityEngine.UI.ButtonPlus)o).NormalAction = (System.Action)v;
        }

        static StackObject* AssignFromStack_NormalAction_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @NormalAction = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((UnityEngine.UI.ButtonPlus)o).NormalAction = @NormalAction;
            return ptr_of_this_method;
        }



    }
}
