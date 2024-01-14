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
    unsafe class UnityEngine_UI_Extensions_UIPolygon_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.Extensions.UIPolygon);

            field = type.GetField("VerticesDistances", flag);
            app.RegisterCLRFieldGetter(field, get_VerticesDistances_0);
            app.RegisterCLRFieldSetter(field, set_VerticesDistances_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_VerticesDistances_0, AssignFromStack_VerticesDistances_0);


        }



        static object get_VerticesDistances_0(ref object o)
        {
            return ((UnityEngine.UI.Extensions.UIPolygon)o).VerticesDistances;
        }

        static StackObject* CopyToStack_VerticesDistances_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.UI.Extensions.UIPolygon)o).VerticesDistances;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_VerticesDistances_0(ref object o, object v)
        {
            ((UnityEngine.UI.Extensions.UIPolygon)o).VerticesDistances = (System.Single[])v;
        }

        static StackObject* AssignFromStack_VerticesDistances_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single[] @VerticesDistances = (System.Single[])typeof(System.Single[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((UnityEngine.UI.Extensions.UIPolygon)o).VerticesDistances = @VerticesDistances;
            return ptr_of_this_method;
        }



    }
}