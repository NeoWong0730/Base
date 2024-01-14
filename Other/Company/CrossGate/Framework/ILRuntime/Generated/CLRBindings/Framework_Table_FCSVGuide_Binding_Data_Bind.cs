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
    unsafe class Framework_Table_FCSVGuide_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVGuide.Data);
            args = new Type[]{};
            method = type.GetMethod("get_force", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_force_0);

            field = type.GetField("Location", flag);
            app.RegisterCLRFieldGetter(field, get_Location_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Location_0, null);
            field = type.GetField("prefab_range", flag);
            app.RegisterCLRFieldGetter(field, get_prefab_range_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_prefab_range_1, null);
            field = type.GetField("prefab_path", flag);
            app.RegisterCLRFieldGetter(field, get_prefab_path_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_prefab_path_2, null);
            field = type.GetField("effect", flag);
            app.RegisterCLRFieldGetter(field, get_effect_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_effect_3, null);
            field = type.GetField("Animation_path", flag);
            app.RegisterCLRFieldGetter(field, get_Animation_path_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_Animation_path_4, null);
            field = type.GetField("Motion", flag);
            app.RegisterCLRFieldGetter(field, get_Motion_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_Motion_5, null);


        }


        static StackObject* get_force_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGuide.Data instance_of_this_method = (Framework.Table.FCSVGuide.Data)typeof(Framework.Table.FCSVGuide.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.force;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_Location_0(ref object o)
        {
            return ((Framework.Table.FCSVGuide.Data)o).Location;
        }

        static StackObject* CopyToStack_Location_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuide.Data)o).Location;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_prefab_range_1(ref object o)
        {
            return ((Framework.Table.FCSVGuide.Data)o).prefab_range;
        }

        static StackObject* CopyToStack_prefab_range_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuide.Data)o).prefab_range;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_prefab_path_2(ref object o)
        {
            return ((Framework.Table.FCSVGuide.Data)o).prefab_path;
        }

        static StackObject* CopyToStack_prefab_path_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuide.Data)o).prefab_path;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_effect_3(ref object o)
        {
            return ((Framework.Table.FCSVGuide.Data)o).effect;
        }

        static StackObject* CopyToStack_effect_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuide.Data)o).effect;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Animation_path_4(ref object o)
        {
            return ((Framework.Table.FCSVGuide.Data)o).Animation_path;
        }

        static StackObject* CopyToStack_Animation_path_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuide.Data)o).Animation_path;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Motion_5(ref object o)
        {
            return ((Framework.Table.FCSVGuide.Data)o).Motion;
        }

        static StackObject* CopyToStack_Motion_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGuide.Data)o).Motion;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
