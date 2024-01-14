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

	sealed public partial class CSVTeam : Framework.Table.TableBase<CSVTeam.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint seq_id;
			public readonly uint play_name;
			public readonly uint play_type;
			public readonly uint subclass_name;
			public readonly uint FunctionOpenId;
			public readonly uint subclass;
			public readonly uint module;
			public readonly uint OpeningLevel;
			public readonly uint HideLevel;
			public readonly uint lv_min;
			public readonly uint lv_max;
			public readonly uint play_desc;
			public readonly uint jump_id;
			public readonly uint is_come;
			public readonly uint is_show;
			public readonly uint is_captain;
			public bool only_guild { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				seq_id = ReadHelper.ReadUInt(binaryReader);
				play_name = ReadHelper.ReadUInt(binaryReader);
				play_type = ReadHelper.ReadUInt(binaryReader);
				subclass_name = ReadHelper.ReadUInt(binaryReader);
				FunctionOpenId = ReadHelper.ReadUInt(binaryReader);
				subclass = ReadHelper.ReadUInt(binaryReader);
				module = ReadHelper.ReadUInt(binaryReader);
				OpeningLevel = ReadHelper.ReadUInt(binaryReader);
				HideLevel = ReadHelper.ReadUInt(binaryReader);
				lv_min = ReadHelper.ReadUInt(binaryReader);
				lv_max = ReadHelper.ReadUInt(binaryReader);
				play_desc = ReadHelper.ReadUInt(binaryReader);
				jump_id = ReadHelper.ReadUInt(binaryReader);
				is_come = ReadHelper.ReadUInt(binaryReader);
				is_show = ReadHelper.ReadUInt(binaryReader);
				is_captain = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTeam.bytes";
		}

		private static CSVTeam instance = null;			
		public static CSVTeam Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTeam 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTeam forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTeam();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTeam");

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

    sealed public partial class CSVTeam : FCSVTeam
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTeam.bytes";
		}

		private static CSVTeam instance = null;			
		public static CSVTeam Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTeam 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTeam forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTeam();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTeam");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}