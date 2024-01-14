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
    public class FCSVBuff
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint success_probability { get; private set; }
			public uint effect_buff { get; private set; }
			public int buff_effective { get; private set; }
			public uint buff_state { get; private set; }
			public uint is_show { get; private set; }
			public uint icon { get; private set; }
			public uint hud_icon { get; private set; }
			public bool show_buff_duration { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public uint name { get; private set; }
			public uint desc { get; private set; }
			public uint duration_type { get; private set; }
			public uint buff_type { get; private set; }
			public readonly List<List<uint>> effective_attr;
			public uint clear_trigger { get; private set; }
			public uint effect_parma { get; private set; }
			public uint effect_parma1 { get; private set; }
			public uint effectAdd { get; private set; }
			public uint effectProc { get; private set; }
			public uint effecDel { get; private set; }
			public uint skill_id { get; private set; }
			public uint hp_skill_id { get; private set; }
			public uint ui_param1 { get; private set; }
			public uint ui_param2 { get; private set; }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				success_probability = ReadHelper.ReadUInt(binaryReader);
				effect_buff = ReadHelper.ReadUInt(binaryReader);
				buff_effective = ReadHelper.ReadInt(binaryReader);
				buff_state = ReadHelper.ReadUInt(binaryReader);
				is_show = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				hud_icon = ReadHelper.ReadUInt(binaryReader);
				name = ReadHelper.ReadUInt(binaryReader);
				desc = ReadHelper.ReadUInt(binaryReader);
				duration_type = ReadHelper.ReadUInt(binaryReader);
				buff_type = ReadHelper.ReadUInt(binaryReader);
				effective_attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				clear_trigger = ReadHelper.ReadUInt(binaryReader);
				effect_parma = ReadHelper.ReadUInt(binaryReader);
				effect_parma1 = ReadHelper.ReadUInt(binaryReader);
				effectAdd = ReadHelper.ReadUInt(binaryReader);
				effectProc = ReadHelper.ReadUInt(binaryReader);
				effecDel = ReadHelper.ReadUInt(binaryReader);
				skill_id = ReadHelper.ReadUInt(binaryReader);
				hp_skill_id = ReadHelper.ReadUInt(binaryReader);
				ui_param1 = ReadHelper.ReadUInt(binaryReader);
				ui_param2 = ReadHelper.ReadUInt(binaryReader);

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
			TableShareData shareData = new TableShareData(2);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 1, 0);

            return shareData;
        }
    }

    public class FCSVBuffAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVBuff);
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
    
        public class Adapter : FCSVBuff, CrossBindingAdaptorType
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