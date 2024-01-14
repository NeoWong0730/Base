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

	sealed public partial class CSVFamilyPostAuthority : Framework.Table.TableBase<CSVFamilyPostAuthority.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint PostNum;
			public readonly uint PostName;
			public readonly uint DividendRatio;
			public readonly uint IsAppointment;
			public readonly uint ModifyName;
			public readonly uint BuildingUp;
			public readonly uint ModifyDeclaration;
			public readonly uint GroupMessage;
			public readonly uint InitiataMerger;
			public readonly uint AcceptMerger;
			public readonly uint EstablishBranch;
			public readonly uint RemoveBranch;
			public readonly uint MergeBranch;
			public readonly uint Invitation;
			public readonly uint ApplicationAcceptance;
			public readonly uint ModifyApproval;
			public readonly uint ModifyApprovalLevel;
			public readonly uint Worker;
			public readonly uint IsForbiddenWords;
			public readonly uint Clear;
			public readonly uint BattleEnroll;
			public readonly uint FamilyPetName;
			public readonly uint FamilyPetNotice;
			public readonly uint FamilyPetEgg;
			public readonly uint FamilyPetTraining;
			public readonly uint FamilyBonus;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				PostNum = ReadHelper.ReadUInt(binaryReader);
				PostName = ReadHelper.ReadUInt(binaryReader);
				DividendRatio = ReadHelper.ReadUInt(binaryReader);
				IsAppointment = ReadHelper.ReadUInt(binaryReader);
				ModifyName = ReadHelper.ReadUInt(binaryReader);
				BuildingUp = ReadHelper.ReadUInt(binaryReader);
				ModifyDeclaration = ReadHelper.ReadUInt(binaryReader);
				GroupMessage = ReadHelper.ReadUInt(binaryReader);
				InitiataMerger = ReadHelper.ReadUInt(binaryReader);
				AcceptMerger = ReadHelper.ReadUInt(binaryReader);
				EstablishBranch = ReadHelper.ReadUInt(binaryReader);
				RemoveBranch = ReadHelper.ReadUInt(binaryReader);
				MergeBranch = ReadHelper.ReadUInt(binaryReader);
				Invitation = ReadHelper.ReadUInt(binaryReader);
				ApplicationAcceptance = ReadHelper.ReadUInt(binaryReader);
				ModifyApproval = ReadHelper.ReadUInt(binaryReader);
				ModifyApprovalLevel = ReadHelper.ReadUInt(binaryReader);
				Worker = ReadHelper.ReadUInt(binaryReader);
				IsForbiddenWords = ReadHelper.ReadUInt(binaryReader);
				Clear = ReadHelper.ReadUInt(binaryReader);
				BattleEnroll = ReadHelper.ReadUInt(binaryReader);
				FamilyPetName = ReadHelper.ReadUInt(binaryReader);
				FamilyPetNotice = ReadHelper.ReadUInt(binaryReader);
				FamilyPetEgg = ReadHelper.ReadUInt(binaryReader);
				FamilyPetTraining = ReadHelper.ReadUInt(binaryReader);
				FamilyBonus = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPostAuthority.bytes";
		}

		private static CSVFamilyPostAuthority instance = null;			
		public static CSVFamilyPostAuthority Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPostAuthority 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPostAuthority forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPostAuthority();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPostAuthority");

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

    sealed public partial class CSVFamilyPostAuthority : FCSVFamilyPostAuthority
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPostAuthority.bytes";
		}

		private static CSVFamilyPostAuthority instance = null;			
		public static CSVFamilyPostAuthority Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPostAuthority 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPostAuthority forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPostAuthority();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPostAuthority");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}