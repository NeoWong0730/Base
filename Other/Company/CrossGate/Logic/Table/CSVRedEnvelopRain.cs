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

	sealed public partial class CSVRedEnvelopRain : Framework.Table.TableBase<CSVRedEnvelopRain.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activity_Id;
			public readonly uint Activity_Date;
			public readonly List<List<uint>> Envelop_Begin;
			public readonly List<uint> Duration_Second;
			public readonly List<uint> Limit_Max;
			public readonly List<List<uint>> RedEnvelop_Drop;
			public readonly List<List<uint>> RedRare_Drop;
			public readonly List<uint> Red_Quantity;
			public readonly List<List<uint>> GoldEnvelop_Drop;
			public readonly List<List<uint>> GoldRare_Drop;
			public readonly List<uint> Gold_Quantity;
			public readonly uint Envelop_Quantity;
			public readonly List<uint> Envelop_Weight;
			public readonly uint Rolling_Id;
			public readonly uint AnnounceTime;
			public readonly string Bottom_Back;
			public readonly string Bottom_Effect;
			public readonly string Bottom_Tittle;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_Id = ReadHelper.ReadUInt(binaryReader);
				Activity_Date = ReadHelper.ReadUInt(binaryReader);
				Envelop_Begin = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				Duration_Second = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Limit_Max = shareData.GetShareData<List<uint>>(binaryReader, 1);
				RedEnvelop_Drop = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				RedRare_Drop = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				Red_Quantity = shareData.GetShareData<List<uint>>(binaryReader, 1);
				GoldEnvelop_Drop = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				GoldRare_Drop = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				Gold_Quantity = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Envelop_Quantity = ReadHelper.ReadUInt(binaryReader);
				Envelop_Weight = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Rolling_Id = ReadHelper.ReadUInt(binaryReader);
				AnnounceTime = ReadHelper.ReadUInt(binaryReader);
				Bottom_Back = shareData.GetShareData<string>(binaryReader, 0);
				Bottom_Effect = shareData.GetShareData<string>(binaryReader, 0);
				Bottom_Tittle = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVRedEnvelopRain.bytes";
		}

		private static CSVRedEnvelopRain instance = null;			
		public static CSVRedEnvelopRain Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRedEnvelopRain 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRedEnvelopRain forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRedEnvelopRain();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRedEnvelopRain");

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
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVRedEnvelopRain : FCSVRedEnvelopRain
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVRedEnvelopRain.bytes";
		}

		private static CSVRedEnvelopRain instance = null;			
		public static CSVRedEnvelopRain Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRedEnvelopRain 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRedEnvelopRain forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRedEnvelopRain();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRedEnvelopRain");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}