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
    unsafe class UnityEngine_UI_CP_ScrollRect_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.CP_ScrollRect);

            field = type.GetField("onBeginDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onBeginDrag_0);
            app.RegisterCLRFieldSetter(field, set_onBeginDrag_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onBeginDrag_0, AssignFromStack_onBeginDrag_0);
            field = type.GetField("onEndDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onEndDrag_1);
            app.RegisterCLRFieldSetter(field, set_onEndDrag_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onEndDrag_1, AssignFromStack_onEndDrag_1);


        }



        static object get_onBeginDrag_0(ref object o)
        {
            return ((UnityEngine.UI.CP_ScrollRect)o).onBeginDrag;
        }

        static StackObject* CopyToStack_onBeginDrag_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.UI.CP_ScrollRect)o).onBeginDrag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onBeginDrag_0(ref object o, object v)
        {
            ((UnityEngine.UI.CP_ScrollRect)o).onBeginDrag = (System.Action<UnityEngine.EventSystems.PointerEventData>)v;
        }

        static StackObject* AssignFromStack_onBeginDrag_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.EventSystems.PointerEventData> @onBeginDrag = (System.Action<UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((UnityEngine.UI.CP_ScrollRect)o).onBeginDrag = @onBeginDrag;
            return ptr_of_this_method;
        }

        static object get_onEndDrag_1(ref object o)
        {
            return ((UnityEngine.UI.CP_ScrollRect)o).onEndDrag;
        }

        static StackObject* CopyToStack_onEndDrag_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.UI.CP_ScrollRect)o).onEndDrag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onEndDrag_1(ref object o, object v)
        {
            ((UnityEngine.UI.CP_ScrollRect)o).onEndDrag = (System.Action<UnityEngine.EventSystems.PointerEventData>)v;
        }

        static StackObject* AssignFromStack_onEndDrag_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.EventSystems.PointerEventData> @onEndDrag = (System.Action<UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((UnityEngine.UI.CP_ScrollRect)o).onEndDrag = @onEndDrag;
            return ptr_of_this_method;
        }



    }
}
