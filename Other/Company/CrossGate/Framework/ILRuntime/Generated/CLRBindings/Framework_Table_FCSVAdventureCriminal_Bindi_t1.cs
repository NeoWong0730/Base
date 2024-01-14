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
    unsafe class Framework_Table_FCSVAdventureCriminal_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVAdventureCriminal.Data);
            args = new Type[]{};
            method = type.GetMethod("get_finishTaskId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_finishTaskId_0);
            args = new Type[]{};
            method = type.GetMethod("get_id", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_id_1);
            args = new Type[]{};
            method = type.GetMethod("get_isUrgent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_isUrgent_2);
            args = new Type[]{};
            method = type.GetMethod("get_preTaskId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_preTaskId_3);
            args = new Type[]{};
            method = type.GetMethod("get_acceptTaskId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_acceptTaskId_4);
            args = new Type[]{};
            method = type.GetMethod("get_preLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_preLevel_5);
            args = new Type[]{};
            method = type.GetMethod("get_name", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_name_6);
            args = new Type[]{};
            method = type.GetMethod("get_detailedDes", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_detailedDes_7);
            args = new Type[]{};
            method = type.GetMethod("get_simpleDes", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_simpleDes_8);

            field = type.GetField("image", flag);
            app.RegisterCLRFieldGetter(field, get_image_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_image_0, null);
            field = type.GetField("greyImage", flag);
            app.RegisterCLRFieldGetter(field, get_greyImage_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_greyImage_1, null);
            field = type.GetField("nodeTask", flag);
            app.RegisterCLRFieldGetter(field, get_nodeTask_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_nodeTask_2, null);


        }


        static StackObject* get_finishTaskId_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAdventureCriminal.Data instance_of_this_method = (Framework.Table.FCSVAdventureCriminal.Data)typeof(Framework.Table.FCSVAdventureCriminal.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.finishTaskId;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_id_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAdventureCriminal.Data instance_of_this_method = (Framework.Table.FCSVAdventureCriminal.Data)typeof(Framework.Table.FCSVAdventureCriminal.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.id;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_isUrgent_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAdventureCriminal.Data instance_of_this_method = (Framework.Table.FCSVAdventureCriminal.Data)typeof(Framework.Table.FCSVAdventureCriminal.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.isUrgent;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_preTaskId_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAdventureCriminal.Data instance_of_this_method = (Framework.Table.FCSVAdventureCriminal.Data)typeof(Framework.Table.FCSVAdventureCriminal.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.preTaskId;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_acceptTaskId_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAdventureCriminal.Data instance_of_this_method = (Framework.Table.FCSVAdventureCriminal.Data)typeof(Framework.Table.FCSVAdventureCriminal.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.acceptTaskId;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_preLevel_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAdventureCriminal.Data instance_of_this_method = (Framework.Table.FCSVAdventureCriminal.Data)typeof(Framework.Table.FCSVAdventureCriminal.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.preLevel;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_name_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAdventureCriminal.Data instance_of_this_method = (Framework.Table.FCSVAdventureCriminal.Data)typeof(Framework.Table.FCSVAdventureCriminal.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.name;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_detailedDes_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAdventureCriminal.Data instance_of_this_method = (Framework.Table.FCSVAdventureCriminal.Data)typeof(Framework.Table.FCSVAdventureCriminal.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.detailedDes;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_simpleDes_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVAdventureCriminal.Data instance_of_this_method = (Framework.Table.FCSVAdventureCriminal.Data)typeof(Framework.Table.FCSVAdventureCriminal.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.simpleDes;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_image_0(ref object o)
        {
            return ((Framework.Table.FCSVAdventureCriminal.Data)o).image;
        }

        static StackObject* CopyToStack_image_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAdventureCriminal.Data)o).image;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_greyImage_1(ref object o)
        {
            return ((Framework.Table.FCSVAdventureCriminal.Data)o).greyImage;
        }

        static StackObject* CopyToStack_greyImage_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAdventureCriminal.Data)o).greyImage;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_nodeTask_2(ref object o)
        {
            return ((Framework.Table.FCSVAdventureCriminal.Data)o).nodeTask;
        }

        static StackObject* CopyToStack_nodeTask_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVAdventureCriminal.Data)o).nodeTask;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
