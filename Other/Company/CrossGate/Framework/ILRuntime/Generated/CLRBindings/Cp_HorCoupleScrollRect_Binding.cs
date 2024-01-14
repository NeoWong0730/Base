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
    unsafe class Cp_HorCoupleScrollRect_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::Cp_HorCoupleScrollRect);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("SetCouple", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetCouple_0);

            field = type.GetField("leftRect", flag);
            app.RegisterCLRFieldGetter(field, get_leftRect_0);
            app.RegisterCLRFieldSetter(field, set_leftRect_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_leftRect_0, AssignFromStack_leftRect_0);
            field = type.GetField("rightGrid", flag);
            app.RegisterCLRFieldGetter(field, get_rightGrid_1);
            app.RegisterCLRFieldSetter(field, set_rightGrid_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_rightGrid_1, AssignFromStack_rightGrid_1);
            field = type.GetField("rightCellWidth", flag);
            app.RegisterCLRFieldGetter(field, get_rightCellWidth_2);
            app.RegisterCLRFieldSetter(field, set_rightCellWidth_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_rightCellWidth_2, AssignFromStack_rightCellWidth_2);


        }


        static StackObject* SetCouple_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @rightWidth = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::Cp_HorCoupleScrollRect instance_of_this_method = (global::Cp_HorCoupleScrollRect)typeof(global::Cp_HorCoupleScrollRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetCouple(@rightWidth);

            return __ret;
        }


        static object get_leftRect_0(ref object o)
        {
            return ((global::Cp_HorCoupleScrollRect)o).leftRect;
        }

        static StackObject* CopyToStack_leftRect_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::Cp_HorCoupleScrollRect)o).leftRect;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_leftRect_0(ref object o, object v)
        {
            ((global::Cp_HorCoupleScrollRect)o).leftRect = (UnityEngine.UI.ScrollRect)v;
        }

        static StackObject* AssignFromStack_leftRect_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.UI.ScrollRect @leftRect = (UnityEngine.UI.ScrollRect)typeof(UnityEngine.UI.ScrollRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::Cp_HorCoupleScrollRect)o).leftRect = @leftRect;
            return ptr_of_this_method;
        }

        static object get_rightGrid_1(ref object o)
        {
            return ((global::Cp_HorCoupleScrollRect)o).rightGrid;
        }

        static StackObject* CopyToStack_rightGrid_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::Cp_HorCoupleScrollRect)o).rightGrid;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_rightGrid_1(ref object o, object v)
        {
            ((global::Cp_HorCoupleScrollRect)o).rightGrid = (UnityEngine.UI.GridLayoutGroup)v;
        }

        static StackObject* AssignFromStack_rightGrid_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.UI.GridLayoutGroup @rightGrid = (UnityEngine.UI.GridLayoutGroup)typeof(UnityEngine.UI.GridLayoutGroup).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::Cp_HorCoupleScrollRect)o).rightGrid = @rightGrid;
            return ptr_of_this_method;
        }

        static object get_rightCellWidth_2(ref object o)
        {
            return ((global::Cp_HorCoupleScrollRect)o).rightCellWidth;
        }

        static StackObject* CopyToStack_rightCellWidth_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::Cp_HorCoupleScrollRect)o).rightCellWidth;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_rightCellWidth_2(ref object o, object v)
        {
            ((global::Cp_HorCoupleScrollRect)o).rightCellWidth = (System.Single)v;
        }

        static StackObject* AssignFromStack_rightCellWidth_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @rightCellWidth = *(float*)&ptr_of_this_method->Value;
            ((global::Cp_HorCoupleScrollRect)o).rightCellWidth = @rightCellWidth;
            return ptr_of_this_method;
        }



    }
}
