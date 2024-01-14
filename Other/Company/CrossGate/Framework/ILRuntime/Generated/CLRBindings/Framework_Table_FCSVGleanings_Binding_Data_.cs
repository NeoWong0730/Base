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
    unsafe class Framework_Table_FCSVGleanings_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVGleanings.Data);
            args = new Type[]{};
            method = type.GetMethod("get_SubTypeName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_SubTypeName_0);
            args = new Type[]{};
            method = type.GetMethod("get_TypeName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TypeName_1);
            args = new Type[]{};
            method = type.GetMethod("get_id", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_id_2);
            args = new Type[]{};
            method = type.GetMethod("get_Source", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Source_3);

            field = type.GetField("show_image2", flag);
            app.RegisterCLRFieldGetter(field, get_show_image2_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_show_image2_0, null);
            field = type.GetField("show_image", flag);
            app.RegisterCLRFieldGetter(field, get_show_image_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_show_image_1, null);
            field = type.GetField("icon_id", flag);
            app.RegisterCLRFieldGetter(field, get_icon_id_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_icon_id_2, null);


        }


        static StackObject* get_SubTypeName_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGleanings.Data instance_of_this_method = (Framework.Table.FCSVGleanings.Data)typeof(Framework.Table.FCSVGleanings.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.SubTypeName;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_TypeName_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGleanings.Data instance_of_this_method = (Framework.Table.FCSVGleanings.Data)typeof(Framework.Table.FCSVGleanings.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TypeName;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_id_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGleanings.Data instance_of_this_method = (Framework.Table.FCSVGleanings.Data)typeof(Framework.Table.FCSVGleanings.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.id;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_Source_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVGleanings.Data instance_of_this_method = (Framework.Table.FCSVGleanings.Data)typeof(Framework.Table.FCSVGleanings.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Source;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_show_image2_0(ref object o)
        {
            return ((Framework.Table.FCSVGleanings.Data)o).show_image2;
        }

        static StackObject* CopyToStack_show_image2_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGleanings.Data)o).show_image2;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_show_image_1(ref object o)
        {
            return ((Framework.Table.FCSVGleanings.Data)o).show_image;
        }

        static StackObject* CopyToStack_show_image_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGleanings.Data)o).show_image;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_icon_id_2(ref object o)
        {
            return ((Framework.Table.FCSVGleanings.Data)o).icon_id;
        }

        static StackObject* CopyToStack_icon_id_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVGleanings.Data)o).icon_id;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
