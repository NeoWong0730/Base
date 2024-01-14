//
#define USE_HOTFIX_LOGIC

using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Framework.Table;

namespace Table
{
#if USE_HOTFIX_LOGIC

	sealed public partial class CSVPartner : Framework.Table.TableBase<CSVPartner.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name;
			public readonly uint desc;
			public readonly uint headid;
			public readonly uint battle_headID;
			public readonly string model;
			public readonly string model_show;
			public readonly uint show_weapon_id;
			public readonly uint model_show_workid;
			public readonly uint profession;
			public readonly uint occupation;
			public readonly uint belonging;
			public readonly uint sort;
			public bool list_show { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint inti_attr;
			public readonly uint inti_level;
			public readonly uint auto_level;
			public readonly uint limit_level;
			public readonly uint auto_team;
			public readonly List<uint> open_unlock;
			public readonly uint open_unlock_info;
			public readonly uint deblock_type;
			public readonly List<uint> deblock_condition;
			public readonly uint npcID;
			public readonly uint Favorability_info;
			public readonly uint weaponID;
			public readonly uint SkillGroupID;
			public readonly string AIName;
			public readonly uint ai_mod;
			public readonly List<uint> attr_boost_id;
			public readonly uint attr_boost_limit;
			public readonly List<uint> ability_value;
			public readonly uint label;
			public readonly uint quality;
			public readonly int LeftLocationX;
			public readonly int LeftLocationY;
			public readonly int LeftLocationZ;
			public readonly int LeftLocationRotateX;
			public readonly int LeftLocationRotateY;
			public readonly int LeftLocationRotateZ;
			public readonly int LeftLocationMirrorImage;
			public readonly int RightLocationX;
			public readonly int RightLocationY;
			public readonly int RightLocationZ;
			public readonly int RightLocationRotateX;
			public readonly int RightLocationRotateY;
			public readonly int RightLocationRotateZ;
			public readonly int RightLocationMirrorImage;
			public readonly int BubbleLocationX;
			public readonly int BubbleLocationY;
			public readonly int BubbleLocationZ;
			public readonly int BubbleLocationRotateX;
			public readonly int BubbleLocationRotateY;
			public readonly int BubbleLocationRotateZ;
			public readonly int BubbleLocationMirrorImage;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				desc = ReadHelper.ReadUInt(binaryReader);
				headid = ReadHelper.ReadUInt(binaryReader);
				battle_headID = ReadHelper.ReadUInt(binaryReader);
				model = shareData.GetShareData<string>(binaryReader, 0);
				model_show = shareData.GetShareData<string>(binaryReader, 0);
				show_weapon_id = ReadHelper.ReadUInt(binaryReader);
				model_show_workid = ReadHelper.ReadUInt(binaryReader);
				profession = ReadHelper.ReadUInt(binaryReader);
				occupation = ReadHelper.ReadUInt(binaryReader);
				belonging = ReadHelper.ReadUInt(binaryReader);
				sort = ReadHelper.ReadUInt(binaryReader);
				inti_attr = ReadHelper.ReadUInt(binaryReader);
				inti_level = ReadHelper.ReadUInt(binaryReader);
				auto_level = ReadHelper.ReadUInt(binaryReader);
				limit_level = ReadHelper.ReadUInt(binaryReader);
				auto_team = ReadHelper.ReadUInt(binaryReader);
				open_unlock = shareData.GetShareData<List<uint>>(binaryReader, 1);
				open_unlock_info = ReadHelper.ReadUInt(binaryReader);
				deblock_type = ReadHelper.ReadUInt(binaryReader);
				deblock_condition = shareData.GetShareData<List<uint>>(binaryReader, 1);
				npcID = ReadHelper.ReadUInt(binaryReader);
				Favorability_info = ReadHelper.ReadUInt(binaryReader);
				weaponID = ReadHelper.ReadUInt(binaryReader);
				SkillGroupID = ReadHelper.ReadUInt(binaryReader);
				AIName = shareData.GetShareData<string>(binaryReader, 0);
				ai_mod = ReadHelper.ReadUInt(binaryReader);
				attr_boost_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				attr_boost_limit = ReadHelper.ReadUInt(binaryReader);
				ability_value = shareData.GetShareData<List<uint>>(binaryReader, 1);
				label = ReadHelper.ReadUInt(binaryReader);
				quality = ReadHelper.ReadUInt(binaryReader);
				LeftLocationX = ReadHelper.ReadInt(binaryReader);
				LeftLocationY = ReadHelper.ReadInt(binaryReader);
				LeftLocationZ = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateX = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateY = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				LeftLocationMirrorImage = ReadHelper.ReadInt(binaryReader);
				RightLocationX = ReadHelper.ReadInt(binaryReader);
				RightLocationY = ReadHelper.ReadInt(binaryReader);
				RightLocationZ = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateX = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateY = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				RightLocationMirrorImage = ReadHelper.ReadInt(binaryReader);
				BubbleLocationX = ReadHelper.ReadInt(binaryReader);
				BubbleLocationY = ReadHelper.ReadInt(binaryReader);
				BubbleLocationZ = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateX = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateY = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				BubbleLocationMirrorImage = ReadHelper.ReadInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPartner.bytes";
		}

		private static CSVPartner instance = null;			
		public static CSVPartner Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPartner 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPartner forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPartner();
                instance.ReadByFilePath(ConfigPath(), OnCreat, OnReadShareData);
            }
            else if (forceReload)
            {
                instance.Clear();
                instance.ReadByFilePath(ConfigPath(), OnCreat, OnReadShareData);
            }
        }

        public static void Unload()
        {
            DebugUtil.Log(ELogType.eTable, "卸载CSVPartner");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }

        private static Data OnCreat(uint id, BinaryReader binaryReader, TableShareData shareData)
        {
            Data data = new Data(id, binaryReader, shareData);
            return data;
        }

        private static TableShareData OnReadShareData(BinaryReader binaryReader)
		{
			TableShareData shareData = new TableShareData(2);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVPartner : FCSVPartner
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPartner.bytes";
		}

		private static CSVPartner instance = null;			
		public static CSVPartner Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPartner 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPartner forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPartner();
                instance.ReadByFilePath(ConfigPath());
            }
            else if (forceReload)
            {
                instance.Clear();
                instance.ReadByFilePath(ConfigPath());
            }
        }

        public static void Unload()
        {
            DebugUtil.Log(ELogType.eTable, "卸载CSVPartner");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}