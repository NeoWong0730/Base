using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using static FileDataOperationManager;

public class FileDataOperationEditor : Editor
{
    private static readonly string _savePath = "Assets/Scripts/Logic/Combat/Struct/";

    private static StringBuilder _stringBuilder = new StringBuilder();

    private static Dictionary<Type, FileDataObjTypeInfo> _fileDataObjTypeInfoDic = new Dictionary<Type, FileDataObjTypeInfo>();

    private static Dictionary<Type, StringBuilder> _methodSBDic = new Dictionary<Type, StringBuilder>();

    [MenuItem("Tools/Combat/生成FileDataOperation类型文件")]
    public static void FileDataOperationEditorCS()
    {
        GenerateFileDataOperationCs(typeof(WorkStreamData));

        GenerateFileDataOperationCs(typeof(Bezier3CurvesData));

        GenerateFileDataOperationCs(typeof(Bezier1CurvesData));
    }

    public static void GenerateFileDataOperationCs(Type objType)
    {
        _stringBuilder.Clear();
        _fileDataObjTypeInfoDic.Clear();
        _methodSBDic.Clear();
        
        _stringBuilder.Append($"using System.Collections.Generic;");
        _stringBuilder.Append($"\nusing System.IO;\n\n");
        _stringBuilder.Append($"\npublic class {objType.Name}_ConfigTool\n{"{"}");
        _stringBuilder.Append($"\n\tpublic static {objType.Name} Load(BinaryReader br)\n\t{"{"}");
        
        _stringBuilder.Append($"\n\t\treturn {GetCS(objType)};\n\t{"}"}");

        foreach (var kv in _methodSBDic)
        {
            _stringBuilder.Append(kv.Value.ToString());
        }
        
        _stringBuilder.Append($"\n{"}"}");

        File.WriteAllText($"{_savePath}{objType.Name}_ConfigTool.cs", _stringBuilder.ToString());

        AssetDatabase.Refresh();
    }
    
    public static string GetCS(Type objType)
    {
        if (objType.IsArray)
        {
            return GenerateArray(objType);
        }
        else if (objType.IsGenericType)
        {
            Type genericType = objType.GetGenericTypeDefinition();
            if (genericType == typeof(List<>))
            {
                return GenerateGeneric(objType);
            }
            else
                return "null";
        }
        else if (objType.IsClass && objType.Name != CombatHelp.s_String)
        {
            return GenerateClass(objType);
        }
        else if (objType.IsValueType && !objType.IsEnum && !objType.IsPrimitive)  //struct
        {
            return GenerateStruct(objType);
        }
        else
        {
            return CombatHelp.GenerateBaseType(objType);
        }
    }

    private static string GenerateArray(Type objType)
    {
        Type arrayType = objType.GetElementType();

        string arrayTypeStr = CombatHelp.ChangeFieldTypeStr(arrayType.Name);

        if (!_methodSBDic.TryGetValue(objType, out StringBuilder stringBuilder) || stringBuilder == null)
        {
            stringBuilder = new StringBuilder();
            _methodSBDic[objType] = stringBuilder;

            string className = $"{arrayTypeStr.ToLower()}Array";

            stringBuilder.Append($@"

        public static {arrayTypeStr}[] Get{arrayTypeStr}Array(BinaryReader br)
		{{
            bool haveRef = br.ReadBoolean();
			if (!haveRef)
				return null;

			int listCount = br.ReadUInt16();
			if (listCount <= 0)
				return null;

			{arrayTypeStr}[] {className} = new {arrayTypeStr}[listCount];

			for (int i = 0; i < listCount; i++)
			{{
                {className}[i] = {GetCS(arrayType)};
			}}

			return {className};
		}}");
        }

        return $"Get{arrayTypeStr}Array(br)";
    }

    private static string GenerateGeneric(Type objType)
    {
        Type genericArgumentType = objType.GetGenericArguments()[0];

        if (!_methodSBDic.TryGetValue(objType, out StringBuilder stringBuilder) || stringBuilder == null)
        {
            stringBuilder = new StringBuilder();
            _methodSBDic[objType] = stringBuilder;

            stringBuilder.Append($"\n\n\tpublic static List<{genericArgumentType.Name}> Get{genericArgumentType.Name}List(BinaryReader br) \n\t{"{"}\n\t\t");
            stringBuilder.Append($"bool haveRef = br.ReadBoolean();\n\t\tif (!haveRef)\n\t\t\treturn null;\n\n");
            string className = $"{genericArgumentType.Name.ToLower()}List";
            stringBuilder.Append($"\t\tList<{genericArgumentType.Name}> {className} = new List<{genericArgumentType.Name}>();");
            stringBuilder.Append($"\n\t\tint listCount = br.ReadUInt16();");
            stringBuilder.Append($"\n\t\tif (listCount <= 0)");
            stringBuilder.Append($"\n\t\t\treturn {className};");

            stringBuilder.Append($"\n\n\t\tfor (int i = 0; i < listCount; i++)\n\t\t{"{"}");
            stringBuilder.Append($"\n\t\t\t{className}.Add({GetCS(genericArgumentType)});");
            stringBuilder.Append($"\n\t\t{"}"}");
            
            stringBuilder.Append($"\n\n\t\treturn {className};\n\t{"}"}");
        }

        return $"Get{genericArgumentType.Name}List(br)";
    }

    private static string GenerateClass(Type objType)
    {
        if (!_methodSBDic.TryGetValue(objType, out StringBuilder stringBuilder) || stringBuilder == null)
        {
            stringBuilder = new StringBuilder();
            _methodSBDic[objType] = stringBuilder;

            stringBuilder.Append($"\n\n\tpublic static {objType.Name} Get{objType.Name}(BinaryReader br) \n\t{"{"}\n\t\t");
            stringBuilder.Append($"bool haveRef = br.ReadBoolean();\n\t\tif (!haveRef)\n\t\t\treturn null;\n\n");
            string className = objType.Name.ToLower();
            stringBuilder.Append($"\t\t{objType.Name} {className} = new {objType.Name}();");

            FileDataObjTypeInfo fileDataObjTypeInfo = FileDataOperationManager.GetSortFieldInfos(objType, _fileDataObjTypeInfoDic);
            foreach (FieldInfo fieldInfo in fileDataObjTypeInfo.FieldInfos)
            {
                if (fieldInfo == null)
                    continue;

                stringBuilder.Append($"\n\t\t{className}.{fieldInfo.Name} = {GetCS(fieldInfo.FieldType)};");
            }

            stringBuilder.Append($"\n\n\t\treturn {className};\n\t{"}"}");
        }

        return $"Get{objType.Name}(br)";
    }

    public static string GenerateStruct(Type objType)
    {
        if (objType == typeof(Vector3))
        {
            return GenerateVector3();
        }
        else
        {
            Debug.LogError($"暂时不支持类型：{objType}");
            return null;
        }
    }

    public static string GenerateVector3()
    {
        return "CombatHelp.GetVector3(br)";
    }
}
