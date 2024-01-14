//
//#define USE_HOTFIX_LOGIC

using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Framework.Table;

namespace Table
{
#if USE_HOTFIX_LOGIC

	sealed public partial class CSVTalent : Framework.Table.TableBase<CSVTalent.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint branch_id;
			public readonly uint branch_lan;
			public readonly uint branchIconId;
			public readonly uint lev;
			public readonly uint career_id;
			public readonly uint name_lan;
			public readonly uint icon_id;
			public readonly List<uint> skill_id;
			public readonly uint pre_unm;
			public readonly List<uint> position;
			public readonly List<List<uint>> pre_skill;
			public readonly List<uint> score;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				branch_id = ReadHelper.ReadUInt(binaryReader);
				branch_lan = ReadHelper.ReadUInt(binaryReader);
				branchIconId = ReadHelper.ReadUInt(binaryReader);
				lev = ReadHelper.ReadUInt(binaryReader);
				career_id = ReadHelper.ReadUInt(binaryReader);
				name_lan = ReadHelper.ReadUInt(binaryReader);
				icon_id = ReadHelper.ReadUInt(binaryReader);
				skill_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				pre_unm = ReadHelper.ReadUInt(binaryReader);
				position = shareData.GetShareData<List<uint>>(binaryReader, 0);
				pre_skill = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				score = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTalent.bytes";
		}

		private static CSVTalent instance = null;			
		public static CSVTalent Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTalent 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTalent forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTalent();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTalent");

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

    sealed public partial class CSVTalent : FCSVTalent
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTalent.bytes";
		}

		private static CSVTalent instance = null;			
		public static CSVTalent Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTalent 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTalent forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTalent();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTalent");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}