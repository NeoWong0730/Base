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
    public class FCSVTitle
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint titleLan { get; private set; }
			public uint professionId { get; private set; }
			public uint professionChange { get; private set; }
			public uint titleShowLan { get; private set; }
			public readonly List<Color32> titleShow;
			public uint titleShowIcon { get; private set; }
			public uint titleShowEffect { get; private set; }
			public uint titleShowClass { get; private set; }
			public int titleType { get; private set; }
			public int titleTypeNum { get; private set; }
			public int titleTypeLimit { get; private set; }
			public readonly List<uint> titleSeries;
			public uint titleGetType { get; private set; }
			public uint titleGetLan { get; private set; }
			public readonly List<uint> titleGet;
			public readonly List<uint> titleGo;
			public int titleLimitTime { get; private set; }
			public int titleOrder { get; private set; }
			public readonly List<List<uint>> titleProperty;
			public int titlePoint { get; private set; }


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				titleLan = ReadHelper.ReadUInt(binaryReader);
				professionId = ReadHelper.ReadUInt(binaryReader);
				professionChange = ReadHelper.ReadUInt(binaryReader);
				titleShowLan = ReadHelper.ReadUInt(binaryReader);
				titleShow = shareData.GetShareData<List<Color32>>(binaryReader, 0);
				titleShowIcon = ReadHelper.ReadUInt(binaryReader);
				titleShowEffect = ReadHelper.ReadUInt(binaryReader);
				titleShowClass = ReadHelper.ReadUInt(binaryReader);
				titleType = ReadHelper.ReadInt(binaryReader);
				titleTypeNum = ReadHelper.ReadInt(binaryReader);
				titleTypeLimit = ReadHelper.ReadInt(binaryReader);
				titleSeries = shareData.GetShareData<List<uint>>(binaryReader, 1);
				titleGetType = ReadHelper.ReadUInt(binaryReader);
				titleGetLan = ReadHelper.ReadUInt(binaryReader);
				titleGet = shareData.GetShareData<List<uint>>(binaryReader, 1);
				titleGo = shareData.GetShareData<List<uint>>(binaryReader, 1);
				titleLimitTime = ReadHelper.ReadInt(binaryReader);
				titleOrder = ReadHelper.ReadInt(binaryReader);
				titleProperty = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				titlePoint = ReadHelper.ReadInt(binaryReader);

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
			shareData.ReadArrays<Color32>(binaryReader, 0, ReadHelper.ReadArray_ReadColor32);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

            return shareData;
        }
    }

    public class FCSVTitleAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVTitle);
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
    
        public class Adapter : FCSVTitle, CrossBindingAdaptorType
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