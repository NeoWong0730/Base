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
    unsafe class Framework_Table_FCSVGuideArrow_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVGuideArrow.Data);

            field = type.GetField("arrow_anchors", flag);
            app.RegisterCLRFieldGetter(field, get_arrow_anchors_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_arrow_anchors_0, null);
            field = type.GetField("arrow_pos", flag);
            app.RegisterCLRFieldGetter(field, get_arrow_pos_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_arrow_pos_1, null);
            field = type.GetField("arrow_rotation", flag);
            app.RegisterCLRFieldGetter(field, get_arrow_rotation_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_arrow_rotation_2, null);
            field = type.GetField("arrow_scale", flag);
            app.RegisterCLRFieldGetter(field, get_arrow_scale_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_arrow_scale_3, null);


        }



        static object get_arrow_anchors_0(ref object o)
        {
            return ((Framework.Table.FCSVGuideArrow.Data)o).arrow_anchors;
        }

        static StackObject* CopyToStack_arrow_anchors_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideArrow.Data)o).arrow_anchors;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_arrow_pos_1(ref object o)
        {
            return ((Framework.Table.FCSVGuideArrow.Data)o).arrow_pos;
        }

        static StackObject* CopyToStack_arrow_pos_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideArrow.Data)o).arrow_pos;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_arrow_rotation_2(ref object o)
        {
            return ((Framework.Table.FCSVGuideArrow.Data)o).arrow_rotation;
        }

        static StackObject* CopyToStack_arrow_rotation_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideArrow.Data)o).arrow_rotation;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_arrow_scale_3(ref object o)
        {
            return ((Framework.Table.FCSVGuideArrow.Data)o).arrow_scale;
        }

        static StackObject* CopyToStack_arrow_scale_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuideArrow.Data)o).arrow_scale;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
