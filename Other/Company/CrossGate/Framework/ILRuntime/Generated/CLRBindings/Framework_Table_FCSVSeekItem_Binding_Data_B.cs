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
    unsafe class Framework_Table_FCSVSeekItem_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVSeekItem.Data);
            args = new Type[]{};
            method = type.GetMethod("get_showItem", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_showItem_0);
            args = new Type[]{};
            method = type.GetMethod("get_taskName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_taskName_1);
            args = new Type[]{};
            method = type.GetMethod("get_taskDescribe", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_taskDescribe_2);
            args = new Type[]{};
            method = type.GetMethod("get_trueTips", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_trueTips_3);
            args = new Type[]{};
            method = type.GetMethod("get_errorTips", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_errorTips_4);
            args = new Type[]{};
            method = type.GetMethod("get_effectType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_effectType_5);
            args = new Type[]{};
            method = type.GetMethod("get_spaceTips", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_spaceTips_6);

            field = type.GetField("camera", flag);
            app.RegisterCLRFieldGetter(field, get_camera_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_camera_0, null);
            field = type.GetField("dialogueParameter", flag);
            app.RegisterCLRFieldGetter(field, get_dialogueParameter_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_dialogueParameter_1, null);
            field = type.GetField("consultPosition", flag);
            app.RegisterCLRFieldGetter(field, get_consultPosition_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_consultPosition_2, null);
            field = type.GetField("showId", flag);
            app.RegisterCLRFieldGetter(field, get_showId_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_showId_3, null);
            field = type.GetField("itemId", flag);
            app.RegisterCLRFieldGetter(field, get_itemId_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_itemId_4, null);
            field = type.GetField("showSeat", flag);
            app.RegisterCLRFieldGetter(field, get_showSeat_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_showSeat_5, null);
            field = type.GetField("showRotate", flag);
            app.RegisterCLRFieldGetter(field, get_showRotate_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_showRotate_6, null);
            field = type.GetField("showScale", flag);
            app.RegisterCLRFieldGetter(field, get_showScale_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_showScale_7, null);


        }


        static StackObject* get_showItem_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSeekItem.Data instance_of_this_method = (Framework.Table.FCSVSeekItem.Data)typeof(Framework.Table.FCSVSeekItem.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.showItem;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_taskName_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSeekItem.Data instance_of_this_method = (Framework.Table.FCSVSeekItem.Data)typeof(Framework.Table.FCSVSeekItem.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.taskName;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_taskDescribe_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSeekItem.Data instance_of_this_method = (Framework.Table.FCSVSeekItem.Data)typeof(Framework.Table.FCSVSeekItem.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.taskDescribe;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_trueTips_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSeekItem.Data instance_of_this_method = (Framework.Table.FCSVSeekItem.Data)typeof(Framework.Table.FCSVSeekItem.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.trueTips;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_errorTips_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSeekItem.Data instance_of_this_method = (Framework.Table.FCSVSeekItem.Data)typeof(Framework.Table.FCSVSeekItem.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.errorTips;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_effectType_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSeekItem.Data instance_of_this_method = (Framework.Table.FCSVSeekItem.Data)typeof(Framework.Table.FCSVSeekItem.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.effectType;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_spaceTips_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSeekItem.Data instance_of_this_method = (Framework.Table.FCSVSeekItem.Data)typeof(Framework.Table.FCSVSeekItem.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.spaceTips;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_camera_0(ref object o)
        {
            return ((Framework.Table.FCSVSeekItem.Data)o).camera;
        }

        static StackObject* CopyToStack_camera_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSeekItem.Data)o).camera;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_dialogueParameter_1(ref object o)
        {
            return ((Framework.Table.FCSVSeekItem.Data)o).dialogueParameter;
        }

        static StackObject* CopyToStack_dialogueParameter_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSeekItem.Data)o).dialogueParameter;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_consultPosition_2(ref object o)
        {
            return ((Framework.Table.FCSVSeekItem.Data)o).consultPosition;
        }

        static StackObject* CopyToStack_consultPosition_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSeekItem.Data)o).consultPosition;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_showId_3(ref object o)
        {
            return ((Framework.Table.FCSVSeekItem.Data)o).showId;
        }

        static StackObject* CopyToStack_showId_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSeekItem.Data)o).showId;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_itemId_4(ref object o)
        {
            return ((Framework.Table.FCSVSeekItem.Data)o).itemId;
        }

        static StackObject* CopyToStack_itemId_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSeekItem.Data)o).itemId;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_showSeat_5(ref object o)
        {
            return ((Framework.Table.FCSVSeekItem.Data)o).showSeat;
        }

        static StackObject* CopyToStack_showSeat_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSeekItem.Data)o).showSeat;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_showRotate_6(ref object o)
        {
            return ((Framework.Table.FCSVSeekItem.Data)o).showRotate;
        }

        static StackObject* CopyToStack_showRotate_6(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSeekItem.Data)o).showRotate;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_showScale_7(ref object o)
        {
            return ((Framework.Table.FCSVSeekItem.Data)o).showScale;
        }

        static StackObject* CopyToStack_showScale_7(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSeekItem.Data)o).showScale;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
