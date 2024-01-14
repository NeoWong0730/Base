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
    unsafe class GME_ITMGPTT_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(GME.ITMGPTT);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetSpeakerVolume", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetSpeakerVolume_0);
            args = new Type[]{typeof(System.Byte[])};
            method = type.GetMethod("ApplyPTTAuthbuffer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ApplyPTTAuthbuffer_1);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetMaxMessageLength", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetMaxMessageLength_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetMicVolume", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetMicVolume_3);
            args = new Type[]{typeof(GME.QAVRecordFileCompleteCallback)};
            method = type.GetMethod("add_OnRecordFileComplete", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, add_OnRecordFileComplete_4);
            args = new Type[]{typeof(GME.QAVUploadFileCompleteCallback)};
            method = type.GetMethod("add_OnUploadFileComplete", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, add_OnUploadFileComplete_5);
            args = new Type[]{typeof(GME.QAVDownloadFileCompleteCallback)};
            method = type.GetMethod("add_OnDownloadFileComplete", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, add_OnDownloadFileComplete_6);
            args = new Type[]{typeof(GME.QAVPlayFileCompleteCallback)};
            method = type.GetMethod("add_OnPlayFileComplete", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, add_OnPlayFileComplete_7);
            args = new Type[]{typeof(GME.QAVSpeechToTextWithAuditCallback)};
            method = type.GetMethod("add_OnSpeechToTextAuditComplete", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, add_OnSpeechToTextAuditComplete_8);
            args = new Type[]{typeof(GME.QAVRecordFileCompleteCallback)};
            method = type.GetMethod("remove_OnRecordFileComplete", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, remove_OnRecordFileComplete_9);
            args = new Type[]{typeof(GME.QAVUploadFileCompleteCallback)};
            method = type.GetMethod("remove_OnUploadFileComplete", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, remove_OnUploadFileComplete_10);
            args = new Type[]{typeof(GME.QAVDownloadFileCompleteCallback)};
            method = type.GetMethod("remove_OnDownloadFileComplete", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, remove_OnDownloadFileComplete_11);
            args = new Type[]{typeof(GME.QAVPlayFileCompleteCallback)};
            method = type.GetMethod("remove_OnPlayFileComplete", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, remove_OnPlayFileComplete_12);
            args = new Type[]{typeof(GME.QAVSpeechToTextWithAuditCallback)};
            method = type.GetMethod("remove_OnSpeechToTextAuditComplete", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, remove_OnSpeechToTextAuditComplete_13);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("StartRecording", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StartRecording_14);
            args = new Type[]{};
            method = type.GetMethod("CancelRecording", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CancelRecording_15);
            args = new Type[]{};
            method = type.GetMethod("StopRecording", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StopRecording_16);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("UploadRecordedFile", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UploadRecordedFile_17);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("GetVoiceFileDuration", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetVoiceFileDuration_18);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("PlayRecordedFile", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlayRecordedFile_19);
            args = new Type[]{typeof(System.String), typeof(System.String), typeof(System.String)};
            method = type.GetMethod("SpeechToText", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SpeechToText_20);
            args = new Type[]{};
            method = type.GetMethod("StopPlayFile", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StopPlayFile_21);
            args = new Type[]{typeof(System.String), typeof(System.String)};
            method = type.GetMethod("DownloadRecordedFile", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DownloadRecordedFile_22);


        }


        static StackObject* SetSpeakerVolume_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @volume = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.SetSpeakerVolume(@volume);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ApplyPTTAuthbuffer_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte[] @authBuffer = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ApplyPTTAuthbuffer(@authBuffer);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* SetMaxMessageLength_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @msTime = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.SetMaxMessageLength(@msTime);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* SetMicVolume_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @volume = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.SetMicVolume(@volume);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* add_OnRecordFileComplete_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVRecordFileCompleteCallback @value = (GME.QAVRecordFileCompleteCallback)typeof(GME.QAVRecordFileCompleteCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnRecordFileComplete += value;

            return __ret;
        }

        static StackObject* add_OnUploadFileComplete_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVUploadFileCompleteCallback @value = (GME.QAVUploadFileCompleteCallback)typeof(GME.QAVUploadFileCompleteCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnUploadFileComplete += value;

            return __ret;
        }

        static StackObject* add_OnDownloadFileComplete_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVDownloadFileCompleteCallback @value = (GME.QAVDownloadFileCompleteCallback)typeof(GME.QAVDownloadFileCompleteCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnDownloadFileComplete += value;

            return __ret;
        }

        static StackObject* add_OnPlayFileComplete_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVPlayFileCompleteCallback @value = (GME.QAVPlayFileCompleteCallback)typeof(GME.QAVPlayFileCompleteCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnPlayFileComplete += value;

            return __ret;
        }

        static StackObject* add_OnSpeechToTextAuditComplete_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVSpeechToTextWithAuditCallback @value = (GME.QAVSpeechToTextWithAuditCallback)typeof(GME.QAVSpeechToTextWithAuditCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnSpeechToTextAuditComplete += value;

            return __ret;
        }

        static StackObject* remove_OnRecordFileComplete_9(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVRecordFileCompleteCallback @value = (GME.QAVRecordFileCompleteCallback)typeof(GME.QAVRecordFileCompleteCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnRecordFileComplete -= value;

            return __ret;
        }

        static StackObject* remove_OnUploadFileComplete_10(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVUploadFileCompleteCallback @value = (GME.QAVUploadFileCompleteCallback)typeof(GME.QAVUploadFileCompleteCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnUploadFileComplete -= value;

            return __ret;
        }

        static StackObject* remove_OnDownloadFileComplete_11(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVDownloadFileCompleteCallback @value = (GME.QAVDownloadFileCompleteCallback)typeof(GME.QAVDownloadFileCompleteCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnDownloadFileComplete -= value;

            return __ret;
        }

        static StackObject* remove_OnPlayFileComplete_12(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVPlayFileCompleteCallback @value = (GME.QAVPlayFileCompleteCallback)typeof(GME.QAVPlayFileCompleteCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnPlayFileComplete -= value;

            return __ret;
        }

        static StackObject* remove_OnSpeechToTextAuditComplete_13(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVSpeechToTextWithAuditCallback @value = (GME.QAVSpeechToTextWithAuditCallback)typeof(GME.QAVSpeechToTextWithAuditCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnSpeechToTextAuditComplete -= value;

            return __ret;
        }

        static StackObject* StartRecording_14(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @filePath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.StartRecording(@filePath);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* CancelRecording_15(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CancelRecording();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* StopRecording_16(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.StopRecording();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* UploadRecordedFile_17(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @filePath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.UploadRecordedFile(@filePath);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetVoiceFileDuration_18(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @filePath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetVoiceFileDuration(@filePath);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* PlayRecordedFile_19(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @filePath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.PlayRecordedFile(@filePath);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* SpeechToText_20(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @translatelanguage = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @speechLanguage = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @fileID = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.SpeechToText(@fileID, @speechLanguage, @translatelanguage);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* StopPlayFile_21(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.StopPlayFile();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* DownloadRecordedFile_22(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @downloadFilePath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @fileID = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            GME.ITMGPTT instance_of_this_method = (GME.ITMGPTT)typeof(GME.ITMGPTT).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.DownloadRecordedFile(@fileID, @downloadFilePath);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }



    }
}
