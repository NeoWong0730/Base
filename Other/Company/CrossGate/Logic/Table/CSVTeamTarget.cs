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

	sealed public partial class CSVTeamTarget : Framework.Table.TableBase<CSVTeamTarget.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint play_name;
			public readonly uint client_in;
			public readonly List<uint> param1;
			public readonly uint target_in;
			public readonly uint client_out;
			public readonly List<uint> param2;
			public readonly uint target_out;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				play_name = ReadHelper.ReadUInt(binaryReader);
				client_in = ReadHelper.ReadUInt(binaryReader);
				param1 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				target_in = ReadHelper.ReadUInt(binaryReader);
				client_out = ReadHelper.ReadUInt(binaryReader);
				param2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				target_out = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTeamTarget.bytes";
		}

		private static CSVTeamTarget instance = null;			
		public static CSVTeamTarget Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTeamTarget 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTeamTarget forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTeamTarget();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTeamTarget");

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

    sealed public partial class CSVTeamTarget : FCSVTeamTarget
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTeamTarget.bytes";
		}

		private static CSVTeamTarget instance = null;			
		public static CSVTeamTarget Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTeamTarget 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTeamTarget forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTeamTarget();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTeamTarget");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}