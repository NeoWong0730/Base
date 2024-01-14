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
    unsafe class GME_QAVError_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(GME.QAVError);

            field = type.GetField("AV_ERR_AUTH_FIALD", flag);
            app.RegisterCLRFieldGetter(field, get_AV_ERR_AUTH_FIALD_0);
            app.RegisterCLRFieldSetter(field, set_AV_ERR_AUTH_FIALD_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_AV_ERR_AUTH_FIALD_0, AssignFromStack_AV_ERR_AUTH_FIALD_0);
            field = type.GetField("OK", flag);
            app.RegisterCLRFieldGetter(field, get_OK_1);
            app.RegisterCLRFieldSetter(field, set_OK_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_OK_1, AssignFromStack_OK_1);
            field = type.GetField("ERR_VOICE_RECORD_AUDIO_TOO_SHORT", flag);
            app.RegisterCLRFieldGetter(field, get_ERR_VOICE_RECORD_AUDIO_TOO_SHORT_2);
            app.RegisterCLRFieldSetter(field, set_ERR_VOICE_RECORD_AUDIO_TOO_SHORT_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_ERR_VOICE_RECORD_AUDIO_TOO_SHORT_2, AssignFromStack_ERR_VOICE_RECORD_AUDIO_TOO_SHORT_2);


        }



        static object get_AV_ERR_AUTH_FIALD_0(ref object o)
        {
            return GME.QAVError.AV_ERR_AUTH_FIALD;
        }

        static StackObject* CopyToStack_AV_ERR_AUTH_FIALD_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = GME.QAVError.AV_ERR_AUTH_FIALD;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_AV_ERR_AUTH_FIALD_0(ref object o, object v)
        {
            GME.QAVError.AV_ERR_AUTH_FIALD = (System.Int32)v;
        }

        static StackObject* AssignFromStack_AV_ERR_AUTH_FIALD_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @AV_ERR_AUTH_FIALD = ptr_of_this_method->Value;
            GME.QAVError.AV_ERR_AUTH_FIALD = @AV_ERR_AUTH_FIALD;
            return ptr_of_this_method;
        }

        static object get_OK_1(ref object o)
        {
            return GME.QAVError.OK;
        }

        static StackObject* CopyToStack_OK_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = GME.QAVError.OK;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_OK_1(ref object o, object v)
        {
            GME.QAVError.OK = (System.Int32)v;
        }

        static StackObject* AssignFromStack_OK_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @OK = ptr_of_this_method->Value;
            GME.QAVError.OK = @OK;
            return ptr_of_this_method;
        }

        static object get_ERR_VOICE_RECORD_AUDIO_TOO_SHORT_2(ref object o)
        {
            return GME.QAVError.ERR_VOICE_RECORD_AUDIO_TOO_SHORT;
        }

        static StackObject* CopyToStack_ERR_VOICE_RECORD_AUDIO_TOO_SHORT_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = GME.QAVError.ERR_VOICE_RECORD_AUDIO_TOO_SHORT;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_ERR_VOICE_RECORD_AUDIO_TOO_SHORT_2(ref object o, object v)
        {
            GME.QAVError.ERR_VOICE_RECORD_AUDIO_TOO_SHORT = (System.Int32)v;
        }

        static StackObject* AssignFromStack_ERR_VOICE_RECORD_AUDIO_TOO_SHORT_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @ERR_VOICE_RECORD_AUDIO_TOO_SHORT = ptr_of_this_method->Value;
            GME.QAVError.ERR_VOICE_RECORD_AUDIO_TOO_SHORT = @ERR_VOICE_RECORD_AUDIO_TOO_SHORT;
            return ptr_of_this_method;
        }



    }
}
