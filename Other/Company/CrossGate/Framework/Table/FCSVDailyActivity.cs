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
    public class FCSVDailyActivity
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint ActiveName { get; private set; }
			public uint ActiveDes { get; private set; }
			public uint OutputDisplay { get; private set; }
			public uint AvtiveIcon { get; private set; }
			public uint Active_lan { get; private set; }
			public uint ActiveOrder { get; private set; }
			public uint OpeningLevel { get; private set; }
			public uint HideLevel { get; private set; }
			public uint FunctionOpenid { get; private set; }
			public uint limite { get; private set; }
			public uint Times { get; private set; }
			public readonly List<uint> Play_Lv;
			public readonly List<uint> ActivityNum;
			public readonly List<uint> ActivityNumMax;
			public uint OpeningMode1 { get; private set; }
			public readonly List<uint> OpeningMode2;
			public uint ActiveType { get; private set; }
			public readonly List<List<int>> OpeningTime;
			public uint Duration { get; private set; }
			public readonly List<List<int>> NoticeTime;
			public uint NoticeLong { get; private set; }
			public uint Playing { get; private set; }
			public uint ResetTime { get; private set; }
			public bool IsFamilyActive { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public uint Npcid { get; private set; }
			public uint Uiid { get; private set; }
			public uint UiidSonId { get; private set; }
			public uint WayDesc { get; private set; }
			public uint WayIcon { get; private set; }
			public readonly List<uint> Reward;
			public readonly List<uint> Reward_int;
			public uint IsShow { get; private set; }
			public uint special_des_lan { get; private set; }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				ActiveName = ReadHelper.ReadUInt(binaryReader);
				ActiveDes = ReadHelper.ReadUInt(binaryReader);
				OutputDisplay = ReadHelper.ReadUInt(binaryReader);
				AvtiveIcon = ReadHelper.ReadUInt(binaryReader);
				Active_lan = ReadHelper.ReadUInt(binaryReader);
				ActiveOrder = ReadHelper.ReadUInt(binaryReader);
				OpeningLevel = ReadHelper.ReadUInt(binaryReader);
				HideLevel = ReadHelper.ReadUInt(binaryReader);
				FunctionOpenid = ReadHelper.ReadUInt(binaryReader);
				limite = ReadHelper.ReadUInt(binaryReader);
				Times = ReadHelper.ReadUInt(binaryReader);
				Play_Lv = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ActivityNum = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ActivityNumMax = shareData.GetShareData<List<uint>>(binaryReader, 0);
				OpeningMode1 = ReadHelper.ReadUInt(binaryReader);
				OpeningMode2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ActiveType = ReadHelper.ReadUInt(binaryReader);
				OpeningTime = shareData.GetShareData<List<List<int>>>(binaryReader, 2);
				Duration = ReadHelper.ReadUInt(binaryReader);
				NoticeTime = shareData.GetShareData<List<List<int>>>(binaryReader, 2);
				NoticeLong = ReadHelper.ReadUInt(binaryReader);
				Playing = ReadHelper.ReadUInt(binaryReader);
				ResetTime = ReadHelper.ReadUInt(binaryReader);
				Npcid = ReadHelper.ReadUInt(binaryReader);
				Uiid = ReadHelper.ReadUInt(binaryReader);
				UiidSonId = ReadHelper.ReadUInt(binaryReader);
				WayDesc = ReadHelper.ReadUInt(binaryReader);
				WayIcon = ReadHelper.ReadUInt(binaryReader);
				Reward = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Reward_int = shareData.GetShareData<List<uint>>(binaryReader, 0);
				IsShow = ReadHelper.ReadUInt(binaryReader);
				special_des_lan = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<int>(binaryReader, 2, 1);

            return shareData;
        }
    }

    public class FCSVDailyActivityAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVDailyActivity);
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
    
        public class Adapter : FCSVDailyActivity, CrossBindingAdaptorType
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