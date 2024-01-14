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
    unsafe class Framework_Table_FCSVTalkChoose_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVTalkChoose.Data);
            args = new Type[]{};
            method = type.GetMethod("get_TalkChoose1", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TalkChoose1_0);
            args = new Type[]{};
            method = type.GetMethod("get_ChooseEndTalk1", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ChooseEndTalk1_1);
            args = new Type[]{};
            method = type.GetMethod("get_ChooseRightAndWrong1", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ChooseRightAndWrong1_2);
            args = new Type[]{};
            method = type.GetMethod("get_ChooseWrongResult", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ChooseWrongResult_3);
            args = new Type[]{};
            method = type.GetMethod("get_DetachWrong", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_DetachWrong_4);
            args = new Type[]{};
            method = type.GetMethod("get_id", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_id_5);
            args = new Type[]{};
            method = type.GetMethod("get_TalkChoose2", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TalkChoose2_6);
            args = new Type[]{};
            method = type.GetMethod("get_ChooseEndTalk2", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ChooseEndTalk2_7);
            args = new Type[]{};
            method = type.GetMethod("get_ChooseRightAndWrong2", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ChooseRightAndWrong2_8);
            args = new Type[]{};
            method = type.GetMethod("get_TalkChoose3", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TalkChoose3_9);
            args = new Type[]{};
            method = type.GetMethod("get_ChooseEndTalk3", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ChooseEndTalk3_10);
            args = new Type[]{};
            method = type.GetMethod("get_ChooseRightAndWrong3", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ChooseRightAndWrong3_11);
            args = new Type[]{};
            method = type.GetMethod("get_TalkChoose4", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TalkChoose4_12);
            args = new Type[]{};
            method = type.GetMethod("get_ChooseEndTalk4", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ChooseEndTalk4_13);
            args = new Type[]{};
            method = type.GetMethod("get_ChooseRightAndWrong4", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ChooseRightAndWrong4_14);
            args = new Type[]{};
            method = type.GetMethod("get_TalkChoose5", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TalkChoose5_15);
            args = new Type[]{};
            method = type.GetMethod("get_ChooseEndTalk5", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ChooseEndTalk5_16);
            args = new Type[]{};
            method = type.GetMethod("get_ChooseRightAndWrong5", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ChooseRightAndWrong5_17);

            field = type.GetField("ChooseType1", flag);
            app.RegisterCLRFieldGetter(field, get_ChooseType1_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_ChooseType1_0, null);
            field = type.GetField("ChooseValue1", flag);
            app.RegisterCLRFieldGetter(field, get_ChooseValue1_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_ChooseValue1_1, null);
            field = type.GetField("ChooseType2", flag);
            app.RegisterCLRFieldGetter(field, get_ChooseType2_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_ChooseType2_2, null);
            field = type.GetField("ChooseValue2", flag);
            app.RegisterCLRFieldGetter(field, get_ChooseValue2_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_ChooseValue2_3, null);
            field = type.GetField("ChooseType3", flag);
            app.RegisterCLRFieldGetter(field, get_ChooseType3_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_ChooseType3_4, null);
            field = type.GetField("ChooseValue3", flag);
            app.RegisterCLRFieldGetter(field, get_ChooseValue3_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_ChooseValue3_5, null);
            field = type.GetField("ChooseType4", flag);
            app.RegisterCLRFieldGetter(field, get_ChooseType4_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_ChooseType4_6, null);
            field = type.GetField("ChooseValue4", flag);
            app.RegisterCLRFieldGetter(field, get_ChooseValue4_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_ChooseValue4_7, null);
            field = type.GetField("ChooseType5", flag);
            app.RegisterCLRFieldGetter(field, get_ChooseType5_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_ChooseType5_8, null);
            field = type.GetField("ChooseValue5", flag);
            app.RegisterCLRFieldGetter(field, get_ChooseValue5_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_ChooseValue5_9, null);


        }


        static StackObject* get_TalkChoose1_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TalkChoose1;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ChooseEndTalk1_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ChooseEndTalk1;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ChooseRightAndWrong1_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ChooseRightAndWrong1;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ChooseWrongResult_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ChooseWrongResult;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_DetachWrong_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.DetachWrong;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_id_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.id;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_TalkChoose2_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TalkChoose2;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ChooseEndTalk2_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ChooseEndTalk2;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ChooseRightAndWrong2_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ChooseRightAndWrong2;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_TalkChoose3_9(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TalkChoose3;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ChooseEndTalk3_10(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ChooseEndTalk3;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ChooseRightAndWrong3_11(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ChooseRightAndWrong3;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_TalkChoose4_12(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TalkChoose4;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ChooseEndTalk4_13(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ChooseEndTalk4;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ChooseRightAndWrong4_14(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ChooseRightAndWrong4;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_TalkChoose5_15(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TalkChoose5;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ChooseEndTalk5_16(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ChooseEndTalk5;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ChooseRightAndWrong5_17(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVTalkChoose.Data instance_of_this_method = (Framework.Table.FCSVTalkChoose.Data)typeof(Framework.Table.FCSVTalkChoose.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ChooseRightAndWrong5;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_ChooseType1_0(ref object o)
        {
            return ((Framework.Table.FCSVTalkChoose.Data)o).ChooseType1;
        }

        static StackObject* CopyToStack_ChooseType1_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTalkChoose.Data)o).ChooseType1;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_ChooseValue1_1(ref object o)
        {
            return ((Framework.Table.FCSVTalkChoose.Data)o).ChooseValue1;
        }

        static StackObject* CopyToStack_ChooseValue1_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTalkChoose.Data)o).ChooseValue1;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_ChooseType2_2(ref object o)
        {
            return ((Framework.Table.FCSVTalkChoose.Data)o).ChooseType2;
        }

        static StackObject* CopyToStack_ChooseType2_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTalkChoose.Data)o).ChooseType2;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_ChooseValue2_3(ref object o)
        {
            return ((Framework.Table.FCSVTalkChoose.Data)o).ChooseValue2;
        }

        static StackObject* CopyToStack_ChooseValue2_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTalkChoose.Data)o).ChooseValue2;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_ChooseType3_4(ref object o)
        {
            return ((Framework.Table.FCSVTalkChoose.Data)o).ChooseType3;
        }

        static StackObject* CopyToStack_ChooseType3_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTalkChoose.Data)o).ChooseType3;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_ChooseValue3_5(ref object o)
        {
            return ((Framework.Table.FCSVTalkChoose.Data)o).ChooseValue3;
        }

        static StackObject* CopyToStack_ChooseValue3_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTalkChoose.Data)o).ChooseValue3;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_ChooseType4_6(ref object o)
        {
            return ((Framework.Table.FCSVTalkChoose.Data)o).ChooseType4;
        }

        static StackObject* CopyToStack_ChooseType4_6(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTalkChoose.Data)o).ChooseType4;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_ChooseValue4_7(ref object o)
        {
            return ((Framework.Table.FCSVTalkChoose.Data)o).ChooseValue4;
        }

        static StackObject* CopyToStack_ChooseValue4_7(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTalkChoose.Data)o).ChooseValue4;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_ChooseType5_8(ref object o)
        {
            return ((Framework.Table.FCSVTalkChoose.Data)o).ChooseType5;
        }

        static StackObject* CopyToStack_ChooseType5_8(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTalkChoose.Data)o).ChooseType5;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_ChooseValue5_9(ref object o)
        {
            return ((Framework.Table.FCSVTalkChoose.Data)o).ChooseValue5;
        }

        static StackObject* CopyToStack_ChooseValue5_9(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVTalkChoose.Data)o).ChooseValue5;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
