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
    public class FCSVTalkChoose
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint InitialTalkChooseId { get; private set; }
			public uint TalkChoose1 { get; private set; }
			public readonly List<uint> ChooseType1;
			public readonly List<uint> ChooseValue1;
			public uint ChooseEndTalk1 { get; private set; }
			public uint ChooseRightAndWrong1 { get; private set; }
			public uint TalkChoose2 { get; private set; }
			public readonly List<uint> ChooseType2;
			public readonly List<uint> ChooseValue2;
			public uint ChooseEndTalk2 { get; private set; }
			public uint ChooseRightAndWrong2 { get; private set; }
			public uint TalkChoose3 { get; private set; }
			public readonly List<uint> ChooseType3;
			public readonly List<uint> ChooseValue3;
			public uint ChooseEndTalk3 { get; private set; }
			public uint ChooseRightAndWrong3 { get; private set; }
			public uint TalkChoose4 { get; private set; }
			public readonly List<uint> ChooseType4;
			public readonly List<uint> ChooseValue4;
			public uint ChooseEndTalk4 { get; private set; }
			public uint ChooseRightAndWrong4 { get; private set; }
			public uint TalkChoose5 { get; private set; }
			public readonly List<uint> ChooseType5;
			public readonly List<uint> ChooseValue5;
			public uint ChooseEndTalk5 { get; private set; }
			public uint ChooseRightAndWrong5 { get; private set; }
			public uint ChooseWrongResult { get; private set; }
			public uint DetachWrong { get; private set; }


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				InitialTalkChooseId = ReadHelper.ReadUInt(binaryReader);
				TalkChoose1 = ReadHelper.ReadUInt(binaryReader);
				ChooseType1 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseValue1 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseEndTalk1 = ReadHelper.ReadUInt(binaryReader);
				ChooseRightAndWrong1 = ReadHelper.ReadUInt(binaryReader);
				TalkChoose2 = ReadHelper.ReadUInt(binaryReader);
				ChooseType2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseValue2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseEndTalk2 = ReadHelper.ReadUInt(binaryReader);
				ChooseRightAndWrong2 = ReadHelper.ReadUInt(binaryReader);
				TalkChoose3 = ReadHelper.ReadUInt(binaryReader);
				ChooseType3 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseValue3 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseEndTalk3 = ReadHelper.ReadUInt(binaryReader);
				ChooseRightAndWrong3 = ReadHelper.ReadUInt(binaryReader);
				TalkChoose4 = ReadHelper.ReadUInt(binaryReader);
				ChooseType4 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseValue4 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseEndTalk4 = ReadHelper.ReadUInt(binaryReader);
				ChooseRightAndWrong4 = ReadHelper.ReadUInt(binaryReader);
				TalkChoose5 = ReadHelper.ReadUInt(binaryReader);
				ChooseType5 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseValue5 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ChooseEndTalk5 = ReadHelper.ReadUInt(binaryReader);
				ChooseRightAndWrong5 = ReadHelper.ReadUInt(binaryReader);
				ChooseWrongResult = ReadHelper.ReadUInt(binaryReader);
				DetachWrong = ReadHelper.ReadUInt(binaryReader);

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
			TableShareData shareData = new TableShareData(1);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);

            return shareData;
        }
    }

    public class FCSVTalkChooseAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVTalkChoose);
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
    
        public class Adapter : FCSVTalkChoose, CrossBindingAdaptorType
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