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

	sealed public partial class CSVOneCoinLottey : Framework.Table.TableBase<CSVOneCoinLottey.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activity_Id;
			public readonly uint Type;
			public readonly uint Date;
			public readonly uint Price;
			public readonly List<List<uint>> Begin_Time;
			public readonly List<uint> Duration_Time;
			public readonly List<uint> Drop_Id;
			public readonly List<uint> Prize_Number;
			public readonly uint LuckyMail_Id;
			public readonly uint ReturnMail_Id;
			public readonly uint ErrorCode_Id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_Id = ReadHelper.ReadUInt(binaryReader);
				Type = ReadHelper.ReadUInt(binaryReader);
				Date = ReadHelper.ReadUInt(binaryReader);
				Price = ReadHelper.ReadUInt(binaryReader);
				Begin_Time = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				Duration_Time = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Drop_Id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Prize_Number = shareData.GetShareData<List<uint>>(binaryReader, 0);
				LuckyMail_Id = ReadHelper.ReadUInt(binaryReader);
				ReturnMail_Id = ReadHelper.ReadUInt(binaryReader);
				ErrorCode_Id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVOneCoinLottey.bytes";
		}

		private static CSVOneCoinLottey instance = null;			
		public static CSVOneCoinLottey Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVOneCoinLottey 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVOneCoinLottey forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVOneCoinLottey();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVOneCoinLottey");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVOneCoinLottey : FCSVOneCoinLottey
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVOneCoinLottey.bytes";
		}

		private static CSVOneCoinLottey instance = null;			
		public static CSVOneCoinLottey Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVOneCoinLottey 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVOneCoinLottey forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVOneCoinLottey();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVOneCoinLottey");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}