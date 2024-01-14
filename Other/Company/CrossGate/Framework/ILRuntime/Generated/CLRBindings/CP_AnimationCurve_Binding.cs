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
    unsafe class CP_AnimationCurve_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CP_AnimationCurve);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("Set", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Set_0);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetPositionIndex", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetPositionIndex_1);

            field = type.GetField("useCurve", flag);
            app.RegisterCLRFieldGetter(field, get_useCurve_0);
            app.RegisterCLRFieldSetter(field, set_useCurve_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_useCurve_0, AssignFromStack_useCurve_0);
            field = type.GetField("onChange", flag);
            app.RegisterCLRFieldGetter(field, get_onChange_1);
            app.RegisterCLRFieldSetter(field, set_onChange_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onChange_1, AssignFromStack_onChange_1);
            field = type.GetField("fadeTime", flag);
            app.RegisterCLRFieldGetter(field, get_fadeTime_2);
            app.RegisterCLRFieldSetter(field, set_fadeTime_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_fadeTime_2, AssignFromStack_fadeTime_2);


        }


        static StackObject* Set_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @set = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_AnimationCurve instance_of_this_method = (global::CP_AnimationCurve)typeof(global::CP_AnimationCurve).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Set(@set);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SetPositionIndex_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_AnimationCurve instance_of_this_method = (global::CP_AnimationCurve)typeof(global::CP_AnimationCurve).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.SetPositionIndex(@index);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_useCurve_0(ref object o)
        {
            return ((global::CP_AnimationCurve)o).useCurve;
        }

        static StackObject* CopyToStack_useCurve_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_AnimationCurve)o).useCurve;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_useCurve_0(ref object o, object v)
        {
            ((global::CP_AnimationCurve)o).useCurve = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_useCurve_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @useCurve = ptr_of_this_method->Value == 1;
            ((global::CP_AnimationCurve)o).useCurve = @useCurve;
            return ptr_of_this_method;
        }

        static object get_onChange_1(ref object o)
        {
            return ((global::CP_AnimationCurve)o).onChange;
        }

        static StackObject* CopyToStack_onChange_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_AnimationCurve)o).onChange;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onChange_1(ref object o, object v)
        {
            ((global::CP_AnimationCurve)o).onChange = (System.Action<System.Single, System.Single, System.Single>)v;
        }

        static StackObject* AssignFromStack_onChange_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.Single, System.Single, System.Single> @onChange = (System.Action<System.Single, System.Single, System.Single>)typeof(System.Action<System.Single, System.Single, System.Single>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::CP_AnimationCurve)o).onChange = @onChange;
            return ptr_of_this_method;
        }

        static object get_fadeTime_2(ref object o)
        {
            return ((global::CP_AnimationCurve)o).fadeTime;
        }

        static StackObject* CopyToStack_fadeTime_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_AnimationCurve)o).fadeTime;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_fadeTime_2(ref object o, object v)
        {
            ((global::CP_AnimationCurve)o).fadeTime = (System.Single)v;
        }

        static StackObject* AssignFromStack_fadeTime_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @fadeTime = *(float*)&ptr_of_this_method->Value;
            ((global::CP_AnimationCurve)o).fadeTime = @fadeTime;
            return ptr_of_this_method;
        }



    }
}
