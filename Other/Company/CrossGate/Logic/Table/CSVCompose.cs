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

	sealed public partial class CSVCompose : Framework.Table.TableBase<CSVCompose.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint drop_id;
			public readonly uint name_id;
			public readonly List<uint> RecyclingItem1;
			public readonly List<uint> RecyclingItem2;
			public readonly List<uint> RecyclingItem3;
			public readonly List<uint> RecyclingItem4;
			public readonly List<uint> RecyclingItem5;
			public readonly uint maxBatchCompse;
			public readonly uint SynthesisTab;
			public readonly uint Number;
			public readonly uint SeePet;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				drop_id = ReadHelper.ReadUInt(binaryReader);
				name_id = ReadHelper.ReadUInt(binaryReader);
				RecyclingItem1 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				RecyclingItem2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				RecyclingItem3 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				RecyclingItem4 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				RecyclingItem5 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				maxBatchCompse = ReadHelper.ReadUInt(binaryReader);
				SynthesisTab = ReadHelper.ReadUInt(binaryReader);
				Number = ReadHelper.ReadUInt(binaryReader);
				SeePet = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCompose.bytes";
		}

		private static CSVCompose instance = null;			
		public static CSVCompose Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCompose 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCompose forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCompose();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCompose");

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

    sealed public partial class CSVCompose : FCSVCompose
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCompose.bytes";
		}

		private static CSVCompose instance = null;			
		public static CSVCompose Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCompose 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCompose forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCompose();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCompose");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}