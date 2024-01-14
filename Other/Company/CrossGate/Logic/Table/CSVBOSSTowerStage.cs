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

	sealed public partial class CSVBOSSTowerStage : Framework.Table.TableBase<CSVBOSSTowerStage.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name;
			public readonly uint tower_id;
			public readonly uint stage_number;
			public readonly uint nextFloor_id;
			public readonly uint floor_drop;
			public readonly uint levelGrade_id;
			public readonly List<uint> levelGrade_taxt;
			public readonly int recommondPoint;
			public readonly List<uint> playerLimit;
			public readonly uint diffcultyDetails;
			public readonly uint battle_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				tower_id = ReadHelper.ReadUInt(binaryReader);
				stage_number = ReadHelper.ReadUInt(binaryReader);
				nextFloor_id = ReadHelper.ReadUInt(binaryReader);
				floor_drop = ReadHelper.ReadUInt(binaryReader);
				levelGrade_id = ReadHelper.ReadUInt(binaryReader);
				levelGrade_taxt = shareData.GetShareData<List<uint>>(binaryReader, 0);
				recommondPoint = ReadHelper.ReadInt(binaryReader);
				playerLimit = shareData.GetShareData<List<uint>>(binaryReader, 0);
				diffcultyDetails = ReadHelper.ReadUInt(binaryReader);
				battle_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBOSSTowerStage.bytes";
		}

		private static CSVBOSSTowerStage instance = null;			
		public static CSVBOSSTowerStage Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBOSSTowerStage 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBOSSTowerStage forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBOSSTowerStage();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBOSSTowerStage");

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

    sealed public partial class CSVBOSSTowerStage : FCSVBOSSTowerStage
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBOSSTowerStage.bytes";
		}

		private static CSVBOSSTowerStage instance = null;			
		public static CSVBOSSTowerStage Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBOSSTowerStage 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBOSSTowerStage forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBOSSTowerStage();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBOSSTowerStage");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}