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
    unsafe class Framework_Table_FCSVLittleGame_Elimination_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVLittleGame_Elimination.Data);
            args = new Type[]{};
            method = type.GetMethod("get_tips", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_tips_0);
            args = new Type[]{};
            method = type.GetMethod("get_Goaltips", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Goaltips_1);
            args = new Type[]{};
            method = type.GetMethod("get_time", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_time_2);
            args = new Type[]{};
            method = type.GetMethod("get_gameDescribe", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_gameDescribe_3);

            field = type.GetField("imageId", flag);
            app.RegisterCLRFieldGetter(field, get_imageId_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_imageId_0, null);
            field = type.GetField("Goal", flag);
            app.RegisterCLRFieldGetter(field, get_Goal_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_Goal_1, null);
            field = type.GetField("image_path", flag);
            app.RegisterCLRFieldGetter(field, get_image_path_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_image_path_2, null);


        }


        static StackObject* get_tips_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVLittleGame_Elimination.Data instance_of_this_method = (Framework.Table.FCSVLittleGame_Elimination.Data)typeof(Framework.Table.FCSVLittleGame_Elimination.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.tips;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_Goaltips_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVLittleGame_Elimination.Data instance_of_this_method = (Framework.Table.FCSVLittleGame_Elimination.Data)typeof(Framework.Table.FCSVLittleGame_Elimination.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Goaltips;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_time_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVLittleGame_Elimination.Data instance_of_this_method = (Framework.Table.FCSVLittleGame_Elimination.Data)typeof(Framework.Table.FCSVLittleGame_Elimination.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.time;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_gameDescribe_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVLittleGame_Elimination.Data instance_of_this_method = (Framework.Table.FCSVLittleGame_Elimination.Data)typeof(Framework.Table.FCSVLittleGame_Elimination.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.gameDescribe;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_imageId_0(ref object o)
        {
            return ((Framework.Table.FCSVLittleGame_Elimination.Data)o).imageId;
        }

        static StackObject* CopyToStack_imageId_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVLittleGame_Elimination.Data)o).imageId;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_Goal_1(ref object o)
        {
            return ((Framework.Table.FCSVLittleGame_Elimination.Data)o).Goal;
        }

        static StackObject* CopyToStack_Goal_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVLittleGame_Elimination.Data)o).Goal;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_image_path_2(ref object o)
        {
            return ((Framework.Table.FCSVLittleGame_Elimination.Data)o).image_path;
        }

        static StackObject* CopyToStack_image_path_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVLittleGame_Elimination.Data)o).image_path;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
