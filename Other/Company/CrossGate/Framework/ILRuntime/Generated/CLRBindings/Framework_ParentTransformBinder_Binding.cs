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
    unsafe class Framework_ParentTransformBinder_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.ParentTransformBinder);

            field = type.GetField("parent", flag);
            app.RegisterCLRFieldGetter(field, get_parent_0);
            app.RegisterCLRFieldSetter(field, set_parent_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_parent_0, AssignFromStack_parent_0);
            field = type.GetField("scaleX", flag);
            app.RegisterCLRFieldGetter(field, get_scaleX_1);
            app.RegisterCLRFieldSetter(field, set_scaleX_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_scaleX_1, AssignFromStack_scaleX_1);
            field = type.GetField("scaleY", flag);
            app.RegisterCLRFieldGetter(field, get_scaleY_2);
            app.RegisterCLRFieldSetter(field, set_scaleY_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_scaleY_2, AssignFromStack_scaleY_2);
            field = type.GetField("scaleZ", flag);
            app.RegisterCLRFieldGetter(field, get_scaleZ_3);
            app.RegisterCLRFieldSetter(field, set_scaleZ_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_scaleZ_3, AssignFromStack_scaleZ_3);


        }



        static object get_parent_0(ref object o)
        {
            return ((Framework.ParentTransformBinder)o).parent;
        }

        static StackObject* CopyToStack_parent_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.ParentTransformBinder)o).parent;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_parent_0(ref object o, object v)
        {
            ((Framework.ParentTransformBinder)o).parent = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_parent_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @parent = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((Framework.ParentTransformBinder)o).parent = @parent;
            return ptr_of_this_method;
        }

        static object get_scaleX_1(ref object o)
        {
            return ((Framework.ParentTransformBinder)o).scaleX;
        }

        static StackObject* CopyToStack_scaleX_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.ParentTransformBinder)o).scaleX;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_scaleX_1(ref object o, object v)
        {
            ((Framework.ParentTransformBinder)o).scaleX = (System.Single)v;
        }

        static StackObject* AssignFromStack_scaleX_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @scaleX = *(float*)&ptr_of_this_method->Value;
            ((Framework.ParentTransformBinder)o).scaleX = @scaleX;
            return ptr_of_this_method;
        }

        static object get_scaleY_2(ref object o)
        {
            return ((Framework.ParentTransformBinder)o).scaleY;
        }

        static StackObject* CopyToStack_scaleY_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.ParentTransformBinder)o).scaleY;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_scaleY_2(ref object o, object v)
        {
            ((Framework.ParentTransformBinder)o).scaleY = (System.Single)v;
        }

        static StackObject* AssignFromStack_scaleY_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @scaleY = *(float*)&ptr_of_this_method->Value;
            ((Framework.ParentTransformBinder)o).scaleY = @scaleY;
            return ptr_of_this_method;
        }

        static object get_scaleZ_3(ref object o)
        {
            return ((Framework.ParentTransformBinder)o).scaleZ;
        }

        static StackObject* CopyToStack_scaleZ_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.ParentTransformBinder)o).scaleZ;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_scaleZ_3(ref object o, object v)
        {
            ((Framework.ParentTransformBinder)o).scaleZ = (System.Single)v;
        }

        static StackObject* AssignFromStack_scaleZ_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @scaleZ = *(float*)&ptr_of_this_method->Value;
            ((Framework.ParentTransformBinder)o).scaleZ = @scaleZ;
            return ptr_of_this_method;
        }



    }
}
