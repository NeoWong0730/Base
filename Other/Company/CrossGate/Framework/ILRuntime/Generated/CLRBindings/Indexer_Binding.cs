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
    unsafe class Indexer_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::Indexer);

            field = type.GetField("id", flag);
            app.RegisterCLRFieldGetter(field, get_id_0);
            app.RegisterCLRFieldSetter(field, set_id_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_id_0, AssignFromStack_id_0);
            field = type.GetField("cp", flag);
            app.RegisterCLRFieldGetter(field, get_cp_1);
            app.RegisterCLRFieldSetter(field, set_cp_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_cp_1, AssignFromStack_cp_1);


        }



        static object get_id_0(ref object o)
        {
            return ((global::Indexer)o).id;
        }

        static StackObject* CopyToStack_id_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::Indexer)o).id;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static void set_id_0(ref object o, object v)
        {
            ((global::Indexer)o).id = (System.UInt32)v;
        }

        static StackObject* AssignFromStack_id_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.UInt32 @id = (uint)ptr_of_this_method->Value;
            ((global::Indexer)o).id = @id;
            return ptr_of_this_method;
        }

        static object get_cp_1(ref object o)
        {
            return ((global::Indexer)o).cp;
        }

        static StackObject* CopyToStack_cp_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::Indexer)o).cp;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_cp_1(ref object o, object v)
        {
            ((global::Indexer)o).cp = (global::CP_ScrolCircleListItem)v;
        }

        static StackObject* AssignFromStack_cp_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::CP_ScrolCircleListItem @cp = (global::CP_ScrolCircleListItem)typeof(global::CP_ScrolCircleListItem).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::Indexer)o).cp = @cp;
            return ptr_of_this_method;
        }



    }
}
