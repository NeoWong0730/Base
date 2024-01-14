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
    unsafe class Framework_Table_FCSVAdventureLevel_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVAdventureLevel.Data);
            args = new Type[]{};
            method = type.GetMethod("get_exp", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_exp_0);
            args = new Type[]{};
            method = type.GetMethod("get_name", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_name_1);

            field = type.GetField("addPrivilegeAttribute", flag);
            app.RegisterCLRFieldGetter(field, get_addPrivilegeAttribute_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_addPrivilegeAttribute_0, null);
            field = type.GetField("addPrivilege", flag);
            app.RegisterCLRFieldGetter(field, get_addPrivilege_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_addPrivilege_1, null);
            field = type.GetField("icon", flag);
            app.RegisterCLRFieldGetter(field, get_icon_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_icon_2, null);
            field = type.GetField("addAttribute", flag);
            app.RegisterCLRFieldGetter(field, get_addAttribute_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_addAttribute_3, null);
            field = type.GetField("privilegeText", flag);
            app.RegisterCLRFieldGetter(field, get_privilegeText_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_privilegeText_4, null);


        }


        static StackObject* get_exp_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAdventureLevel.Data instance_of_this_method = (Framework.Table.FCSVAdventureLevel.Data)typeof(Framework.Table.FCSVAdventureLevel.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.exp;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_name_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAdventureLevel.Data instance_of_this_method = (Framework.Table.FCSVAdventureLevel.Data)typeof(Framework.Table.FCSVAdventureLevel.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.name;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_addPrivilegeAttribute_0(ref object o)
        {
            return ((Framework.Table.FCSVAdventureLevel.Data)o).addPrivilegeAttribute;
        }

        static StackObject* CopyToStack_addPrivilegeAttribute_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAdventureLevel.Data)o).addPrivilegeAttribute;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_addPrivilege_1(ref object o)
        {
            return ((Framework.Table.FCSVAdventureLevel.Data)o).addPrivilege;
        }

        static StackObject* CopyToStack_addPrivilege_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAdventureLevel.Data)o).addPrivilege;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_icon_2(ref object o)
        {
            return ((Framework.Table.FCSVAdventureLevel.Data)o).icon;
        }

        static StackObject* CopyToStack_icon_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAdventureLevel.Data)o).icon;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_addAttribute_3(ref object o)
        {
            return ((Framework.Table.FCSVAdventureLevel.Data)o).addAttribute;
        }

        static StackObject* CopyToStack_addAttribute_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAdventureLevel.Data)o).addAttribute;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_privilegeText_4(ref object o)
        {
            return ((Framework.Table.FCSVAdventureLevel.Data)o).privilegeText;
        }

        static StackObject* CopyToStack_privilegeText_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAdventureLevel.Data)o).privilegeText;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
