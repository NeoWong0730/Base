using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;

using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using UnityEngine;

namespace Framework.Table
{
    public class FCSVFamilyPostAuthority
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint PostNum { get; private set; }
			public uint PostName { get; private set; }
			public uint DividendRatio { get; private set; }
			public uint IsAppointment { get; private set; }
			public uint ModifyName { get; private set; }
			public uint BuildingUp { get; private set; }
			public uint ModifyDeclaration { get; private set; }
			public uint GroupMessage { get; private set; }
			public uint InitiataMerger { get; private set; }
			public uint AcceptMerger { get; private set; }
			public uint EstablishBranch { get; private set; }
			public uint RemoveBranch { get; private set; }
			public uint MergeBranch { get; private set; }
			public uint Invitation { get; private set; }
			public uint ApplicationAcceptance { get; private set; }
			public uint ModifyApproval { get; private set; }
			public uint ModifyApprovalLevel { get; private set; }
			public uint Worker { get; private set; }
			public uint IsForbiddenWords { get; private set; }
			public uint Clear { get; private set; }
			public uint BattleEnroll { get; private set; }
			public uint FamilyPetName { get; private set; }
			public uint FamilyPetNotice { get; private set; }
			public uint FamilyPetEgg { get; private set; }
			public uint FamilyPetTraining { get; private set; }


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

            }
	    }	

        protected Dictionary<uint, int> Datas;
        protected List<Data> DataList;

        public int Count { get { return DataList.Count; } }

        public void ReadByFilePath(string path)
        {            
            Stream stream = AssetMananger.Instance.LoadStream(path);
            BinaryReader binaryReader = new BinaryReader(stream);
            Read(path, binaryReader);
            binaryReader.Close();
            stream.Close();
        }

        public void Read(string path, BinaryReader binaryReader)
        {
            if (binaryReader == null)
            {
                DebugUtil.LogErrorFormat("{0} binaryReader为空", path);
                return;
            }

            if (Datas != null)
            {
                DebugUtil.LogErrorFormat("{0} 多次读取配置", path);
                return;
            }

            TableShareData shareData = OnReadShareData(binaryReader);

            //buff的实际长度
            int bufferSize = binaryReader.ReadInt32();

            //读取内容
            int count = binaryReader.ReadInt32();

            Datas = new Dictionary<uint, int>(count);
            DataList = new List<Data>(count);

            if (count <= 0)
            {
                DebugUtil.LogErrorFormat("{0} 是空表", path);
                return;
            }

            int entrySize = bufferSize / count;

            if (entrySize * count != bufferSize)
            {
                DebugUtil.LogErrorFormat("{0} 数据长度异常", path);
                return;
            }

            for (int i = 0; i < count; ++i)
            {
#if DEBUG_MODE
                try
                {                                        
                    uint id = ReadHelper.ReadUInt(binaryReader);
                    Data data = new Data(id, binaryReader, shareData);
                    if (Datas.ContainsKey(id))
                    {
                        DebugUtil.LogErrorFormat("表格 {1} 有重复id{0}", id.ToString(), path);
                    }
                    Datas[id] = i;
                    DataList.Add(data);
                }
                catch (System.Exception e)
                {
                    DebugUtil.LogErrorFormat("表格 {2} 第 {0} 行错误： {1}", (i + 1).ToString(), e.StackTrace, path);
                    break;
                }
#else
                uint id = ReadHelper.ReadUInt(binaryReader);
                Data data = new Data(id, binaryReader, shareData);
                Datas[id] = i;
                DataList.Add(data);
#endif
            }
        }

        public Data GetConfData(uint id)
        {
            if (Datas.TryGetValue(id, out int index))
                return GetByIndex(index);
            return null;
        }

        public Data GetByIndex(int index)
        {
            Data tableData = DataList[index];
            return tableData;
        }

        public bool TryGetValue(uint id, out Data data)
        {
            if (Datas.TryGetValue(id, out int index))
            {
                data = GetByIndex(index);
                return true;
            }

            data = null;
            return false;
        }

        public IReadOnlyList<Data> GetAll()
        {
            return DataList;
        }

        public Dictionary<uint, int>.KeyCollection GetKeys()
        {
            return Datas.Keys;
        }

        public bool ContainsKey(uint id)
        {
            return Datas.ContainsKey(id);
        }

        public void Clear()
        {
            Datas?.Clear();
            DataList?.Clear();            
        }

        public static TableShareData OnReadShareData(BinaryReader binaryReader)
        {
			TableShareData shareData = null;

            return shareData;
        }
    }

    public class FCSVFamilyPostAuthorityAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVFamilyPostAuthority);
            }
        }
    
        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }
    
        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }
    
        public class Adapter : FCSVFamilyPostAuthority, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    
            public Adapter()
            {
            
            }
    
            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }
    
            public ILTypeInstance ILInstance { get { return instance; } }
    
            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}