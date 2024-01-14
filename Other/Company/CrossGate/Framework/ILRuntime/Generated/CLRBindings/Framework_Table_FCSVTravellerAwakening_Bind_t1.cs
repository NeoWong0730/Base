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
    unsafe class Framework_Table_FCSVTravellerAwakening_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVTravellerAwakening.Data);
            args = new Type[]{};
            method = type.GetMethod("get_id", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_id_0);
            args = new Type[]{};
            method = type.GetMethod("get_BuffID", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_BuffID_1);
            args = new Type[]{};
            method = type.GetMethod("get_NameId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_NameId_2);
            args = new Type[]{};
            method = type.GetMethod("get_StepsId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_StepsId_3);

            field = type.GetField("ActProject", flag);
            app.RegisterCLRFieldGetter(field, get_ActProject_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_ActProject_0, null);
            field = type.GetField("ActCondition", flag);
            app.RegisterCLRFieldGetter(field, get_ActCondition_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_ActCondition_1, null);
            field = type.GetField("show_attr_name", flag);
            app.RegisterCLRFieldGetter(field, get_show_attr_name_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_show_attr_name_2, null);
            field = type.GetField("show_attr_value", flag);
            app.RegisterCLRFieldGetter(field, get_show_attr_value_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_show_attr_value_3, null);
            field = type.GetField("Award", flag);
            app.RegisterCLRFieldGetter(field, get_Award_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_Award_4, null);


        }


        static StackObject* get_id_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTravellerAwakening.Data instance_of_this_method = (Framework.Table.FCSVTravellerAwakening.Data)typeof(Framework.Table.FCSVTravellerAwakening.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.id;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_BuffID_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTravellerAwakening.Data instance_of_this_method = (Framework.Table.FCSVTravellerAwakening.Data)typeof(Framework.Table.FCSVTravellerAwakening.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.BuffID;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_NameId_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTravellerAwakening.Data instance_of_this_method = (Framework.Table.FCSVTravellerAwakening.Data)typeof(Framework.Table.FCSVTravellerAwakening.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.NameId;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_StepsId_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTravellerAwakening.Data instance_of_this_method = (Framework.Table.FCSVTravellerAwakening.Data)typeof(Framework.Table.FCSVTravellerAwakening.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.StepsId;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_ActProject_0(ref object o)
        {
            return ((Framework.Table.FCSVTravellerAwakening.Data)o).ActProject;
        }

        static StackObject* CopyToStack_ActProject_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTravellerAwakening.Data)o).ActProject;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_ActCondition_1(ref object o)
        {
            return ((Framework.Table.FCSVTravellerAwakening.Data)o).ActCondition;
        }

        static StackObject* CopyToStack_ActCondition_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTravellerAwakening.Data)o).ActCondition;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_show_attr_name_2(ref object o)
        {
            return ((Framework.Table.FCSVTravellerAwakening.Data)o).show_attr_name;
        }

        static StackObject* CopyToStack_show_attr_name_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTravellerAwakening.Data)o).show_attr_name;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_show_attr_value_3(ref object o)
        {
            return ((Framework.Table.FCSVTravellerAwakening.Data)o).show_attr_value;
        }

        static StackObject* CopyToStack_show_attr_value_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTravellerAwakening.Data)o).show_attr_value;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Award_4(ref object o)
        {
            return ((Framework.Table.FCSVTravellerAwakening.Data)o).Award;
        }

        static StackObject* CopyToStack_Award_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTravellerAwakening.Data)o).Award;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
