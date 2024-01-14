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
    unsafe class CP_ScrolCircleListItem_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CP_ScrolCircleListItem);
            args = new Type[]{};
            method = type.GetMethod("get_inCircle", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_inCircle_0);

            field = type.GetField("binder", flag);
            app.RegisterCLRFieldGetter(field, get_binder_0);
            app.RegisterCLRFieldSetter(field, set_binder_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_binder_0, AssignFromStack_binder_0);
            field = type.GetField("id", flag);
            app.RegisterCLRFieldGetter(field, get_id_1);
            app.RegisterCLRFieldSetter(field, set_id_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_id_1, AssignFromStack_id_1);


        }


        static StackObject* get_inCircle_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_ScrolCircleListItem instance_of_this_method = (global::CP_ScrolCircleListItem)typeof(global::CP_ScrolCircleListItem).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.inCircle;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }


        static object get_binder_0(ref object o)
        {
            return ((global::CP_ScrolCircleListItem)o).binder;
        }

        static StackObject* CopyToStack_binder_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_ScrolCircleListItem)o).binder;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_binder_0(ref object o, object v)
        {
            ((global::CP_ScrolCircleListItem)o).binder = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_binder_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @binder = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_ScrolCircleListItem)o).binder = @binder;
            return ptr_of_this_method;
        }

        static object get_id_1(ref object o)
        {
            return ((global::CP_ScrolCircleListItem)o).id;
        }

        static StackObject* CopyToStack_id_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_ScrolCircleListItem)o).id;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static void set_id_1(ref object o, object v)
        {
            ((global::CP_ScrolCircleListItem)o).id = (System.UInt32)v;
        }

        static StackObject* AssignFromStack_id_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.UInt32 @id = (uint)ptr_of_this_method->Value;
            ((global::CP_ScrolCircleListItem)o).id = @id;
            return ptr_of_this_method;
        }



    }
}
