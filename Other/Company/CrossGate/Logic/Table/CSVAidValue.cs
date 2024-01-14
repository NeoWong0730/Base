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

	sealed public partial class CSVAidValue : Framework.Table.TableBase<CSVAidValue.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint type;
			public readonly uint param;
			public readonly List<uint> AidBasePoint;
			public readonly uint AidUpperLimit;
			public readonly uint AidLevel;
			public readonly int AidLevelScope;
			public readonly uint AidMultiple;
			public readonly uint condition_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				type = ReadHelper.ReadUInt(binaryReader);
				param = ReadHelper.ReadUInt(binaryReader);
				AidBasePoint = shareData.GetShareData<List<uint>>(binaryReader, 0);
				AidUpperLimit = ReadHelper.ReadUInt(binaryReader);
				AidLevel = ReadHelper.ReadUInt(binaryReader);
				AidLevelScope = ReadHelper.ReadInt(binaryReader);
				AidMultiple = ReadHelper.ReadUInt(binaryReader);
				condition_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAidValue.bytes";
		}

		private static CSVAidValue instance = null;			
		public static CSVAidValue Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAidValue 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAidValue forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAidValue();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAidValue");

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

    sealed public partial class CSVAidValue : FCSVAidValue
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAidValue.bytes";
		}

		private static CSVAidValue instance = null;			
		public static CSVAidValue Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAidValue 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAidValue forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAidValue();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAidValue");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}