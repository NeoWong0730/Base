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
    unsafe class Framework_Table_FCSVAttrConvert_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVAttrConvert.Data);

            field = type.GetField("vtl_convert", flag);
            app.RegisterCLRFieldGetter(field, get_vtl_convert_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_vtl_convert_0, null);
            field = type.GetField("str_convert", flag);
            app.RegisterCLRFieldGetter(field, get_str_convert_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_str_convert_1, null);
            field = type.GetField("tgh_convert", flag);
            app.RegisterCLRFieldGetter(field, get_tgh_convert_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_tgh_convert_2, null);
            field = type.GetField("qui_convert", flag);
            app.RegisterCLRFieldGetter(field, get_qui_convert_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_qui_convert_3, null);
            field = type.GetField("mgc_convert", flag);
            app.RegisterCLRFieldGetter(field, get_mgc_convert_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_mgc_convert_4, null);


        }



        static object get_vtl_convert_0(ref object o)
        {
            return ((Framework.Table.FCSVAttrConvert.Data)o).vtl_convert;
        }

        static StackObject* CopyToStack_vtl_convert_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAttrConvert.Data)o).vtl_convert;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_str_convert_1(ref object o)
        {
            return ((Framework.Table.FCSVAttrConvert.Data)o).str_convert;
        }

        static StackObject* CopyToStack_str_convert_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAttrConvert.Data)o).str_convert;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_tgh_convert_2(ref object o)
        {
            return ((Framework.Table.FCSVAttrConvert.Data)o).tgh_convert;
        }

        static StackObject* CopyToStack_tgh_convert_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAttrConvert.Data)o).tgh_convert;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_qui_convert_3(ref object o)
        {
            return ((Framework.Table.FCSVAttrConvert.Data)o).qui_convert;
        }

        static StackObject* CopyToStack_qui_convert_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAttrConvert.Data)o).qui_convert;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_mgc_convert_4(ref object o)
        {
            return ((Framework.Table.FCSVAttrConvert.Data)o).mgc_convert;
        }

        static StackObject* CopyToStack_mgc_convert_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAttrConvert.Data)o).mgc_convert;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
