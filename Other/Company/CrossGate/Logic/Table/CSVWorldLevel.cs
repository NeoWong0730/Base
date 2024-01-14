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

	sealed public partial class CSVWorldLevel : Framework.Table.TableBase<CSVWorldLevel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint world_level;
			public readonly List<uint> up_level;
			public readonly List<uint> up_percent;
			public readonly List<uint> down_level;
			public readonly List<uint> down_percent;
			public readonly uint LvRepuTotal;
			public readonly uint LvRecom;
			public readonly uint RepuRecom;
			public readonly uint TalentRecom;
			public readonly uint TalentPoint;
			public readonly uint PracRecom;
			public readonly uint TameRecom;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				world_level = ReadHelper.ReadUInt(binaryReader);
				up_level = shareData.GetShareData<List<uint>>(binaryReader, 0);
				up_percent = shareData.GetShareData<List<uint>>(binaryReader, 0);
				down_level = shareData.GetShareData<List<uint>>(binaryReader, 0);
				down_percent = shareData.GetShareData<List<uint>>(binaryReader, 0);
				LvRepuTotal = ReadHelper.ReadUInt(binaryReader);
				LvRecom = ReadHelper.ReadUInt(binaryReader);
				RepuRecom = ReadHelper.ReadUInt(binaryReader);
				TalentRecom = ReadHelper.ReadUInt(binaryReader);
				TalentPoint = ReadHelper.ReadUInt(binaryReader);
				PracRecom = ReadHelper.ReadUInt(binaryReader);
				TameRecom = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVWorldLevel.bytes";
		}

		private static CSVWorldLevel instance = null;			
		public static CSVWorldLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVWorldLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVWorldLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVWorldLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVWorldLevel");

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
			TableShareData shareData = new TableShareData(1);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVWorldLevel : FCSVWorldLevel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVWorldLevel.bytes";
		}

		private static CSVWorldLevel instance = null;			
		public static CSVWorldLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVWorldLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVWorldLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVWorldLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVWorldLevel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}