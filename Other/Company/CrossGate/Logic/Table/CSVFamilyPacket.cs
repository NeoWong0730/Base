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

	sealed public partial class CSVFamilyPacket : Framework.Table.TableBase<CSVFamilyPacket.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Packet_Type;
			public readonly uint parameter1;
			public readonly uint parameter2;
			public readonly uint Item_Id;
			public readonly uint Item_Num;
			public readonly uint Packet_Part;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Packet_Type = ReadHelper.ReadUInt(binaryReader);
				parameter1 = ReadHelper.ReadUInt(binaryReader);
				parameter2 = ReadHelper.ReadUInt(binaryReader);
				Item_Id = ReadHelper.ReadUInt(binaryReader);
				Item_Num = ReadHelper.ReadUInt(binaryReader);
				Packet_Part = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPacket.bytes";
		}

		private static CSVFamilyPacket instance = null;			
		public static CSVFamilyPacket Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPacket 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPacket forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPacket();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPacket");

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

    sealed public partial class CSVFamilyPacket : FCSVFamilyPacket
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPacket.bytes";
		}

		private static CSVFamilyPacket instance = null;			
		public static CSVFamilyPacket Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPacket 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPacket forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPacket();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPacket");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}