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
    unsafe class Framework_Table_FCSVBoxType_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVBoxType.Data);

            field = type.GetField("tab_name", flag);
            app.RegisterCLRFieldGetter(field, get_tab_name_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_tab_name_0, null);
            field = type.GetField("uarray2_value", flag);
            app.RegisterCLRFieldGetter(field, get_uarray2_value_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_uarray2_value_1, null);


        }



        static object get_tab_name_0(ref object o)
        {
            return ((Framework.Table.FCSVBoxType.Data)o).tab_name;
        }

        static StackObject* CopyToStack_tab_name_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVBoxType.Data)o).tab_name;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_uarray2_value_1(ref object o)
        {
            return ((Framework.Table.FCSVBoxType.Data)o).uarray2_value;
        }

        static StackObject* CopyToStack_uarray2_value_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVBoxType.Data)o).uarray2_value;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}