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
    unsafe class Framework_Table_FCSVPedigreedTaskGroup_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVPedigreedTaskGroup.Data);
            args = new Type[]{};
            method = type.GetMethod("get_Tip", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Tip_0);

            field = type.GetField("ReachTypeAchievement", flag);
            app.RegisterCLRFieldGetter(field, get_ReachTypeAchievement_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_ReachTypeAchievement_0, null);
            field = type.GetField("Change_UI", flag);
            app.RegisterCLRFieldGetter(field, get_Change_UI_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_Change_UI_1, null);


        }


        static StackObject* get_Tip_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVPedigreedTaskGroup.Data instance_of_this_method = (Framework.Table.FCSVPedigreedTaskGroup.Data)typeof(Framework.Table.FCSVPedigreedTaskGroup.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Tip;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_ReachTypeAchievement_0(ref object o)
        {
            return ((Framework.Table.FCSVPedigreedTaskGroup.Data)o).ReachTypeAchievement;
        }

        static StackObject* CopyToStack_ReachTypeAchievement_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVPedigreedTaskGroup.Data)o).ReachTypeAchievement;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Change_UI_1(ref object o)
        {
            return ((Framework.Table.FCSVPedigreedTaskGroup.Data)o).Change_UI;
        }

        static StackObject* CopyToStack_Change_UI_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVPedigreedTaskGroup.Data)o).Change_UI;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}