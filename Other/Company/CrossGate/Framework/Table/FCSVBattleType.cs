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
    public class FCSVBattleType
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint battle_type { get; private set; }
			public uint battle_type_param { get; private set; }
			public uint is_escape { get; private set; }
			public uint IntoAnimationTime { get; private set; }
			public uint battle_end_recovery { get; private set; }
			public uint victory_condition_type { get; private set; }
			public readonly List<uint> victory_condition;
			public uint auto_fight_mode { get; private set; }
			public uint setup_time { get; private set; }
			public bool is_seal { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool is_order_cancel { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public uint init_show_speed { get; private set; }
			public bool is_speed_up { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public uint show_boss_id { get; private set; }
			public bool Auto_Fight_Func { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
			public uint battle_type_name { get; private set; }
			public bool show_self_hp { get { return ReadHelper.GetBoolByIndex(boolArray0, 4); } }
			public bool show_self_mp { get { return ReadHelper.GetBoolByIndex(boolArray0, 5); } }
			public bool show_enemy_hp { get { return ReadHelper.GetBoolByIndex(boolArray0, 6); } }
			public bool show_enemy_mp { get { return ReadHelper.GetBoolByIndex(boolArray0, 7); } }
			public bool show_self_BuffHUD { get { return ReadHelper.GetBoolByIndex(boolArray1, 0); } }
			public bool show_enemy_BuffHUD { get { return ReadHelper.GetBoolByIndex(boolArray1, 1); } }
			public bool show_self_name { get { return ReadHelper.GetBoolByIndex(boolArray1, 2); } }
			public bool show_enemy_name { get { return ReadHelper.GetBoolByIndex(boolArray1, 3); } }
			public bool show_UI_hp { get { return ReadHelper.GetBoolByIndex(boolArray1, 4); } }
			public bool show_instruct_info { get { return ReadHelper.GetBoolByIndex(boolArray1, 5); } }
			public bool show_self_element { get { return ReadHelper.GetBoolByIndex(boolArray1, 6); } }
			public bool show_enemy_element { get { return ReadHelper.GetBoolByIndex(boolArray1, 7); } }
			public bool mirror_position { get { return ReadHelper.GetBoolByIndex(boolArray2, 0); } }
			public uint position_type { get; private set; }
			public bool running_enter_self { get { return ReadHelper.GetBoolByIndex(boolArray2, 1); } }
			public uint enter_battle_effect { get; private set; }
			public uint exit_battle_effect { get; private set; }
			public bool is_quickfight { get { return ReadHelper.GetBoolByIndex(boolArray2, 2); } }
			public uint battle_end_workid { get; private set; }
			public uint barrage { get; private set; }
			public uint ob { get; private set; }
			public uint CrystalDurability { get; private set; }
			public uint durability { get; private set; }
			public uint MountEnergy { get; private set; }
			public readonly List<uint> normal_medic;
			public uint normal_medic_num { get; private set; }
			public readonly List<uint> special_medic;
			public uint special_medic_num { get; private set; }
			public uint pet_battletimes { get; private set; }
			public readonly List<uint> forbid_medic;
			public bool proficiency { get { return ReadHelper.GetBoolByIndex(boolArray2, 3); } }
			public readonly List<uint> AidBasePoint;
			public uint AidUpperLimit { get; private set; }
			public uint AidLevel { get; private set; }
			public int AidLevelScope { get; private set; }
			public uint AidMultiple { get; private set; }
			public readonly List<uint> CaptainBasePoint;
			public uint CaptainUpperLimit { get; private set; }
			public uint battle_bgm { get; private set; }
			public uint battle_Bubble { get; private set; }
			public uint CloseReward { get; private set; }
			public uint Battle_Watch { get; private set; }
			public uint enter_battle_effectON { get; private set; }
			public uint enter_battle_effectOFF { get; private set; }
			public uint exit_battle_effectON { get; private set; }
			public uint exit_battle_effectOFF { get; private set; }
			public uint enter_battle_bgmON { get; private set; }
			public uint enter_battle_bgmOFF { get; private set; }
			public uint block_family_advice { get; private set; }
			public uint block_friend_advice { get; private set; }
			public uint videoFirst { get; private set; }
		private readonly byte boolArray0;
		private readonly byte boolArray1;
		private readonly byte boolArray2;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				battle_type = ReadHelper.ReadUInt(binaryReader);
				battle_type_param = ReadHelper.ReadUInt(binaryReader);
				is_escape = ReadHelper.ReadUInt(binaryReader);
				IntoAnimationTime = ReadHelper.ReadUInt(binaryReader);
				battle_end_recovery = ReadHelper.ReadUInt(binaryReader);
				victory_condition_type = ReadHelper.ReadUInt(binaryReader);
				victory_condition = shareData.GetShareData<List<uint>>(binaryReader, 0);
				auto_fight_mode = ReadHelper.ReadUInt(binaryReader);
				setup_time = ReadHelper.ReadUInt(binaryReader);
				init_show_speed = ReadHelper.ReadUInt(binaryReader);
				show_boss_id = ReadHelper.ReadUInt(binaryReader);
				battle_type_name = ReadHelper.ReadUInt(binaryReader);
				position_type = ReadHelper.ReadUInt(binaryReader);
				enter_battle_effect = ReadHelper.ReadUInt(binaryReader);
				exit_battle_effect = ReadHelper.ReadUInt(binaryReader);
				battle_end_workid = ReadHelper.ReadUInt(binaryReader);
				barrage = ReadHelper.ReadUInt(binaryReader);
				ob = ReadHelper.ReadUInt(binaryReader);
				CrystalDurability = ReadHelper.ReadUInt(binaryReader);
				durability = ReadHelper.ReadUInt(binaryReader);
				MountEnergy = ReadHelper.ReadUInt(binaryReader);
				normal_medic = shareData.GetShareData<List<uint>>(binaryReader, 0);
				normal_medic_num = ReadHelper.ReadUInt(binaryReader);
				special_medic = shareData.GetShareData<List<uint>>(binaryReader, 0);
				special_medic_num = ReadHelper.ReadUInt(binaryReader);
				pet_battletimes = ReadHelper.ReadUInt(binaryReader);
				forbid_medic = shareData.GetShareData<List<uint>>(binaryReader, 0);
				AidBasePoint = shareData.GetShareData<List<uint>>(binaryReader, 0);
				AidUpperLimit = ReadHelper.ReadUInt(binaryReader);
				AidLevel = ReadHelper.ReadUInt(binaryReader);
				AidLevelScope = ReadHelper.ReadInt(binaryReader);
				AidMultiple = ReadHelper.ReadUInt(binaryReader);
				CaptainBasePoint = shareData.GetShareData<List<uint>>(binaryReader, 0);
				CaptainUpperLimit = ReadHelper.ReadUInt(binaryReader);
				battle_bgm = ReadHelper.ReadUInt(binaryReader);
				battle_Bubble = ReadHelper.ReadUInt(binaryReader);
				CloseReward = ReadHelper.ReadUInt(binaryReader);
				Battle_Watch = ReadHelper.ReadUInt(binaryReader);
				enter_battle_effectON = ReadHelper.ReadUInt(binaryReader);
				enter_battle_effectOFF = ReadHelper.ReadUInt(binaryReader);
				exit_battle_effectON = ReadHelper.ReadUInt(binaryReader);
				exit_battle_effectOFF = ReadHelper.ReadUInt(binaryReader);
				enter_battle_bgmON = ReadHelper.ReadUInt(binaryReader);
				enter_battle_bgmOFF = ReadHelper.ReadUInt(binaryReader);
				block_family_advice = ReadHelper.ReadUInt(binaryReader);
				block_friend_advice = ReadHelper.ReadUInt(binaryReader);
				videoFirst = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
			boolArray1 = ReadHelper.ReadByte(binaryReader);
			boolArray2 = ReadHelper.ReadByte(binaryReader);
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

    public class FCSVBattleTypeAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVBattleType);
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
    
        public class Adapter : FCSVBattleType, CrossBindingAdaptorType
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