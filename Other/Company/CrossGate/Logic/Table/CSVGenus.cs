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

	sealed public partial class CSVGenus : Framework.Table.TableBase<CSVGenus.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> damageRate;
			public readonly uint damageRate1;
			public readonly uint damageRate2;
			public readonly uint damageRate3;
			public readonly uint damageRate4;
			public readonly uint damageRate5;
			public readonly uint damageRate6;
			public readonly uint damageRate7;
			public readonly uint damageRate8;
			public readonly uint damageRate9;
			public readonly uint damageRate10;
			public readonly uint rale_icon;
			public readonly uint rale_name;
			public readonly string back_ground;
			public readonly uint sort;
			public readonly uint race_change_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				damageRate = shareData.GetShareData<List<uint>>(binaryReader, 1);
				damageRate1 = ReadHelper.ReadUInt(binaryReader);
				damageRate2 = ReadHelper.ReadUInt(binaryReader);
				damageRate3 = ReadHelper.ReadUInt(binaryReader);
				damageRate4 = ReadHelper.ReadUInt(binaryReader);
				damageRate5 = ReadHelper.ReadUInt(binaryReader);
				damageRate6 = ReadHelper.ReadUInt(binaryReader);
				damageRate7 = ReadHelper.ReadUInt(binaryReader);
				damageRate8 = ReadHelper.ReadUInt(binaryReader);
				damageRate9 = ReadHelper.ReadUInt(binaryReader);
				damageRate10 = ReadHelper.ReadUInt(binaryReader);
				rale_icon = ReadHelper.ReadUInt(binaryReader);
				rale_name = ReadHelper.ReadUInt(binaryReader);
				back_ground = shareData.GetShareData<string>(binaryReader, 0);
				sort = ReadHelper.ReadUInt(binaryReader);
				race_change_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVGenus.bytes";
		}

		private static CSVGenus instance = null;			
		public static CSVGenus Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGenus 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGenus forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGenus();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGenus");

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

    sealed public partial class CSVGenus : FCSVGenus
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVGenus.bytes";
		}

		private static CSVGenus instance = null;			
		public static CSVGenus Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGenus 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGenus forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGenus();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGenus");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}