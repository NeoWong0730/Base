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
    unsafe class CameraManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CameraManager);
            args = new Type[]{};
            method = type.GetMethod("get_mCamera", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_mCamera_0);
            args = new Type[]{};
            method = type.GetMethod("GetMainCameraCullingMask", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetMainCameraCullingMask_1);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetMainCameraCullingMask", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetMainCameraCullingMask_2);
            args = new Type[]{typeof(UnityEngine.Vector3), typeof(UnityEngine.Camera), typeof(UnityEngine.Camera)};
            method = type.GetMethod("GetUIPositionByWorldPosition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetUIPositionByWorldPosition_3);
            args = new Type[]{};
            method = type.GetMethod("get_mSkillPlayCamera", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_mSkillPlayCamera_4);
            args = new Type[]{typeof(UnityEngine.GameObject), typeof(UnityEngine.Vector3), typeof(UnityEngine.Camera), typeof(UnityEngine.Camera)};
            method = type.GetMethod("World2UI", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, World2UI_5);
            args = new Type[]{typeof(UnityEngine.GameObject), typeof(UnityEngine.Vector3), typeof(UnityEngine.Camera), typeof(UnityEngine.Camera), typeof(System.Single), typeof(System.Single), typeof(System.Single), typeof(System.Single)};
            method = type.GetMethod("World2UI", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, World2UI_6);
            args = new Type[]{};
            method = type.GetMethod("get_mUICamera", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_mUICamera_7);
            args = new Type[]{typeof(UnityEngine.Camera)};
            method = type.GetMethod("SetSkillPlayCamera", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetSkillPlayCamera_8);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("SetActivePostProcess", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetActivePostProcess_9);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("SetActiveShadow", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetActiveShadow_10);
            args = new Type[]{};
            method = type.GetMethod("ReduceMainCameraQuality", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReduceMainCameraQuality_11);
            args = new Type[]{};
            method = type.GetMethod("CancelReduceMainCameraQuality", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CancelReduceMainCameraQuality_12);
            args = new Type[]{};
            method = type.GetMethod("Hide", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Hide_13);
            args = new Type[]{};
            method = type.GetMethod("CancelHide", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CancelHide_14);

            field = type.GetField("relativePos", flag);
            app.RegisterCLRFieldGetter(field, get_relativePos_0);
            app.RegisterCLRFieldSetter(field, set_relativePos_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_relativePos_0, AssignFromStack_relativePos_0);
            field = type.GetField("b_MatchWitchHeight", flag);
            app.RegisterCLRFieldGetter(field, get_b_MatchWitchHeight_1);
            app.RegisterCLRFieldSetter(field, set_b_MatchWitchHeight_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_b_MatchWitchHeight_1, AssignFromStack_b_MatchWitchHeight_1);
            field = type.GetField("onCameraChange", flag);
            app.RegisterCLRFieldGetter(field, get_onCameraChange_2);
            app.RegisterCLRFieldSetter(field, set_onCameraChange_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_onCameraChange_2, AssignFromStack_onCameraChange_2);


        }


        static StackObject* get_mCamera_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::CameraManager.mCamera;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetMainCameraCullingMask_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::CameraManager.GetMainCameraCullingMask();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* SetMainCameraCullingMask_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @cullingMask = ptr_of_this_method->Value;


            global::CameraManager.SetMainCameraCullingMask(@cullingMask);

            return __ret;
        }

        static StackObject* GetUIPositionByWorldPosition_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Camera @c2 = (UnityEngine.Camera)typeof(UnityEngine.Camera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Camera @c3 = (UnityEngine.Camera)typeof(UnityEngine.Camera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.Vector3 @worldPosition = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @worldPosition, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @worldPosition = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }


            var result_of_this_method = global::CameraManager.GetUIPositionByWorldPosition(@worldPosition, @c3, @c2);

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static StackObject* get_mSkillPlayCamera_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::CameraManager.mSkillPlayCamera;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* World2UI_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Camera @c2 = (UnityEngine.Camera)typeof(UnityEngine.Camera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Camera @c3 = (UnityEngine.Camera)typeof(UnityEngine.Camera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.Vector3 @worldPosition = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @worldPosition, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @worldPosition = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            UnityEngine.GameObject @go = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            global::CameraManager.World2UI(@go, @worldPosition, @c3, @c2);

            return __ret;
        }

        static StackObject* World2UI_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 8);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @yoffest = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @xoffest = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @targetUiheight = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Single @targetUiwidth = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            UnityEngine.Camera @c2 = (UnityEngine.Camera)typeof(UnityEngine.Camera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            UnityEngine.Camera @c3 = (UnityEngine.Camera)typeof(UnityEngine.Camera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 7);
            UnityEngine.Vector3 @worldPosition = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @worldPosition, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @worldPosition = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 8);
            UnityEngine.GameObject @uigo = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            global::CameraManager.World2UI(@uigo, @worldPosition, @c3, @c2, @targetUiwidth, @targetUiheight, @xoffest, @yoffest);

            return __ret;
        }

        static StackObject* get_mUICamera_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::CameraManager.mUICamera;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SetSkillPlayCamera_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Camera @camera = (UnityEngine.Camera)typeof(UnityEngine.Camera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            global::CameraManager.SetSkillPlayCamera(@camera);

            return __ret;
        }

        static StackObject* SetActivePostProcess_9(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @active = ptr_of_this_method->Value == 1;


            global::CameraManager.SetActivePostProcess(@active);

            return __ret;
        }

        static StackObject* SetActiveShadow_10(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @active = ptr_of_this_method->Value == 1;


            global::CameraManager.SetActiveShadow(@active);

            return __ret;
        }

        static StackObject* ReduceMainCameraQuality_11(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::CameraManager.ReduceMainCameraQuality();

            return __ret;
        }

        static StackObject* CancelReduceMainCameraQuality_12(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::CameraManager.CancelReduceMainCameraQuality();

            return __ret;
        }

        static StackObject* Hide_13(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::CameraManager.Hide();

            return __ret;
        }

        static StackObject* CancelHide_14(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::CameraManager.CancelHide();

            return __ret;
        }


        static object get_relativePos_0(ref object o)
        {
            return global::CameraManager.relativePos;
        }

        static StackObject* CopyToStack_relativePos_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::CameraManager.relativePos;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_relativePos_0(ref object o, object v)
        {
            global::CameraManager.relativePos = (UnityEngine.Vector2)v;
        }

        static StackObject* AssignFromStack_relativePos_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector2 @relativePos = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @relativePos, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @relativePos = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
            }
            global::CameraManager.relativePos = @relativePos;
            return ptr_of_this_method;
        }

        static object get_b_MatchWitchHeight_1(ref object o)
        {
            return global::CameraManager.b_MatchWitchHeight;
        }

        static StackObject* CopyToStack_b_MatchWitchHeight_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::CameraManager.b_MatchWitchHeight;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_b_MatchWitchHeight_1(ref object o, object v)
        {
            global::CameraManager.b_MatchWitchHeight = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_b_MatchWitchHeight_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @b_MatchWitchHeight = ptr_of_this_method->Value == 1;
            global::CameraManager.b_MatchWitchHeight = @b_MatchWitchHeight;
            return ptr_of_this_method;
        }

        static object get_onCameraChange_2(ref object o)
        {
            return global::CameraManager.onCameraChange;
        }

        static StackObject* CopyToStack_onCameraChange_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::CameraManager.onCameraChange;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onCameraChange_2(ref object o, object v)
        {
            global::CameraManager.onCameraChange = (System.Action)v;
        }

        static StackObject* AssignFromStack_onCameraChange_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @onCameraChange = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            global::CameraManager.onCameraChange = @onCameraChange;
            return ptr_of_this_method;
        }



    }
}
