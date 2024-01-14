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
    public class FCSVCollectionProcess
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public readonly List<uint> collectionScheme;
			public readonly string collectingAction;
			public uint collectingArms { get; private set; }
			public uint collectionBirthEffect { get; private set; }
			public uint collectionNoMayEffect { get; private set; }
			public uint collectionMayEffect { get; private set; }
			public uint collectingEffect { get; private set; }
			public readonly string collectingObjectAction;
			public uint collectObjectEffect { get; private set; }
			public uint collectedDialogue { get; private set; }
			public readonly string collectedAction;
			public uint collectedEffect { get; private set; }
			public readonly string collectObjectHpAction;
			public uint collectObjectHpEffect { get; private set; }
			public readonly string collectObjectDeathAction;
			public uint collectObjectDeathEffect { get; private set; }


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				collectionScheme = shareData.GetShareData<List<uint>>(binaryReader, 1);
				collectingAction = shareData.GetShareData<string>(binaryReader, 0);
				collectingArms = ReadHelper.ReadUInt(binaryReader);
				collectionBirthEffect = ReadHelper.ReadUInt(binaryReader);
				collectionNoMayEffect = ReadHelper.ReadUInt(binaryReader);
				collectionMayEffect = ReadHelper.ReadUInt(binaryReader);
				collectingEffect = ReadHelper.ReadUInt(binaryReader);
				collectingObjectAction = shareData.GetShareData<string>(binaryReader, 0);
				collectObjectEffect = ReadHelper.ReadUInt(binaryReader);
				collectedDialogue = ReadHelper.ReadUInt(binaryReader);
				collectedAction = shareData.GetShareData<string>(binaryReader, 0);
				collectedEffect = ReadHelper.ReadUInt(binaryReader);
				collectObjectHpAction = shareData.GetShareData<string>(binaryReader, 0);
				collectObjectHpEffect = ReadHelper.ReadUInt(binaryReader);
				collectObjectDeathAction = shareData.GetShareData<string>(binaryReader, 0);
				collectObjectDeathEffect = ReadHelper.ReadUInt(binaryReader);

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
			TableShareData shareData = new TableShareData(2);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

            return shareData;
        }
    }

    public class FCSVCollectionProcessAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVCollectionProcess);
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
    
        public class Adapter : FCSVCollectionProcess, CrossBindingAdaptorType
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