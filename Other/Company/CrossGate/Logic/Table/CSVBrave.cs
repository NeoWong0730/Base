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

	sealed public partial class CSVBrave : Framework.Table.TableBase<CSVBrave.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint icon;
			public readonly uint name_id;
			public readonly string show_modle;
			public readonly uint Occupation;
			public readonly uint Ideal;
			public readonly uint Height;
			public readonly uint Weight;
			public readonly uint Character;
			public readonly uint Hobby;
			public readonly List<uint> story_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				icon = ReadHelper.ReadUInt(binaryReader);
				name_id = ReadHelper.ReadUInt(binaryReader);
				show_modle = shareData.GetShareData<string>(binaryReader, 0);
				Occupation = ReadHelper.ReadUInt(binaryReader);
				Ideal = ReadHelper.ReadUInt(binaryReader);
				Height = ReadHelper.ReadUInt(binaryReader);
				Weight = ReadHelper.ReadUInt(binaryReader);
				Character = ReadHelper.ReadUInt(binaryReader);
				Hobby = ReadHelper.ReadUInt(binaryReader);
				story_id = shareData.GetShareData<List<uint>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBrave.bytes";
		}

		private static CSVBrave instance = null;			
		public static CSVBrave Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBrave 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBrave forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBrave();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBrave");

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

    sealed public partial class CSVBrave : FCSVBrave
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBrave.bytes";
		}

		private static CSVBrave instance = null;			
		public static CSVBrave Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBrave 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBrave forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBrave();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBrave");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}