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
    unsafe class HUDPositionCorrect_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::HUDPositionCorrect);
            args = new Type[]{typeof(UnityEngine.Vector3)};
            method = type.GetMethod("SetbaseOffest", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetbaseOffest_0);
            args = new Type[]{typeof(UnityEngine.Transform)};
            method = type.GetMethod("SetTarget", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetTarget_1);
            args = new Type[]{};
            method = type.GetMethod("CalPos_Trans", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CalPos_Trans_2);
            args = new Type[]{typeof(UnityEngine.Vector3)};
            method = type.GetMethod("CalPos_Vec", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CalPos_Vec_3);
            args = new Type[]{};
            method = type.GetMethod("Dispose", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Dispose_4);
            args = new Type[]{typeof(UnityEngine.Vector3)};
            method = type.GetMethod("CalOffest", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CalOffest_5);
            args = new Type[]{typeof(UnityEngine.Vector2)};
            method = type.GetMethod("SetuiRootOffest", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetuiRootOffest_6);
            args = new Type[]{typeof(UnityEngine.Camera)};
            method = type.GetMethod("SetCamera", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetCamera_7);

            field = type.GetField("NeedCorrectAtSkillPlay", flag);
            app.RegisterCLRFieldGetter(field, get_NeedCorrectAtSkillPlay_0);
            app.RegisterCLRFieldSetter(field, set_NeedCorrectAtSkillPlay_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_NeedCorrectAtSkillPlay_0, AssignFromStack_NeedCorrectAtSkillPlay_0);
            field = type.GetField("NeedFollow", flag);
            app.RegisterCLRFieldGetter(field, get_NeedFollow_1);
            app.RegisterCLRFieldSetter(field, set_NeedFollow_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_NeedFollow_1, AssignFromStack_NeedFollow_1);


        }


        static StackObject* SetbaseOffest_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector3 @baseoffest = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @baseoffest, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @baseoffest = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::HUDPositionCorrect instance_of_this_method = (global::HUDPositionCorrect)typeof(global::HUDPositionCorrect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetbaseOffest(@baseoffest);

            return __ret;
        }

        static StackObject* SetTarget_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Transform @transform = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::HUDPositionCorrect instance_of_this_method = (global::HUDPositionCorrect)typeof(global::HUDPositionCorrect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetTarget(@transform);

            return __ret;
        }

        static StackObject* CalPos_Trans_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::HUDPositionCorrect instance_of_this_method = (global::HUDPositionCorrect)typeof(global::HUDPositionCorrect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CalPos_Trans();

            return __ret;
        }

        static StackObject* CalPos_Vec_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector3 @vector3 = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @vector3, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @vector3 = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::HUDPositionCorrect instance_of_this_method = (global::HUDPositionCorrect)typeof(global::HUDPositionCorrect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CalPos_Vec(@vector3);

            return __ret;
        }

        static StackObject* Dispose_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::HUDPositionCorrect instance_of_this_method = (global::HUDPositionCorrect)typeof(global::HUDPositionCorrect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Dispose();

            return __ret;
        }

        static StackObject* CalOffest_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector3 @_offest = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @_offest, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @_offest = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::HUDPositionCorrect instance_of_this_method = (global::HUDPositionCorrect)typeof(global::HUDPositionCorrect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CalOffest(@_offest);

            return __ret;
        }

        static StackObject* SetuiRootOffest_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector2 @uiRootOffest = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @uiRootOffest, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @uiRootOffest = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::HUDPositionCorrect instance_of_this_method = (global::HUDPositionCorrect)typeof(global::HUDPositionCorrect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetuiRootOffest(@uiRootOffest);

            return __ret;
        }

        static StackObject* SetCamera_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Camera @camera = (UnityEngine.Camera)typeof(UnityEngine.Camera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::HUDPositionCorrect instance_of_this_method = (global::HUDPositionCorrect)typeof(global::HUDPositionCorrect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetCamera(@camera);

            return __ret;
        }


        static object get_NeedCorrectAtSkillPlay_0(ref object o)
        {
            return ((global::HUDPositionCorrect)o).NeedCorrectAtSkillPlay;
        }

        static StackObject* CopyToStack_NeedCorrectAtSkillPlay_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HUDPositionCorrect)o).NeedCorrectAtSkillPlay;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_NeedCorrectAtSkillPlay_0(ref object o, object v)
        {
            ((global::HUDPositionCorrect)o).NeedCorrectAtSkillPlay = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_NeedCorrectAtSkillPlay_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @NeedCorrectAtSkillPlay = ptr_of_this_method->Value == 1;
            ((global::HUDPositionCorrect)o).NeedCorrectAtSkillPlay = @NeedCorrectAtSkillPlay;
            return ptr_of_this_method;
        }

        static object get_NeedFollow_1(ref object o)
        {
            return ((global::HUDPositionCorrect)o).NeedFollow;
        }

        static StackObject* CopyToStack_NeedFollow_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::HUDPositionCorrect)o).NeedFollow;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_NeedFollow_1(ref object o, object v)
        {
            ((global::HUDPositionCorrect)o).NeedFollow = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_NeedFollow_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @NeedFollow = ptr_of_this_method->Value == 1;
            ((global::HUDPositionCorrect)o).NeedFollow = @NeedFollow;
            return ptr_of_this_method;
        }



    }
}
