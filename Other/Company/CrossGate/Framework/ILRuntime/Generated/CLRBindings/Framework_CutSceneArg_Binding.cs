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
    unsafe class Framework_CutSceneArg_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.CutSceneArg);

            field = type.GetField("group", flag);
            app.RegisterCLRFieldGetter(field, get_group_0);
            app.RegisterCLRFieldSetter(field, set_group_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_group_0, AssignFromStack_group_0);
            field = type.GetField("tag", flag);
            app.RegisterCLRFieldGetter(field, get_tag_1);
            app.RegisterCLRFieldSetter(field, set_tag_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_tag_1, AssignFromStack_tag_1);
            field = type.GetField("transform", flag);
            app.RegisterCLRFieldGetter(field, get_transform_2);
            app.RegisterCLRFieldSetter(field, set_transform_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_transform_2, AssignFromStack_transform_2);
            field = type.GetField("id", flag);
            app.RegisterCLRFieldGetter(field, get_id_3);
            app.RegisterCLRFieldSetter(field, set_id_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_id_3, AssignFromStack_id_3);
            field = type.GetField("offset", flag);
            app.RegisterCLRFieldGetter(field, get_offset_4);
            app.RegisterCLRFieldSetter(field, set_offset_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_offset_4, AssignFromStack_offset_4);
            field = type.GetField("boolValue", flag);
            app.RegisterCLRFieldGetter(field, get_boolValue_5);
            app.RegisterCLRFieldSetter(field, set_boolValue_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_boolValue_5, AssignFromStack_boolValue_5);
            field = type.GetField("value", flag);
            app.RegisterCLRFieldGetter(field, get_value_6);
            app.RegisterCLRFieldSetter(field, set_value_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_value_6, AssignFromStack_value_6);


        }



        static object get_group_0(ref object o)
        {
            return ((Framework.CutSceneArg)o).group;
        }

        static StackObject* CopyToStack_group_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.CutSceneArg)o).group;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_group_0(ref object o, object v)
        {
            ((Framework.CutSceneArg)o).group = (System.String)v;
        }

        static StackObject* AssignFromStack_group_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @group = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((Framework.CutSceneArg)o).group = @group;
            return ptr_of_this_method;
        }

        static object get_tag_1(ref object o)
        {
            return ((Framework.CutSceneArg)o).tag;
        }

        static StackObject* CopyToStack_tag_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.CutSceneArg)o).tag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_tag_1(ref object o, object v)
        {
            ((Framework.CutSceneArg)o).tag = (System.String)v;
        }

        static StackObject* AssignFromStack_tag_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @tag = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((Framework.CutSceneArg)o).tag = @tag;
            return ptr_of_this_method;
        }

        static object get_transform_2(ref object o)
        {
            return ((Framework.CutSceneArg)o).transform;
        }

        static StackObject* CopyToStack_transform_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.CutSceneArg)o).transform;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_transform_2(ref object o, object v)
        {
            ((Framework.CutSceneArg)o).transform = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_transform_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @transform = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((Framework.CutSceneArg)o).transform = @transform;
            return ptr_of_this_method;
        }

        static object get_id_3(ref object o)
        {
            return ((Framework.CutSceneArg)o).id;
        }

        static StackObject* CopyToStack_id_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.CutSceneArg)o).id;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static void set_id_3(ref object o, object v)
        {
            ((Framework.CutSceneArg)o).id = (System.UInt32)v;
        }

        static StackObject* AssignFromStack_id_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.UInt32 @id = (uint)ptr_of_this_method->Value;
            ((Framework.CutSceneArg)o).id = @id;
            return ptr_of_this_method;
        }

        static object get_offset_4(ref object o)
        {
            return ((Framework.CutSceneArg)o).offset;
        }

        static StackObject* CopyToStack_offset_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.CutSceneArg)o).offset;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_offset_4(ref object o, object v)
        {
            ((Framework.CutSceneArg)o).offset = (UnityEngine.Vector3)v;
        }

        static StackObject* AssignFromStack_offset_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector3 @offset = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @offset, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @offset = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
            }
            ((Framework.CutSceneArg)o).offset = @offset;
            return ptr_of_this_method;
        }

        static object get_boolValue_5(ref object o)
        {
            return ((Framework.CutSceneArg)o).boolValue;
        }

        static StackObject* CopyToStack_boolValue_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.CutSceneArg)o).boolValue;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_boolValue_5(ref object o, object v)
        {
            ((Framework.CutSceneArg)o).boolValue = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_boolValue_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @boolValue = ptr_of_this_method->Value == 1;
            ((Framework.CutSceneArg)o).boolValue = @boolValue;
            return ptr_of_this_method;
        }

        static object get_value_6(ref object o)
        {
            return ((Framework.CutSceneArg)o).value;
        }

        static StackObject* CopyToStack_value_6(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.CutSceneArg)o).value;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_value_6(ref object o, object v)
        {
            ((Framework.CutSceneArg)o).value = (System.Single)v;
        }

        static StackObject* AssignFromStack_value_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @value = *(float*)&ptr_of_this_method->Value;
            ((Framework.CutSceneArg)o).value = @value;
            return ptr_of_this_method;
        }



    }
}
