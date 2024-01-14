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
    public class FCSVPetNew
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint name { get; private set; }
			public uint PetBooks { get; private set; }
			public byte card_type { get; private set; }
			public byte card_lv { get; private set; }
			public byte race { get; private set; }
			public bool reborn { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool mount { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public byte mountgrade { get; private set; }
			public readonly List<int> mountsignposition;
			public bool show_pet { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public bool lose_loyalty { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
			public uint icon_id { get; private set; }
			public uint action_id { get; private set; }
			public uint action_id_show { get; private set; }
			public uint action_id_mount { get; private set; }
			public readonly string model;
			public readonly string model_show;
			public uint bust { get; private set; }
			public uint weapon { get; private set; }
			public byte participation_lv { get; private set; }
			public readonly List<List<uint>> init_attr;
			public uint attr_id { get; private set; }
			public uint init_score { get; private set; }
			public readonly List<uint> quality_score;
			public byte max_lost_gear { get; private set; }
			public readonly List<uint> gear_param;
			public byte endurance { get; private set; }
			public byte strength { get; private set; }
			public byte strong { get; private set; }
			public byte speed { get; private set; }
			public byte magic { get; private set; }
			public byte skill_num { get; private set; }
			public readonly List<List<uint>> required_skills;
			public readonly List<List<uint>> unique_skills;
			public readonly List<uint> remake_skills;
			public byte first_remake_num { get; private set; }
			public byte max_remake_num { get; private set; }
			public readonly string action_idle;
			public uint shadow { get; private set; }
			public float translation { get; private set; }
			public float angle1 { get; private set; }
			public float angle2 { get; private set; }
			public float angle3 { get; private set; }
			public float height { get; private set; }
			public float size { get; private set; }
			public uint haunted_area { get; private set; }
			public readonly List<uint> activity;
			public readonly List<uint> location;
			public readonly List<uint> another_location;
			public readonly List<List<uint>> add_point_num;
			public uint zooming { get; private set; }
			public uint is_follow { get; private set; }
			public float follow_distance { get; private set; }
			public uint map_show { get; private set; }
			public bool search_pet { get { return ReadHelper.GetBoolByIndex(boolArray0, 4); } }
			public readonly List<string> SubPackageShow;
			public uint is_defauit { get; private set; }
			public uint PetBooks_is_act { get; private set; }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				PetBooks = ReadHelper.ReadUInt(binaryReader);
				card_type = ReadHelper.ReadByte(binaryReader);
				card_lv = ReadHelper.ReadByte(binaryReader);
				race = ReadHelper.ReadByte(binaryReader);
				mountgrade = ReadHelper.ReadByte(binaryReader);
				mountsignposition = shareData.GetShareData<List<int>>(binaryReader, 1);
				icon_id = ReadHelper.ReadUInt(binaryReader);
				action_id = ReadHelper.ReadUInt(binaryReader);
				action_id_show = ReadHelper.ReadUInt(binaryReader);
				action_id_mount = ReadHelper.ReadUInt(binaryReader);
				model = shareData.GetShareData<string>(binaryReader, 0);
				model_show = shareData.GetShareData<string>(binaryReader, 0);
				bust = ReadHelper.ReadUInt(binaryReader);
				weapon = ReadHelper.ReadUInt(binaryReader);
				participation_lv = ReadHelper.ReadByte(binaryReader);
				init_attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				attr_id = ReadHelper.ReadUInt(binaryReader);
				init_score = ReadHelper.ReadUInt(binaryReader);
				quality_score = shareData.GetShareData<List<uint>>(binaryReader, 2);
				max_lost_gear = ReadHelper.ReadByte(binaryReader);
				gear_param = shareData.GetShareData<List<uint>>(binaryReader, 2);
				endurance = ReadHelper.ReadByte(binaryReader);
				strength = ReadHelper.ReadByte(binaryReader);
				strong = ReadHelper.ReadByte(binaryReader);
				speed = ReadHelper.ReadByte(binaryReader);
				magic = ReadHelper.ReadByte(binaryReader);
				skill_num = ReadHelper.ReadByte(binaryReader);
				required_skills = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				unique_skills = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				remake_skills = shareData.GetShareData<List<uint>>(binaryReader, 2);
				first_remake_num = ReadHelper.ReadByte(binaryReader);
				max_remake_num = ReadHelper.ReadByte(binaryReader);
				action_idle = shareData.GetShareData<string>(binaryReader, 0);
				shadow = ReadHelper.ReadUInt(binaryReader);
				translation = ReadHelper.ReadFloat(binaryReader);
				angle1 = ReadHelper.ReadFloat(binaryReader);
				angle2 = ReadHelper.ReadFloat(binaryReader);
				angle3 = ReadHelper.ReadFloat(binaryReader);
				height = ReadHelper.ReadFloat(binaryReader);
				size = ReadHelper.ReadFloat(binaryReader);
				haunted_area = ReadHelper.ReadUInt(binaryReader);
				activity = shareData.GetShareData<List<uint>>(binaryReader, 2);
				location = shareData.GetShareData<List<uint>>(binaryReader, 2);
				another_location = shareData.GetShareData<List<uint>>(binaryReader, 2);
				add_point_num = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				zooming = ReadHelper.ReadUInt(binaryReader);
				is_follow = ReadHelper.ReadUInt(binaryReader);
				follow_distance = ReadHelper.ReadFloat(binaryReader);
				map_show = ReadHelper.ReadUInt(binaryReader);
				SubPackageShow = shareData.GetShareData<List<string>>(binaryReader, 3);
				is_defauit = ReadHelper.ReadUInt(binaryReader);
				PetBooks_is_act = ReadHelper.ReadUInt(binaryReader);

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
			TableShareData shareData = new TableShareData(5);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArrays<uint>(binaryReader, 2, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadStringArrays(binaryReader, 3, 0);
			shareData.ReadArray2s<uint>(binaryReader, 4, 2);

            return shareData;
        }
    }

    public class FCSVPetNewAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVPetNew);
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
    
        public class Adapter : FCSVPetNew, CrossBindingAdaptorType
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