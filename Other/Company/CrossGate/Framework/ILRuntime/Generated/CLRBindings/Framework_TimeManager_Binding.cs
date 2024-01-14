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
    unsafe class Framework_TimeManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.TimeManager);
            args = new Type[]{typeof(System.Int32), typeof(System.Int32)};
            method = type.GetMethod("CanExecute", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CanExecute_0);
            args = new Type[]{};
            method = type.GetMethod("get_FPS", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_FPS_1);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("GetDateTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetDateTime_2);
            args = new Type[]{typeof(System.Int64)};
            method = type.GetMethod("ConvertFromZeroTimeZone", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ConvertFromZeroTimeZone_3);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("GetServerTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetServerTime_4);
            args = new Type[]{};
            method = type.GetMethod("get_TimeZoneOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TimeZoneOffset_5);
            args = new Type[]{};
            method = type.GetMethod("GetElapseTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetElapseTime_6);
            args = new Type[]{};
            method = type.GetMethod("ClientNowMillisecond", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ClientNowMillisecond_7);
            args = new Type[]{};
            method = type.GetMethod("GetLocalNow", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetLocalNow_8);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("CorrectServerTimeZone", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CorrectServerTimeZone_9);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("CorrectServerTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CorrectServerTime_10);
            args = new Type[]{};
            method = type.GetMethod("StartFPSCalculate", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StartFPSCalculate_11);
            args = new Type[]{typeof(System.Int64), typeof(System.Int64)};
            method = type.GetMethod("ConvertToLocalTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ConvertToLocalTime_12);

            field = type.GetField("START_TIME", flag);
            app.RegisterCLRFieldGetter(field, get_START_TIME_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_START_TIME_0, null);


        }


        static StackObject* CanExecute_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @intervalFrame = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @offsetFrame = ptr_of_this_method->Value;


            var result_of_this_method = Framework.TimeManager.CanExecute(@offsetFrame, @intervalFrame);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* get_FPS_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Framework.TimeManager.FPS;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetDateTime_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 @localTimeStamp = (uint)ptr_of_this_method->Value;


            var result_of_this_method = Framework.TimeManager.GetDateTime(@localTimeStamp);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ConvertFromZeroTimeZone_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int64 @timeWithoutTimeZone = *(long*)&ptr_of_this_method->Value;


            var result_of_this_method = Framework.TimeManager.ConvertFromZeroTimeZone(@timeWithoutTimeZone);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetServerTime_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @withoutTimeZone = ptr_of_this_method->Value == 1;


            var result_of_this_method = Framework.TimeManager.GetServerTime(@withoutTimeZone);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_TimeZoneOffset_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Framework.TimeManager.TimeZoneOffset;

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetElapseTime_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Framework.TimeManager.GetElapseTime();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ClientNowMillisecond_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Framework.TimeManager.ClientNowMillisecond();

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetLocalNow_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Framework.TimeManager.GetLocalNow();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* CorrectServerTimeZone_9(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @timeZone = ptr_of_this_method->Value;


            Framework.TimeManager.CorrectServerTimeZone(@timeZone);

            return __ret;
        }

        static StackObject* CorrectServerTime_10(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 @serverTime = (uint)ptr_of_this_method->Value;


            Framework.TimeManager.CorrectServerTime(@serverTime);

            return __ret;
        }

        static StackObject* StartFPSCalculate_11(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            Framework.TimeManager.StartFPSCalculate();

            return __ret;
        }

        static StackObject* ConvertToLocalTime_12(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int64 @timeZoneOffse = *(long*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int64 @timeStampWithTimeZone = *(long*)&ptr_of_this_method->Value;


            var result_of_this_method = Framework.TimeManager.ConvertToLocalTime(@timeStampWithTimeZone, @timeZoneOffse);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_START_TIME_0(ref object o)
        {
            return Framework.TimeManager.START_TIME;
        }

        static StackObject* CopyToStack_START_TIME_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = Framework.TimeManager.START_TIME;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
