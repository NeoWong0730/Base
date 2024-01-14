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
    unsafe class Framework_Table_FCSVGoddessTopic_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVGoddessTopic.Data);
            args = new Type[]{};
            method = type.GetMethod("get_topicDifficulty", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_topicDifficulty_0);
            args = new Type[]{};
            method = type.GetMethod("get_topicId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_topicId_1);
            args = new Type[]{};
            method = type.GetMethod("get_topicLan", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_topicLan_2);
            args = new Type[]{};
            method = type.GetMethod("get_id", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_id_3);
            args = new Type[]{};
            method = type.GetMethod("get_teamTarget", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_teamTarget_4);

            field = type.GetField("topicIcon", flag);
            app.RegisterCLRFieldGetter(field, get_topicIcon_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_topicIcon_0, null);
            field = type.GetField("copyLevel", flag);
            app.RegisterCLRFieldGetter(field, get_copyLevel_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_copyLevel_1, null);
            field = type.GetField("InstanceId", flag);
            app.RegisterCLRFieldGetter(field, get_InstanceId_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_InstanceId_2, null);
            field = type.GetField("iconId", flag);
            app.RegisterCLRFieldGetter(field, get_iconId_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_iconId_3, null);
            field = type.GetField("lanID", flag);
            app.RegisterCLRFieldGetter(field, get_lanID_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_lanID_4, null);
            field = type.GetField("monsterCharacter", flag);
            app.RegisterCLRFieldGetter(field, get_monsterCharacter_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_monsterCharacter_5, null);
            field = type.GetField("aiCharacter", flag);
            app.RegisterCLRFieldGetter(field, get_aiCharacter_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_aiCharacter_6, null);
            field = type.GetField("EndReward", flag);
            app.RegisterCLRFieldGetter(field, get_EndReward_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_EndReward_7, null);
            field = type.GetField("rankReward", flag);
            app.RegisterCLRFieldGetter(field, get_rankReward_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_rankReward_8, null);


        }


        static StackObject* get_topicDifficulty_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGoddessTopic.Data instance_of_this_method = (Framework.Table.FCSVGoddessTopic.Data)typeof(Framework.Table.FCSVGoddessTopic.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.topicDifficulty;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_topicId_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGoddessTopic.Data instance_of_this_method = (Framework.Table.FCSVGoddessTopic.Data)typeof(Framework.Table.FCSVGoddessTopic.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.topicId;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_topicLan_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGoddessTopic.Data instance_of_this_method = (Framework.Table.FCSVGoddessTopic.Data)typeof(Framework.Table.FCSVGoddessTopic.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.topicLan;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_id_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGoddessTopic.Data instance_of_this_method = (Framework.Table.FCSVGoddessTopic.Data)typeof(Framework.Table.FCSVGoddessTopic.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.id;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_teamTarget_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGoddessTopic.Data instance_of_this_method = (Framework.Table.FCSVGoddessTopic.Data)typeof(Framework.Table.FCSVGoddessTopic.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.teamTarget;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_topicIcon_0(ref object o)
        {
            return ((Framework.Table.FCSVGoddessTopic.Data)o).topicIcon;
        }

        static StackObject* CopyToStack_topicIcon_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGoddessTopic.Data)o).topicIcon;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_copyLevel_1(ref object o)
        {
            return ((Framework.Table.FCSVGoddessTopic.Data)o).copyLevel;
        }

        static StackObject* CopyToStack_copyLevel_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGoddessTopic.Data)o).copyLevel;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_InstanceId_2(ref object o)
        {
            return ((Framework.Table.FCSVGoddessTopic.Data)o).InstanceId;
        }

        static StackObject* CopyToStack_InstanceId_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGoddessTopic.Data)o).InstanceId;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_iconId_3(ref object o)
        {
            return ((Framework.Table.FCSVGoddessTopic.Data)o).iconId;
        }

        static StackObject* CopyToStack_iconId_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGoddessTopic.Data)o).iconId;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_lanID_4(ref object o)
        {
            return ((Framework.Table.FCSVGoddessTopic.Data)o).lanID;
        }

        static StackObject* CopyToStack_lanID_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGoddessTopic.Data)o).lanID;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_monsterCharacter_5(ref object o)
        {
            return ((Framework.Table.FCSVGoddessTopic.Data)o).monsterCharacter;
        }

        static StackObject* CopyToStack_monsterCharacter_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGoddessTopic.Data)o).monsterCharacter;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_aiCharacter_6(ref object o)
        {
            return ((Framework.Table.FCSVGoddessTopic.Data)o).aiCharacter;
        }

        static StackObject* CopyToStack_aiCharacter_6(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGoddessTopic.Data)o).aiCharacter;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_EndReward_7(ref object o)
        {
            return ((Framework.Table.FCSVGoddessTopic.Data)o).EndReward;
        }

        static StackObject* CopyToStack_EndReward_7(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGoddessTopic.Data)o).EndReward;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_rankReward_8(ref object o)
        {
            return ((Framework.Table.FCSVGoddessTopic.Data)o).rankReward;
        }

        static StackObject* CopyToStack_rankReward_8(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGoddessTopic.Data)o).rankReward;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
