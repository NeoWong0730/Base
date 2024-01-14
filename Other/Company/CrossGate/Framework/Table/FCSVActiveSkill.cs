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
    public class FCSVActiveSkill
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint main_skill_id { get; private set; }
			public uint active_skill_behavior_id { get; private set; }
			public uint active_skill_lv { get; private set; }
			public uint new_behavior { get; private set; }
			public uint behavior_tool { get; private set; }
			public uint show_skill_name { get; private set; }
			public uint min_spirit { get; private set; }
			public uint skill_type { get; private set; }
			public uint no_mana_behavior { get; private set; }
			public uint have_behavior { get; private set; }
			public uint attack_range { get; private set; }
			public uint cold_time { get; private set; }
			public readonly List<uint> before_action;
			public uint buff_limit_condition { get; private set; }
			public uint buff_condition { get; private set; }
			public uint mana_cost { get; private set; }
			public uint energy_cost { get; private set; }
			public uint attack_type { get; private set; }
			public uint second_target { get; private set; }
			public uint min_attack_num { get; private set; }
			public uint max_attack_num { get; private set; }
			public readonly List<uint> skill_effect_id;
			public int choose_type { get; private set; }
			public readonly List<uint> choose_skill_condition;
			public readonly List<uint> choose_wrong_target;
			public readonly List<uint> choose_req;
			public uint choose_AI { get; private set; }


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				main_skill_id = ReadHelper.ReadUInt(binaryReader);
				active_skill_behavior_id = ReadHelper.ReadUInt(binaryReader);
				active_skill_lv = ReadHelper.ReadUInt(binaryReader);
				new_behavior = ReadHelper.ReadUInt(binaryReader);
				behavior_tool = ReadHelper.ReadUInt(binaryReader);
				show_skill_name = ReadHelper.ReadUInt(binaryReader);
				min_spirit = ReadHelper.ReadUInt(binaryReader);
				skill_type = ReadHelper.ReadUInt(binaryReader);
				no_mana_behavior = ReadHelper.ReadUInt(binaryReader);
				have_behavior = ReadHelper.ReadUInt(binaryReader);
				attack_range = ReadHelper.ReadUInt(binaryReader);
				cold_time = ReadHelper.ReadUInt(binaryReader);
				before_action = shareData.GetShareData<List<uint>>(binaryReader, 0);
				buff_limit_condition = ReadHelper.ReadUInt(binaryReader);
				buff_condition = ReadHelper.ReadUInt(binaryReader);
				mana_cost = ReadHelper.ReadUInt(binaryReader);
				energy_cost = ReadHelper.ReadUInt(binaryReader);
				attack_type = ReadHelper.ReadUInt(binaryReader);
				second_target = ReadHelper.ReadUInt(binaryReader);
				min_attack_num = ReadHelper.ReadUInt(binaryReader);
				max_attack_num = ReadHelper.ReadUInt(binaryReader);
				skill_effect_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				choose_type = ReadHelper.ReadInt(binaryReader);
				choose_skill_condition = shareData.GetShareData<List<uint>>(binaryReader, 0);
				choose_wrong_target = shareData.GetShareData<List<uint>>(binaryReader, 0);
				choose_req = shareData.GetShareData<List<uint>>(binaryReader, 0);
				choose_AI = ReadHelper.ReadUInt(binaryReader);

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

    public class FCSVActiveSkillAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVActiveSkill);
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
    
        public class Adapter : FCSVActiveSkill, CrossBindingAdaptorType
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