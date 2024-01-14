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
    unsafe class UI_ScrollGrid_Element_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UI_ScrollGrid_Element);

            field = type.GetField("ValueChange", flag);
            app.RegisterCLRFieldGetter(field, get_ValueChange_0);
            app.RegisterCLRFieldSetter(field, set_ValueChange_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_ValueChange_0, AssignFromStack_ValueChange_0);


        }



        static object get_ValueChange_0(ref object o)
        {
            return ((global::UI_ScrollGrid_Element)o).ValueChange;
        }

        static StackObject* CopyToStack_ValueChange_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::UI_ScrollGrid_Element)o).ValueChange;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ValueChange_0(ref object o, object v)
        {
            ((global::UI_ScrollGrid_Element)o).ValueChange = (global::UI_ScrollGrid_Element.ValueChangeEvent)v;
        }

        static StackObject* AssignFromStack_ValueChange_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UI_ScrollGrid_Element.ValueChangeEvent @ValueChange = (global::UI_ScrollGrid_Element.ValueChangeEvent)typeof(global::UI_ScrollGrid_Element.ValueChangeEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::UI_ScrollGrid_Element)o).ValueChange = @ValueChange;
            return ptr_of_this_method;
        }



    }
}
