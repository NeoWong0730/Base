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

	sealed public partial class CSVBubble : Framework.Table.TableBase<CSVBubble.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint BubbleTime;
			public readonly uint BubbleText;
			public readonly uint BubbleTextInterval;
			public readonly uint BubbleSounds;
			public readonly uint BubbleDelay;
			public readonly uint BubblePriority;
			public readonly uint BubbleType;
			public readonly List<int> offset;
			public readonly uint Type;
			public readonly uint MoodId;
			public readonly uint PeopleType;
			public readonly uint CharacterHead;
			public readonly uint NPCHead;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				BubbleTime = ReadHelper.ReadUInt(binaryReader);
				BubbleText = ReadHelper.ReadUInt(binaryReader);
				BubbleTextInterval = ReadHelper.ReadUInt(binaryReader);
				BubbleSounds = ReadHelper.ReadUInt(binaryReader);
				BubbleDelay = ReadHelper.ReadUInt(binaryReader);
				BubblePriority = ReadHelper.ReadUInt(binaryReader);
				BubbleType = ReadHelper.ReadUInt(binaryReader);
				offset = shareData.GetShareData<List<int>>(binaryReader, 0);
				Type = ReadHelper.ReadUInt(binaryReader);
				MoodId = ReadHelper.ReadUInt(binaryReader);
				PeopleType = ReadHelper.ReadUInt(binaryReader);
				CharacterHead = ReadHelper.ReadUInt(binaryReader);
				NPCHead = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBubble.bytes";
		}

		private static CSVBubble instance = null;			
		public static CSVBubble Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBubble 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBubble forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBubble();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBubble");

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
			shareData.ReadArrays<int>(binaryReader, 0, ReadHelper.ReadArray_ReadInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVBubble : FCSVBubble
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBubble.bytes";
		}

		private static CSVBubble instance = null;			
		public static CSVBubble Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBubble 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBubble forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBubble();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBubble");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}