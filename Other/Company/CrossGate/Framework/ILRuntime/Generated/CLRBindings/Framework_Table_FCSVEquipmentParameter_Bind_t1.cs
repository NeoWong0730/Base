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
    unsafe class Framework_Table_FCSVEquipmentParameter_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVEquipmentParameter.Data);

            field = type.GetField("quality_weight", flag);
            app.RegisterCLRFieldGetter(field, get_quality_weight_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_quality_weight_0, null);
            field = type.GetField("green", flag);
            app.RegisterCLRFieldGetter(field, get_green_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_green_1, null);
            field = type.GetField("effect", flag);
            app.RegisterCLRFieldGetter(field, get_effect_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_effect_2, null);


        }



        static object get_quality_weight_0(ref object o)
        {
            return ((Framework.Table.FCSVEquipmentParameter.Data)o).quality_weight;
        }

        static StackObject* CopyToStack_quality_weight_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVEquipmentParameter.Data)o).quality_weight;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_green_1(ref object o)
        {
            return ((Framework.Table.FCSVEquipmentParameter.Data)o).green;
        }

        static StackObject* CopyToStack_green_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVEquipmentParameter.Data)o).green;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_effect_2(ref object o)
        {
            return ((Framework.Table.FCSVEquipmentParameter.Data)o).effect;
        }

        static StackObject* CopyToStack_effect_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVEquipmentParameter.Data)o).effect;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
