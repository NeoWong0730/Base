using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Reflection;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using Lib.Core;
using System;
using System.Collections.Generic;
using ILRuntime.Runtime.Generated;
using UnityEngine;

namespace Framework
{
    public static class ILHelper
    {
        public static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain _appdomain)
        {
            RegisterILRuntime(_appdomain);

#if DEBUG_MODE
            //启动调试服务
            _appdomain.DebugService.StartDebugService(56000);
#endif

        }


        //此方法供运行时注册 和 生成绑定代码使用 ---值类型一定要绑定
        public static void RegisterILRuntime(ILRuntime.Runtime.Enviorment.AppDomain _appdomain)
        {
            RegisterTypes(_appdomain);
            RegisterAOTFunctions(_appdomain);
            RegisterValueTypeBinder(_appdomain);

            RegisterCrossBindingAdaptor(_appdomain);
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(_appdomain);
            RegisterCLRMethodRedirection(_appdomain);
            RegisterDelegates(_appdomain);
            RegisterDelegateConvertors(_appdomain);            
            RegisterBinder(_appdomain);//手动添加的Binder
//#if DEBUG_MODE
            RegisterDebugBinder(_appdomain);
//#endif
            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.Initialize(_appdomain); //--官方提供了新的方法，避免打包时来回注释
        }


        static void RegisterDebugBinder(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            ILRuntime.Runtime.Generated.UnityEngine_Debug_Binder.Register(appdomain);
            ILRuntime.Runtime.Generated.Lib_Core_DebugUtil_Binder.Register(appdomain);
            ILRuntime.Runtime.Generated.Lib_Core_StringBuilderPool_Binder.Register(appdomain);
        }

        static void RegisterBinder(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            Framework_Core_UI_FUIBase_Binder.Register(appdomain);            
            Framework_Core_UI_FUIStack_Binder.Register(appdomain);
            Framework_Core_UI_UIConfigData_Binder.Register(appdomain);
            Framework_Table_TableBase_1_ILTypeInstance_Binder.Register(appdomain);
            Framework_Table_TableShareData_Binder.Register(appdomain);
        }

        static void RegisterTypes(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            appdomain.GetType(typeof(Dictionary<Camera, UnityEngine.Rendering.CommandBuffer>));

            appdomain.GetType(typeof(EventEmitter<System.Int32>));
        }

        static void RegisterAOTFunctions(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            EventEmitter<System.Int32> eventEmitter = new EventEmitter<int>();

            eventEmitter.Handle<ILTypeInstance>(-1, null, true);
            eventEmitter.Trigger<ILTypeInstance>(-1, null);

            eventEmitter.Handle<System.Boolean>(-1, null, true);
            eventEmitter.Trigger<System.Boolean>(-1, false);

            eventEmitter.Handle<System.Byte>(-1, null, true);
            eventEmitter.Trigger<System.Byte>(-1, 0);

            eventEmitter.Handle<System.Int16>(-1, null, true);
            eventEmitter.Trigger<System.Int16>(-1, 0);

            eventEmitter.Handle<System.UInt16>(-1, null, true);
            eventEmitter.Trigger<System.UInt16>(-1, 0);

            eventEmitter.Handle<System.Int32>(-1, null, true);
            eventEmitter.Trigger<System.Int32>(-1, 0);

            eventEmitter.Handle<System.UInt32>(-1, null, true);
            eventEmitter.Trigger<System.UInt32>(-1, 0);

            eventEmitter.Handle<System.Int64>(-1, null, true);
            eventEmitter.Trigger<System.Int64>(-1, 0);

            eventEmitter.Handle<System.UInt64>(-1, null, true);
            eventEmitter.Trigger<System.UInt64>(-1, 0);

            eventEmitter.Handle<System.Single>(-1, null, true);
            eventEmitter.Trigger<System.Single>(-1, 0);

            eventEmitter.Handle<System.String>(-1, null, true);
            eventEmitter.Trigger<System.String>(-1, string.Empty);

            eventEmitter.Handle<Vector4>(-1, null, true);
            eventEmitter.Trigger<Vector4>(-1, Vector4.zero);

            eventEmitter.Handle<Vector3>(-1, null, true);
            eventEmitter.Trigger<Vector3>(-1, Vector3.zero);

            eventEmitter.Handle<Vector2>(-1, null, true);
            eventEmitter.Trigger<Vector2>(-1, Vector2.zero);

            eventEmitter.Handle<Quaternion>(-1, null, true);
            eventEmitter.Trigger<Quaternion>(-1, Quaternion.identity);

            eventEmitter = null;

            //ReadHelper.ReadArray<ILTypeInstance>(null, null);
            //ReadHelper.ReadArray<System.Boolean>(null, null);
            //ReadHelper.ReadArray<System.Byte>(null, null);
            //ReadHelper.ReadArray<System.Int16>(null, null);
            //ReadHelper.ReadArray<System.UInt16>(null, null);
            //ReadHelper.ReadArray<System.Int32>(null, null);
            //ReadHelper.ReadArray<System.UInt32>(null, null);
            //ReadHelper.ReadArray<System.Int64>(null, null);
            //ReadHelper.ReadArray<System.UInt64>(null, null);
            //ReadHelper.ReadArray<System.Single>(null, null);
            //ReadHelper.ReadArray<System.String>(null, null);
            //ReadHelper.ReadArray<Vector4>(null, null);
            //ReadHelper.ReadArray<Vector3>(null, null);
            //ReadHelper.ReadArray<Vector2>(null, null);
            //ReadHelper.ReadArray<Quaternion>(null, null);
            //
            //ReadHelper.ReadArray2<ILTypeInstance>(null, null);
            //ReadHelper.ReadArray2<System.Boolean>(null, null);
            //ReadHelper.ReadArray2<System.Byte>(null, null);
            //ReadHelper.ReadArray2<System.Int16>(null, null);
            //ReadHelper.ReadArray2<System.UInt16>(null, null);
            //ReadHelper.ReadArray2<System.Int32>(null, null);
            //ReadHelper.ReadArray2<System.UInt32>(null, null);
            //ReadHelper.ReadArray2<System.Int64>(null, null);
            //ReadHelper.ReadArray2<System.UInt64>(null, null);
            //ReadHelper.ReadArray2<System.Single>(null, null);
            //ReadHelper.ReadArray2<System.String>(null, null);
            //ReadHelper.ReadArray2<Vector4>(null, null);
            //ReadHelper.ReadArray2<Vector3>(null, null);
            //ReadHelper.ReadArray2<Vector2>(null, null);
            //ReadHelper.ReadArray2<Quaternion>(null, null);

            ReadHelper.ReadArray2_ReadInt(null);
            ReadHelper.ReadArray2_ReadUInt(null);
            ReadHelper.ReadArray2_ReadInt64(null);
            ReadHelper.ReadArray2_ReadUInt64(null);
            ReadHelper.ReadArray2_ReadFloat(null);

            ReadHelper.ReadArray_ReadInt(null);
            ReadHelper.ReadArray_ReadUInt(null);
            ReadHelper.ReadArray_ReadInt64(null);
            ReadHelper.ReadArray_ReadUInt64(null);
            ReadHelper.ReadArray_ReadFloat(null);

            ReadHelper.ReadArray_ReadString(null);
            ReadHelper.ReadArray_ReadColor32(null);
            
            MaunalBinding_AudioManager.Register(appdomain);
            MaunalBinding_ListHelper.Register(appdomain);
        }

