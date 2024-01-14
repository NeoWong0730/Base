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
    public class FCSVFavorabilityBanquet
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint Name { get; private set; }
			public uint Des { get; private set; }
			public uint FavorabilityValue { get; private set; }
			public uint IncreaseMoodValue { get; private set; }
			public uint ItemID1 { get; private set; }
			public uint Num1 { get; private set; }
			public uint ItemID2 { get; private set; }
			public uint Num2 { get; private set; }
			public uint ItemID3 { get; private set; }
			public uint Num3 { get; private set; }
			public uint ItemID4 { get; private set; }
			public uint Num4 { get; private set; }
			public uint ItemID5 { get; private set; }
			public uint Num5 { get; private set; }
			public uint ItemID6 { get; private set; }
			public uint Num6 { get; private set; }
			public uint ItemID7 { get; private set; }
			public uint Num7 { get; private set; }
			public uint ItemID8 { get; private set; }
			public uint Num8 { get; private set; }
			public uint ItemID9 { get; private set; }
			public uint Num9 { get; private set; }
			public uint ItemID10 { get; private set; }
			public uint Num10 { get; private set; }


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Name = ReadHelper.ReadUInt(binaryReader);
				Des = ReadHelper.ReadUInt(binaryReader);
				FavorabilityValue = ReadHelper.ReadUInt(binaryReader);
				IncreaseMoodValue = ReadHelper.ReadUInt(binaryReader);
				ItemID1 = ReadHelper.ReadUInt(binaryReader);
				Num1 = ReadHelper.ReadUInt(binaryReader);
				ItemID2 = ReadHelper.ReadUInt(binaryReader);
				Num2 = ReadHelper.ReadUInt(binaryReader);
				ItemID3 = ReadHelper.ReadUInt(binaryReader);
				Num3 = ReadHelper.ReadUInt(binaryReader);
				ItemID4 = ReadHelper.ReadUInt(binaryReader);
				Num4 = ReadHelper.ReadUInt(binaryReader);
				ItemID5 = ReadHelper.ReadUInt(binaryReader);
				Num5 = ReadHelper.ReadUInt(binaryReader);
				ItemID6 = ReadHelper.ReadUInt(binaryReader);
				Num6 = ReadHelper.ReadUInt(binaryReader);
				ItemID7 = ReadHelper.ReadUInt(binaryReader);
				Num7 = ReadHelper.ReadUInt(binaryReader);
				ItemID8 = ReadHelper.ReadUInt(binaryReader);
				Num8 = ReadHelper.ReadUInt(binaryReader);
				ItemID9 = ReadHelper.ReadUInt(binaryReader);
				Num9 = ReadHelper.ReadUInt(binaryReader);
				ItemID10 = ReadHelper.ReadUInt(binaryReader);
				Num10 = ReadHelper.ReadUInt(binaryReader);

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
			TableShareData shareData = null;

            return shareData;
        }
    }

    public class FCSVFavorabilityBanquetAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVFavorabilityBanquet);
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
    
        public class Adapter : FCSVFavorabilityBanquet, CrossBindingAdaptorType
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