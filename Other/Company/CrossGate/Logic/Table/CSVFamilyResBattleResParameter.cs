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

	sealed public partial class CSVFamilyResBattleResParameter : Framework.Table.TableBase<CSVFamilyResBattleResParameter.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint WhetherExist;
			public readonly uint RefreshInterval;
			public readonly uint GetResource;
			public readonly uint GetPersonalScore;
			public readonly uint NpcId;
			public readonly uint NameID;
			public readonly uint IconID;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				WhetherExist = ReadHelper.ReadUInt(binaryReader);
				RefreshInterval = ReadHelper.ReadUInt(binaryReader);
				GetResource = ReadHelper.ReadUInt(binaryReader);
				GetPersonalScore = ReadHelper.ReadUInt(binaryReader);
				NpcId = ReadHelper.ReadUInt(binaryReader);
				NameID = ReadHelper.ReadUInt(binaryReader);
				IconID = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyResBattleResParameter.bytes";
		}

		private static CSVFamilyResBattleResParameter instance = null;			
		public static CSVFamilyResBattleResParameter Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyResBattleResParameter 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyResBattleResParameter forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyResBattleResParameter();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyResBattleResParameter");

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
			TableShareData shareData = null;

			return shareData;
		}
	}

#else

    sealed public partial class CSVFamilyResBattleResParameter : FCSVFamilyResBattleResParameter
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyResBattleResParameter.bytes";
		}

		private static CSVFamilyResBattleResParameter instance = null;			
		public static CSVFamilyResBattleResParameter Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyResBattleResParameter 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyResBattleResParameter forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyResBattleResParameter();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyResBattleResParameter");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}