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
    unsafe class Framework_AttitudeAngleTransform_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.AttitudeAngleTransform);
            args = new Type[]{};
            method = type.GetMethod("get_distance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_distance_0);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_distance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_distance_1);
            args = new Type[]{};
            method = type.GetMethod("get_pith", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_pith_2);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_pith", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_pith_3);
            args = new Type[]{};
            method = type.GetMethod("get_yaw", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_yaw_4);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_yaw", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_yaw_5);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_fov", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_fov_6);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_clipFar", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_clipFar_7);
            args = new Type[]{typeof(UnityEngine.Vector3)};
            method = type.GetMethod("set_lookPointOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_lookPointOffset_8);
            args = new Type[]{};
            method = type.GetMethod("Recalculation", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Recalculation_9);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_roll", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_roll_10);
            args = new Type[]{};
            method = type.GetMethod("get_TargetCamera", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TargetCamera_11);
            args = new Type[]{};
            method = type.GetMethod("get_roll", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_roll_12);
            args = new Type[]{};
            method = type.GetMethod("get_fov", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_fov_13);
            args = new Type[]{};
            method = type.GetMethod("get_clipFar", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_clipFar_14);
            args = new Type[]{};
            method = type.GetMethod("get_lookPointOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_lookPointOffset_15);
            args = new Type[]{typeof(System.Single), typeof(UnityEngine.Vector3), typeof(System.Int32), typeof(System.Single), typeof(System.Boolean)};
            method = type.GetMethod("DoShake", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DoShake_16);
            args = new Type[]{};
            method = type.GetMethod("StopShark", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StopShark_17);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("ForceRecalculation", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ForceRecalculation_18);

            field = type.GetField("autoFollowTarget", flag);
            app.RegisterCLRFieldGetter(field, get_autoFollowTarget_0);
            app.RegisterCLRFieldSetter(field, set_autoFollowTarget_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_autoFollowTarget_0, AssignFromStack_autoFollowTarget_0);
            field = type.GetField("fixedLookPoint", flag);
            app.RegisterCLRFieldGetter(field, get_fixedLookPoint_1);
            app.RegisterCLRFieldSetter(field, set_fixedLookPoint_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_fixedLookPoint_1, AssignFromStack_fixedLookPoint_1);
            field = type.GetField("target", flag);
            app.RegisterCLRFieldGetter(field, get_target_2);
            app.RegisterCLRFieldSetter(field, set_target_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_target_2, AssignFromStack_target_2);


        }


        static StackObject* get_distance_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.distance;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_distance_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.distance = value;

            return __ret;
        }

        static StackObject* get_pith_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.pith;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_pith_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.pith = value;

            return __ret;
        }

        static StackObject* get_yaw_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.yaw;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_yaw_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.yaw = value;

            return __ret;
        }

        static StackObject* set_fov_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.fov = value;

            return __ret;
        }

        static StackObject* set_clipFar_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.clipFar = value;

            return __ret;
        }

        static StackObject* set_lookPointOffset_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector3 @value = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @value, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @value = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.lookPointOffset = value;

            return __ret;
        }

        static StackObject* Recalculation_9(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Recalculation();

            return __ret;
        }

        static StackObject* set_roll_10(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.roll = value;

            return __ret;
        }

        static StackObject* get_TargetCamera_11(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TargetCamera;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_roll_12(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.roll;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_fov_13(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.fov;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_clipFar_14(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.clipFar;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_lookPointOffset_15(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.lookPointOffset;

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static StackObject* DoShake_16(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 6);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @fadeOut = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @randomness = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @vibrato = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            UnityEngine.Vector3 @strength = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @strength, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @strength = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.Single @duration = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DoShake(@duration, @strength, @vibrato, @randomness, @fadeOut);

            return __ret;
        }

        static StackObject* StopShark_17(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.StopShark();

            return __ret;
        }

        static StackObject* ForceRecalculation_18(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @immediately = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.AttitudeAngleTransform instance_of_this_method = (Framework.AttitudeAngleTransform)typeof(Framework.AttitudeAngleTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ForceRecalculation(@immediately);

            return __ret;
        }


        static object get_autoFollowTarget_0(ref object o)
        {
            return ((Framework.AttitudeAngleTransform)o).autoFollowTarget;
        }

        static StackObject* CopyToStack_autoFollowTarget_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.AttitudeAngleTransform)o).autoFollowTarget;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_autoFollowTarget_0(ref object o, object v)
        {
            ((Framework.AttitudeAngleTransform)o).autoFollowTarget = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_autoFollowTarget_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @autoFollowTarget = ptr_of_this_method->Value == 1;
            ((Framework.AttitudeAngleTransform)o).autoFollowTarget = @autoFollowTarget;
            return ptr_of_this_method;
        }

        static object get_fixedLookPoint_1(ref object o)
        {
            return ((Framework.AttitudeAngleTransform)o).fixedLookPoint;
        }

        static StackObject* CopyToStack_fixedLookPoint_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.AttitudeAngleTransform)o).fixedLookPoint;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_fixedLookPoint_1(ref object o, object v)
        {
            ((Framework.AttitudeAngleTransform)o).fixedLookPoint = (UnityEngine.Vector3)v;
        }

        static StackObject* AssignFromStack_fixedLookPoint_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector3 @fixedLookPoint = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @fixedLookPoint, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @fixedLookPoint = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
            }
            ((Framework.AttitudeAngleTransform)o).fixedLookPoint = @fixedLookPoint;
            return ptr_of_this_method;
        }

        static object get_target_2(ref object o)
        {
            return ((Framework.AttitudeAngleTransform)o).target;
        }

        static StackObject* CopyToStack_target_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.AttitudeAngleTransform)o).target;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_target_2(ref object o, object v)
        {
            ((Framework.AttitudeAngleTransform)o).target = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_target_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @target = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((Framework.AttitudeAngleTransform)o).target = @target;
            return ptr_of_this_method;
        }



    }
}
