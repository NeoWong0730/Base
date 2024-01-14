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

	sealed public partial class CSVVitality : Framework.Table.TableBase<CSVVitality.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> Way;
			public readonly List<uint> Level;
			public readonly uint Icon;
			public readonly uint Name;
			public readonly uint Description;
			public readonly uint Button;
			public readonly uint Type;
			public readonly string Prarameter;
			public readonly List<uint> Shop;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Way = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Level = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Icon = ReadHelper.ReadUInt(binaryReader);
				Name = ReadHelper.ReadUInt(binaryReader);
				Description = ReadHelper.ReadUInt(binaryReader);
				Button = ReadHelper.ReadUInt(binaryReader);
				Type = ReadHelper.ReadUInt(binaryReader);
				Prarameter = shareData.GetShareData<string>(binaryReader, 0);
				Shop = shareData.GetShareData<List<uint>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVVitality.bytes";
		}

		private static CSVVitality instance = null;			
		public static CSVVitality Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVVitality 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVVitality forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVVitality();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVVitality");

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

    sealed public partial class CSVVitality : FCSVVitality
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVVitality.bytes";
		}

		private static CSVVitality instance = null;			
		public static CSVVitality Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVVitality 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVVitality forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVVitality();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVVitality");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}