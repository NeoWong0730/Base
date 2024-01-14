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

	sealed public partial class CSVImprintNode : Framework.Table.TableBase<CSVImprintNode.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Label_Id;
			public readonly uint Imprint_Name;
			public readonly uint Node_Type;
			public readonly List<uint> Coordinate;
			public readonly uint Farme;
			public readonly uint Farme_Text;
			public readonly List<uint> Front_Node;
			public readonly uint Level_Cap;
			public readonly uint Node_Icon;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Label_Id = ReadHelper.ReadUInt(binaryReader);
				Imprint_Name = ReadHelper.ReadUInt(binaryReader);
				Node_Type = ReadHelper.ReadUInt(binaryReader);
				Coordinate = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Farme = ReadHelper.ReadUInt(binaryReader);
				Farme_Text = ReadHelper.ReadUInt(binaryReader);
				Front_Node = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Level_Cap = ReadHelper.ReadUInt(binaryReader);
				Node_Icon = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVImprintNode.bytes";
		}

		private static CSVImprintNode instance = null;			
		public static CSVImprintNode Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVImprintNode 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVImprintNode forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVImprintNode();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVImprintNode");

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

    sealed public partial class CSVImprintNode : FCSVImprintNode
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVImprintNode.bytes";
		}

		private static CSVImprintNode instance = null;			
		public static CSVImprintNode Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVImprintNode 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVImprintNode forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVImprintNode();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVImprintNode");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}