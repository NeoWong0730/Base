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
    unsafe class Framework_SurfaceLanguage_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.SurfaceLanguage);

            field = type.GetField("key", flag);
            app.RegisterCLRFieldGetter(field, get_key_0);
            app.RegisterCLRFieldSetter(field, set_key_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_key_0, AssignFromStack_key_0);


        }



        static object get_key_0(ref object o)
        {
            return ((Framework.SurfaceLanguage)o).key;
        }

        static StackObject* CopyToStack_key_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.SurfaceLanguage)o).key;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static void set_key_0(ref object o, object v)
        {
            ((Framework.SurfaceLanguage)o).key = (System.UInt32)v;
        }

        static StackObject* AssignFromStack_key_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.UInt32 @key = (uint)ptr_of_this_method->Value;
            ((Framework.SurfaceLanguage)o).key = @key;
            return ptr_of_this_method;
        }



    }
}
