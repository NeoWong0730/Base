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
    unsafe class Framework_Table_FCSVGuideTip_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVGuideTip.Data);
            args = new Type[]{};
            method = type.GetMethod("get_Model", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Model_0);

            field = type.GetField("Rotation", flag);
            app.RegisterCLRFieldGetter(field, get_Rotation_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Rotation_0, null);
            field = type.GetField("Scale", flag);
            app.RegisterCLRFieldGetter(field, get_Scale_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_Scale_1, null);
            field = type.GetField("Position", flag);
            app.RegisterCLRFieldGetter(field, get_Position_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_Position_2, null);
            field = type.GetField("tip_anchors", flag);
            app.RegisterCLRFieldGetter(field, get_tip_anchors_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_tip_anchors_3, null);
            field = type.GetField("tip_pos", flag);
            app.RegisterCLRFieldGetter(field, get_tip_pos_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_tip_pos_4, null);
            field = type.GetField("tip_size", flag);
            app.RegisterCLRFieldGetter(field, get_tip_size_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_tip_size_5, null);
            field = type.GetField("Motion", flag);
            app.RegisterCLRFieldGetter(field, get_Motion_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_Motion_6, null);


        }


        static StackObject* get_Model_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGuideTip.Data instance_of_this_method = (Framework.Table.FCSVGuideTip.Data)typeof(Framework.Table.FCSVGuideTip.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Model;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_Rotation_0(ref object o)
        {
            return ((Framework.Table.FCSVGuideTip.Data)o).Rotation;
        }

        static StackObject* CopyToStack_Rotation_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideTip.Data)o).Rotation;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Scale_1(ref object o)
        {
            return ((Framework.Table.FCSVGuideTip.Data)o).Scale;
        }

        static StackObject* CopyToStack_Scale_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideTip.Data)o).Scale;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Position_2(ref object o)
        {
            return ((Framework.Table.FCSVGuideTip.Data)o).Position;
        }

        static StackObject* CopyToStack_Position_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideTip.Data)o).Position;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_tip_anchors_3(ref object o)
        {
            return ((Framework.Table.FCSVGuideTip.Data)o).tip_anchors;
        }

        static StackObject* CopyToStack_tip_anchors_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideTip.Data)o).tip_anchors;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_tip_pos_4(ref object o)
        {
            return ((Framework.Table.FCSVGuideTip.Data)o).tip_pos;
        }

        static StackObject* CopyToStack_tip_pos_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideTip.Data)o).tip_pos;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_tip_size_5(ref object o)
        {
            return ((Framework.Table.FCSVGuideTip.Data)o).tip_size;
        }

        static StackObject* CopyToStack_tip_size_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideTip.Data)o).tip_size;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Motion_6(ref object o)
        {
            return ((Framework.Table.FCSVGuideTip.Data)o).Motion;
        }

        static StackObject* CopyToStack_Motion_6(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideTip.Data)o).Motion;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
