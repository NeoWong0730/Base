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

	sealed public partial class CSVFashionDraw : Framework.Table.TableBase<CSVFashionDraw.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint ActivityId;
			public readonly uint itemId;
			public readonly uint Langid;
			public readonly uint itemNum;
			public readonly uint isBroadCast;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				ActivityId = ReadHelper.ReadUInt(binaryReader);
				itemId = ReadHelper.ReadUInt(binaryReader);
				Langid = ReadHelper.ReadUInt(binaryReader);
				itemNum = ReadHelper.ReadUInt(binaryReader);
				isBroadCast = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFashionDraw.bytes";
		}

		private static CSVFashionDraw instance = null;			
		public static CSVFashionDraw Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFashionDraw 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFashionDraw forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFashionDraw();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFashionDraw");

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

    sealed public partial class CSVFashionDraw : FCSVFashionDraw
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFashionDraw.bytes";
		}

		private static CSVFashionDraw instance = null;			
		public static CSVFashionDraw Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFashionDraw 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFashionDraw forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFashionDraw();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFashionDraw");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}