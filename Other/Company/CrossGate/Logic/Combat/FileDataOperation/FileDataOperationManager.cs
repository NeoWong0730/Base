using Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class FileDataOperationManager : Logic.Singleton<FileDataOperationManager>
{
    public class FileDataObjTypeInfo
    {
        public FieldInfo[] FieldInfos;
        //public FileDataOperationAttribute[] FileDataOperationAttributes;
    }

    private Dictionary<Type, FileDataObjTypeInfo> _cacheFileDataObjTypeInfoDic = new Dictionary<Type, FileDataObjTypeInfo>();

    public static FileDataObjTypeInfo GetSortFieldInfos(Type objType, Dictionary<Type, FileDataObjTypeInfo> dic)
    {
        if (dic == null && Instance != null)
            dic = Instance._cacheFileDataObjTypeInfoDic;

        if (!dic.TryGetValue(objType, out FileDataObjTypeInfo fileDataObjTypeInfo) || fileDataObjTypeInfo == null)
        {
            FieldInfo[] fis = objType.GetFields();
            if (fis.Length == 0)
                return null;

            FileDataOperationAttribute[] fdoas = new FileDataOperationAttribute[fis.Length];
            int index = 0;
            for (int i = 0, length = fis.Length; i < length; i++)
            {
                FieldInfo iFi = fis[i];
                if (iFi == null)
                    continue;

                FileDataOperationAttribute iFdoa = fdoas[i] = iFi.GetCustomAttribute<FileDataOperationAttribute>();
                if (iFdoa == null)
                {
                    fis[i] = null;
                    continue;
                }

                if (iFdoa.Index == index)
                {
                    ++index;
                    continue;
                }

                bool isExist = false;
                for (int m = i + 1; m < length; m++)
                {
                    FieldInfo mFi = fis[m];
                    if (mFi == null)
                        continue;

                    FileDataOperationAttribute mFdoa = fdoas[m];
                    if (mFdoa == null)
                    {
                        mFdoa = fdoas[m] = mFi.GetCustomAttribute<FileDataOperationAttribute>();
                        if (mFdoa == null)
                        {
                            fis[m] = null;
                            continue;
                        }
                    }

                    if (iFdoa.Index == mFdoa.Index)
                    {
                        Lib.Core.DebugUtil.LogError($"FileDataOperationAttribute标记的Index：{iFdoa.Index.ToString()}有重复的");
                        return null;
                    }

                    if (mFdoa.Index == index)
                    {
                        isExist = true;

                        fis[i] = mFi;
                        fdoas[i] = mFdoa;

                        fis[m] = iFi;
                        fdoas[m] = iFdoa;

                        break;
                    }
                }

                if (!isExist)
                {
                    Lib.Core.DebugUtil.LogError($"FileDataOperationAttribute不存在标记Index为：{i.ToString()}");
                    return null;
                }

                ++index;
            }

            fileDataObjTypeInfo = new FileDataObjTypeInfo();
            fileDataObjTypeInfo.FieldInfos = fis;
            //fileDataObjTypeInfo.FileDataOperationAttributes = fdoas;

            dic[objType] = fileDataObjTypeInfo;
        }

        return fileDataObjTypeInfo;
    }

    public byte[] ParseObjToBytes<T>(T obj, Action<BinaryWriter> beginAction = null)
    {
        if (obj == null)
            return null;

        FileDataOperationWriteEntity fileDataOperationWriteEntity = new FileDataOperationWriteEntity();
        byte[] bs = null;
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(stream))
            {
                beginAction?.Invoke(binaryWriter);

                fileDataOperationWriteEntity.WriteData(binaryWriter, obj, typeof(T));
                bs = stream.GetBuffer();
            }
        }

        return bs;
    }

    public static T DeepCopyObj<T>(T obj) where T : class
    {
        if (obj == null)
            return null;

        FileDataOperationWriteEntity fileDataOperationWriteEntity = new FileDataOperationWriteEntity();
        byte[] bs = null;
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(stream))
            {
                fileDataOperationWriteEntity.WriteData(binaryWriter, obj, typeof(T));
                bs = stream.GetBuffer();
            }
        }

        if (bs != null && bs.Length > 0)
        {
            FileDataOperationReadEntity readEntity = new FileDataOperationReadEntity();
            return readEntity.ReadDataByReflection<T>(bs);
        }

        return null;
    }
}