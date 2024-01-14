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
    unsafe class Framework_Table_FCSVActiveSkillShow_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVActiveSkillShow.Data);

            field = type.GetField("enemy_position", flag);
            app.RegisterCLRFieldGetter(field, get_enemy_position_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_enemy_position_0, null);
            field = type.GetField("friend_position", flag);
            app.RegisterCLRFieldGetter(field, get_friend_position_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_friend_position_1, null);
            field = type.GetField("interval", flag);
            app.RegisterCLRFieldGetter(field, get_interval_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_interval_2, null);
            field = type.GetField("skill_combat_id", flag);
            app.RegisterCLRFieldGetter(field, get_skill_combat_id_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_skill_combat_id_3, null);


        }



        static object get_enemy_position_0(ref object o)
        {
            return ((Framework.Table.FCSVActiveSkillShow.Data)o).enemy_position;
        }

        static StackObject* CopyToStack_enemy_position_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVActiveSkillShow.Data)o).enemy_position;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_friend_position_1(ref object o)
        {
            return ((Framework.Table.FCSVActiveSkillShow.Data)o).friend_position;
        }

        static StackObject* CopyToStack_friend_position_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVActiveSkillShow.Data)o).friend_position;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_interval_2(ref object o)
        {
            return ((Framework.Table.FCSVActiveSkillShow.Data)o).interval;
        }

        static StackObject* CopyToStack_interval_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVActiveSkillShow.Data)o).interval;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_skill_combat_id_3(ref object o)
        {
            return ((Framework.Table.FCSVActiveSkillShow.Data)o).skill_combat_id;
        }

        static StackObject* CopyToStack_skill_combat_id_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVActiveSkillShow.Data)o).skill_combat_id;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