        static void RegisterValueTypeBinder(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            /*****通过对值类型 添加绑定，可以大幅增加值类型的执行效率，以及避免GC Alloc内存分配。****/

            //1.官方提供的 3个脚本
            appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            appdomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
            appdomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());

            //新增加，干净的属性，确定没有问题的
            appdomain.RegisterValueTypeBinder(typeof(Color), new ColorBinder());
            appdomain.RegisterValueTypeBinder(typeof(Matrix4x4), new Matrix4x4Binder());
            appdomain.RegisterValueTypeBinder(typeof(Vector4), new Vector4Binder());//---生成绑定代码没问题
            appdomain.RegisterValueTypeBinder(typeof(Rect), new RectBinder());
            appdomain.RegisterValueTypeBinder(typeof(Resolution), new ResolutionBinder());
            appdomain.RegisterValueTypeBinder(typeof(Vector3Int), new Vector3IntBinder());
            appdomain.RegisterValueTypeBinder(typeof(Vector2Int), new Vector2IntBinder());

            //测试没有问题的
            appdomain.RegisterValueTypeBinder(typeof(JoystickData), new JoystickDataBinder());//pc测试没问题
            appdomain.RegisterValueTypeBinder(typeof(Color32), new Color32Binder());
            appdomain.RegisterValueTypeBinder(typeof(UnityEngine.AI.NavMeshHit), new NavMeshHitBinder());

            //不确定，可能会有问题的
            //appdomain.RegisterValueTypeBinder(typeof(UnityEngine.SceneManagement.Scene), new SceneBinder());//不知道什么时候会用到
            //appdomain.RegisterValueTypeBinder(typeof(CharacterInfo), new CharacterInfoBinder());//暂时没法测试
            //appdomain.RegisterValueTypeBinder(typeof(RichTextData), new RichTextDataBinder()); //有问题
            //appdomain.RegisterValueTypeBinder(typeof(Bounds), new BoundsBinder());//有问题
            //appdomain.RegisterValueTypeBinder(typeof(Touch), new TouchBinder());//必须真机测试
            //appdomain.RegisterValueTypeBinder(typeof(Net.NetMsg), new Net_NetMsgBinder());//特殊--只有类型用到，成员没有被logic引用（所以实际没有绑定）

