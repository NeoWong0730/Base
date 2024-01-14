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

namespace ILRuntime.Runtime.Generated
{
    unsafe class Lib_Core_DebugUtil_Binder
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Lib.Core.DebugUtil);

            args = new Type[] { typeof(System.String) };
            method = type.GetMethod("LogError", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogError_1);

            args = new Type[] { typeof(System.String), typeof(System.Object[]) };
            method = type.GetMethod("LogErrorFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogErrorFormat_4);

            args = new Type[] { typeof(System.Exception) };
            method = type.GetMethod("LogException", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogException_3);

            args = new Type[] { typeof(System.Int32) };
            method = type.GetMethod("IsOpenLogType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsOpenLogType_5);

#if DEBUG_MODE
            args = new Type[]{typeof(Lib.Core.ELogType), typeof(System.String)};
            method = type.GetMethod("Log", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Log_0);

            args = new Type[] { typeof(Lib.Core.ELogType), typeof(System.String), typeof(System.Object[]) };
            method = type.GetMethod("LogFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogFormat_8);

            args = new Type[] { typeof(Lib.Core.ELogType), typeof(System.Boolean), typeof(System.String), typeof(System.Object[]) };
            method = type.GetMethod("LogFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogFormat_10);

            args = new Type[] { typeof(System.String) };
            method = type.GetMethod("LogWarning", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogWarning_9);

            args = new Type[]{typeof(System.String), typeof(System.Object[])};
            method = type.GetMethod("LogWarningFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogWarningFormat_2);            

            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("CloseLogType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CloseLogType_6);

            //args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("OpenLogType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OpenLogType_7);                                  
#endif
        }


        static StackObject* Log_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Lib.Core.ELogType @logType = (Lib.Core.ELogType)ptr_of_this_method->Value;
            __intp.Free(ptr_of_this_method);

            if(Lib.Core.DebugUtil.IsOpenLogType((int)@logType))
            {
                ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
                System.String @log = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                __intp.Free(ptr_of_this_method);

                string stackTrace = __domain.DebugService.GetStackTrace(__intp);

                Lib.Core.DebugUtil.Log(@logType, @log + "\n" + stackTrace);
            }

            return __ret;
        }

        static StackObject* LogError_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @message = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            string stackTrace = __domain.DebugService.GetStackTrace(__intp);

            Lib.Core.DebugUtil.LogError(@message + "\n" + stackTrace);

            return __ret;
        }

        static StackObject* LogWarningFormat_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @args = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @format = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            string warning = string.Format(@format, @args) + "\n" + stackTrace;

            Lib.Core.DebugUtil.LogWarning(warning);

            return __ret;
        }

        static StackObject* LogException_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Exception @exception = (System.Exception)typeof(System.Exception).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            
            //Lib.Core.DebugUtil.LogException(@exception.Message);
            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            Lib.Core.DebugUtil.LogErrorFormat(@exception.Message + "\n" + stackTrace);

            return __ret;
        }

        static StackObject* LogErrorFormat_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @args = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @format = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            string error = string.Format(@format, @args) + "\n" + stackTrace;

            Lib.Core.DebugUtil.LogErrorFormat(error);

            return __ret;
        }

        static StackObject* IsOpenLogType_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @logType = ptr_of_this_method->Value;


            var result_of_this_method = Lib.Core.DebugUtil.IsOpenLogType(@logType);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* CloseLogType_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @logType = ptr_of_this_method->Value;


            Lib.Core.DebugUtil.CloseLogType(@logType);

            return __ret;
        }

        static StackObject* OpenLogType_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @logType = ptr_of_this_method->Value;


            Lib.Core.DebugUtil.OpenLogType(@logType);

            return __ret;
        }

        static StackObject* LogFormat_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Lib.Core.ELogType @logType = (Lib.Core.ELogType)ptr_of_this_method->Value;
            __intp.Free(ptr_of_this_method);
            if (Lib.Core.DebugUtil.IsOpenLogType((int)@logType))
            {
                ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
                System.Object[] @args = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                __intp.Free(ptr_of_this_method);

                ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
                System.String @format = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                __intp.Free(ptr_of_this_method);

                string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                string log = string.Format(@format, @args) + "\n" + stackTrace;

                Lib.Core.DebugUtil.Log(@logType, log);
            }            

            return __ret;
        }

        static StackObject* LogWarning_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @message = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            string stackTrace = __domain.DebugService.GetStackTrace(__intp);

            Lib.Core.DebugUtil.LogWarning(@message + "\n" + stackTrace);

            return __ret;
        }

        static StackObject* LogFormat_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            Lib.Core.ELogType @logType = (Lib.Core.ELogType)ptr_of_this_method->Value;
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Boolean @conditional = ptr_of_this_method->Value == 1;

            if (Lib.Core.DebugUtil.IsOpenLogType((int)@logType) && @conditional)
            {
                ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
                System.Object[] @args = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                __intp.Free(ptr_of_this_method);

                ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
                System.String @format = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                __intp.Free(ptr_of_this_method);

                string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                string log = string.Format(@format, @args) + "\n" + stackTrace;

                Lib.Core.DebugUtil.Log(@logType, log);
            }

            return __ret;
        }
    }
}
