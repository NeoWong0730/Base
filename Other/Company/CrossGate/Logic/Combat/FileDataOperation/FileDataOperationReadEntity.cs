using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using static FileDataOperationManager;

public class FileDataOperationReadEntity : AEntity
{
    private Dictionary<Type, FileDataObjTypeInfo> _fileDataObjTypeInfoDic = new Dictionary<Type, FileDataObjTypeInfo>();

    private Stream _stream;
    private BinaryReader _binaryReader;
    
    public T DoRead<T>(Stream stream, Func<BinaryReader, T> readerFunc, Action<Stream, BinaryReader> beginAction = null)
    {
        _stream = stream;
        _binaryReader = new BinaryReader(_stream);

        beginAction?.Invoke(_stream, _binaryReader);

        //T t = (T)ReadData(typeof(T));
        T t = readerFunc(_binaryReader);

        _binaryReader.Close();
        _stream.Close();

        _binaryReader = null;
        _stream = null;

        return t;
    }

    #region 配套使用
    /// <summary>
    /// 一定要和EndRead()配合使用
    /// </summary>
    public void StartRead(Stream stream, Action<Stream, BinaryReader> startAction = null)
    {
        _stream = stream;
        _binaryReader = new BinaryReader(_stream);

        startAction?.Invoke(_stream, _binaryReader);
    }

    public T GetRead<T>(Func<BinaryReader, T> readerFunc, Action<BinaryReader> beginAction = null)
    {
        beginAction?.Invoke(_binaryReader);

        //T t = (T)ReadData(typeof(T));
        T t = readerFunc(_binaryReader);

        return t;
    }

    public void EndRead()
    {
        _binaryReader.Close();
        _stream.Close();

        _binaryReader = null;
        _stream = null;
    }
    #endregion

    #region 运行时反射读取
    public T ReadDataByReflection<T>(byte[] bytes)
    {
        _stream = new MemoryStream(bytes);
        _binaryReader = new BinaryReader(_stream);

        T t = (T)ReadData(typeof(T));

        _binaryReader.Close();
        _stream.Close();

        _binaryReader = null;
        _stream = null;

        return t;
    }

    private object ReadData(Type objType)
    {
        if (objType == null)
            return null;

        if (objType.IsGenericType)
        {
            Type genericType = objType.GetGenericTypeDefinition();
            if (genericType == typeof(List<>))
            {
                return SetGeneric(objType);
            }
            else
                return null;
        }
        else if (objType.IsClass && objType.Name != CombatHelp.s_String)
        {
            return SetClass(objType);
        }
        else
        {
            return CombatHelp.ReadValueByType(_binaryReader, objType.Name);
        }
    }

    private object SetGeneric(Type objType)
    {
        bool haveRef = _binaryReader.ReadBoolean();
        if (!haveRef)
            return null;

        object obj = Activator.CreateInstance(objType);
        if (obj == null)
        {
            Lib.Core.DebugUtil.LogError($"反射创建类失败：{objType.ToString()}");
            return null;
        }

        IList list = (IList)obj;

        int listCount = _binaryReader.ReadUInt16();
        if (listCount <= 0)
            return list;

        Type genericType = objType.GetGenericArguments()[0];
        for (int i = 0; i < listCount; i++)
        {
            list.Add(ReadData(genericType));
        }

        return list;
    }

    private object SetClass(Type objType)
    {
        bool haveRef = _binaryReader.ReadBoolean();
        if (!haveRef)
            return null;

        object obj = Activator.CreateInstance(objType);
        if (obj == null)
        {
            Lib.Core.DebugUtil.LogError($"反射创建类失败：{objType.ToString()}");
            return null;
        }

        FileDataObjTypeInfo fileDataObjTypeInfo = FileDataOperationManager.GetSortFieldInfos(objType, _fileDataObjTypeInfoDic);
        foreach (FieldInfo fieldInfo in fileDataObjTypeInfo.FieldInfos)
        {
            if (fieldInfo == null)
                continue;

            fieldInfo.SetValue(obj, ReadData(fieldInfo.FieldType));
        }

        return obj;
    }
    #endregion
}
