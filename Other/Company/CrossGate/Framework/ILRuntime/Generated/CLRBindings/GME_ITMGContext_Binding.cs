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
    unsafe class GME_ITMGContext_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(GME.ITMGContext);
            args = new Type[]{};
            method = type.GetMethod("GetInstance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetInstance_0);
            args = new Type[]{};
            method = type.GetMethod("IsRoomEntered", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsRoomEntered_1);
            args = new Type[]{};
            method = type.GetMethod("GetAudioCtrl", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetAudioCtrl_2);
            args = new Type[]{};
            method = type.GetMethod("GetPttCtrl", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetPttCtrl_3);
            args = new Type[]{};
            method = type.GetMethod("CheckMicPermission", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CheckMicPermission_4);
            args = new Type[]{typeof(GME.QAVEnterRoomComplete)};
            method = type.GetMethod("add_OnEnterRoomCompleteEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, add_OnEnterRoomCompleteEvent_5);
            args = new Type[]{typeof(GME.QAVExitRoomComplete)};
            method = type.GetMethod("add_OnExitRoomCompleteEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, add_OnExitRoomCompleteEvent_6);
            args = new Type[]{typeof(GME.QAVEndpointsUpdateInfo)};
            method = type.GetMethod("add_OnEndpointsUpdateInfoEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, add_OnEndpointsUpdateInfoEvent_7);
            args = new Type[]{typeof(GME.QAVEnterRoomComplete)};
            method = type.GetMethod("remove_OnEnterRoomCompleteEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, remove_OnEnterRoomCompleteEvent_8);
            args = new Type[]{typeof(GME.QAVExitRoomComplete)};
            method = type.GetMethod("remove_OnExitRoomCompleteEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, remove_OnExitRoomCompleteEvent_9);
            args = new Type[]{typeof(GME.QAVEndpointsUpdateInfo)};
            method = type.GetMethod("remove_OnEndpointsUpdateInfoEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, remove_OnEndpointsUpdateInfoEvent_10);
            args = new Type[]{typeof(System.String), typeof(GME.ITMGRoomType), typeof(System.Byte[])};
            method = type.GetMethod("EnterRoom", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EnterRoom_11);
            args = new Type[]{};
            method = type.GetMethod("ExitRoom", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ExitRoom_12);

            field = type.GetField("EVENT_ID_ENDPOINT_HAS_AUDIO", flag);
            app.RegisterCLRFieldGetter(field, get_EVENT_ID_ENDPOINT_HAS_AUDIO_0);
            app.RegisterCLRFieldSetter(field, set_EVENT_ID_ENDPOINT_HAS_AUDIO_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_EVENT_ID_ENDPOINT_HAS_AUDIO_0, AssignFromStack_EVENT_ID_ENDPOINT_HAS_AUDIO_0);
            field = type.GetField("EVENT_ID_ENDPOINT_NO_AUDIO", flag);
            app.RegisterCLRFieldGetter(field, get_EVENT_ID_ENDPOINT_NO_AUDIO_1);
            app.RegisterCLRFieldSetter(field, set_EVENT_ID_ENDPOINT_NO_AUDIO_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_EVENT_ID_ENDPOINT_NO_AUDIO_1, AssignFromStack_EVENT_ID_ENDPOINT_NO_AUDIO_1);


        }


        static StackObject* GetInstance_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = GME.ITMGContext.GetInstance();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* IsRoomEntered_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.ITMGContext instance_of_this_method = (GME.ITMGContext)typeof(GME.ITMGContext).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsRoomEntered();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* GetAudioCtrl_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.ITMGContext instance_of_this_method = (GME.ITMGContext)typeof(GME.ITMGContext).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetAudioCtrl();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetPttCtrl_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.ITMGContext instance_of_this_method = (GME.ITMGContext)typeof(GME.ITMGContext).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetPttCtrl();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* CheckMicPermission_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.ITMGContext instance_of_this_method = (GME.ITMGContext)typeof(GME.ITMGContext).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CheckMicPermission();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* add_OnEnterRoomCompleteEvent_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVEnterRoomComplete @value = (GME.QAVEnterRoomComplete)typeof(GME.QAVEnterRoomComplete).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGContext instance_of_this_method = (GME.ITMGContext)typeof(GME.ITMGContext).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnEnterRoomCompleteEvent += value;

            return __ret;
        }

        static StackObject* add_OnExitRoomCompleteEvent_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVExitRoomComplete @value = (GME.QAVExitRoomComplete)typeof(GME.QAVExitRoomComplete).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGContext instance_of_this_method = (GME.ITMGContext)typeof(GME.ITMGContext).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnExitRoomCompleteEvent += value;

            return __ret;
        }

        static StackObject* add_OnEndpointsUpdateInfoEvent_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVEndpointsUpdateInfo @value = (GME.QAVEndpointsUpdateInfo)typeof(GME.QAVEndpointsUpdateInfo).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGContext instance_of_this_method = (GME.ITMGContext)typeof(GME.ITMGContext).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnEndpointsUpdateInfoEvent += value;

            return __ret;
        }

        static StackObject* remove_OnEnterRoomCompleteEvent_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVEnterRoomComplete @value = (GME.QAVEnterRoomComplete)typeof(GME.QAVEnterRoomComplete).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGContext instance_of_this_method = (GME.ITMGContext)typeof(GME.ITMGContext).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnEnterRoomCompleteEvent -= value;

            return __ret;
        }

        static StackObject* remove_OnExitRoomCompleteEvent_9(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVExitRoomComplete @value = (GME.QAVExitRoomComplete)typeof(GME.QAVExitRoomComplete).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGContext instance_of_this_method = (GME.ITMGContext)typeof(GME.ITMGContext).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnExitRoomCompleteEvent -= value;

            return __ret;
        }

        static StackObject* remove_OnEndpointsUpdateInfoEvent_10(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.QAVEndpointsUpdateInfo @value = (GME.QAVEndpointsUpdateInfo)typeof(GME.QAVEndpointsUpdateInfo).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGContext instance_of_this_method = (GME.ITMGContext)typeof(GME.ITMGContext).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnEndpointsUpdateInfoEvent -= value;

            return __ret;
        }

        static StackObject* EnterRoom_11(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte[] @authBuffer = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GME.ITMGRoomType @roomType = (GME.ITMGRoomType)typeof(GME.ITMGRoomType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @roomID = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            GME.ITMGContext instance_of_this_method = (GME.ITMGContext)typeof(GME.ITMGContext).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.EnterRoom(@roomID, @roomType, @authBuffer);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ExitRoom_12(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GME.ITMGContext instance_of_this_method = (GME.ITMGContext)typeof(GME.ITMGContext).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ExitRoom();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }


        static object get_EVENT_ID_ENDPOINT_HAS_AUDIO_0(ref object o)
        {
            return GME.ITMGContext.EVENT_ID_ENDPOINT_HAS_AUDIO;
        }

        static StackObject* CopyToStack_EVENT_ID_ENDPOINT_HAS_AUDIO_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = GME.ITMGContext.EVENT_ID_ENDPOINT_HAS_AUDIO;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_EVENT_ID_ENDPOINT_HAS_AUDIO_0(ref object o, object v)
        {
            GME.ITMGContext.EVENT_ID_ENDPOINT_HAS_AUDIO = (System.Int32)v;
        }

        static StackObject* AssignFromStack_EVENT_ID_ENDPOINT_HAS_AUDIO_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @EVENT_ID_ENDPOINT_HAS_AUDIO = ptr_of_this_method->Value;
            GME.ITMGContext.EVENT_ID_ENDPOINT_HAS_AUDIO = @EVENT_ID_ENDPOINT_HAS_AUDIO;
            return ptr_of_this_method;
        }

        static object get_EVENT_ID_ENDPOINT_NO_AUDIO_1(ref object o)
        {
            return GME.ITMGContext.EVENT_ID_ENDPOINT_NO_AUDIO;
        }

        static StackObject* CopyToStack_EVENT_ID_ENDPOINT_NO_AUDIO_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = GME.ITMGContext.EVENT_ID_ENDPOINT_NO_AUDIO;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_EVENT_ID_ENDPOINT_NO_AUDIO_1(ref object o, object v)
        {
            GME.ITMGContext.EVENT_ID_ENDPOINT_NO_AUDIO = (System.Int32)v;
        }

        static StackObject* AssignFromStack_EVENT_ID_ENDPOINT_NO_AUDIO_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @EVENT_ID_ENDPOINT_NO_AUDIO = ptr_of_this_method->Value;
            GME.ITMGContext.EVENT_ID_ENDPOINT_NO_AUDIO = @EVENT_ID_ENDPOINT_NO_AUDIO;
            return ptr_of_this_method;
        }



    }
}
