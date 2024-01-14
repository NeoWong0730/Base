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
    unsafe class Framework_Table_FCSVImprintUpgrade_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVImprintUpgrade.Data);
            args = new Type[]{};
            method = type.GetMethod("get_score", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_score_0);

            field = type.GetField("Target_Type", flag);
            app.RegisterCLRFieldGetter(field, get_Target_Type_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Target_Type_0, null);
            field = type.GetField("Attribute_Bonus", flag);
            app.RegisterCLRFieldGetter(field, get_Attribute_Bonus_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_Attribute_Bonus_1, null);
            field = type.GetField("Consume_Item", flag);
            app.RegisterCLRFieldGetter(field, get_Consume_Item_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_Consume_Item_2, null);


        }


        static StackObject* get_score_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVImprintUpgrade.Data instance_of_this_method = (Framework.Table.FCSVImprintUpgrade.Data)typeof(Framework.Table.FCSVImprintUpgrade.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.score;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_Target_Type_0(ref object o)
        {
            return ((Framework.Table.FCSVImprintUpgrade.Data)o).Target_Type;
        }

        static StackObject* CopyToStack_Target_Type_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVImprintUpgrade.Data)o).Target_Type;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Attribute_Bonus_1(ref object o)
        {
            return ((Framework.Table.FCSVImprintUpgrade.Data)o).Attribute_Bonus;
        }

        static StackObject* CopyToStack_Attribute_Bonus_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVImprintUpgrade.Data)o).Attribute_Bonus;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Consume_Item_2(ref object o)
        {
            return ((Framework.Table.FCSVImprintUpgrade.Data)o).Consume_Item;
        }

        static StackObject* CopyToStack_Consume_Item_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVImprintUpgrade.Data)o).Consume_Item;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