            //keyvaluePair的多种参数 值类型的重定向
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, System.Single>), new keyValuePair_2_UInt32_Single_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, UnityEngine.RectTransform>), new KeyValuePair_2_UInt32_RectTransform_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, System.UInt32>), new KeyValuePair_2_UInt32_UInt32_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, System.Int32>), new KeyValuePair_2_UInt32_Int32_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, System.Collections.Generic.List<System.UInt32>>), new KeyValuePair_2_UInt32_List_UInt32_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, System.Int64>), new KeyValuePair_2_UInt32_Int64_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, global::CoroutineAdapter.Adaptor>), new KeyValuePair_2_UInt32_CoroutineAdapter_Adaptor_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, ILRuntime.Runtime.Intepreter.ILTypeInstance>), new KeyValuePair_2_UInt32_ILTypeInstance_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, System.Boolean>), new KeyValuePair_2_UInt32_Boolean_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, System.String>), new KeyValuePair_2_UInt32_String_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, System.Collections.Generic.List<ILRuntime.Runtime.Intepreter.ILTypeInstance>>), new KeyValuePair_2_UInt32_List_ILTypeInstance_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, global::Adapt_IMessage.Adaptor>), new KeyValuePair_2_UInt32_Adapt_IMessage_Adaptor_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt32, System.Collections.Generic.Dictionary<System.UInt64, ILRuntime.Runtime.Intepreter.ILTypeInstance>>), new KeyValuePair_2_UInt32_Dictionary_2_UInt64_ILTypeInstance_Binder());

            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Int32, System.UInt64>), new keyValuePair_2_Int32_UInt64_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Int32, global::ScrollViewAttach>), new keyValuePair_2_Int32_ScrollViewAttach_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Int32, System.Type>), new keyValuePair_2_Int32_Type_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Int32, System.Collections.Generic.List<global::CoroutineAdapter.Adaptor>>), new keyValuePair_2_Int32_List_CoroutineAdapter_Adaptor_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Int32, System.UInt32>), new keyValuePair_2_Int32_UInt32_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Int32, System.Collections.Generic.List<ILRuntime.Runtime.Intepreter.ILTypeInstance>>), new keyValuePair_2_Int32_List_ILTypeInstance_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Int32, System.Collections.Generic.SortedDictionary<System.UInt32, ILRuntime.Runtime.Intepreter.ILTypeInstance>>), new keyValuePair_2_Int32_SortedDictionary_2_UInt32_ILTypeInstance_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>), new keyValuePair_2_Int32_ILTypeInstance_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Int32, UnityEngine.Color32>), new keyValuePair_2_Int32_Color32_Binder());

            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt64, global::CoroutineAdapter.Adaptor>), new keyValuePair_2_UInt64_CoroutineAdapter_Adaptor_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt64, ILRuntime.Runtime.Intepreter.ILTypeInstance>), new keyValuePair_2_UInt64_ILTypeInstance_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.UInt64, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.GameObject>>), new keyValuePair_2_UInt64_AsyncOperationHandle_GameObject_Binder());

            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Int64, global::CoroutineAdapter.Adaptor>), new keyValuePair_2_Int64_CoroutineAdapter_Adaptor_Binder());

            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<UnityEngine.GameObject, global::CoroutineAdapter.Adaptor>), new keyValuePair_2_GameObject_CoroutineAdapter_Adaptor_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<UnityEngine.GameObject, ILRuntime.Runtime.Intepreter.ILTypeInstance>), new KeyValuePair_2_GameObject_ILTypeInstance_Binder());

            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.String, System.Boolean>), new keyValuePair_2_String_Boolean_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.String, System.Json.JsonValue>), new keyValuePair_2_String_JsonValue_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>), new keyValuePair_2_String_ILTypeInstance_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.String, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.GameObject>>), new keyValuePair_2_String_AsyncOperationHandle_GameObject_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.String, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.AnimationClip>>), new keyValuePair_2_String_AsyncOperationHandle_AnimationClip_Binder());

            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Type, System.Collections.Generic.Dictionary<System.UInt64, ILRuntime.Runtime.Intepreter.ILTypeInstance>>), new keyValuePair_2_Type_Dictionary_2_UInt64_ILTypeInstance_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Type, global::CoroutineAdapter.Adaptor>), new keyValuePair_2_Type_CoroutineAdapter_Adaptor_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<System.Type, ILRuntime.Runtime.Intepreter.ILTypeInstance>), new keyValuePair_2_Type_ILTypeInstance_Binder());

            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance>), new keyValuePair_2_ILTypeInstance_ILTypeInstance_Binder());
            appdomain.RegisterValueTypeBinder(typeof(KeyValuePair<ILRuntime.Runtime.Intepreter.ILTypeInstance, global::Adapt_IMessage.Adaptor>), new keyValuePair_2_ILTypeInstance_Adapt_IMessage_Adaptor_Binder());

        }


        static void RegisterCrossBindingAdaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            //AdapterRegister.RegisterCrossBindingAdaptor(appdomain);

            appdomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
            appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            appdomain.RegisterCrossBindingAdaptor(new Adapt_IMessage());
            appdomain.RegisterCrossBindingAdaptor(new IComparableAdapter());
            appdomain.RegisterCrossBindingAdaptor(new IEnumerableAdapter());
            appdomain.RegisterCrossBindingAdaptor(new IMessageEnumerableAdapter());
            appdomain.RegisterCrossBindingAdaptor(new IEqualityComparerAdapter());
            appdomain.RegisterCrossBindingAdaptor(new IEnumEqualityComparerAdapter());
            appdomain.RegisterCrossBindingAdaptor(new IComparerAdapter());
            appdomain.RegisterCrossBindingAdaptor(new IComparerIntAdapter());
            appdomain.RegisterCrossBindingAdaptor(new IComparerUIntAdapter());
            appdomain.RegisterCrossBindingAdaptor(new Framework.Adaptor.FUIBaseAdapter());
            appdomain.RegisterCrossBindingAdaptor(new Framework.Adaptor.FUIStackAdapter());            
            appdomain.RegisterCrossBindingAdaptor(new Framework.Adaptor.TableBase_1_ILTypeInstanceAdapter());

            Framework.Table.CSVAdapters.RegisterCrossBindingAdaptor(appdomain);
        }

        unsafe static void RegisterCLRMethodRedirection(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            var arr = typeof(GameObject).GetMethods();
            foreach (var i in arr)
            {
                if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1)
                {
                    appdomain.RegisterCLRMethodRedirection(i, AddComponentGeneric);
                }
                else if (i.Name == "AddComponent" && i.GetParameters().Length == 1)
                {
                    appdomain.RegisterCLRMethodRedirection(i, AddComponent);
                }
                else if (i.Name == "GetComponent" && i.GetGenericArguments().Length == 1)
                {
                    appdomain.RegisterCLRMethodRedirection(i, GetComponentGeneric);
                }
                else if (i.Name == "GetComponent" && i.GetParameters().Length == 1)
                {
                    appdomain.RegisterCLRMethodRedirection(i, GetComponent);
                }
            }
        }

        public static void RegisterDelegates(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.UI.Text, float, bool>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt32, ILRuntime.Runtime.Intepreter.ILTypeInstance>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.Single, System.Single, System.Single>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Int32, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<global::EGiftStatus>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt32, System.Boolean, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.UI.Text, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.RectTransform, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.Int64>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVTaskLevelConfine.Data, System.Boolean>();

            appdomain.DelegateManager.RegisterFunctionDelegate<Google.Protobuf.IMessage, Google.Protobuf.IMessage>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt32, System.UInt32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Single, System.Single, System.Single, System.Single>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();

            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.GameObject>();
            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Quaternion>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.String>();

            appdomain.DelegateManager.RegisterMethodDelegate<Framework.CutSceneArg>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, UnityEngine.Vector2>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.UInt32, System.UInt32, System.Boolean>();


            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt64, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt64, System.UInt32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Boolean>();

            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Transform>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.UInt32, System.UInt32, System.Int32>();

            appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.UInt32>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.BaseEventData>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVFormula.Data, Framework.Table.FCSVFormula.Data, System.Int32>();

            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Color>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, UnityEngine.GameObject>();
            appdomain.DelegateManager.RegisterMethodDelegate<global::CP_ScrolCircleListItem, System.Boolean, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, global::CP_ScrolCircleListItem>();

            appdomain.DelegateManager.RegisterMethodDelegate<global::Adapt_IMessage.Adaptor>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Adapt_IMessage.Adaptor>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILTypeInstance>();
            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Color>();

            appdomain.DelegateManager.RegisterMethodDelegate<Net.NetMsg>();

            appdomain.DelegateManager.RegisterMethodDelegate<global::InfinityGridCell>();
            appdomain.DelegateManager.RegisterMethodDelegate<global::InfinityGridCell, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<JoystickData>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.KeyCode>();

            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.AsyncOperation>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.SceneManagement.Scene>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.Scene>();

            appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Byte>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int16>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt16>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Single>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int64>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt64>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<Vector4>();
            appdomain.DelegateManager.RegisterMethodDelegate<Vector3>();
            appdomain.DelegateManager.RegisterMethodDelegate<Vector2>();
            appdomain.DelegateManager.RegisterMethodDelegate<Quaternion>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, ILTypeInstance>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Byte>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Int16>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.UInt16>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.UInt32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Single>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Int64>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.UInt64>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, Vector4>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, Vector3>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, Vector2>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, Quaternion>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Playables.PlayableDirector>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.UInt32>();

            appdomain.DelegateManager.RegisterFunctionDelegate<System.Single>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILTypeInstance, ILTypeInstance, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<global::Adapt_IMessage.Adaptor, global::Adapt_IMessage.Adaptor, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.List<System.UInt32>>();
            appdomain.DelegateManager.RegisterFunctionDelegate<global::CP_TransformBinder, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Boolean>();

            appdomain.DelegateManager.RegisterFunctionDelegate<global::Adapt_IMessage.Adaptor, global::Adapt_IMessage.Adaptor, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.UI.Image>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.UInt32, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector3>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.List<System.UInt32>, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.UInt64>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.UInt32, System.Boolean>();

            appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>();
            //appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Int32, System.Int32, System.Int32, System.Int32, System.Int32, System.Int32, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.PointerEventData>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.List<ILRuntime.Runtime.Intepreter.ILTypeInstance>>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.Action>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.NetworkReachability, UnityEngine.NetworkReachability>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.Boolean, System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.UInt64>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Transform, System.Int32>();

            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.GameObject>>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.Texture>>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.Sprite>>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.AnimationClip>>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.Dictionary<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Int32, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.Dictionary<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector3>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Video.VideoPlayer, System.String>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, UnityEngine.Transform>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt64>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt64, System.Boolean>();


            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.Collections.Generic.List<System.UInt32>, System.Collections.Generic.List<System.UInt32>>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Object[]>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt32, System.UInt32, System.UInt32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.UInt32, System.UInt32>, System.UInt32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.List<global::Adapt_IMessage.Adaptor>>();
            appdomain.DelegateManager.RegisterFunctionDelegate<global::Adapt_IMessage.Adaptor, System.Boolean>();

            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.UInt32, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.UInt32, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.UInt32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.UInt32, ILRuntime.Runtime.Intepreter.ILTypeInstance>, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.UInt32, System.Int32>, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.UInt32, System.Int32>, System.UInt32>();

            appdomain.DelegateManager.RegisterMethodDelegate<global::ScrollGridCell>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Int32, System.Char, System.Char>();

            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UI.Toggle, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.Dictionary<System.UInt64, ILRuntime.Runtime.Intepreter.ILTypeInstance>>();

            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Transform, UnityEngine.Transform>();
            appdomain.DelegateManager.RegisterMethodDelegate<Net.NetClient.ENetState, Net.NetClient.ENetState>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Int32, System.String[]>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.String, System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.UInt32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.UInt32, System.Int64>, System.UInt32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.UInt32, System.Int64>, System.Int64>();
            appdomain.DelegateManager.RegisterFunctionDelegate<global::CoroutineAdapter.Adaptor, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<global::CoroutineAdapter.Adaptor, global::CoroutineAdapter.Adaptor, System.Int32>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Int32, System.Int32, System.Int32>();

            appdomain.DelegateManager.RegisterMethodDelegate<global::CoroutineAdapter.Adaptor>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt32, ILRuntime.Runtime.Intepreter.ILTypeInstance>();            
            appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();            
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt32, System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt32, System.UInt32, System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt32, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<global::Adapt_IMessage.Adaptor, System.Int32, System.UInt32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt32, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.String, System.String, System.String>();

            appdomain.DelegateManager.RegisterFunctionDelegate<System.IO.BinaryReader, Framework.Table.TableShareData>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.UInt32, System.IO.BinaryReader, System.String[], ILRuntime.Runtime.Intepreter.ILTypeInstance>();            
            appdomain.DelegateManager.RegisterFunctionDelegate<System.UInt32, System.IO.BinaryReader, Framework.Table.TableShareData, ILRuntime.Runtime.Intepreter.ILTypeInstance>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt32, System.Int64>();
            appdomain.DelegateManager.RegisterFunctionDelegate<global::Adapt_IMessage.Adaptor, System.UInt32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt64, System.UInt32, System.UInt32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32, System.UInt32, System.Int32>();

            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVAssessType.Data, Framework.Table.FCSVAssessType.Data, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<Framework.Table.FCSVCollection.Data>();
            appdomain.DelegateManager.RegisterMethodDelegate<Framework.Table.FCSVDetect.Data>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVAssessMain.Data, Framework.Table.FCSVAssessMain.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVPetNewShowFilter.Data, Framework.Table.FCSVPetNewShowFilter.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVDailyActivity.Data, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVBattleMenuFunction.Data, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVBattleMenuFunction.Data, Framework.Table.FCSVBattleMenuFunction.Data, System.Int32>();

            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVChapterFunctionList.Data, Framework.Table.FCSVChapterFunctionList.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVCompose.Data, Framework.Table.FCSVCompose.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVRuneSlot.Data, Framework.Table.FCSVRuneSlot.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVStone.Data, Framework.Table.FCSVStone.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVFamilyPet.Data, Framework.Table.FCSVFamilyPet.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVRanklistsort.Data, Framework.Table.FCSVRanklistsort.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVTeam.Data, Framework.Table.FCSVTeam.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVInstanceDaily.Data, Framework.Table.FCSVInstanceDaily.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVFriendIntimacy.Data, Framework.Table.FCSVFriendIntimacy.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVItem.Data, Framework.Table.FCSVItem.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVFamilyPetMood.Data, Framework.Table.FCSVFamilyPetMood.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVFamilyPetHealth.Data, Framework.Table.FCSVFamilyPetHealth.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVTreasures.Data, Framework.Table.FCSVTreasures.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVDailyActivity.Data, Framework.Table.FCSVDailyActivity.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVNewBiographySeries.Data, Framework.Table.FCSVNewBiographySeries.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVDailyActivityWeek.Data, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVBattleMenuType.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVBattleMenuFunction.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVDailyActivityShow.Data, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVChoseSkillLimit.Data, Framework.Table.FCSVChoseSkillLimit.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVWelfareMenu.Data, Framework.Table.FCSVWelfareMenu.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVActionState.Data, Framework.Table.FCSVActionState.Data, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVTask.Data, System.Boolean>();

            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVInstanceDaily.Data, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<global::SDKManager.SDKReportState>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Framework.Table.FCSVStoneStage.Data, Framework.Table.FCSVStoneStage.Data, System.Int32>();
#if !USE_OLDSDK
            appdomain.DelegateManager.RegisterMethodDelegate<com.kwai.game.features.PayResultModel,System.String>();
#endif

        }

        public static void RegisterDelegateConvertors(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            appdomain.DelegateManager.RegisterDelegateConvertor<GME.QAVRecordFileCompleteCallback>((act) =>
            {
                return new GME.QAVRecordFileCompleteCallback((code, filepath) =>
                {
                    ((Action<System.Int32, System.String>)act)(code, filepath);
                });
            });
            

            appdomain.DelegateManager.RegisterDelegateConvertor<GME.QAVUploadFileCompleteCallback>((act) =>
            {
                return new GME.QAVUploadFileCompleteCallback((code, filepath, fileid) =>
                {
                    ((Action<System.Int32, System.String, System.String>)act)(code, filepath, fileid);
                });
            });
            
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Converter<System.Int32, System.UInt32>>((act) =>
            {
                return new System.Converter<System.Int32, System.UInt32>((input) =>
                {
                    return ((Func<System.Int32, System.UInt32>)act)(input);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<GME.QAVDownloadFileCompleteCallback>((act) =>
            {
                return new GME.QAVDownloadFileCompleteCallback((code, filepath, fileid) =>
                {
                    ((Action<System.Int32, System.String, System.String>)act)(code, filepath, fileid);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<GME.QAVDownloadFileWithAuditCompleteCallback>((act) =>
            {
                return new GME.QAVDownloadFileWithAuditCompleteCallback((code, filepath, fileid, auditResult) =>
                {
                    ((Action<System.Int32, System.String, System.String, System.String>)act)(code, filepath, fileid, auditResult);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<GME.QAVPlayFileCompleteCallback>((act) =>
            {
                return new GME.QAVPlayFileCompleteCallback((code, filepath) =>
                {
                    ((Action<System.Int32, System.String>)act)(code, filepath);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<GME.QAVSpeechToTextCallback>((act) =>
            {
                return new GME.QAVSpeechToTextCallback((code, fileid, result) =>
                {
                    ((Action<System.Int32, System.String, System.String>)act)(code, fileid, result);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<GME.QAVSpeechToTextWithAuditCallback>((act) =>
            {
                return new GME.QAVSpeechToTextWithAuditCallback((code, fileid, result, auditResult) =>
                {
                    ((Action<System.Int32, System.String, System.String, System.String>)act)(code, fileid, result, auditResult);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<GME.QAVEnterRoomComplete>((act) =>
            {
                return new GME.QAVEnterRoomComplete((result, error_info) =>
                {
                    ((Action<System.Int32, System.String>)act)(result, error_info);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<GME.QAVExitRoomComplete>((act) =>
            {
                return new GME.QAVExitRoomComplete(() =>
                {
                    ((Action)act)();
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<GME.QAVEndpointsUpdateInfo>((act) => 
            {
                return new GME.QAVEndpointsUpdateInfo((eventID, count, openIdList) => 
                {
                    ((Action<System.Int32, System.Int32, System.String[]>)act)(eventID, count, openIdList);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVFormula.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVFormula.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVFormula.Data, Framework.Table.FCSVFormula.Data, System.Int32>)act)(x, y);
                });
            });
            
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<Framework.Table.FCSVTaskLevelConfine.Data>>((act) =>
            {
                return new System.Predicate<Framework.Table.FCSVTaskLevelConfine.Data>((obj) =>
                {
                    return ((Func<Framework.Table.FCSVTaskLevelConfine.Data, System.Boolean>)act)(obj);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<UnityEngine.Quaternion>>((act) =>
            {
                return new DG.Tweening.Core.DOSetter<UnityEngine.Quaternion>((pNewValue) =>
                {
                    ((Action<UnityEngine.Quaternion>)act)(pNewValue);
                });
            });


            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.Quaternion>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<UnityEngine.Quaternion>(() =>
                {
                    return ((Func<UnityEngine.Quaternion>)act)();
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Int32>>((act) =>
            {
                return new System.Predicate<System.Int32>((obj) =>
                {
                    return ((Func<System.Int32, System.Boolean>)act)(obj);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<Lib.Core.EventTrigger.VectorDelegate>((act) =>
            {
                return new Lib.Core.EventTrigger.VectorDelegate((go, delta) =>
                {
                    ((Action<UnityEngine.GameObject, UnityEngine.Vector2>)act)(go, delta);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<global::DropdownEx.VoidDelegate>((act) =>
            {
                return new global::DropdownEx.VoidDelegate((eventData) =>
                {
                    ((Action<UnityEngine.EventSystems.BaseEventData>)act)(eventData);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.UInt32>>((act) =>
            {
                return new System.Comparison<System.UInt32>((x, y) =>
                {
                    return ((Func<System.UInt32, System.UInt32, System.Int32>)act)(x, y);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<global::UICenterOnChild.OnCenterHandler>((act) =>
            {
                return new global::UICenterOnChild.OnCenterHandler((centerChild) =>
                {
                    ((Action<UnityEngine.GameObject>)act)(centerChild);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<global::InfinityGridLayoutGroup.UpdateChildrenCallbackDelegate>((act) =>
            {
                return new global::InfinityGridLayoutGroup.UpdateChildrenCallbackDelegate((index, trans) =>
                {
                    ((Action<System.Int32, UnityEngine.Transform>)act)(index, trans);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Video.VideoPlayer.ErrorEventHandler>((act) =>
            {
                return new UnityEngine.Video.VideoPlayer.ErrorEventHandler((source, message) =>
                {
                    ((Action<UnityEngine.Video.VideoPlayer, System.String>)act)(source, message);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<Lib.Core.EventTrigger.VoidDelegate>((act) =>
            {
                return new Lib.Core.EventTrigger.VoidDelegate((go) =>
                {
                    ((Action<UnityEngine.GameObject>)act)(go);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.Vector3>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<UnityEngine.Vector3>(() =>
                {
                    return ((Func<UnityEngine.Vector3>)act)();
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Collections.Generic.List<System.UInt32>>>((act) =>
            {
                return new System.Predicate<System.Collections.Generic.List<System.UInt32>>((obj) =>
                {
                    return ((Func<System.Collections.Generic.List<System.UInt32>, System.Boolean>)act)(obj);
                });
            });


            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<UnityEngine.Vector3>>((act) =>
            {
                return new DG.Tweening.Core.DOSetter<UnityEngine.Vector3>((pNewValue) =>
                {
                    ((Action<UnityEngine.Vector3>)act)(pNewValue);
                });
            });


            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<global::CP_TransformBinder>>((act) =>
            {
                return new System.Predicate<global::CP_TransformBinder>((obj) =>
                {
                    return ((Func<global::CP_TransformBinder, System.Boolean>)act)(obj);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>((obj) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>)act)(obj);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<global::Adapt_IMessage.Adaptor>>((act) =>
            {
                return new System.Comparison<global::Adapt_IMessage.Adaptor>((x, y) =>
                {
                    return ((Func<global::Adapt_IMessage.Adaptor, global::Adapt_IMessage.Adaptor, System.Int32>)act)(x, y);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.GUI.WindowFunction>((act) =>
            {
                return new UnityEngine.GUI.WindowFunction((id) =>
                {
                    ((Action<System.Int32>)act)(id);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>((arg0) =>
                {
                    ((Action<UnityEngine.EventSystems.BaseEventData>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<UnityEngine.Color>>((act) =>
            {
                return new DG.Tweening.Core.DOSetter<UnityEngine.Color>((pNewValue) =>
                {
                    ((Action<UnityEngine.Color>)act)(pNewValue);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.Color>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<UnityEngine.Color>(() =>
                {
                    return ((Func<UnityEngine.Color>)act)();
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<Net.EventListenerDelegate>((act) =>
            {
                return new Net.EventListenerDelegate((msg) =>
                {
                    ((Action<Net.NetMsg>)act)(msg);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>((arg0, arg1) =>
                {
                    ((Action<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>)act)(arg0, arg1);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene>((arg0) =>
                {
                    ((Action<UnityEngine.SceneManagement.Scene>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.Scene>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.Scene>((arg0, arg1) =>
                {
                    ((Action<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.Scene>)act)(arg0, arg1);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((Action)act)();
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Boolean>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Boolean>((arg0) =>
                {
                    ((Action<System.Boolean>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Byte>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Byte>((arg0) =>
                {
                    ((Action<System.Byte>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Int16>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Int16>((arg0) =>
                {
                    ((Action<System.Int16>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.UInt16>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.UInt16>((arg0) =>
                {
                    ((Action<System.UInt16>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Int32>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Int32>((arg0) =>
                {
                    ((Action<System.Int32>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.UInt32>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.UInt32>((arg0) =>
                {
                    ((Action<System.UInt32>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Int64>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Int64>((arg0) =>
                {
                    ((Action<System.Int64>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.UInt64>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.UInt64>((arg0) =>
                {
                    ((Action<System.UInt64>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Single>((arg0) =>
                {
                    ((Action<System.Single>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.String>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.String>((arg0) =>
                {
                    ((Action<System.String>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<Vector4>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<Vector4>((arg0) =>
                {
                    ((Action<Vector4>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<Vector3>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<Vector3>((arg0) =>
                {
                    ((Action<Vector3>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<Vector2>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<Vector2>((arg0) =>
                {
                    ((Action<Vector2>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<Quaternion>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<Quaternion>((arg0) =>
                {
                    ((Action<Quaternion>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<System.Single>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<System.Single>(() =>
                {
                    return ((Func<System.Single>)act)();
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<System.Single>>((act) =>
            {
                return new DG.Tweening.Core.DOSetter<System.Single>((pNewValue) =>
                {
                    ((Action<System.Single>)act)(pNewValue);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
            {
                return new DG.Tweening.TweenCallback(() =>
                {
                    ((Action)act)();
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>)act)(x, y);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.UInt32>>((act) =>
            {
                return new System.Predicate<System.UInt32>((obj) =>
                {
                    return ((Func<System.UInt32, System.Boolean>)act)(obj);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<global::UIScrollCenterEx.UpdateChildrenCallbackDelegate>((act) =>
            {
                return new global::UIScrollCenterEx.UpdateChildrenCallbackDelegate((index, trans) =>
                {
                    ((Action<System.Int32, UnityEngine.Transform>)act)(index, trans);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<global::UIScrollCenterEx.OnCenterDelegate>((act) =>
            {
                return new global::UIScrollCenterEx.OnCenterDelegate((index) =>
                {
                    ((Action<System.Int32>)act)(index);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<Lib.Core.DoubleClickEvent.VoidDelegate>((act) =>
            {
                return new Lib.Core.DoubleClickEvent.VoidDelegate((go) =>
                {
                    ((Action<UnityEngine.GameObject>)act)(go);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<global::Adapt_IMessage.Adaptor>>((act) =>
            {
                return new System.Predicate<global::Adapt_IMessage.Adaptor>((obj) =>
                {
                    return ((Func<global::Adapt_IMessage.Adaptor, System.Boolean>)act)(obj);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.TimerCallback>((act) =>
            {
                return new System.Threading.TimerCallback((state) =>
                {
                    ((Action<System.Object>)act)(state);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.UI.InputField.OnValidateInput>((act) =>
            {
                return new UnityEngine.UI.InputField.OnValidateInput((text, charIndex, addedChar) =>
                {
                    return ((Func<System.String, System.Int32, System.Char, System.Char>)act)(text, charIndex, addedChar);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<UnityEngine.UI.Toggle>>((act) =>
            {
                return new System.Predicate<UnityEngine.UI.Toggle>((obj) =>
                {
                    return ((Func<UnityEngine.UI.Toggle, System.Boolean>)act)(obj);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<global::ScrollOrDragControl.OnCopyDataCallback>((act) =>
            {
                return new global::ScrollOrDragControl.OnCopyDataCallback((curTransform, copyTransform) =>
                {
                    ((Action<UnityEngine.Transform, UnityEngine.Transform>)act)(curTransform, copyTransform);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<global::ScrollOrDragControl.OnChangeDataCallback>((act) =>
            {
                return new global::ScrollOrDragControl.OnChangeDataCallback((curTransform, copyTransform) =>
                {
                    ((Action<UnityEngine.Transform, UnityEngine.Transform>)act)(curTransform, copyTransform);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.WaitCallback>((act) =>
            {
                return new System.Threading.WaitCallback((state) =>
                {
                    ((Action<System.Object>)act)(state);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.EventSystems.PointerEventData>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<UnityEngine.EventSystems.PointerEventData>((arg0) =>
                {
                    ((Action<UnityEngine.EventSystems.PointerEventData>)act)(arg0);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<global::CoroutineAdapter.Adaptor>>((act) =>
            {
                return new System.Comparison<global::CoroutineAdapter.Adaptor>((x, y) =>
                {
                    return ((Func<global::CoroutineAdapter.Adaptor, global::CoroutineAdapter.Adaptor, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.ThreadStart>((act) =>
            {
                return new System.Threading.ThreadStart(() =>
                {
                    ((Action)act)();
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVAssessType.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVAssessType.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVAssessType.Data, Framework.Table.FCSVAssessType.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVAssessMain.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVAssessMain.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVAssessMain.Data, Framework.Table.FCSVAssessMain.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVPetNewShowFilter.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVPetNewShowFilter.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVPetNewShowFilter.Data, Framework.Table.FCSVPetNewShowFilter.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVRanklistsort.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVRanklistsort.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVRanklistsort.Data, Framework.Table.FCSVRanklistsort.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<Framework.Table.FCSVDailyActivity.Data>>((act) =>
            {
                return new System.Predicate<Framework.Table.FCSVDailyActivity.Data>((obj) =>
                {
                    return ((Func<Framework.Table.FCSVDailyActivity.Data, System.Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVStone.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVStone.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVStone.Data, Framework.Table.FCSVStone.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVRuneSlot.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVRuneSlot.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVRuneSlot.Data, Framework.Table.FCSVRuneSlot.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVCompose.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVCompose.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVCompose.Data, Framework.Table.FCSVCompose.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVChapterFunctionList.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVChapterFunctionList.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVChapterFunctionList.Data, Framework.Table.FCSVChapterFunctionList.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVFamilyPet.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVFamilyPet.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVFamilyPet.Data, Framework.Table.FCSVFamilyPet.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVTeam.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVTeam.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVTeam.Data, Framework.Table.FCSVTeam.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVInstanceDaily.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVInstanceDaily.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVInstanceDaily.Data, Framework.Table.FCSVInstanceDaily.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVFamilyPetMood.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVFamilyPetMood.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVFamilyPetMood.Data, Framework.Table.FCSVFamilyPetMood.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVFamilyPetHealth.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVFamilyPetHealth.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVFamilyPetHealth.Data, Framework.Table.FCSVFamilyPetHealth.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVTreasures.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVTreasures.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVTreasures.Data, Framework.Table.FCSVTreasures.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVFriendIntimacy.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVFriendIntimacy.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVFriendIntimacy.Data, Framework.Table.FCSVFriendIntimacy.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVItem.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVItem.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVItem.Data, Framework.Table.FCSVItem.Data, System.Int32>)act)(x, y);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVDailyActivity.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVDailyActivity.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVDailyActivity.Data, Framework.Table.FCSVDailyActivity.Data, System.Int32>)act)(x, y);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<Framework.Table.FCSVBattleMenuFunction.Data>>((act) =>
            {
                return new System.Predicate<Framework.Table.FCSVBattleMenuFunction.Data>((obj) =>
                {
                    return ((Func<Framework.Table.FCSVBattleMenuFunction.Data, System.Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVBattleMenuFunction.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVBattleMenuFunction.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVBattleMenuFunction.Data, Framework.Table.FCSVBattleMenuFunction.Data, System.Int32>)act)(x, y);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<Framework.Table.FCSVDailyActivityWeek.Data>>((act) =>
            {
                return new System.Predicate<Framework.Table.FCSVDailyActivityWeek.Data>((obj) =>
                {
                    return ((Func<Framework.Table.FCSVDailyActivityWeek.Data, System.Boolean>)act)(obj);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVNewBiographySeries.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVNewBiographySeries.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVNewBiographySeries.Data, Framework.Table.FCSVNewBiographySeries.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<Framework.Table.FCSVDailyActivityShow.Data>>((act) =>
            {
                return new System.Predicate<Framework.Table.FCSVDailyActivityShow.Data>((obj) =>
                {
                    return ((Func<Framework.Table.FCSVDailyActivityShow.Data, System.Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVWelfareMenu.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVWelfareMenu.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVWelfareMenu.Data, Framework.Table.FCSVWelfareMenu.Data, System.Int32>)act)(x, y);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVChoseSkillLimit.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVChoseSkillLimit.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVChoseSkillLimit.Data, Framework.Table.FCSVChoseSkillLimit.Data, System.Int32>)act)(x, y);
                });
            });            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVActionState.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVActionState.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVActionState.Data, Framework.Table.FCSVActionState.Data, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<Framework.Table.FCSVTask.Data>>((act) =>
            {
                return new System.Predicate<Framework.Table.FCSVTask.Data>((obj) =>
                {
                    return ((Func<Framework.Table.FCSVTask.Data, System.Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<Framework.Table.FCSVInstanceDaily.Data>>((act) =>
            {
                return new System.Predicate<Framework.Table.FCSVInstanceDaily.Data>((obj) =>
                {
                    return ((Func<Framework.Table.FCSVInstanceDaily.Data, System.Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Framework.Table.FCSVStoneStage.Data>>((act) =>
            {
                return new System.Comparison<Framework.Table.FCSVStoneStage.Data>((x, y) =>
                {
                    return ((Func<Framework.Table.FCSVStoneStage.Data, Framework.Table.FCSVStoneStage.Data, System.Int32>)act)(x, y);
                });
            });
        }

        unsafe static StackObject* AddComponentGeneric(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                object res;
                if (type is CLRType)
                {
                    res = instance.AddComponent(type.TypeForCLR);
                }
                else
                {
                    var ilInstance = new ILTypeInstance(type as ILType, false);
                    var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                    clrInstance.ILInstance = ilInstance;
                    clrInstance.AppDomain = __domain;
                    ilInstance.CLRInstance = clrInstance;
                    res = clrInstance.ILInstance;
                    clrInstance.Awake();
                }

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }

            return __esp;
        }

        unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 2;
            var o = StackObject.ToObject(ptr, __domain, __mStack);
            GameObject instance = o as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            ptr = __esp - 1;
            Type t = (Type)StackObject.ToObject(ptr, __domain, __mStack);
            ILType it = ((ILRuntimeType)t).ILType;
            __intp.Free(ptr);

            if (it != null)
            {
                object res;
                var ilInstance = new ILTypeInstance(it as ILType, false);
                var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                clrInstance.ILInstance = ilInstance;
                clrInstance.AppDomain = __domain;
                ilInstance.CLRInstance = clrInstance;

                res = clrInstance.ILInstance;

                clrInstance.Awake();

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }

            return __esp;
        }

        unsafe static StackObject* GetComponentGeneric(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                object res = null;
                if (type is CLRType)
                {
                    res = instance.GetComponent(type.TypeForCLR);
                }
                else
                {
                    var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
                    for (int i = 0; i < clrInstances.Length; i++)
                    {
                        var clrInstance = clrInstances[i];
                        if (clrInstance.ILInstance != null)
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                res = clrInstance.ILInstance;
                                break;
                            }
                        }
                    }
                }

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }

            return __esp;
        }

        unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 2;
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            ptr = __esp - 1;
            Type t = (Type)StackObject.ToObject(ptr, __domain, __mStack);
            ILType it = ((ILRuntimeType)t).ILType;
            __intp.Free(ptr);

            if (it != null)
            {
                object res = null;
                var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
                for (int i = 0; i < clrInstances.Length; i++)
                {
                    var clrInstance = clrInstances[i];
                    if (clrInstance.ILInstance != null)
                    {
                        if (clrInstance.ILInstance.Type == it)
                        {
                            res = clrInstance.ILInstance;
                            break;
                        }
                    }
                }

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }

            return __esp;
        }
    }
}
