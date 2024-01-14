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
    public class FCSVFamilySkillUp
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint SkillName { get; private set; }
			public uint SkillLevel { get; private set; }
			public uint Maxlevel { get; private set; }
			public uint UpgradeCost { get; private set; }
			public uint TotalUpgradeCost { get; private set; }
			public uint SkillType { get; private set; }
			public uint Parameter1 { get; private set; }
			public uint Parameter2 { get; private set; }
			public uint Parameter3 { get; private set; }
			public uint UpgradeConditions { get; private set; }
			public uint HallUpgradeConditions { get; private set; }
			public uint Upgrade { get; private set; }
			public uint SkillLag { get; private set; }
			public uint SkillIcon { get; private set; }
			public uint BulidName { get; private set; }
			public uint CurrentEffectDescription { get; private set; }
			public uint MaxWords { get; private set; }
			public uint UpgradeTime { get; private set; }


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				SkillName = ReadHelper.ReadUInt(binaryReader);
				SkillLevel = ReadHelper.ReadUInt(binaryReader);
				Maxlevel = ReadHelper.ReadUInt(binaryReader);
				UpgradeCost = ReadHelper.ReadUInt(binaryReader);
				TotalUpgradeCost = ReadHelper.ReadUInt(binaryReader);
				SkillType = ReadHelper.ReadUInt(binaryReader);
				Parameter1 = ReadHelper.ReadUInt(binaryReader);
				Parameter2 = ReadHelper.ReadUInt(binaryReader);
				Parameter3 = ReadHelper.ReadUInt(binaryReader);
				UpgradeConditions = ReadHelper.ReadUInt(binaryReader);
				HallUpgradeConditions = ReadHelper.ReadUInt(binaryReader);
				Upgrade = ReadHelper.ReadUInt(binaryReader);
				SkillLag = ReadHelper.ReadUInt(binaryReader);
				SkillIcon = ReadHelper.ReadUInt(binaryReader);
				BulidName = ReadHelper.ReadUInt(binaryReader);
				CurrentEffectDescription = ReadHelper.ReadUInt(binaryReader);
				MaxWords = ReadHelper.ReadUInt(binaryReader);
				UpgradeTime = ReadHelper.ReadUInt(binaryReader);

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

    public class FCSVFamilySkillUpAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVFamilySkillUp);
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
    
        public class Adapter : FCSVFamilySkillUp, CrossBindingAdaptorType
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