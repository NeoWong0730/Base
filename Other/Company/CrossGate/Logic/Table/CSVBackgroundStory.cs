﻿//
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

	sealed public partial class CSVBackgroundStory : Framework.Table.TableBase<CSVBackgroundStory.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> actication_method;
			public readonly uint txt;
			public readonly uint contests;
			public readonly uint actication_reward;
			public readonly List<uint> mission;
			public readonly List<List<uint>> reward;
			public readonly uint last;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				actication_method = shareData.GetShareData<List<uint>>(binaryReader, 0);
				txt = ReadHelper.ReadUInt(binaryReader);
				contests = ReadHelper.ReadUInt(binaryReader);
				actication_reward = ReadHelper.ReadUInt(binaryReader);
				mission = shareData.GetShareData<List<uint>>(binaryReader, 0);
				reward = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				last = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBackgroundStory.bytes";
		}

		private static CSVBackgroundStory instance = null;			
		public static CSVBackgroundStory Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBackgroundStory 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBackgroundStory forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBackgroundStory();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBackgroundStory");

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

    sealed public partial class CSVBackgroundStory : FCSVBackgroundStory
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBackgroundStory.bytes";
		}

		private static CSVBackgroundStory instance = null;			
		public static CSVBackgroundStory Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBackgroundStory 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBackgroundStory forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBackgroundStory();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBackgroundStory");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}