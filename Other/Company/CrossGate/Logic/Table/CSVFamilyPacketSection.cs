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

	sealed public partial class CSVFamilyPacketSection : Framework.Table.TableBase<CSVFamilyPacketSection.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Packet_Mix;
			public readonly uint Packet_Max;
			public readonly uint Packet_Part;
			public readonly uint Packet_Extra_Part;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Packet_Mix = ReadHelper.ReadUInt(binaryReader);
				Packet_Max = ReadHelper.ReadUInt(binaryReader);
				Packet_Part = ReadHelper.ReadUInt(binaryReader);
				Packet_Extra_Part = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPacketSection.bytes";
		}

		private static CSVFamilyPacketSection instance = null;			
		public static CSVFamilyPacketSection Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPacketSection 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPacketSection forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPacketSection();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPacketSection");

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

    sealed public partial class CSVFamilyPacketSection : FCSVFamilyPacketSection
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPacketSection.bytes";
		}

		private static CSVFamilyPacketSection instance = null;			
		public static CSVFamilyPacketSection Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPacketSection 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPacketSection forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPacketSection();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPacketSection");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}