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
    unsafe class CP_ToggleRegistry_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CP_ToggleRegistry);
            args = new Type[]{typeof(System.Int32), typeof(System.Boolean), typeof(System.Boolean)};
            method = type.GetMethod("SwitchTo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SwitchTo_0);
            args = new Type[]{};
            method = type.GetMethod("ClearCondition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ClearCondition_1);
            args = new Type[]{typeof(System.Int32), typeof(System.Func<System.Boolean>)};
            method = type.GetMethod("AddCondition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddCondition_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetHighLight", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetHighLight_3);
            args = new Type[]{};
            method = type.GetMethod("get_toggles", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_toggles_4);
            args = new Type[]{};
            method = type.GetMethod("get_currentToggleID", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_currentToggleID_5);
            args = new Type[]{typeof(global::CP_Toggle)};
            method = type.GetMethod("RegisterToggle", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RegisterToggle_6);

            field = type.GetField("onToggleChange", flag);
            app.RegisterCLRFieldGetter(field, get_onToggleChange_0);
            app.RegisterCLRFieldSetter(field, set_onToggleChange_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onToggleChange_0, AssignFromStack_onToggleChange_0);
            field = type.GetField("allowSwitchOff", flag);
            app.RegisterCLRFieldGetter(field, get_allowSwitchOff_1);
            app.RegisterCLRFieldSetter(field, set_allowSwitchOff_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_allowSwitchOff_1, AssignFromStack_allowSwitchOff_1);


        }


        static StackObject* SwitchTo_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @sendMessage = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @ignoreOther = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @id = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            global::CP_ToggleRegistry instance_of_this_method = (global::CP_ToggleRegistry)typeof(global::CP_ToggleRegistry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SwitchTo(@id, @ignoreOther, @sendMessage);

            return __ret;
        }

        static StackObject* ClearCondition_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_ToggleRegistry instance_of_this_method = (global::CP_ToggleRegistry)typeof(global::CP_ToggleRegistry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ClearCondition();

            return __ret;
        }

        static StackObject* AddCondition_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Func<System.Boolean> @condition = (System.Func<System.Boolean>)typeof(System.Func<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @id = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::CP_ToggleRegistry instance_of_this_method = (global::CP_ToggleRegistry)typeof(global::CP_ToggleRegistry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddCondition(@id, @condition);

            return __ret;
        }

        static StackObject* SetHighLight_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @id = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_ToggleRegistry instance_of_this_method = (global::CP_ToggleRegistry)typeof(global::CP_ToggleRegistry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetHighLight(@id);

            return __ret;
        }

        static StackObject* get_toggles_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_ToggleRegistry instance_of_this_method = (global::CP_ToggleRegistry)typeof(global::CP_ToggleRegistry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.toggles;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_currentToggleID_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_ToggleRegistry instance_of_this_method = (global::CP_ToggleRegistry)typeof(global::CP_ToggleRegistry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.currentToggleID;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* RegisterToggle_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_Toggle @toggle = (global::CP_Toggle)typeof(global::CP_Toggle).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_ToggleRegistry instance_of_this_method = (global::CP_ToggleRegistry)typeof(global::CP_ToggleRegistry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RegisterToggle(@toggle);

            return __ret;
        }


        static object get_onToggleChange_0(ref object o)
        {
            return ((global::CP_ToggleRegistry)o).onToggleChange;
        }

        static StackObject* CopyToStack_onToggleChange_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_ToggleRegistry)o).onToggleChange;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onToggleChange_0(ref object o, object v)
        {
            ((global::CP_ToggleRegistry)o).onToggleChange = (System.Action<System.Int32, System.Int32>)v;
        }

        static StackObject* AssignFromStack_onToggleChange_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.Int32, System.Int32> @onToggleChange = (System.Action<System.Int32, System.Int32>)typeof(System.Action<System.Int32, System.Int32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::CP_ToggleRegistry)o).onToggleChange = @onToggleChange;
            return ptr_of_this_method;
        }

        static object get_allowSwitchOff_1(ref object o)
        {
            return ((global::CP_ToggleRegistry)o).allowSwitchOff;
        }

        static StackObject* CopyToStack_allowSwitchOff_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_ToggleRegistry)o).allowSwitchOff;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_allowSwitchOff_1(ref object o, object v)
        {
            ((global::CP_ToggleRegistry)o).allowSwitchOff = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_allowSwitchOff_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @allowSwitchOff = ptr_of_this_method->Value == 1;
            ((global::CP_ToggleRegistry)o).allowSwitchOff = @allowSwitchOff;
            return ptr_of_this_method;
        }



    }
}
