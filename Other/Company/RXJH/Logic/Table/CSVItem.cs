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

	sealed public partial class CSVItem : Framework.Table.TableBase<CSVItem.Data>
	{
	    sealed public partial class Data
	    {
			public readonly uint id; // 道具id
			public readonly uint typeId; // 道具类型
			public readonly uint typeName; // 道具类型名
			public readonly uint stackNum; // 最大堆叠
			public readonly uint bag; // 背包类型
			public readonly uint quality; // 品质
			public readonly uint useLevel; // 使用等级
			public readonly uint useCondition; // 使用条件
			public readonly string useFunc; // 使用方法
			public readonly List<List<uint>> funcParams; // 方法参数
			public readonly uint cdGroup; // 冷却组
			public readonly uint cdTime; // 冷却时间
			public readonly List<List<uint>> sellPrice; // 出售获得
			public readonly uint recommendSale; // 推荐出售
			public readonly uint recommendUndo; // 推荐分解
			public readonly List<List<uint>> undoItem; // 分解获得
			public readonly uint overDue; // 过期时效
			public readonly uint expirationTime; // 过期时间
			public readonly List<uint> appraisal; // 鉴定消耗
			public readonly List<List<uint>> appraisalDrop; // 鉴定获得


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				typeId = ReadHelper.ReadUInt(binaryReader);
				typeName = ReadHelper.ReadUInt(binaryReader);
				stackNum = ReadHelper.ReadUInt(binaryReader);
				bag = ReadHelper.ReadUInt(binaryReader);
				quality = ReadHelper.ReadUInt(binaryReader);
				useLevel = ReadHelper.ReadUInt(binaryReader);
				useCondition = ReadHelper.ReadUInt(binaryReader);
				useFunc = shareData.GetShareData<string>(binaryReader, 0);
				funcParams = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				cdGroup = ReadHelper.ReadUInt(binaryReader);
				cdTime = ReadHelper.ReadUInt(binaryReader);
				sellPrice = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				recommendSale = ReadHelper.ReadUInt(binaryReader);
				recommendUndo = ReadHelper.ReadUInt(binaryReader);
				undoItem = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				overDue = ReadHelper.ReadUInt(binaryReader);
				expirationTime = ReadHelper.ReadUInt(binaryReader);
				appraisal = shareData.GetShareData<List<uint>>(binaryReader, 1);
				appraisalDrop = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVItem.bytes";
		}

		private static CSVItem instance = null;			
		public static CSVItem Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVItem 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVItem forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVItem();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVItem");

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

    sealed public partial class CSVItem : FCSVItem
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVItem.bytes";
		}

		private static CSVItem instance = null;			
		public static CSVItem Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVItem 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVItem forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVItem();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVItem");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}