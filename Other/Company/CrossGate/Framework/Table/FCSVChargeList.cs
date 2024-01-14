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
    public class FCSVChargeList
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public readonly string Ks_Apple;
			public uint PlatformId { get; private set; }
			public uint GoodsType { get; private set; }
			public uint LanguageId { get; private set; }
			public readonly string Describe;
			public uint RechargeType { get; private set; }
			public uint RechargeCurrency { get; private set; }
			public uint RechargeDiamond { get; private set; }
			public uint ReturnMoneyId { get; private set; }
			public uint ReturnMoneyNum { get; private set; }
			public uint MoneyId { get; private set; }
			public uint MoneyNum { get; private set; }
			public uint MoneyItem { get; private set; }
			public uint MoneyItemNum { get; private set; }
			public bool Is_Activate_Firstcharge { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool Is_Charge_Exp { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public uint Charge_Exp { get; private set; }
			public uint Act_Charge_Exp { get; private set; }
			public readonly string BackGround;
			public uint CashCoupon { get; private set; }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Ks_Apple = shareData.GetShareData<string>(binaryReader, 0);
				PlatformId = ReadHelper.ReadUInt(binaryReader);
				GoodsType = ReadHelper.ReadUInt(binaryReader);
				LanguageId = ReadHelper.ReadUInt(binaryReader);
				Describe = shareData.GetShareData<string>(binaryReader, 0);
				RechargeType = ReadHelper.ReadUInt(binaryReader);
				RechargeCurrency = ReadHelper.ReadUInt(binaryReader);
				RechargeDiamond = ReadHelper.ReadUInt(binaryReader);
				ReturnMoneyId = ReadHelper.ReadUInt(binaryReader);
				ReturnMoneyNum = ReadHelper.ReadUInt(binaryReader);
				MoneyId = ReadHelper.ReadUInt(binaryReader);
				MoneyNum = ReadHelper.ReadUInt(binaryReader);
				MoneyItem = ReadHelper.ReadUInt(binaryReader);
				MoneyItemNum = ReadHelper.ReadUInt(binaryReader);
				Charge_Exp = ReadHelper.ReadUInt(binaryReader);
				Act_Charge_Exp = ReadHelper.ReadUInt(binaryReader);
				BackGround = shareData.GetShareData<string>(binaryReader, 0);
				CashCoupon = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
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
			TableShareData shareData = new TableShareData(1);
			shareData.ReadStrings(binaryReader, 0);

            return shareData;
        }
    }

    public class FCSVChargeListAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVChargeList);
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
    
        public class Adapter : FCSVChargeList, CrossBindingAdaptorType
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