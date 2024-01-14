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
    unsafe class Framework_Table_FCSVFamilyPet_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVFamilyPet.Data);
            args = new Type[]{};
            method = type.GetMethod("get_id", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_id_0);
            args = new Type[]{};
            method = type.GetMethod("get_stage", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_stage_1);
            args = new Type[]{};
            method = type.GetMethod("get_growthValueMax", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_growthValueMax_2);
            args = new Type[]{};
            method = type.GetMethod("get_rotationx", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_rotationx_3);
            args = new Type[]{};
            method = type.GetMethod("get_rotationy", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_rotationy_4);
            args = new Type[]{};
            method = type.GetMethod("get_rotationz", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_rotationz_5);
            args = new Type[]{};
            method = type.GetMethod("get_scale", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_scale_6);
            args = new Type[]{};
            method = type.GetMethod("get_positionx", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_positionx_7);
            args = new Type[]{};
            method = type.GetMethod("get_positiony", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_positiony_8);
            args = new Type[]{};
            method = type.GetMethod("get_positionz", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_positionz_9);
            args = new Type[]{};
            method = type.GetMethod("get_dailyGrowthValueMax", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_dailyGrowthValueMax_10);
            args = new Type[]{};
            method = type.GetMethod("get_trainingIntegralCondition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_trainingIntegralCondition_11);
            args = new Type[]{};
            method = type.GetMethod("get_simpleText", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_simpleText_12);
            args = new Type[]{};
            method = type.GetMethod("get_rewardPreview", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_rewardPreview_13);
            args = new Type[]{};
            method = type.GetMethod("get_diffcultyDetails", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_diffcultyDetails_14);

            field = type.GetField("name", flag);
            app.RegisterCLRFieldGetter(field, get_name_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_name_0, null);
            field = type.GetField("model_show", flag);
            app.RegisterCLRFieldGetter(field, get_model_show_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_model_show_1, null);
            field = type.GetField("icon2_id", flag);
            app.RegisterCLRFieldGetter(field, get_icon2_id_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_icon2_id_2, null);
            field = type.GetField("train_id", flag);
            app.RegisterCLRFieldGetter(field, get_train_id_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_train_id_3, null);


        }


        static StackObject* get_id_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.id;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_stage_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.stage;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_growthValueMax_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.growthValueMax;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_rotationx_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.rotationx;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_rotationy_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.rotationy;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_rotationz_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.rotationz;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_scale_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.scale;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_positionx_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.positionx;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_positiony_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.positiony;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_positionz_9(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.positionz;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_dailyGrowthValueMax_10(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.dailyGrowthValueMax;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_trainingIntegralCondition_11(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.trainingIntegralCondition;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_simpleText_12(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.simpleText;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_rewardPreview_13(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.rewardPreview;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_diffcultyDetails_14(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVFamilyPet.Data instance_of_this_method = (Framework.Table.FCSVFamilyPet.Data)typeof(Framework.Table.FCSVFamilyPet.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.diffcultyDetails;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_name_0(ref object o)
        {
            return ((Framework.Table.FCSVFamilyPet.Data)o).name;
        }

        static StackObject* CopyToStack_name_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFamilyPet.Data)o).name;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_model_show_1(ref object o)
        {
            return ((Framework.Table.FCSVFamilyPet.Data)o).model_show;
        }

        static StackObject* CopyToStack_model_show_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFamilyPet.Data)o).model_show;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_icon2_id_2(ref object o)
        {
            return ((Framework.Table.FCSVFamilyPet.Data)o).icon2_id;
        }

        static StackObject* CopyToStack_icon2_id_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFamilyPet.Data)o).icon2_id;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_train_id_3(ref object o)
        {
            return ((Framework.Table.FCSVFamilyPet.Data)o).train_id;
        }

        static StackObject* CopyToStack_train_id_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVFamilyPet.Data)o).train_id;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
