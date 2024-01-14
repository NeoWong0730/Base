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

	sealed public partial class CSVFamilyPacketLimit : Framework.Table.TableBase<CSVFamilyPacketLimit.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Cumulative_Amount_Dowm;
			public readonly uint Item_Id;
			public readonly uint Item_Num;
			public readonly uint Packet_Day_Quota;
			public readonly uint Packet_Week_Quota;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Cumulative_Amount_Dowm = ReadHelper.ReadUInt(binaryReader);
				Item_Id = ReadHelper.ReadUInt(binaryReader);
				Item_Num = ReadHelper.ReadUInt(binaryReader);
				Packet_Day_Quota = ReadHelper.ReadUInt(binaryReader);
				Packet_Week_Quota = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPacketLimit.bytes";
		}

		private static CSVFamilyPacketLimit instance = null;			
		public static CSVFamilyPacketLimit Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPacketLimit 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPacketLimit forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPacketLimit();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPacketLimit");

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

    sealed public partial class CSVFamilyPacketLimit : FCSVFamilyPacketLimit
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPacketLimit.bytes";
		}

		private static CSVFamilyPacketLimit instance = null;			
		public static CSVFamilyPacketLimit Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPacketLimit 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPacketLimit forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPacketLimit();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPacketLimit");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}