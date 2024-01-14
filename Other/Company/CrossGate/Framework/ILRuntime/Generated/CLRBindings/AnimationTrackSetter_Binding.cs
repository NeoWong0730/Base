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
    unsafe class AnimationTrackSetter_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::AnimationTrackSetter);
            args = new Type[]{};
            method = type.GetMethod("Recovery", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Recovery_0);
            args = new Type[]{};
            method = type.GetMethod("Restore", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Restore_1);
            args = new Type[]{typeof(System.Int32), typeof(UnityEngine.AnimationClip), typeof(System.Boolean)};
            method = type.GetMethod("SetClip", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetClip_2);

            field = type.GetField("clips", flag);
            app.RegisterCLRFieldGetter(field, get_clips_0);
            app.RegisterCLRFieldSetter(field, set_clips_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_clips_0, AssignFromStack_clips_0);


        }


        static StackObject* Recovery_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::AnimationTrackSetter instance_of_this_method = (global::AnimationTrackSetter)typeof(global::AnimationTrackSetter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Recovery();

            return __ret;
        }

        static StackObject* Restore_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::AnimationTrackSetter instance_of_this_method = (global::AnimationTrackSetter)typeof(global::AnimationTrackSetter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Restore();

            return __ret;
        }

        static StackObject* SetClip_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @includeSpeedMultify = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.AnimationClip @targetClip = (UnityEngine.AnimationClip)typeof(UnityEngine.AnimationClip).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @clipIndex = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            global::AnimationTrackSetter instance_of_this_method = (global::AnimationTrackSetter)typeof(global::AnimationTrackSetter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetClip(@clipIndex, @targetClip, @includeSpeedMultify);

            return __ret;
        }


        static object get_clips_0(ref object o)
        {
            return ((global::AnimationTrackSetter)o).clips;
        }

        static StackObject* CopyToStack_clips_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::AnimationTrackSetter)o).clips;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_clips_0(ref object o, object v)
        {
            ((global::AnimationTrackSetter)o).clips = (UnityEngine.Timeline.TimelineClip[])v;
        }

        static StackObject* AssignFromStack_clips_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Timeline.TimelineClip[] @clips = (UnityEngine.Timeline.TimelineClip[])typeof(UnityEngine.Timeline.TimelineClip[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::AnimationTrackSetter)o).clips = @clips;
            return ptr_of_this_method;
        }



    }
}
