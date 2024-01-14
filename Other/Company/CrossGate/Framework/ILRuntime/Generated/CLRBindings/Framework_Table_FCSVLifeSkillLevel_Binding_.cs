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
    unsafe class Framework_Table_FCSVLifeSkillLevel_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVLifeSkillLevel.Data);
            args = new Type[]{};
            method = type.GetMethod("get_proficiency", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_proficiency_0);
            args = new Type[]{};
            method = type.GetMethod("get_level", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_level_1);

            field = type.GetField("collection_item_id", flag);
            app.RegisterCLRFieldGetter(field, get_collection_item_id_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_collection_item_id_0, null);
            field = type.GetField("cost_item", flag);
            app.RegisterCLRFieldGetter(field, get_cost_item_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_cost_item_1, null);
            field = type.GetField("active_npc", flag);
            app.RegisterCLRFieldGetter(field, get_active_npc_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_active_npc_2, null);


        }


        static StackObject* get_proficiency_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVLifeSkillLevel.Data instance_of_this_method = (Framework.Table.FCSVLifeSkillLevel.Data)typeof(Framework.Table.FCSVLifeSkillLevel.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.proficiency;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_level_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVLifeSkillLevel.Data instance_of_this_method = (Framework.Table.FCSVLifeSkillLevel.Data)typeof(Framework.Table.FCSVLifeSkillLevel.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.level;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_collection_item_id_0(ref object o)
        {
            return ((Framework.Table.FCSVLifeSkillLevel.Data)o).collection_item_id;
        }

        static StackObject* CopyToStack_collection_item_id_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVLifeSkillLevel.Data)o).collection_item_id;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_cost_item_1(ref object o)
        {
            return ((Framework.Table.FCSVLifeSkillLevel.Data)o).cost_item;
        }

        static StackObject* CopyToStack_cost_item_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVLifeSkillLevel.Data)o).cost_item;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_active_npc_2(ref object o)
        {
            return ((Framework.Table.FCSVLifeSkillLevel.Data)o).active_npc;
        }

        static StackObject* CopyToStack_active_npc_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVLifeSkillLevel.Data)o).active_npc;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
