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

	sealed public partial class CSVAttr : Framework.Table.TableBase<CSVAttr.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name;
			public readonly uint desc;
			public readonly uint type;
			public readonly uint BPValue;
			public readonly uint isShow;
			public readonly uint attr_type;
			public readonly uint attr_icon;
			public readonly uint show_type;
			public readonly int add_type;
			public readonly byte pet_show_type;
			public readonly uint pet_show_sor;
			public readonly uint attr_show;
			public readonly uint order_list;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				desc = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				BPValue = ReadHelper.ReadUInt(binaryReader);
				isShow = ReadHelper.ReadUInt(binaryReader);
				attr_type = ReadHelper.ReadUInt(binaryReader);
				attr_icon = ReadHelper.ReadUInt(binaryReader);
				show_type = ReadHelper.ReadUInt(binaryReader);
				add_type = ReadHelper.ReadInt(binaryReader);
				pet_show_type = ReadHelper.ReadByte(binaryReader);
				pet_show_sor = ReadHelper.ReadUInt(binaryReader);
				attr_show = ReadHelper.ReadUInt(binaryReader);
				order_list = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAttr.bytes";
		}

		private static CSVAttr instance = null;			
		public static CSVAttr Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAttr 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAttr forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAttr();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAttr");

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

    sealed public partial class CSVAttr : FCSVAttr
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAttr.bytes";
		}

		private static CSVAttr instance = null;			
		public static CSVAttr Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAttr 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAttr forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAttr();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAttr");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}