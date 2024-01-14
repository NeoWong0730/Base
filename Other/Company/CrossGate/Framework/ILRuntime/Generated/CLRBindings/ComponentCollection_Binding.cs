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
    unsafe class ComponentCollection_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ComponentCollection);

            field = type.GetField("components", flag);
            app.RegisterCLRFieldGetter(field, get_components_0);
            app.RegisterCLRFieldSetter(field, set_components_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_components_0, AssignFromStack_components_0);


        }



        static object get_components_0(ref object o)
        {
            return ((global::ComponentCollection)o).components;
        }

        static StackObject* CopyToStack_components_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::ComponentCollection)o).components;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_components_0(ref object o, object v)
        {
            ((global::ComponentCollection)o).components = (System.Collections.Generic.List<UnityEngine.Component>)v;
        }

        static StackObject* AssignFromStack_components_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<UnityEngine.Component> @components = (System.Collections.Generic.List<UnityEngine.Component>)typeof(System.Collections.Generic.List<UnityEngine.Component>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::ComponentCollection)o).components = @components;
            return ptr_of_this_method;
        }



    }
}
