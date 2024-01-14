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
    unsafe class SDKManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::SDKManager);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("SDK_SetGameFightStatus", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDK_SetGameFightStatus_0);
            args = new Type[]{};
            method = type.GetMethod("UninitGMESDK", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UninitGMESDK_1);
            args = new Type[]{typeof(global::SDKManager.ESDKLogLevel), typeof(System.String)};
            method = type.GetMethod("SDKPrintLog", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKPrintLog_2);
            args = new Type[]{};
            method = type.GetMethod("GetChannel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetChannel_3);
            args = new Type[]{typeof(System.String), typeof(System.String).MakeByRefType()};
            method = type.GetMethod("IsOpenGetExtJsonParam", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsOpenGetExtJsonParam_4);
            args = new Type[]{};
            method = type.GetMethod("AddQQGroupIsOpen", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddQQGroupIsOpen_5);
            args = new Type[]{};
            method = type.GetMethod("SDKPreWarmActivity", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKPreWarmActivity_6);
            args = new Type[]{};
            method = type.GetMethod("GetUID", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetUID_7);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("SDKOpenH5Questionnaire", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKOpenH5Questionnaire_8);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("InitGMESDK", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InitGMESDK_9);
            args = new Type[]{};
            method = type.GetMethod("GetAppid", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetAppid_10);
            args = new Type[]{};
            method = type.GetMethod("GetPublishAppMarket", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetPublishAppMarket_11);
            args = new Type[]{};
            method = type.GetMethod("GetToken", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetToken_12);
            args = new Type[]{};
            method = type.GetMethod("GetDeviceId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetDeviceId_13);
            args = new Type[]{};
            method = type.GetMethod("SDKLogin", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKLogin_14);
            args = new Type[]{};
            method = type.GetMethod("SDKISEmulator", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKISEmulator_15);
            args = new Type[]{};
            method = type.GetMethod("GetDeviceType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetDeviceType_16);
            args = new Type[]{typeof(System.String), typeof(System.Int32)};
            method = type.GetMethod("ThirdACESdkLogin", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ThirdACESdkLogin_17);
            args = new Type[]{};
            method = type.GetMethod("ACESDKLoginOff", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ACESDKLoginOff_18);
            args = new Type[]{};
            method = type.GetMethod("SDKSetPhoneBind", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKSetPhoneBind_19);
            args = new Type[]{};
            method = type.GetMethod("GetEnableSwitchAccount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetEnableSwitchAccount_20);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("SDKApiAvailable", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKApiAvailable_21);
            args = new Type[]{};
            method = type.GetMethod("GetAppVersion", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetAppVersion_22);
            args = new Type[]{};
            method = type.GetMethod("SDKLogout", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKLogout_23);
            args = new Type[]{};
            method = type.GetMethod("SDKPrivacyPolicy", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKPrivacyPolicy_24);
            args = new Type[]{};
            method = type.GetMethod("SDKUserAgreement", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKUserAgreement_25);
            args = new Type[]{};
            method = type.GetMethod("SDKScanQRCode", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKScanQRCode_26);
            args = new Type[]{};
            method = type.GetMethod("SDKCustomService", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKCustomService_27);
            args = new Type[]{};
            method = type.GetMethod("SDKSDKWarmTips", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKSDKWarmTips_28);
            args = new Type[]{};
            method = type.GetMethod("SDKOpenUserCenter", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKOpenUserCenter_29);
            args = new Type[]{};
            method = type.GetMethod("SDKjoinQQGroup", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKjoinQQGroup_30);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("SDKReportErrorToChannel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKReportErrorToChannel_31);
            args = new Type[]{};
            method = type.GetMethod("SDKSetCanExitVariable", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SDKSetCanExitVariable_32);
            args = new Type[]{};
            method = type.GetMethod("GetiOSPhaseCanPlay", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetiOSPhaseCanPlay_33);

            field = type.GetField("eventEmitter", flag);
            app.RegisterCLRFieldGetter(field, get_eventEmitter_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_eventEmitter_0, null);
            field = type.GetField("sdk", flag);
            app.RegisterCLRFieldGetter(field, get_sdk_1);
            app.RegisterCLRFieldSetter(field, set_sdk_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_sdk_1, AssignFromStack_sdk_1);
            field = type.GetField("iAccountType", flag);
            app.RegisterCLRFieldGetter(field, get_iAccountType_2);
            app.RegisterCLRFieldSetter(field, set_iAccountType_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_iAccountType_2, AssignFromStack_iAccountType_2);
            field = type.GetField("bPaiLianTu", flag);
            app.RegisterCLRFieldGetter(field, get_bPaiLianTu_3);
            app.RegisterCLRFieldSetter(field, set_bPaiLianTu_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_bPaiLianTu_3, AssignFromStack_bPaiLianTu_3);
            field = type.GetField("bSwitchAccount", flag);
            app.RegisterCLRFieldGetter(field, get_bSwitchAccount_4);
            app.RegisterCLRFieldSetter(field, set_bSwitchAccount_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_bSwitchAccount_4, AssignFromStack_bSwitchAccount_4);
            field = type.GetField("officialChannel", flag);
            app.RegisterCLRFieldGetter(field, get_officialChannel_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_officialChannel_5, null);


        }


        static StackObject* SDK_SetGameFightStatus_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isFighting = ptr_of_this_method->Value == 1;


            global::SDKManager.SDK_SetGameFightStatus(@isFighting);

            return __ret;
        }

        static StackObject* UninitGMESDK_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SDKManager.UninitGMESDK();

            return __ret;
        }

        static StackObject* SDKPrintLog_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @msg = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::SDKManager.ESDKLogLevel @logLevel = (global::SDKManager.ESDKLogLevel)typeof(global::SDKManager.ESDKLogLevel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);


            global::SDKManager.SDKPrintLog(@logLevel, @msg);

            return __ret;
        }

        static StackObject* GetChannel_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.GetChannel();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* IsOpenGetExtJsonParam_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @paramValue = (System.String)typeof(System.String).CheckCLRTypes(__intp.RetriveObject(ptr_of_this_method, __mStack), (CLR.Utils.Extensions.TypeFlags)0);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @paramKey = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);


            var result_of_this_method = global::SDKManager.IsOpenGetExtJsonParam(@paramKey, out @paramValue);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            switch(ptr_of_this_method->ObjectType)
            {
                case ObjectTypes.StackObjectReference:
                    {
                        var ___dst = ILIntepreter.ResolveReference(ptr_of_this_method);
                        object ___obj = @paramValue;
                        if (___dst->ObjectType >= ObjectTypes.Object)
                        {
                            if (___obj is CrossBindingAdaptorType)
                                ___obj = ((CrossBindingAdaptorType)___obj).ILInstance;
                            __mStack[___dst->Value] = ___obj;
                        }
                        else
                        {
                            ILIntepreter.UnboxObject(___dst, ___obj, __mStack, __domain);
                        }
                    }
                    break;
                case ObjectTypes.FieldReference:
                    {
                        var ___obj = __mStack[ptr_of_this_method->Value];
                        if(___obj is ILTypeInstance)
                        {
                            ((ILTypeInstance)___obj)[ptr_of_this_method->ValueLow] = @paramValue;
                        }
                        else
                        {
                            var ___type = __domain.GetType(___obj.GetType()) as CLRType;
                            ___type.SetFieldValue(ptr_of_this_method->ValueLow, ref ___obj, @paramValue);
                        }
                    }
                    break;
                case ObjectTypes.StaticFieldReference:
                    {
                        var ___type = __domain.GetType(ptr_of_this_method->Value);
                        if(___type is ILType)
                        {
                            ((ILType)___type).StaticInstance[ptr_of_this_method->ValueLow] = @paramValue;
                        }
                        else
                        {
                            ((CLRType)___type).SetStaticFieldValue(ptr_of_this_method->ValueLow, @paramValue);
                        }
                    }
                    break;
                 case ObjectTypes.ArrayReference:
                    {
                        var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as System.String[];
                        instance_of_arrayReference[ptr_of_this_method->ValueLow] = @paramValue;
                    }
                    break;
            }

            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            __intp.Free(ptr_of_this_method);
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* AddQQGroupIsOpen_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.AddQQGroupIsOpen();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* SDKPreWarmActivity_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.SDKPreWarmActivity();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* GetUID_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.GetUID();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SDKOpenH5Questionnaire_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @urlStr = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            global::SDKManager.SDKOpenH5Questionnaire(@urlStr);

            return __ret;
        }

        static StackObject* InitGMESDK_9(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 @roleID = *(ulong*)&ptr_of_this_method->Value;


            var result_of_this_method = global::SDKManager.InitGMESDK(@roleID);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetAppid_10(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.GetAppid();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetPublishAppMarket_11(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.GetPublishAppMarket();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetToken_12(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.GetToken();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetDeviceId_13(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.GetDeviceId();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SDKLogin_14(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SDKManager.SDKLogin();

            return __ret;
        }

        static StackObject* SDKISEmulator_15(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.SDKISEmulator();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* GetDeviceType_16(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.GetDeviceType();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ThirdACESdkLogin_17(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @serverId = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @role_id = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            global::SDKManager.ThirdACESdkLogin(@role_id, @serverId);

            return __ret;
        }

        static StackObject* ACESDKLoginOff_18(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SDKManager.ACESDKLoginOff();

            return __ret;
        }

        static StackObject* SDKSetPhoneBind_19(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SDKManager.SDKSetPhoneBind();

            return __ret;
        }

        static StackObject* GetEnableSwitchAccount_20(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.GetEnableSwitchAccount();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* SDKApiAvailable_21(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @funcName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::SDKManager.SDKApiAvailable(@funcName);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* GetAppVersion_22(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.GetAppVersion();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SDKLogout_23(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SDKManager.SDKLogout();

            return __ret;
        }

        static StackObject* SDKPrivacyPolicy_24(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SDKManager.SDKPrivacyPolicy();

            return __ret;
        }

        static StackObject* SDKUserAgreement_25(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SDKManager.SDKUserAgreement();

            return __ret;
        }

        static StackObject* SDKScanQRCode_26(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SDKManager.SDKScanQRCode();

            return __ret;
        }

        static StackObject* SDKCustomService_27(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SDKManager.SDKCustomService();

            return __ret;
        }

        static StackObject* SDKSDKWarmTips_28(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SDKManager.SDKSDKWarmTips();

            return __ret;
        }

        static StackObject* SDKOpenUserCenter_29(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SDKManager.SDKOpenUserCenter();

            return __ret;
        }

        static StackObject* SDKjoinQQGroup_30(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.SDKjoinQQGroup();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* SDKReportErrorToChannel_31(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @flagName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            global::SDKManager.SDKReportErrorToChannel(@flagName);

            return __ret;
        }

        static StackObject* SDKSetCanExitVariable_32(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SDKManager.SDKSetCanExitVariable();

            return __ret;
        }

        static StackObject* GetiOSPhaseCanPlay_33(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SDKManager.GetiOSPhaseCanPlay();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }


        static object get_eventEmitter_0(ref object o)
        {
            return global::SDKManager.eventEmitter;
        }

        static StackObject* CopyToStack_eventEmitter_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::SDKManager.eventEmitter;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_sdk_1(ref object o)
        {
            return global::SDKManager.sdk;
        }

        static StackObject* CopyToStack_sdk_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::SDKManager.sdk;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_sdk_1(ref object o, object v)
        {
            global::SDKManager.sdk = (global::SDKBase)v;
        }

        static StackObject* AssignFromStack_sdk_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::SDKBase @sdk = (global::SDKBase)typeof(global::SDKBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            global::SDKManager.sdk = @sdk;
            return ptr_of_this_method;
        }

        static object get_iAccountType_2(ref object o)
        {
            return global::SDKManager.iAccountType;
        }

        static StackObject* CopyToStack_iAccountType_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::SDKManager.iAccountType;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_iAccountType_2(ref object o, object v)
        {
            global::SDKManager.iAccountType = (System.Int32)v;
        }

        static StackObject* AssignFromStack_iAccountType_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @iAccountType = ptr_of_this_method->Value;
            global::SDKManager.iAccountType = @iAccountType;
            return ptr_of_this_method;
        }

        static object get_bPaiLianTu_3(ref object o)
        {
            return global::SDKManager.bPaiLianTu;
        }

        static StackObject* CopyToStack_bPaiLianTu_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::SDKManager.bPaiLianTu;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_bPaiLianTu_3(ref object o, object v)
        {
            global::SDKManager.bPaiLianTu = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_bPaiLianTu_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @bPaiLianTu = ptr_of_this_method->Value == 1;
            global::SDKManager.bPaiLianTu = @bPaiLianTu;
            return ptr_of_this_method;
        }

        static object get_bSwitchAccount_4(ref object o)
        {
            return global::SDKManager.bSwitchAccount;
        }

        static StackObject* CopyToStack_bSwitchAccount_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::SDKManager.bSwitchAccount;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_bSwitchAccount_4(ref object o, object v)
        {
            global::SDKManager.bSwitchAccount = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_bSwitchAccount_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @bSwitchAccount = ptr_of_this_method->Value == 1;
            global::SDKManager.bSwitchAccount = @bSwitchAccount;
            return ptr_of_this_method;
        }

        static object get_officialChannel_5(ref object o)
        {
            return global::SDKManager.officialChannel;
        }

        static StackObject* CopyToStack_officialChannel_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::SDKManager.officialChannel;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
