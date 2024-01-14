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
    unsafe class Framework_Table_FCSVAward_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVAward.Data);
            args = new Type[]{};
            method = type.GetMethod("get_isBroadCast", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_isBroadCast_0);
            args = new Type[]{};
            method = type.GetMethod("get_quality", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_quality_1);
            args = new Type[]{};
            method = type.GetMethod("get_itemId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_itemId_2);
            args = new Type[]{};
            method = type.GetMethod("get_itemNum", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_itemNum_3);
            args = new Type[]{};
            method = type.GetMethod("get_isExclusive", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_isExclusive_4);
            args = new Type[]{};
            method = type.GetMethod("get_iconId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_iconId_5);

            field = type.GetField("backgroundType", flag);
            app.RegisterCLRFieldGetter(field, get_backgroundType_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_backgroundType_0, null);
            field = type.GetField("backgroundlightType", flag);
            app.RegisterCLRFieldGetter(field, get_backgroundlightType_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_backgroundlightType_1, null);


        }


        static StackObject* get_isBroadCast_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAward.Data instance_of_this_method = (Framework.Table.FCSVAward.Data)typeof(Framework.Table.FCSVAward.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.isBroadCast;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_quality_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAward.Data instance_of_this_method = (Framework.Table.FCSVAward.Data)typeof(Framework.Table.FCSVAward.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.quality;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_itemId_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAward.Data instance_of_this_method = (Framework.Table.FCSVAward.Data)typeof(Framework.Table.FCSVAward.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.itemId;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_itemNum_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAward.Data instance_of_this_method = (Framework.Table.FCSVAward.Data)typeof(Framework.Table.FCSVAward.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.itemNum;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_isExclusive_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAward.Data instance_of_this_method = (Framework.Table.FCSVAward.Data)typeof(Framework.Table.FCSVAward.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.isExclusive;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* get_iconId_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAward.Data instance_of_this_method = (Framework.Table.FCSVAward.Data)typeof(Framework.Table.FCSVAward.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.iconId;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_backgroundType_0(ref object o)
        {
            return ((Framework.Table.FCSVAward.Data)o).backgroundType;
        }

        static StackObject* CopyToStack_backgroundType_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAward.Data)o).backgroundType;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_backgroundlightType_1(ref object o)
        {
            return ((Framework.Table.FCSVAward.Data)o).backgroundlightType;
        }

        static StackObject* CopyToStack_backgroundlightType_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAward.Data)o).backgroundlightType;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
