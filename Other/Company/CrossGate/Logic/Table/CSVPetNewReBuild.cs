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

	sealed public partial class CSVPetNewReBuild : Framework.Table.TableBase<CSVPetNewReBuild.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint need_pet_lv;
			public readonly uint remake_hint;
			public readonly uint gold_remake_hint;
			public readonly uint max_skill;
			public readonly uint present_max_skill;
			public readonly List<List<uint>> reset_cost;
			public readonly uint skill_success_rate;
			public readonly List<List<uint>> skill_cost;
			public readonly List<List<uint>> senior_skill_cost;
			public readonly List<uint> grade_need;
			public readonly ushort max_luck;
			public readonly ushort remake_value;
			public readonly uint remake_lan;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				need_pet_lv = ReadHelper.ReadUInt(binaryReader);
				remake_hint = ReadHelper.ReadUInt(binaryReader);
				gold_remake_hint = ReadHelper.ReadUInt(binaryReader);
				max_skill = ReadHelper.ReadUInt(binaryReader);
				present_max_skill = ReadHelper.ReadUInt(binaryReader);
				reset_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				skill_success_rate = ReadHelper.ReadUInt(binaryReader);
				skill_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				senior_skill_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				grade_need = shareData.GetShareData<List<uint>>(binaryReader, 0);
				max_luck = ReadHelper.ReadUShort(binaryReader);
				remake_value = ReadHelper.ReadUShort(binaryReader);
				remake_lan = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetNewReBuild.bytes";
		}

		private static CSVPetNewReBuild instance = null;			
		public static CSVPetNewReBuild Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetNewReBuild 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetNewReBuild forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetNewReBuild();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetNewReBuild");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVPetNewReBuild : FCSVPetNewReBuild
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetNewReBuild.bytes";
		}

		private static CSVPetNewReBuild instance = null;			
		public static CSVPetNewReBuild Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetNewReBuild 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetNewReBuild forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetNewReBuild();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetNewReBuild");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}