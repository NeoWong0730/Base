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
    public class FCSVEquipment
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint equipment_category { get; private set; }
			public uint equipment_type { get; private set; }
			public uint special_action { get; private set; }
			public readonly List<uint> career_condition;
			public uint equipment_level { get; private set; }
			public uint attr_id { get; private set; }
			public uint forge_id { get; private set; }
			public uint active_skillid { get; private set; }
			public uint icon { get; private set; }
			public readonly string model;
			public readonly string show_model;
			public readonly string equip_pos;
			public readonly List<uint> slot_id;
			public readonly List<uint> jewel_type;
			public uint jewel_number { get; private set; }
			public uint jewel_level { get; private set; }
			public readonly List<List<uint>> attr;
			public uint green_id { get; private set; }
			public uint special_id { get; private set; }
			public readonly List<uint> special_range;
			public readonly List<List<uint>> common_forge;
			public readonly List<List<uint>> intensify_forge;
			public readonly List<List<uint>> smelt;
			public readonly List<List<uint>> quenching;
			public uint dur { get; private set; }
			public uint distance { get; private set; }
			public readonly List<uint> suit_id;
			public readonly List<List<uint>> common_repair;
			public readonly List<List<uint>> intensify_repair;
			public uint score_coe { get; private set; }
			public readonly List<uint> score_sec;
			public readonly List<List<uint>> re_smelt;
			public uint quen_item { get; private set; }
			public bool doublehand { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly List<uint> decompose;
			public uint sale_least { get; private set; }
			public readonly List<List<uint>> suit_item_base;
			public readonly List<List<uint>> suit_pro_base;
			public readonly List<List<uint>> suit_item_special;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				equipment_category = ReadHelper.ReadUInt(binaryReader);
				equipment_type = ReadHelper.ReadUInt(binaryReader);
				special_action = ReadHelper.ReadUInt(binaryReader);
				career_condition = shareData.GetShareData<List<uint>>(binaryReader, 1);
				equipment_level = ReadHelper.ReadUInt(binaryReader);
				attr_id = ReadHelper.ReadUInt(binaryReader);
				forge_id = ReadHelper.ReadUInt(binaryReader);
				active_skillid = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				model = shareData.GetShareData<string>(binaryReader, 0);
				show_model = shareData.GetShareData<string>(binaryReader, 0);
				equip_pos = shareData.GetShareData<string>(binaryReader, 0);
				slot_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				jewel_type = shareData.GetShareData<List<uint>>(binaryReader, 1);
				jewel_number = ReadHelper.ReadUInt(binaryReader);
				jewel_level = ReadHelper.ReadUInt(binaryReader);
				attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				green_id = ReadHelper.ReadUInt(binaryReader);
				special_id = ReadHelper.ReadUInt(binaryReader);
				special_range = shareData.GetShareData<List<uint>>(binaryReader, 1);
				common_forge = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				intensify_forge = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				smelt = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				quenching = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				dur = ReadHelper.ReadUInt(binaryReader);
				distance = ReadHelper.ReadUInt(binaryReader);
				suit_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				common_repair = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				intensify_repair = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				score_coe = ReadHelper.ReadUInt(binaryReader);
				score_sec = shareData.GetShareData<List<uint>>(binaryReader, 1);
				re_smelt = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				quen_item = ReadHelper.ReadUInt(binaryReader);
				decompose = shareData.GetShareData<List<uint>>(binaryReader, 1);
				sale_least = ReadHelper.ReadUInt(binaryReader);
				suit_item_base = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				suit_pro_base = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				suit_item_special = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);

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

    public class FCSVEquipmentAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVEquipment);
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
    
        public class Adapter : FCSVEquipment, CrossBindingAdaptorType
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