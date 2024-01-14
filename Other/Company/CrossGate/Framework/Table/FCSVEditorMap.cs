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
    public class FCSVEditorMap
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint width { get; private set; }
			public uint height { get; private set; }
			public readonly List<uint> telIds;
			public readonly List<uint> telPosXs;
			public readonly List<uint> telPosYs;
			public readonly List<float> telOffXs;
			public readonly List<float> telOffYs;
			public readonly List<uint> telRangeXs;
			public readonly List<uint> telRangeYs;
			public readonly List<uint> telCondIds;
			public readonly List<uint> npcIds;
			public readonly List<List<uint>> npcPosXs;
			public readonly List<List<uint>> npcPosYs;
			public readonly List<uint> npcWidths;
			public readonly List<uint> npcHeights;
			public readonly List<int> npcOffXs;
			public readonly List<int> npcOffYs;
			public readonly List<int> npcRotaXs;
			public readonly List<int> npcRotaYs;
			public readonly List<int> npcRotaZs;
			public readonly List<uint> monstergpIds;
			public readonly List<List<uint>> monstergpPosXs;
			public readonly List<List<uint>> monstergpPosYs;
			public readonly List<List<uint>> FightSafeAreaSize;
			public readonly List<List<uint>> FightSafeAreaOffset;
			public readonly List<uint> FightSafeAreaCamp;
			public readonly List<List<uint>> FightSafeAreaPos;
			public readonly List<List<uint>> FightBlockSize;
			public readonly List<List<uint>> FightBlockOffset;
			public readonly List<uint> FightBlockCamp;
			public readonly List<List<uint>> FightBlockPos;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				width = ReadHelper.ReadUInt(binaryReader);
				height = ReadHelper.ReadUInt(binaryReader);
				telIds = shareData.GetShareData<List<uint>>(binaryReader, 0);
				telPosXs = shareData.GetShareData<List<uint>>(binaryReader, 0);
				telPosYs = shareData.GetShareData<List<uint>>(binaryReader, 0);
				telOffXs = shareData.GetShareData<List<float>>(binaryReader, 1);
				telOffYs = shareData.GetShareData<List<float>>(binaryReader, 1);
				telRangeXs = shareData.GetShareData<List<uint>>(binaryReader, 0);
				telRangeYs = shareData.GetShareData<List<uint>>(binaryReader, 0);
				telCondIds = shareData.GetShareData<List<uint>>(binaryReader, 0);
				npcIds = shareData.GetShareData<List<uint>>(binaryReader, 0);
				npcPosXs = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				npcPosYs = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				npcWidths = shareData.GetShareData<List<uint>>(binaryReader, 0);
				npcHeights = shareData.GetShareData<List<uint>>(binaryReader, 0);
				npcOffXs = shareData.GetShareData<List<int>>(binaryReader, 2);
				npcOffYs = shareData.GetShareData<List<int>>(binaryReader, 2);
				npcRotaXs = shareData.GetShareData<List<int>>(binaryReader, 2);
				npcRotaYs = shareData.GetShareData<List<int>>(binaryReader, 2);
				npcRotaZs = shareData.GetShareData<List<int>>(binaryReader, 2);
				monstergpIds = shareData.GetShareData<List<uint>>(binaryReader, 0);
				monstergpPosXs = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				monstergpPosYs = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				FightSafeAreaSize = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				FightSafeAreaOffset = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				FightSafeAreaCamp = shareData.GetShareData<List<uint>>(binaryReader, 0);
				FightSafeAreaPos = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				FightBlockSize = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				FightBlockOffset = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				FightBlockCamp = shareData.GetShareData<List<uint>>(binaryReader, 0);
				FightBlockPos = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);

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
			TableShareData shareData = new TableShareData(4);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArrays<float>(binaryReader, 1, ReadHelper.ReadArray_ReadFloat);
			shareData.ReadArrays<int>(binaryReader, 2, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<uint>(binaryReader, 3, 0);

            return shareData;
        }
    }

    public class FCSVEditorMapAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVEditorMap);
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
    
        public class Adapter : FCSVEditorMap, CrossBindingAdaptorType
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