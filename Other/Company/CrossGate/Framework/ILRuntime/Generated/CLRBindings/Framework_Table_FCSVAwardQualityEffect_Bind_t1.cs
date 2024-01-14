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
    unsafe class Framework_Table_FCSVAwardQualityEffect_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVAwardQualityEffect.Data);

            field = type.GetField("effects4_path", flag);
            app.RegisterCLRFieldGetter(field, get_effects4_path_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_effects4_path_0, null);
            field = type.GetField("effects5_path", flag);
            app.RegisterCLRFieldGetter(field, get_effects5_path_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_effects5_path_1, null);


        }



        static object get_effects4_path_0(ref object o)
        {
            return ((Framework.Table.FCSVAwardQualityEffect.Data)o).effects4_path;
        }

        static StackObject* CopyToStack_effects4_path_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAwardQualityEffect.Data)o).effects4_path;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_effects5_path_1(ref object o)
        {
            return ((Framework.Table.FCSVAwardQualityEffect.Data)o).effects5_path;
        }

        static StackObject* CopyToStack_effects5_path_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAwardQualityEffect.Data)o).effects5_path;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
