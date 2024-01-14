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
    unsafe class Framework_Table_FCSVFirstCharge_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVFirstCharge.Data);

            field = type.GetField("Reward_Items_d1", flag);
            app.RegisterCLRFieldGetter(field, get_Reward_Items_d1_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Reward_Items_d1_0, null);
            field = type.GetField("Reward_Items_d2", flag);
            app.RegisterCLRFieldGetter(field, get_Reward_Items_d2_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_Reward_Items_d2_1, null);
            field = type.GetField("Reward_Items_d3", flag);
            app.RegisterCLRFieldGetter(field, get_Reward_Items_d3_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_Reward_Items_d3_2, null);
            field = type.GetField("Item_Des_d1", flag);
            app.RegisterCLRFieldGetter(field, get_Item_Des_d1_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_Item_Des_d1_3, null);
            field = type.GetField("Item_Des_d2", flag);
            app.RegisterCLRFieldGetter(field, get_Item_Des_d2_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_Item_Des_d2_4, null);
            field = type.GetField("Item_Des_d3", flag);
            app.RegisterCLRFieldGetter(field, get_Item_Des_d3_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_Item_Des_d3_5, null);
            field = type.GetField("Show_Item", flag);
            app.RegisterCLRFieldGetter(field, get_Show_Item_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_Show_Item_6, null);
            field = type.GetField("Show_height", flag);
            app.RegisterCLRFieldGetter(field, get_Show_height_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_Show_height_7, null);
            field = type.GetField("spin_coordinate", flag);
            app.RegisterCLRFieldGetter(field, get_spin_coordinate_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_spin_coordinate_8, null);
            field = type.GetField("Item_Size", flag);
            app.RegisterCLRFieldGetter(field, get_Item_Size_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_Item_Size_9, null);
            field = type.GetField("Show_Id", flag);
            app.RegisterCLRFieldGetter(field, get_Show_Id_10);
            app.RegisterCLRFieldBinding(field, CopyToStack_Show_Id_10, null);


        }



        static object get_Reward_Items_d1_0(ref object o)
        {
            return ((Framework.Table.FCSVFirstCharge.Data)o).Reward_Items_d1;
        }

        static StackObject* CopyToStack_Reward_Items_d1_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFirstCharge.Data)o).Reward_Items_d1;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Reward_Items_d2_1(ref object o)
        {
            return ((Framework.Table.FCSVFirstCharge.Data)o).Reward_Items_d2;
        }

        static StackObject* CopyToStack_Reward_Items_d2_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFirstCharge.Data)o).Reward_Items_d2;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Reward_Items_d3_2(ref object o)
        {
            return ((Framework.Table.FCSVFirstCharge.Data)o).Reward_Items_d3;
        }

        static StackObject* CopyToStack_Reward_Items_d3_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFirstCharge.Data)o).Reward_Items_d3;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Item_Des_d1_3(ref object o)
        {
            return ((Framework.Table.FCSVFirstCharge.Data)o).Item_Des_d1;
        }

        static StackObject* CopyToStack_Item_Des_d1_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFirstCharge.Data)o).Item_Des_d1;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Item_Des_d2_4(ref object o)
        {
            return ((Framework.Table.FCSVFirstCharge.Data)o).Item_Des_d2;
        }

        static StackObject* CopyToStack_Item_Des_d2_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFirstCharge.Data)o).Item_Des_d2;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Item_Des_d3_5(ref object o)
        {
            return ((Framework.Table.FCSVFirstCharge.Data)o).Item_Des_d3;
        }

        static StackObject* CopyToStack_Item_Des_d3_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFirstCharge.Data)o).Item_Des_d3;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Show_Item_6(ref object o)
        {
            return ((Framework.Table.FCSVFirstCharge.Data)o).Show_Item;
        }

        static StackObject* CopyToStack_Show_Item_6(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFirstCharge.Data)o).Show_Item;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Show_height_7(ref object o)
        {
            return ((Framework.Table.FCSVFirstCharge.Data)o).Show_height;
        }

        static StackObject* CopyToStack_Show_height_7(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFirstCharge.Data)o).Show_height;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_spin_coordinate_8(ref object o)
        {
            return ((Framework.Table.FCSVFirstCharge.Data)o).spin_coordinate;
        }

        static StackObject* CopyToStack_spin_coordinate_8(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFirstCharge.Data)o).spin_coordinate;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Item_Size_9(ref object o)
        {
            return ((Framework.Table.FCSVFirstCharge.Data)o).Item_Size;
        }

        static StackObject* CopyToStack_Item_Size_9(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFirstCharge.Data)o).Item_Size;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Show_Id_10(ref object o)
        {
            return ((Framework.Table.FCSVFirstCharge.Data)o).Show_Id;
        }

        static StackObject* CopyToStack_Show_Id_10(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFirstCharge.Data)o).Show_Id;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
