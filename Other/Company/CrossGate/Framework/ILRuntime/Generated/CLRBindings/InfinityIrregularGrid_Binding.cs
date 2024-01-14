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
    unsafe class InfinityIrregularGrid_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::InfinityIrregularGrid);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetCapacity", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetCapacity_0);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_MinSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_MinSize_1);
            args = new Type[]{};
            method = type.GetMethod("get_ScrollView", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ScrollView_2);
            args = new Type[]{};
            method = type.GetMethod("get_NormalizedPosition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_NormalizedPosition_3);
            args = new Type[]{};
            method = type.GetMethod("Clear", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Clear_4);
            args = new Type[]{};
            method = type.GetMethod("GetCells", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCells_5);
            args = new Type[]{typeof(System.Int32), typeof(System.Boolean)};
            method = type.GetMethod("RemoveTopRange", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RemoveTopRange_6);
            args = new Type[]{};
            method = type.GetMethod("get_CellCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_CellCount_7);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("Add", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Add_8);
            args = new Type[]{typeof(System.Boolean), typeof(System.Single)};
            method = type.GetMethod("SetLockNormalizedPosition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetLockNormalizedPosition_9);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_CellCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_CellCount_10);
            args = new Type[]{};
            method = type.GetMethod("ForceRefreshActiveCell", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ForceRefreshActiveCell_11);
            args = new Type[]{typeof(UnityEngine.Vector2)};
            method = type.GetMethod("set_NormalizedPosition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_NormalizedPosition_12);
            args = new Type[]{};
            method = type.GetMethod("Update", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Update_13);

            field = type.GetField("onCreateCell", flag);
            app.RegisterCLRFieldGetter(field, get_onCreateCell_0);
            app.RegisterCLRFieldSetter(field, set_onCreateCell_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onCreateCell_0, AssignFromStack_onCreateCell_0);
            field = type.GetField("onCellChange", flag);
            app.RegisterCLRFieldGetter(field, get_onCellChange_1);
            app.RegisterCLRFieldSetter(field, set_onCellChange_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onCellChange_1, AssignFromStack_onCellChange_1);


        }


        static StackObject* SetCapacity_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @capacity = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetCapacity(@capacity);

            return __ret;
        }

        static StackObject* set_MinSize_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.MinSize = value;

            return __ret;
        }

        static StackObject* get_ScrollView_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ScrollView;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_NormalizedPosition_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.NormalizedPosition;

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static StackObject* Clear_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Clear();

            return __ret;
        }

        static StackObject* GetCells_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetCells();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* RemoveTopRange_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @rebuildContent = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @count = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveTopRange(@count, @rebuildContent);

            return __ret;
        }

        static StackObject* get_CellCount_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CellCount;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* Add_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @size = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Add(@size);

            return __ret;
        }

        static StackObject* SetLockNormalizedPosition_9(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @pos = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @locked = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetLockNormalizedPosition(@locked, @pos);

            return __ret;
        }

        static StackObject* set_CellCount_10(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CellCount = value;

            return __ret;
        }

        static StackObject* ForceRefreshActiveCell_11(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ForceRefreshActiveCell();

            return __ret;
        }

        static StackObject* set_NormalizedPosition_12(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector2 @value = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @value, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @value = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.NormalizedPosition = value;

            return __ret;
        }

        static StackObject* Update_13(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::InfinityIrregularGrid instance_of_this_method = (global::InfinityIrregularGrid)typeof(global::InfinityIrregularGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Update();

            return __ret;
        }


        static object get_onCreateCell_0(ref object o)
        {
            return ((global::InfinityIrregularGrid)o).onCreateCell;
        }

        static StackObject* CopyToStack_onCreateCell_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::InfinityIrregularGrid)o).onCreateCell;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onCreateCell_0(ref object o, object v)
        {
            ((global::InfinityIrregularGrid)o).onCreateCell = (System.Action<global::InfinityGridCell>)v;
        }

        static StackObject* AssignFromStack_onCreateCell_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<global::InfinityGridCell> @onCreateCell = (System.Action<global::InfinityGridCell>)typeof(System.Action<global::InfinityGridCell>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::InfinityIrregularGrid)o).onCreateCell = @onCreateCell;
            return ptr_of_this_method;
        }

        static object get_onCellChange_1(ref object o)
        {
            return ((global::InfinityIrregularGrid)o).onCellChange;
        }

        static StackObject* CopyToStack_onCellChange_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::InfinityIrregularGrid)o).onCellChange;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onCellChange_1(ref object o, object v)
        {
            ((global::InfinityIrregularGrid)o).onCellChange = (System.Action<global::InfinityGridCell, System.Int32>)v;
        }

        static StackObject* AssignFromStack_onCellChange_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<global::InfinityGridCell, System.Int32> @onCellChange = (System.Action<global::InfinityGridCell, System.Int32>)typeof(System.Action<global::InfinityGridCell, System.Int32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::InfinityIrregularGrid)o).onCellChange = @onCellChange;
            return ptr_of_this_method;
        }



    }
}
