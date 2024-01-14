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

	sealed public partial class CSVHangup : Framework.Table.TableBase<CSVHangup.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint HangupName;
			public readonly uint HangupDes;
			public readonly List<byte> RecommendLv;
			public readonly uint MapID;
			public readonly uint IslandID;
			public readonly List<int> UIPosition;
			public readonly uint TeamID;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				HangupName = ReadHelper.ReadUInt(binaryReader);
				HangupDes = ReadHelper.ReadUInt(binaryReader);
				RecommendLv = shareData.GetShareData<List<byte>>(binaryReader, 0);
				MapID = ReadHelper.ReadUInt(binaryReader);
				IslandID = ReadHelper.ReadUInt(binaryReader);
				UIPosition = shareData.GetShareData<List<int>>(binaryReader, 1);
				TeamID = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVHangup.bytes";
		}

		private static CSVHangup instance = null;			
		public static CSVHangup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVHangup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVHangup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVHangup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVHangup");

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
			shareData.ReadArrays<byte>(binaryReader, 0, ReadHelper.ReadArray_ReadByte);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVHangup : FCSVHangup
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVHangup.bytes";
		}

		private static CSVHangup instance = null;			
		public static CSVHangup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVHangup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVHangup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVHangup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVHangup");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}