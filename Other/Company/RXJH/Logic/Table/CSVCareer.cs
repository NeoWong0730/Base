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

	sealed public partial class CSVCareer : Framework.Table.TableBase<CSVCareer.Data>
	{
	    sealed public partial class Data
	    {
			public readonly uint id; // id
			public bool active { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } } // 是否展示
			public readonly byte sex; // 性别展示
			public readonly uint attr; // 雷达图属性
			public readonly uint name; // 职业名称
			public readonly uint nameIcon; // 职业名称icon
			public readonly uint desc; // 职业描述
			public readonly uint radarMapDesc; // 雷达图描述
			public readonly uint icon; // 职业图标
			public readonly uint maleIcon; // 男职业头像
			public readonly uint femaleIcon; // 女职业头像
			public readonly uint maleModel; // 男模型
			public readonly uint femaleModel; // 女模型
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				sex = ReadHelper.ReadByte(binaryReader);
				attr = ReadHelper.ReadUInt(binaryReader);
				name = ReadHelper.ReadUInt(binaryReader);
				nameIcon = ReadHelper.ReadUInt(binaryReader);
				desc = ReadHelper.ReadUInt(binaryReader);
				radarMapDesc = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				maleIcon = ReadHelper.ReadUInt(binaryReader);
				femaleIcon = ReadHelper.ReadUInt(binaryReader);
				maleModel = ReadHelper.ReadUInt(binaryReader);
				femaleModel = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCareer.bytes";
		}

		private static CSVCareer instance = null;			
		public static CSVCareer Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCareer 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCareer forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCareer();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCareer");

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

    sealed public partial class CSVCareer : FCSVCareer
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCareer.bytes";
		}

		private static CSVCareer instance = null;			
		public static CSVCareer Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCareer 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCareer forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCareer();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCareer");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}