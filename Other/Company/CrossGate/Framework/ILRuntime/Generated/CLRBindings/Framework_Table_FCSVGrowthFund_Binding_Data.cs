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
    unsafe class Framework_Table_FCSVGrowthFund_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVGrowthFund.Data);
            args = new Type[]{};
            method = type.GetMethod("get_Title", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Title_0);
            args = new Type[]{};
            method = type.GetMethod("get_id", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_id_1);

            field = type.GetField("level", flag);
            app.RegisterCLRFieldGetter(field, get_level_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_level_0, null);
            field = type.GetField("reward_Id", flag);
            app.RegisterCLRFieldGetter(field, get_reward_Id_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_reward_Id_1, null);
            field = type.GetField("Fun_Des", flag);
            app.RegisterCLRFieldGetter(field, get_Fun_Des_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_Fun_Des_2, null);
            field = type.GetField("Show_Icon", flag);
            app.RegisterCLRFieldGetter(field, get_Show_Icon_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_Show_Icon_3, null);


        }


        static StackObject* get_Title_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGrowthFund.Data instance_of_this_method = (Framework.Table.FCSVGrowthFund.Data)typeof(Framework.Table.FCSVGrowthFund.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Title;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_id_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGrowthFund.Data instance_of_this_method = (Framework.Table.FCSVGrowthFund.Data)typeof(Framework.Table.FCSVGrowthFund.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.id;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_level_0(ref object o)
        {
            return ((Framework.Table.FCSVGrowthFund.Data)o).level;
        }

        static StackObject* CopyToStack_level_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGrowthFund.Data)o).level;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_reward_Id_1(ref object o)
        {
            return ((Framework.Table.FCSVGrowthFund.Data)o).reward_Id;
        }

        static StackObject* CopyToStack_reward_Id_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGrowthFund.Data)o).reward_Id;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Fun_Des_2(ref object o)
        {
            return ((Framework.Table.FCSVGrowthFund.Data)o).Fun_Des;
        }

        static StackObject* CopyToStack_Fun_Des_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGrowthFund.Data)o).Fun_Des;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Show_Icon_3(ref object o)
        {
            return ((Framework.Table.FCSVGrowthFund.Data)o).Show_Icon;
        }

        static StackObject* CopyToStack_Show_Icon_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGrowthFund.Data)o).Show_Icon;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
