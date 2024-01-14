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
    unsafe class CP_Toggle_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CP_Toggle);
            args = new Type[]{};
            method = type.GetMethod("get_IsOn", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsOn_0);
            args = new Type[]{typeof(System.Boolean), typeof(System.Boolean)};
            method = type.GetMethod("SetSelected", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetSelected_1);
            args = new Type[]{typeof(System.Func<System.Boolean>)};
            method = type.GetMethod("RegisterCondition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RegisterCondition_2);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("SetToggleIsNotChange", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetToggleIsNotChange_3);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("Highlight", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Highlight_4);
            args = new Type[]{};
            method = type.GetMethod("UnRegisterCondition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UnRegisterCondition_5);

            field = type.GetField("onValueChanged", flag);
            app.RegisterCLRFieldGetter(field, get_onValueChanged_0);
            app.RegisterCLRFieldSetter(field, set_onValueChanged_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onValueChanged_0, AssignFromStack_onValueChanged_0);
            field = type.GetField("id", flag);
            app.RegisterCLRFieldGetter(field, get_id_1);
            app.RegisterCLRFieldSetter(field, set_id_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_id_1, AssignFromStack_id_1);
            field = type.GetField("ownerRegistry", flag);
            app.RegisterCLRFieldGetter(field, get_ownerRegistry_2);
            app.RegisterCLRFieldSetter(field, set_ownerRegistry_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_ownerRegistry_2, AssignFromStack_ownerRegistry_2);
            field = type.GetField("toggleType", flag);
            app.RegisterCLRFieldGetter(field, get_toggleType_3);
            app.RegisterCLRFieldSetter(field, set_toggleType_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_toggleType_3, AssignFromStack_toggleType_3);


        }


        static StackObject* get_IsOn_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_Toggle instance_of_this_method = (global::CP_Toggle)typeof(global::CP_Toggle).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsOn;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* SetSelected_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @sendMessage = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::CP_Toggle instance_of_this_method = (global::CP_Toggle)typeof(global::CP_Toggle).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetSelected(@value, @sendMessage);

            return __ret;
        }

        static StackObject* RegisterCondition_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Func<System.Boolean> @_condition = (System.Func<System.Boolean>)typeof(System.Func<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_Toggle instance_of_this_method = (global::CP_Toggle)typeof(global::CP_Toggle).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RegisterCondition(@_condition);

            return __ret;
        }

        static StackObject* SetToggleIsNotChange_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_Toggle instance_of_this_method = (global::CP_Toggle)typeof(global::CP_Toggle).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetToggleIsNotChange(@value);

            return __ret;
        }

        static StackObject* Highlight_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @highlight = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_Toggle instance_of_this_method = (global::CP_Toggle)typeof(global::CP_Toggle).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Highlight(@highlight);

            return __ret;
        }

        static StackObject* UnRegisterCondition_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_Toggle instance_of_this_method = (global::CP_Toggle)typeof(global::CP_Toggle).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.UnRegisterCondition();

            return __ret;
        }


        static object get_onValueChanged_0(ref object o)
        {
            return ((global::CP_Toggle)o).onValueChanged;
        }

        static StackObject* CopyToStack_onValueChanged_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_Toggle)o).onValueChanged;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onValueChanged_0(ref object o, object v)
        {
            ((global::CP_Toggle)o).onValueChanged = (global::CP_Toggle.ToggleEvent)v;
        }

        static StackObject* AssignFromStack_onValueChanged_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::CP_Toggle.ToggleEvent @onValueChanged = (global::CP_Toggle.ToggleEvent)typeof(global::CP_Toggle.ToggleEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_Toggle)o).onValueChanged = @onValueChanged;
            return ptr_of_this_method;
        }

        static object get_id_1(ref object o)
        {
            return ((global::CP_Toggle)o).id;
        }

        static StackObject* CopyToStack_id_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_Toggle)o).id;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_id_1(ref object o, object v)
        {
            ((global::CP_Toggle)o).id = (System.Int32)v;
        }

        static StackObject* AssignFromStack_id_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @id = ptr_of_this_method->Value;
            ((global::CP_Toggle)o).id = @id;
            return ptr_of_this_method;
        }

        static object get_ownerRegistry_2(ref object o)
        {
            return ((global::CP_Toggle)o).ownerRegistry;
        }

        static StackObject* CopyToStack_ownerRegistry_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_Toggle)o).ownerRegistry;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ownerRegistry_2(ref object o, object v)
        {
            ((global::CP_Toggle)o).ownerRegistry = (global::CP_ToggleRegistry)v;
        }

        static StackObject* AssignFromStack_ownerRegistry_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::CP_ToggleRegistry @ownerRegistry = (global::CP_ToggleRegistry)typeof(global::CP_ToggleRegistry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_Toggle)o).ownerRegistry = @ownerRegistry;
            return ptr_of_this_method;
        }

        static object get_toggleType_3(ref object o)
        {
            return ((global::CP_Toggle)o).toggleType;
        }

        static StackObject* CopyToStack_toggleType_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_Toggle)o).toggleType;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_toggleType_3(ref object o, object v)
        {
            ((global::CP_Toggle)o).toggleType = (global::CP_Toggle.ToggleType)v;
        }

        static StackObject* AssignFromStack_toggleType_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::CP_Toggle.ToggleType @toggleType = (global::CP_Toggle.ToggleType)typeof(global::CP_Toggle.ToggleType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            ((global::CP_Toggle)o).toggleType = @toggleType;
            return ptr_of_this_method;
        }



    }
}
