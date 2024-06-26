﻿using Lib.AssetLoader;
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
    public class FCSVFashionAccessory
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint AccName { get; private set; }
			public uint AccDescribe { get; private set; }
			public uint Acctype { get; private set; }
			public readonly List<uint> AccItem;
			public readonly string AccSlot;
			public uint AccIcon { get; private set; }
			public readonly string model_show;
			public readonly string model;
			public uint MaxColour { get; private set; }
			public readonly List<List<uint>> AccColour;
			public readonly List<List<uint>> itemNeed;
			public readonly List<List<uint>> itemNeed1;
			public uint FashionScore { get; private set; }
			public bool Hide { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public uint Lock { get; private set; }
			public uint LimitedTime { get; private set; }
			public uint suitId { get; private set; }
			public readonly List<List<uint>> attr_id;
			public uint score { get; private set; }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				AccName = ReadHelper.ReadUInt(binaryReader);
				AccDescribe = ReadHelper.ReadUInt(binaryReader);
				Acctype = ReadHelper.ReadUInt(binaryReader);
				AccItem = shareData.GetShareData<List<uint>>(binaryReader, 1);
				AccSlot = shareData.GetShareData<string>(binaryReader, 0);
				AccIcon = ReadHelper.ReadUInt(binaryReader);
				model_show = shareData.GetShareData<string>(binaryReader, 0);
				model = shareData.GetShareData<string>(binaryReader, 0);
				MaxColour = ReadHelper.ReadUInt(binaryReader);
				AccColour = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				itemNeed = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				itemNeed1 = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				FashionScore = ReadHelper.ReadUInt(binaryReader);
				Lock = ReadHelper.ReadUInt(binaryReader);
				LimitedTime = ReadHelper.ReadUInt(binaryReader);
				suitId = ReadHelper.ReadUInt(binaryReader);
				attr_id = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				score = ReadHelper.ReadUInt(binaryReader);

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
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

            return shareData;
        }
    }

    public class FCSVFashionAccessoryAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVFashionAccessory);
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
    
        public class Adapter : FCSVFashionAccessory, CrossBindingAdaptorType
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