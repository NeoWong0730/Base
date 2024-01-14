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
    unsafe class NavMeshAngentMoveBy_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::NavMeshAngentMoveBy);
            args = new Type[]{typeof(System.Single), typeof(System.Single), typeof(UnityEngine.Transform), typeof(System.Func<System.Boolean>), typeof(System.Action), typeof(System.Action)};
            method = type.GetMethod("SetTarget", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetTarget_0);

            field = type.GetField("Index", flag);
            app.RegisterCLRFieldGetter(field, get_Index_0);
            app.RegisterCLRFieldSetter(field, set_Index_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Index_0, AssignFromStack_Index_0);
            field = type.GetField("Velocity", flag);
            app.RegisterCLRFieldGetter(field, get_Velocity_1);
            app.RegisterCLRFieldSetter(field, set_Velocity_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_Velocity_1, AssignFromStack_Velocity_1);
            field = type.GetField("IsHaveTarget", flag);
            app.RegisterCLRFieldGetter(field, get_IsHaveTarget_2);
            app.RegisterCLRFieldSetter(field, set_IsHaveTarget_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_IsHaveTarget_2, AssignFromStack_IsHaveTarget_2);


        }


        static StackObject* SetTarget_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 7);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @stopFollow = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action @onFollow = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Func<System.Boolean> @canFollow = (System.Func<System.Boolean>)typeof(System.Func<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            UnityEngine.Transform @targettrans = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.Single @keepdistance = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            System.Single @speed = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 7);
            global::NavMeshAngentMoveBy instance_of_this_method = (global::NavMeshAngentMoveBy)typeof(global::NavMeshAngentMoveBy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetTarget(@speed, @keepdistance, @targettrans, @canFollow, @onFollow, @stopFollow);

            return __ret;
        }


        static object get_Index_0(ref object o)
        {
            return ((global::NavMeshAngentMoveBy)o).Index;
        }

        static StackObject* CopyToStack_Index_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::NavMeshAngentMoveBy)o).Index;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_Index_0(ref object o, object v)
        {
            ((global::NavMeshAngentMoveBy)o).Index = (System.Int32)v;
        }

        static StackObject* AssignFromStack_Index_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @Index = ptr_of_this_method->Value;
            ((global::NavMeshAngentMoveBy)o).Index = @Index;
            return ptr_of_this_method;
        }

        static object get_Velocity_1(ref object o)
        {
            return ((global::NavMeshAngentMoveBy)o).Velocity;
        }

        static StackObject* CopyToStack_Velocity_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::NavMeshAngentMoveBy)o).Velocity;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_Velocity_1(ref object o, object v)
        {
            ((global::NavMeshAngentMoveBy)o).Velocity = (UnityEngine.Vector3)v;
        }

        static StackObject* AssignFromStack_Velocity_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector3 @Velocity = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @Velocity, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @Velocity = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
            }
            ((global::NavMeshAngentMoveBy)o).Velocity = @Velocity;
            return ptr_of_this_method;
        }

        static object get_IsHaveTarget_2(ref object o)
        {
            return ((global::NavMeshAngentMoveBy)o).IsHaveTarget;
        }

        static StackObject* CopyToStack_IsHaveTarget_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::NavMeshAngentMoveBy)o).IsHaveTarget;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_IsHaveTarget_2(ref object o, object v)
        {
            ((global::NavMeshAngentMoveBy)o).IsHaveTarget = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_IsHaveTarget_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @IsHaveTarget = ptr_of_this_method->Value == 1;
            ((global::NavMeshAngentMoveBy)o).IsHaveTarget = @IsHaveTarget;
            return ptr_of_this_method;
        }



    }
}
