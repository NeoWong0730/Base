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
    unsafe class Framework_Table_FCSVLevelName_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVLevelName.Data);

            field = type.GetField("prompt_title", flag);
            app.RegisterCLRFieldGetter(field, get_prompt_title_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_prompt_title_0, null);
            field = type.GetField("prompt_info", flag);
            app.RegisterCLRFieldGetter(field, get_prompt_info_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_prompt_info_1, null);
            field = type.GetField("prompt_model", flag);
            app.RegisterCLRFieldGetter(field, get_prompt_model_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_prompt_model_2, null);
            field = type.GetField("rotation_y", flag);
            app.RegisterCLRFieldGetter(field, get_rotation_y_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_rotation_y_3, null);
            field = type.GetField("model_zoom", flag);
            app.RegisterCLRFieldGetter(field, get_model_zoom_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_model_zoom_4, null);


        }



        static object get_prompt_title_0(ref object o)
        {
            return ((Framework.Table.FCSVLevelName.Data)o).prompt_title;
        }

        static StackObject* CopyToStack_prompt_title_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVLevelName.Data)o).prompt_title;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_prompt_info_1(ref object o)
        {
            return ((Framework.Table.FCSVLevelName.Data)o).prompt_info;
        }

        static StackObject* CopyToStack_prompt_info_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVLevelName.Data)o).prompt_info;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_prompt_model_2(ref object o)
        {
            return ((Framework.Table.FCSVLevelName.Data)o).prompt_model;
        }

        static StackObject* CopyToStack_prompt_model_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVLevelName.Data)o).prompt_model;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_rotation_y_3(ref object o)
        {
            return ((Framework.Table.FCSVLevelName.Data)o).rotation_y;
        }

        static StackObject* CopyToStack_rotation_y_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVLevelName.Data)o).rotation_y;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_model_zoom_4(ref object o)
        {
            return ((Framework.Table.FCSVLevelName.Data)o).model_zoom;
        }

        static StackObject* CopyToStack_model_zoom_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVLevelName.Data)o).model_zoom;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
