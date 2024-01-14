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
    unsafe class CP_ScrolCircleList_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CP_ScrolCircleList);
            args = new Type[]{typeof(System.Collections.Generic.List<System.UInt32>), typeof(System.Action<System.UInt32, global::CP_ScrolCircleListItem>), typeof(System.Action<System.UInt32, UnityEngine.GameObject>), typeof(System.Action<global::CP_ScrolCircleListItem, System.Boolean, System.Boolean>)};
            method = type.GetMethod("SetData", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetData_0);
            args = new Type[]{};
            method = type.GetMethod("Clear", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Clear_1);
            args = new Type[]{};
            method = type.GetMethod("GetToCenterOnDiffY", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetToCenterOnDiffY_2);
            args = new Type[]{};
            method = type.GetMethod("GetFirst", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetFirst_3);
            args = new Type[]{typeof(global::CP_ScrolCircleListItem)};
            method = type.GetMethod("GetToCenterOnDiffY", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetToCenterOnDiffY_4);

            field = type.GetField("deQueue", flag);
            app.RegisterCLRFieldGetter(field, get_deQueue_0);
            app.RegisterCLRFieldSetter(field, set_deQueue_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_deQueue_0, AssignFromStack_deQueue_0);
            field = type.GetField("scrollRect", flag);
            app.RegisterCLRFieldGetter(field, get_scrollRect_1);
            app.RegisterCLRFieldSetter(field, set_scrollRect_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_scrollRect_1, AssignFromStack_scrollRect_1);
            field = type.GetField("centerOnChild", flag);
            app.RegisterCLRFieldGetter(field, get_centerOnChild_2);
            app.RegisterCLRFieldSetter(field, set_centerOnChild_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_centerOnChild_2, AssignFromStack_centerOnChild_2);
            field = type.GetField("ySize", flag);
            app.RegisterCLRFieldGetter(field, get_ySize_3);
            app.RegisterCLRFieldSetter(field, set_ySize_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_ySize_3, AssignFromStack_ySize_3);
            field = type.GetField("inCircleItem", flag);
            app.RegisterCLRFieldGetter(field, get_inCircleItem_4);
            app.RegisterCLRFieldSetter(field, set_inCircleItem_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_inCircleItem_4, AssignFromStack_inCircleItem_4);


        }


        static StackObject* SetData_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<global::CP_ScrolCircleListItem, System.Boolean, System.Boolean> @onCircleStateChanged = (System.Action<global::CP_ScrolCircleListItem, System.Boolean, System.Boolean>)typeof(System.Action<global::CP_ScrolCircleListItem, System.Boolean, System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action<System.UInt32, UnityEngine.GameObject> @onRefresh = (System.Action<System.UInt32, UnityEngine.GameObject>)typeof(System.Action<System.UInt32, UnityEngine.GameObject>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Action<System.UInt32, global::CP_ScrolCircleListItem> @onClicked = (System.Action<System.UInt32, global::CP_ScrolCircleListItem>)typeof(System.Action<System.UInt32, global::CP_ScrolCircleListItem>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Collections.Generic.List<System.UInt32> @ids = (System.Collections.Generic.List<System.UInt32>)typeof(System.Collections.Generic.List<System.UInt32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            global::CP_ScrolCircleList instance_of_this_method = (global::CP_ScrolCircleList)typeof(global::CP_ScrolCircleList).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetData(@ids, @onClicked, @onRefresh, @onCircleStateChanged);

            return __ret;
        }

        static StackObject* Clear_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_ScrolCircleList instance_of_this_method = (global::CP_ScrolCircleList)typeof(global::CP_ScrolCircleList).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Clear();

            return __ret;
        }

        static StackObject* GetToCenterOnDiffY_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_ScrolCircleList instance_of_this_method = (global::CP_ScrolCircleList)typeof(global::CP_ScrolCircleList).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetToCenterOnDiffY();

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetFirst_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_ScrolCircleList instance_of_this_method = (global::CP_ScrolCircleList)typeof(global::CP_ScrolCircleList).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetFirst();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetToCenterOnDiffY_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_ScrolCircleListItem @cp = (global::CP_ScrolCircleListItem)typeof(global::CP_ScrolCircleListItem).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_ScrolCircleList instance_of_this_method = (global::CP_ScrolCircleList)typeof(global::CP_ScrolCircleList).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetToCenterOnDiffY(@cp);

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }


        static object get_deQueue_0(ref object o)
        {
            return ((global::CP_ScrolCircleList)o).deQueue;
        }

        static StackObject* CopyToStack_deQueue_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_ScrolCircleList)o).deQueue;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_deQueue_0(ref object o, object v)
        {
            ((global::CP_ScrolCircleList)o).deQueue = (System.Collections.Generic.List<global::Indexer>)v;
        }

        static StackObject* AssignFromStack_deQueue_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::Indexer> @deQueue = (System.Collections.Generic.List<global::Indexer>)typeof(System.Collections.Generic.List<global::Indexer>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_ScrolCircleList)o).deQueue = @deQueue;
            return ptr_of_this_method;
        }

        static object get_scrollRect_1(ref object o)
        {
            return ((global::CP_ScrolCircleList)o).scrollRect;
        }

        static StackObject* CopyToStack_scrollRect_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_ScrolCircleList)o).scrollRect;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_scrollRect_1(ref object o, object v)
        {
            ((global::CP_ScrolCircleList)o).scrollRect = (UnityEngine.UI.CP_ScrollRect)v;
        }

        static StackObject* AssignFromStack_scrollRect_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.UI.CP_ScrollRect @scrollRect = (UnityEngine.UI.CP_ScrollRect)typeof(UnityEngine.UI.CP_ScrollRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_ScrolCircleList)o).scrollRect = @scrollRect;
            return ptr_of_this_method;
        }

        static object get_centerOnChild_2(ref object o)
        {
            return ((global::CP_ScrolCircleList)o).centerOnChild;
        }

        static StackObject* CopyToStack_centerOnChild_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_ScrolCircleList)o).centerOnChild;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_centerOnChild_2(ref object o, object v)
        {
            ((global::CP_ScrolCircleList)o).centerOnChild = (global::CP_VerticalCenterOnChild)v;
        }

        static StackObject* AssignFromStack_centerOnChild_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::CP_VerticalCenterOnChild @centerOnChild = (global::CP_VerticalCenterOnChild)typeof(global::CP_VerticalCenterOnChild).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_ScrolCircleList)o).centerOnChild = @centerOnChild;
            return ptr_of_this_method;
        }

        static object get_ySize_3(ref object o)
        {
            return ((global::CP_ScrolCircleList)o).ySize;
        }

        static StackObject* CopyToStack_ySize_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_ScrolCircleList)o).ySize;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_ySize_3(ref object o, object v)
        {
            ((global::CP_ScrolCircleList)o).ySize = (System.Single)v;
        }

        static StackObject* AssignFromStack_ySize_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @ySize = *(float*)&ptr_of_this_method->Value;
            ((global::CP_ScrolCircleList)o).ySize = @ySize;
            return ptr_of_this_method;
        }

        static object get_inCircleItem_4(ref object o)
        {
            return ((global::CP_ScrolCircleList)o).inCircleItem;
        }

        static StackObject* CopyToStack_inCircleItem_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_ScrolCircleList)o).inCircleItem;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_inCircleItem_4(ref object o, object v)
        {
            ((global::CP_ScrolCircleList)o).inCircleItem = (global::CP_ScrolCircleListItem)v;
        }

        static StackObject* AssignFromStack_inCircleItem_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::CP_ScrolCircleListItem @inCircleItem = (global::CP_ScrolCircleListItem)typeof(global::CP_ScrolCircleListItem).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_ScrolCircleList)o).inCircleItem = @inCircleItem;
            return ptr_of_this_method;
        }



    }
}
