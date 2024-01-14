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

	sealed public partial class CSVOperationalActivityRuler : Framework.Table.TableBase<CSVOperationalActivityRuler.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activity_Name;
			public readonly uint Activity_Type;
			public readonly uint Activity_Switch;
			public readonly uint Product_Type;
			public readonly uint Begining_Date;
			public readonly uint Duration_Day;
			public readonly uint Open_Bottom;
			public readonly uint RollingShow_Id;
			public readonly uint Rolling_Time;
			public readonly uint RollingLoop_Time;
			public readonly List<uint> Server_Switch;
			public readonly string Back_Image;
			public readonly string Foreg_Image;
			public readonly string PreformId;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_Name = ReadHelper.ReadUInt(binaryReader);
				Activity_Type = ReadHelper.ReadUInt(binaryReader);
				Activity_Switch = ReadHelper.ReadUInt(binaryReader);
				Product_Type = ReadHelper.ReadUInt(binaryReader);
				Begining_Date = ReadHelper.ReadUInt(binaryReader);
				Duration_Day = ReadHelper.ReadUInt(binaryReader);
				Open_Bottom = ReadHelper.ReadUInt(binaryReader);
				RollingShow_Id = ReadHelper.ReadUInt(binaryReader);
				Rolling_Time = ReadHelper.ReadUInt(binaryReader);
				RollingLoop_Time = ReadHelper.ReadUInt(binaryReader);
				Server_Switch = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Back_Image = shareData.GetShareData<string>(binaryReader, 0);
				Foreg_Image = shareData.GetShareData<string>(binaryReader, 0);
				PreformId = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVOperationalActivityRuler.bytes";
		}

		private static CSVOperationalActivityRuler instance = null;			
		public static CSVOperationalActivityRuler Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVOperationalActivityRuler 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVOperationalActivityRuler forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVOperationalActivityRuler();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVOperationalActivityRuler");

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

    sealed public partial class CSVOperationalActivityRuler : FCSVOperationalActivityRuler
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVOperationalActivityRuler.bytes";
		}

		private static CSVOperationalActivityRuler instance = null;			
		public static CSVOperationalActivityRuler Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVOperationalActivityRuler 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVOperationalActivityRuler forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVOperationalActivityRuler();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVOperationalActivityRuler");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}