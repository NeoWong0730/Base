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

	sealed public partial class CSVPedigreedDraw : Framework.Table.TableBase<CSVPedigreedDraw.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activity_ID;
			public readonly uint Date;
			public readonly uint Special_Signs;
			public readonly List<uint> Reward_ID;
			public readonly uint Show_Item;
			public readonly List<uint> Task_ID;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_ID = ReadHelper.ReadUInt(binaryReader);
				Date = ReadHelper.ReadUInt(binaryReader);
				Special_Signs = ReadHelper.ReadUInt(binaryReader);
				Reward_ID = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Show_Item = ReadHelper.ReadUInt(binaryReader);
				Task_ID = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPedigreedDraw.bytes";
		}

		private static CSVPedigreedDraw instance = null;			
		public static CSVPedigreedDraw Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPedigreedDraw 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPedigreedDraw forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPedigreedDraw();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPedigreedDraw");

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

    sealed public partial class CSVPedigreedDraw : FCSVPedigreedDraw
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPedigreedDraw.bytes";
		}

		private static CSVPedigreedDraw instance = null;			
		public static CSVPedigreedDraw Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPedigreedDraw 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPedigreedDraw forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPedigreedDraw();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPedigreedDraw");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}