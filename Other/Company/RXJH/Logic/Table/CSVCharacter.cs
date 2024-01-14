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

	sealed public partial class CSVCharacter : Framework.Table.TableBase<CSVCharacter.Data>
	{
	    sealed public partial class Data
	    {
			public readonly uint id; // id
			public bool active { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } } // 是否展示
			public readonly byte sex; // 性别
			public readonly byte dressUp; // 调整类型
			public readonly List<uint> proportion; // 比例调整
			public readonly List<string> freeModel; // 免费模型地址
			public readonly List<string> payModel; // 收费模型地址
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				sex = ReadHelper.ReadByte(binaryReader);
				dressUp = ReadHelper.ReadByte(binaryReader);
				proportion = shareData.GetShareData<List<uint>>(binaryReader, 1);
				freeModel = shareData.GetShareData<List<string>>(binaryReader, 2);
				payModel = shareData.GetShareData<List<string>>(binaryReader, 2);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCharacter.bytes";
		}

		private static CSVCharacter instance = null;			
		public static CSVCharacter Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCharacter 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCharacter forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCharacter();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCharacter");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadStringArrays(binaryReader, 2, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVCharacter : FCSVCharacter
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCharacter.bytes";
		}

		private static CSVCharacter instance = null;			
		public static CSVCharacter Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCharacter 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCharacter forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCharacter();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCharacter");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}