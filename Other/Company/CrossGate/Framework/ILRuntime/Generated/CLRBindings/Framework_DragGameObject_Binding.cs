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
    unsafe class Framework_DragGameObject_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.DragGameObject);

            field = type.GetField("onDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onDrag_0);
            app.RegisterCLRFieldSetter(field, set_onDrag_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDrag_0, AssignFromStack_onDrag_0);


        }



        static object get_onDrag_0(ref object o)
        {
            return ((Framework.DragGameObject)o).onDrag;
        }

        static StackObject* CopyToStack_onDrag_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.DragGameObject)o).onDrag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDrag_0(ref object o, object v)
        {
            ((Framework.DragGameObject)o).onDrag = (System.Action)v;
        }

        static StackObject* AssignFromStack_onDrag_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @onDrag = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.DragGameObject)o).onDrag = @onDrag;
            return ptr_of_this_method;
        }



    }
}
