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

	sealed public partial class CSVMonsterPvp : Framework.Table.TableBase<CSVMonsterPvp.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public bool team_sign_type { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint position;
			public readonly uint type;
			public readonly uint icon;
			public readonly uint career_show;
			public readonly uint career;
			public readonly uint fame_lv;
			public readonly List<uint> skill;
			public readonly List<uint> passiv;
			public readonly List<uint> pet_list;
			public readonly List<uint> blink_list;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				position = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				career_show = ReadHelper.ReadUInt(binaryReader);
				career = ReadHelper.ReadUInt(binaryReader);
				fame_lv = ReadHelper.ReadUInt(binaryReader);
				skill = shareData.GetShareData<List<uint>>(binaryReader, 0);
				passiv = shareData.GetShareData<List<uint>>(binaryReader, 0);
				pet_list = shareData.GetShareData<List<uint>>(binaryReader, 0);
				blink_list = shareData.GetShareData<List<uint>>(binaryReader, 0);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVMonsterPvp.bytes";
		}

		private static CSVMonsterPvp instance = null;			
		public static CSVMonsterPvp Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMonsterPvp 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMonsterPvp forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMonsterPvp();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMonsterPvp");

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

    sealed public partial class CSVMonsterPvp : FCSVMonsterPvp
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVMonsterPvp.bytes";
		}

		private static CSVMonsterPvp instance = null;			
		public static CSVMonsterPvp Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMonsterPvp 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMonsterPvp forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMonsterPvp();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMonsterPvp");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}