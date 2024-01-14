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
    unsafe class CP_PageSwitcher_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CP_PageSwitcher);
            args = new Type[]{};
            method = type.GetMethod("get_PageCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_PageCount_0);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetCount_1);
            args = new Type[]{};
            method = type.GetMethod("get_currentPageIndex", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_currentPageIndex_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetCurrentIndex", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetCurrentIndex_3);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("Exec", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Exec_4);

            field = type.GetField("onExec", flag);
            app.RegisterCLRFieldGetter(field, get_onExec_0);
            app.RegisterCLRFieldSetter(field, set_onExec_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onExec_0, AssignFromStack_onExec_0);
            field = type.GetField("mode", flag);
            app.RegisterCLRFieldGetter(field, get_mode_1);
            app.RegisterCLRFieldSetter(field, set_mode_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_mode_1, AssignFromStack_mode_1);
            field = type.GetField("leftArrow", flag);
            app.RegisterCLRFieldGetter(field, get_leftArrow_2);
            app.RegisterCLRFieldSetter(field, set_leftArrow_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_leftArrow_2, AssignFromStack_leftArrow_2);
            field = type.GetField("rightArrow", flag);
            app.RegisterCLRFieldGetter(field, get_rightArrow_3);
            app.RegisterCLRFieldSetter(field, set_rightArrow_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_rightArrow_3, AssignFromStack_rightArrow_3);


        }


        static StackObject* get_PageCount_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_PageSwitcher instance_of_this_method = (global::CP_PageSwitcher)typeof(global::CP_PageSwitcher).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.PageCount;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* SetCount_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @count = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_PageSwitcher instance_of_this_method = (global::CP_PageSwitcher)typeof(global::CP_PageSwitcher).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.SetCount(@count);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_currentPageIndex_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_PageSwitcher instance_of_this_method = (global::CP_PageSwitcher)typeof(global::CP_PageSwitcher).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.currentPageIndex;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* SetCurrentIndex_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_PageSwitcher instance_of_this_method = (global::CP_PageSwitcher)typeof(global::CP_PageSwitcher).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.SetCurrentIndex(@index);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* Exec_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @sendMessage = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_PageSwitcher instance_of_this_method = (global::CP_PageSwitcher)typeof(global::CP_PageSwitcher).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Exec(@sendMessage);

            return __ret;
        }


        static object get_onExec_0(ref object o)
        {
            return ((global::CP_PageSwitcher)o).onExec;
        }

        static StackObject* CopyToStack_onExec_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_PageSwitcher)o).onExec;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onExec_0(ref object o, object v)
        {
            ((global::CP_PageSwitcher)o).onExec = (System.Action<System.Int32, System.Int32, System.Int32>)v;
        }

        static StackObject* AssignFromStack_onExec_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.Int32, System.Int32, System.Int32> @onExec = (System.Action<System.Int32, System.Int32, System.Int32>)typeof(System.Action<System.Int32, System.Int32, System.Int32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::CP_PageSwitcher)o).onExec = @onExec;
            return ptr_of_this_method;
        }

        static object get_mode_1(ref object o)
        {
            return ((global::CP_PageSwitcher)o).mode;
        }

        static StackObject* CopyToStack_mode_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_PageSwitcher)o).mode;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_mode_1(ref object o, object v)
        {
            ((global::CP_PageSwitcher)o).mode = (global::CP_PageSwitcher.ETravelMode)v;
        }

        static StackObject* AssignFromStack_mode_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::CP_PageSwitcher.ETravelMode @mode = (global::CP_PageSwitcher.ETravelMode)typeof(global::CP_PageSwitcher.ETravelMode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            ((global::CP_PageSwitcher)o).mode = @mode;
            return ptr_of_this_method;
        }

        static object get_leftArrow_2(ref object o)
        {
            return ((global::CP_PageSwitcher)o).leftArrow;
        }

        static StackObject* CopyToStack_leftArrow_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_PageSwitcher)o).leftArrow;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_leftArrow_2(ref object o, object v)
        {
            ((global::CP_PageSwitcher)o).leftArrow = (UnityEngine.UI.Button)v;
        }

        static StackObject* AssignFromStack_leftArrow_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.UI.Button @leftArrow = (UnityEngine.UI.Button)typeof(UnityEngine.UI.Button).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_PageSwitcher)o).leftArrow = @leftArrow;
            return ptr_of_this_method;
        }

        static object get_rightArrow_3(ref object o)
        {
            return ((global::CP_PageSwitcher)o).rightArrow;
        }

        static StackObject* CopyToStack_rightArrow_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_PageSwitcher)o).rightArrow;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_rightArrow_3(ref object o, object v)
        {
            ((global::CP_PageSwitcher)o).rightArrow = (UnityEngine.UI.Button)v;
        }

        static StackObject* AssignFromStack_rightArrow_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.UI.Button @rightArrow = (UnityEngine.UI.Button)typeof(UnityEngine.UI.Button).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_PageSwitcher)o).rightArrow = @rightArrow;
            return ptr_of_this_method;
        }



    }
}
