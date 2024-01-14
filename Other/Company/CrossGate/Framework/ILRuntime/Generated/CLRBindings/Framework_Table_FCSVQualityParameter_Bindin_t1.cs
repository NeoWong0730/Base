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
    unsafe class Framework_Table_FCSVQualityParameter_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVQualityParameter.Data);

            field = type.GetField("green_weight", flag);
            app.RegisterCLRFieldGetter(field, get_green_weight_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_green_weight_0, null);
            field = type.GetField("special_weight", flag);
            app.RegisterCLRFieldGetter(field, get_special_weight_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_special_weight_1, null);
            field = type.GetField("base_cor", flag);
            app.RegisterCLRFieldGetter(field, get_base_cor_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_base_cor_2, null);
            field = type.GetField("green_range", flag);
            app.RegisterCLRFieldGetter(field, get_green_range_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_green_range_3, null);


        }



        static object get_green_weight_0(ref object o)
        {
            return ((Framework.Table.FCSVQualityParameter.Data)o).green_weight;
        }

        static StackObject* CopyToStack_green_weight_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVQualityParameter.Data)o).green_weight;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_special_weight_1(ref object o)
        {
            return ((Framework.Table.FCSVQualityParameter.Data)o).special_weight;
        }

        static StackObject* CopyToStack_special_weight_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVQualityParameter.Data)o).special_weight;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_base_cor_2(ref object o)
        {
            return ((Framework.Table.FCSVQualityParameter.Data)o).base_cor;
        }

        static StackObject* CopyToStack_base_cor_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVQualityParameter.Data)o).base_cor;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_green_range_3(ref object o)
        {
            return ((Framework.Table.FCSVQualityParameter.Data)o).green_range;
        }

        static StackObject* CopyToStack_green_range_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVQualityParameter.Data)o).green_range;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
