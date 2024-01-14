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
    unsafe class Framework_Table_FCSVGuideEffect_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVGuideEffect.Data);
            args = new Type[]{};
            method = type.GetMethod("get_type", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_type_0);

            field = type.GetField("effect", flag);
            app.RegisterCLRFieldGetter(field, get_effect_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_effect_0, null);
            field = type.GetField("effect_size", flag);
            app.RegisterCLRFieldGetter(field, get_effect_size_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_effect_size_1, null);
            field = type.GetField("effect_anchors", flag);
            app.RegisterCLRFieldGetter(field, get_effect_anchors_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_effect_anchors_2, null);
            field = type.GetField("effect_pos", flag);
            app.RegisterCLRFieldGetter(field, get_effect_pos_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_effect_pos_3, null);
            field = type.GetField("prefab_path", flag);
            app.RegisterCLRFieldGetter(field, get_prefab_path_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_prefab_path_4, null);
            field = type.GetField("effect_Pivot", flag);
            app.RegisterCLRFieldGetter(field, get_effect_Pivot_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_effect_Pivot_5, null);
            field = type.GetField("effect_rotation", flag);
            app.RegisterCLRFieldGetter(field, get_effect_rotation_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_effect_rotation_6, null);
            field = type.GetField("effect_scale", flag);
            app.RegisterCLRFieldGetter(field, get_effect_scale_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_effect_scale_7, null);


        }


        static StackObject* get_type_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGuideEffect.Data instance_of_this_method = (Framework.Table.FCSVGuideEffect.Data)typeof(Framework.Table.FCSVGuideEffect.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.type;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_effect_0(ref object o)
        {
            return ((Framework.Table.FCSVGuideEffect.Data)o).effect;
        }

        static StackObject* CopyToStack_effect_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideEffect.Data)o).effect;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_effect_size_1(ref object o)
        {
            return ((Framework.Table.FCSVGuideEffect.Data)o).effect_size;
        }

        static StackObject* CopyToStack_effect_size_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideEffect.Data)o).effect_size;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_effect_anchors_2(ref object o)
        {
            return ((Framework.Table.FCSVGuideEffect.Data)o).effect_anchors;
        }

        static StackObject* CopyToStack_effect_anchors_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideEffect.Data)o).effect_anchors;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_effect_pos_3(ref object o)
        {
            return ((Framework.Table.FCSVGuideEffect.Data)o).effect_pos;
        }

        static StackObject* CopyToStack_effect_pos_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideEffect.Data)o).effect_pos;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_prefab_path_4(ref object o)
        {
            return ((Framework.Table.FCSVGuideEffect.Data)o).prefab_path;
        }

        static StackObject* CopyToStack_prefab_path_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideEffect.Data)o).prefab_path;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_effect_Pivot_5(ref object o)
        {
            return ((Framework.Table.FCSVGuideEffect.Data)o).effect_Pivot;
        }

        static StackObject* CopyToStack_effect_Pivot_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideEffect.Data)o).effect_Pivot;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_effect_rotation_6(ref object o)
        {
            return ((Framework.Table.FCSVGuideEffect.Data)o).effect_rotation;
        }

        static StackObject* CopyToStack_effect_rotation_6(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideEffect.Data)o).effect_rotation;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_effect_scale_7(ref object o)
        {
            return ((Framework.Table.FCSVGuideEffect.Data)o).effect_scale;
        }

        static StackObject* CopyToStack_effect_scale_7(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideEffect.Data)o).effect_scale;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
