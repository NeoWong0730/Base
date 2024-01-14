using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;

using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using UnityEngine;

namespace Framework.Table
{
    public class FCSVNpc
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint id_index { get; private set; }
			public uint mapId { get; private set; }
			public uint type { get; private set; }
			public uint subtype { get; private set; }
			public readonly List<uint> mark_type;
			public uint mark_lan { get; private set; }
			public readonly List<float> mark_move;
			public uint ActivationRecord { get; private set; }
			public uint NpcTriggerFrequency { get; private set; }
			public uint mark_des { get; private set; }
			public uint name { get; private set; }
			public uint appellation { get; private set; }
			public uint nameShow { get; private set; }
			public uint ShowState { get; private set; }
			public uint WhetherScopenTrigger { get; private set; }
			public uint TriggerScopen { get; private set; }
			public uint GreaterThanLvCond { get; private set; }
			public uint LessThanLvCond { get; private set; }
			public uint ConditionGroupCond { get; private set; }
			public uint RebirthTime { get; private set; }
			public uint CombatCooling { get; private set; }
			public readonly List<uint> NPCBubbleID;
			public readonly List<uint> NPCBubbleInteral;
			public readonly List<int> signPositionShifting;
			public readonly string model;
			public readonly string model_show;
			public uint action_id { get; private set; }
			public uint action_show_id { get; private set; }
			public readonly List<List<uint>> function;
			public readonly List<uint> OpenShop;
			public uint CollectionID { get; private set; }
			public readonly List<uint> functionCondition;
			public uint InteractiveRange { get; private set; }
			public readonly List<uint> acquiesceDialogue;
			public uint behaviorid { get; private set; }
			public uint TriggerPerformRange { get; private set; }
			public uint PerformCooling { get; private set; }
			public int LeftLocationX { get; private set; }
			public int LeftLocationY { get; private set; }
			public int LeftLocationZ { get; private set; }
			public int LeftLocationRotateX { get; private set; }
			public int LeftLocationRotateY { get; private set; }
			public int LeftLocationRotateZ { get; private set; }
			public int LeftLocationMirrorImage { get; private set; }
			public int RightLocationX { get; private set; }
			public int RightLocationY { get; private set; }
			public int RightLocationZ { get; private set; }
			public int RightLocationRotateX { get; private set; }
			public int RightLocationRotateY { get; private set; }
			public int RightLocationRotateZ { get; private set; }
			public int RightLocationMirrorImage { get; private set; }
			public readonly List<int> dialogueParameter;
			public readonly List<int> dialogueEndParameter;
			public int BubbleLocationX { get; private set; }
			public int BubbleLocationY { get; private set; }
			public int BubbleLocationZ { get; private set; }
			public int BubbleLocationRotateX { get; private set; }
			public int BubbleLocationRotateY { get; private set; }
			public int BubbleLocationRotateZ { get; private set; }
			public int BubbleLocationMirrorImage { get; private set; }
			public uint InteractiveVoice { get; private set; }
			public readonly List<uint> ResourecePara;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				id_index = ReadHelper.ReadUInt(binaryReader);
				mapId = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				subtype = ReadHelper.ReadUInt(binaryReader);
				mark_type = shareData.GetShareData<List<uint>>(binaryReader, 1);
				mark_lan = ReadHelper.ReadUInt(binaryReader);
				mark_move = shareData.GetShareData<List<float>>(binaryReader, 2);
				ActivationRecord = ReadHelper.ReadUInt(binaryReader);
				NpcTriggerFrequency = ReadHelper.ReadUInt(binaryReader);
				mark_des = ReadHelper.ReadUInt(binaryReader);
				name = ReadHelper.ReadUInt(binaryReader);
				appellation = ReadHelper.ReadUInt(binaryReader);
				nameShow = ReadHelper.ReadUInt(binaryReader);
				ShowState = ReadHelper.ReadUInt(binaryReader);
				WhetherScopenTrigger = ReadHelper.ReadUInt(binaryReader);
				TriggerScopen = ReadHelper.ReadUInt(binaryReader);
				GreaterThanLvCond = ReadHelper.ReadUInt(binaryReader);
				LessThanLvCond = ReadHelper.ReadUInt(binaryReader);
				ConditionGroupCond = ReadHelper.ReadUInt(binaryReader);
				RebirthTime = ReadHelper.ReadUInt(binaryReader);
				CombatCooling = ReadHelper.ReadUInt(binaryReader);
				NPCBubbleID = shareData.GetShareData<List<uint>>(binaryReader, 1);
				NPCBubbleInteral = shareData.GetShareData<List<uint>>(binaryReader, 1);
				signPositionShifting = shareData.GetShareData<List<int>>(binaryReader, 3);
				model = shareData.GetShareData<string>(binaryReader, 0);
				model_show = shareData.GetShareData<string>(binaryReader, 0);
				action_id = ReadHelper.ReadUInt(binaryReader);
				action_show_id = ReadHelper.ReadUInt(binaryReader);
				function = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				OpenShop = shareData.GetShareData<List<uint>>(binaryReader, 1);
				CollectionID = ReadHelper.ReadUInt(binaryReader);
				functionCondition = shareData.GetShareData<List<uint>>(binaryReader, 1);
				InteractiveRange = ReadHelper.ReadUInt(binaryReader);
				acquiesceDialogue = shareData.GetShareData<List<uint>>(binaryReader, 1);
				behaviorid = ReadHelper.ReadUInt(binaryReader);
				TriggerPerformRange = ReadHelper.ReadUInt(binaryReader);
				PerformCooling = ReadHelper.ReadUInt(binaryReader);
				LeftLocationX = ReadHelper.ReadInt(binaryReader);
				LeftLocationY = ReadHelper.ReadInt(binaryReader);
				LeftLocationZ = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateX = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateY = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				LeftLocationMirrorImage = ReadHelper.ReadInt(binaryReader);
				RightLocationX = ReadHelper.ReadInt(binaryReader);
				RightLocationY = ReadHelper.ReadInt(binaryReader);
				RightLocationZ = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateX = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateY = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				RightLocationMirrorImage = ReadHelper.ReadInt(binaryReader);
				dialogueParameter = shareData.GetShareData<List<int>>(binaryReader, 3);
				dialogueEndParameter = shareData.GetShareData<List<int>>(binaryReader, 3);
				BubbleLocationX = ReadHelper.ReadInt(binaryReader);
				BubbleLocationY = ReadHelper.ReadInt(binaryReader);
				BubbleLocationZ = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateX = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateY = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				BubbleLocationMirrorImage = ReadHelper.ReadInt(binaryReader);
				InteractiveVoice = ReadHelper.ReadUInt(binaryReader);
				ResourecePara = shareData.GetShareData<List<uint>>(binaryReader, 1);

            }
	    }	

        protected Dictionary<uint, int> Datas;
        protected List<Data> DataList;

        public int Count { get { return DataList.Count; } }

        public void ReadByFilePath(string path)
        {            
            Stream stream = AssetMananger.Instance.LoadStream(path);
            BinaryReader binaryReader = new BinaryReader(stream);
            Read(path, binaryReader);
            binaryReader.Close();
            stream.Close();
        }

        public void Read(string path, BinaryReader binaryReader)
        {
            if (binaryReader == null)
            {
                DebugUtil.LogErrorFormat("{0} binaryReader为空", path);
                return;
            }

            if (Datas != null)
            {
                DebugUtil.LogErrorFormat("{0} 多次读取配置", path);
                return;
            }

            TableShareData shareData = OnReadShareData(binaryReader);

            //buff的实际长度
            int bufferSize = binaryReader.ReadInt32();

            //读取内容
            int count = binaryReader.ReadInt32();

            Datas = new Dictionary<uint, int>(count);
            DataList = new List<Data>(count);

            if (count <= 0)
            {
                DebugUtil.LogErrorFormat("{0} 是空表", path);
                return;
            }

            int entrySize = bufferSize / count;

            if (entrySize * count != bufferSize)
            {
                DebugUtil.LogErrorFormat("{0} 数据长度异常", path);
                return;
            }

            for (int i = 0; i < count; ++i)
            {
#if DEBUG_MODE
                try
                {                                        
                    uint id = ReadHelper.ReadUInt(binaryReader);
                    Data data = new Data(id, binaryReader, shareData);
                    if (Datas.ContainsKey(id))
                    {
                        DebugUtil.LogErrorFormat("表格 {1} 有重复id{0}", id.ToString(), path);
                    }
                    Datas[id] = i;
                    DataList.Add(data);
                }
                catch (System.Exception e)
                {
                    DebugUtil.LogErrorFormat("表格 {2} 第 {0} 行错误： {1}", (i + 1).ToString(), e.StackTrace, path);
                    break;
                }
#else
                uint id = ReadHelper.ReadUInt(binaryReader);
                Data data = new Data(id, binaryReader, shareData);
                Datas[id] = i;
                DataList.Add(data);
#endif
            }
        }

        public Data GetConfData(uint id)
        {
            if (Datas.TryGetValue(id, out int index))
                return GetByIndex(index);
            return null;
        }

        public Data GetByIndex(int index)
        {
            Data tableData = DataList[index];
            return tableData;
        }

        public bool TryGetValue(uint id, out Data data)
        {
            if (Datas.TryGetValue(id, out int index))
            {
                data = GetByIndex(index);
                return true;
            }

            data = null;
            return false;
        }

        public IReadOnlyList<Data> GetAll()
        {
            return DataList;
        }

        public Dictionary<uint, int>.KeyCollection GetKeys()
        {
            return Datas.Keys;
        }

        public bool ContainsKey(uint id)
        {
            return Datas.ContainsKey(id);
        }

        public void Clear()
        {
            Datas?.Clear();
            DataList?.Clear();            
        }

        public static TableShareData OnReadShareData(BinaryReader binaryReader)
        {
			TableShareData shareData = new TableShareData(5);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArrays<float>(binaryReader, 2, ReadHelper.ReadArray_ReadFloat);
			shareData.ReadArrays<int>(binaryReader, 3, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<uint>(binaryReader, 4, 1);

            return shareData;
        }
    }

    public class FCSVNpcAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVNpc);
            }
        }
    
        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }
    
        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }
    
        public class Adapter : FCSVNpc, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    
            public Adapter()
            {
            
            }
    
            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }
    
            public ILTypeInstance ILInstance { get { return instance; } }
    
            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}