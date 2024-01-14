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

	sealed public partial class CSVActiveSkillShowId : Framework.Table.TableBase<CSVActiveSkillShowId.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint skillid;
			public readonly uint attack_type;
			public readonly uint value;
			public readonly uint damage_type;
			public readonly uint hit_to_fly;
			public readonly uint is_dead;
			public readonly List<List<uint>> buffid;
			public readonly List<uint> attack_nums;
			public readonly List<uint> target_nums;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				skillid = ReadHelper.ReadUInt(binaryReader);
				attack_type = ReadHelper.ReadUInt(binaryReader);
				value = ReadHelper.ReadUInt(binaryReader);
				damage_type = ReadHelper.ReadUInt(binaryReader);
				hit_to_fly = ReadHelper.ReadUInt(binaryReader);
				is_dead = ReadHelper.ReadUInt(binaryReader);
				buffid = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				attack_nums = shareData.GetShareData<List<uint>>(binaryReader, 0);
				target_nums = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVActiveSkillShowId.bytes";
		}

		private static CSVActiveSkillShowId instance = null;			
		public static CSVActiveSkillShowId Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActiveSkillShowId 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActiveSkillShowId forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActiveSkillShowId();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActiveSkillShowId");

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

    sealed public partial class CSVActiveSkillShowId : FCSVActiveSkillShowId
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVActiveSkillShowId.bytes";
		}

		private static CSVActiveSkillShowId instance = null;			
		public static CSVActiveSkillShowId Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActiveSkillShowId 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActiveSkillShowId forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActiveSkillShowId();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActiveSkillShowId");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}