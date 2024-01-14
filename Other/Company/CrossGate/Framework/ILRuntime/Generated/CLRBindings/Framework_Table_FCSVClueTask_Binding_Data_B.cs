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
    unsafe class Framework_Table_FCSVClueTask_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVClueTask.Data);
            args = new Type[]{};
            method = type.GetMethod("get_TaskName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TaskName_0);
            args = new Type[]{};
            method = type.GetMethod("get_id", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_id_1);
            args = new Type[]{};
            method = type.GetMethod("get_TaskType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TaskType_2);
            args = new Type[]{};
            method = type.GetMethod("get_TaskStar", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TaskStar_3);
            args = new Type[]{};
            method = type.GetMethod("get_Reward", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Reward_4);
            args = new Type[]{};
            method = type.GetMethod("get_TaskCompleteDes", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TaskCompleteDes_5);

            field = type.GetField("PhasedTasksGroup", flag);
            app.RegisterCLRFieldGetter(field, get_PhasedTasksGroup_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_PhasedTasksGroup_0, null);
            field = type.GetField("TriggerCondition_FinishTasks", flag);
            app.RegisterCLRFieldGetter(field, get_TriggerCondition_FinishTasks_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_TriggerCondition_FinishTasks_1, null);
            field = type.GetField("TriggerCondition_UnlockMaps", flag);
            app.RegisterCLRFieldGetter(field, get_TriggerCondition_UnlockMaps_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_TriggerCondition_UnlockMaps_2, null);
            field = type.GetField("BG", flag);
            app.RegisterCLRFieldGetter(field, get_BG_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_BG_3, null);


        }


        static StackObject* get_TaskName_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVClueTask.Data instance_of_this_method = (Framework.Table.FCSVClueTask.Data)typeof(Framework.Table.FCSVClueTask.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TaskName;

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
            Framework.Table.FCSVClueTask.Data instance_of_this_method = (Framework.Table.FCSVClueTask.Data)typeof(Framework.Table.FCSVClueTask.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.id;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_TaskType_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVClueTask.Data instance_of_this_method = (Framework.Table.FCSVClueTask.Data)typeof(Framework.Table.FCSVClueTask.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TaskType;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_TaskStar_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVClueTask.Data instance_of_this_method = (Framework.Table.FCSVClueTask.Data)typeof(Framework.Table.FCSVClueTask.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TaskStar;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_Reward_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVClueTask.Data instance_of_this_method = (Framework.Table.FCSVClueTask.Data)typeof(Framework.Table.FCSVClueTask.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Reward;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_TaskCompleteDes_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVClueTask.Data instance_of_this_method = (Framework.Table.FCSVClueTask.Data)typeof(Framework.Table.FCSVClueTask.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TaskCompleteDes;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_PhasedTasksGroup_0(ref object o)
        {
            return ((Framework.Table.FCSVClueTask.Data)o).PhasedTasksGroup;
        }

        static StackObject* CopyToStack_PhasedTasksGroup_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVClueTask.Data)o).PhasedTasksGroup;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_TriggerCondition_FinishTasks_1(ref object o)
        {
            return ((Framework.Table.FCSVClueTask.Data)o).TriggerCondition_FinishTasks;
        }

        static StackObject* CopyToStack_TriggerCondition_FinishTasks_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVClueTask.Data)o).TriggerCondition_FinishTasks;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_TriggerCondition_UnlockMaps_2(ref object o)
        {
            return ((Framework.Table.FCSVClueTask.Data)o).TriggerCondition_UnlockMaps;
        }

        static StackObject* CopyToStack_TriggerCondition_UnlockMaps_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVClueTask.Data)o).TriggerCondition_UnlockMaps;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_BG_3(ref object o)
        {
            return ((Framework.Table.FCSVClueTask.Data)o).BG;
        }

        static StackObject* CopyToStack_BG_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVClueTask.Data)o).BG;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
