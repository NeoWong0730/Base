using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Quick.Code
{
    public class CodeConfig
    {
        /// <summary>
        /// 需要注册事件的UI控件类型
        /// </summary>
        public enum EventWidgetType
        {
            Button,
            Toggle,
            Slider,
            InputField,
            ScrollRect,
            Scrollbar,
            Dropdown,
        }        

        public class ComponentInfo
        {
            public string sListenerFormat;
            public string sFuncFormat;

            public ComponentInfo(string a, string b)
            {
                sListenerFormat = a;
                sFuncFormat = b;
            }
        }

        public static Dictionary<string, ComponentInfo> eventCBParamDic = new Dictionary<string, ComponentInfo>
        {
            { "Button", new ComponentInfo("\t\t{0}.onClick.AddListener(listener.On{1}Clicked);", "\t\tvoid On{0}Clicked();") },
            { "Toggle", new ComponentInfo("\t\t{0}.onValueChanged.AddListener(listener.On{1}ValueChanged);", "\t\tvoid On{0}ValueChanged(bool arg);") },
            { "Slider", new ComponentInfo("\t\t{0}.onValueChanged.AddListener(listener.On{1}ValueChanged);", "\t\tvoid On{0}ValueChanged(float arg);") },
            { "InputField", new ComponentInfo("\t\t{0}.onValueChanged.AddListener(listener.On{1}ValueChanged);", "\t\tvoid On{0}ValueChanged(string arg);") },
            { "ScrollRect", new ComponentInfo("\t\t{0}.onValueChanged.AddListener(listener.On{1}ValueChanged);", "\t\tvoid On{0}ValueChanged(Vector2 arg);") },
            { "Scrollbar", new ComponentInfo("\t\t{0}.onValueChanged.AddListener(listener.On{1}ValueChanged);", "\t\tvoid On{0}ValueChanged(float arg);") },
            { "Dropdown", new ComponentInfo("\t\t{0}.onValueChanged.AddListener(listener.On{1}ValueChanged);", "\t\tvoid On{0}ValueChanged(int arg);") },
        };      

        #region cs代码格式
        public const string regionStartFmt = "\n\t#region {0} \n";
        public const string regionEnd = "\t#endregion\n";

        public static string statementRegion = string.Format(regionStartFmt,"UI Variable Statement");
        public static string isInitDeclar = "\tprivate bool isInited;\n";
        public static string isInitContent = string.Format("\t\tif(isInited) return;\n\t\tisInited = true;\n");

        public static string eventRegion = string.Format(regionStartFmt, "UI Event Register");
        public static string assignRegion = string.Format(regionStartFmt, "UI Variable Assignment");

        public const string methodStartFmt = "\tpublic void {0} \n\t{{\n";   //'{'要转义
        public const string methodOverrideEventStartFmt = "\tprotected override void {0}(bool toRegister) \n\t{{\n";   //'{'要转义
        public const string methodEnd = "\t}";

        public const string codeAnnotation = @"//this file is auto created by QuickCode,you can edit it 
//do not need to care initialization of ui widget any more 
//------------------------------------------------------------
/*
*   @Author:{0}
*   DateTime:{1}
*   Purpose:{2}
*/
//-------------------------------------------------------------";
        public const string usingNamespace = "\nusing UnityEngine;\nusing System.Collections;\nusing UnityEngine.UI;\nusing System;\n";
        public const string classMonoStart = "\npublic class {0}\n{{";
        public const string interfaceMonoStart = "\n\tpublic interface {0}\n\t{{\n";
        public const string classStart = "\n\tpublic class {0}\n{{";
        public const string classEnd = "}\n";
        public const string methodAnnotation = "\n\t/// <summary>\n\t/// {0}\n\t/// </summary>\n";
             

        #region 序列化初始化代码格式
        //控件遍历声明,0:类型 1:名称
        public const string serilStateCodeFmt = "\t\tpublic {0} {1} {{ get; private set; }} \n";

        public const string onClickSerilCode        = "\t\t\t{0}.onClick.AddListener(listener.On{1}Clicked);";
        public const string onValueChangeSerilCode  = "\t\t\t{0}.onValueChanged.AddListener(listener.On{1}ValueChanged);";

        public const string btnCallbackSerilCode        = "\t\t\tvoid On{0}Clicked();";
        public const string throwException = "\t\t\t//TODO:Implementation Here";
        public const string eventCallbackSerilCode = "\t\t\tvoid On{0}ValueChanged({1} arg);";
        public const string initComponentCode = "\n\t\t#region InitComponent\n\tprivate void Awake()\n\t{\n\t\tbase.OnAwake();\n\t\tInitUI();\n\t\tProcessComponentEvents();\n\t\tProcessEvents(true);\n\t}\n\t#endregion\n";
        #endregion

        #region 控件查找赋值格式
        public const string assignCodeFmt = "\t\t{0} = mTrans.Find(\"{1}\").GetComponent<{2}>(); \n";
        public const string assignGameObjectCodeFmt = "\t\t{0} = mTrans.Find(\"{1}\").gameObject; \n";
        //根物体上挂载的控件
        public const string assignRootCodeFmt = "\t\t{0} = mTrans.GetComponent<{1}>(); \n";
        #endregion

        #region 查找初始化代码格式        
        public const string stateCodeFmt = "\t\tpublic {0} {1} {{ get; private set; }} \n";
        public const string assignTransform = "\t\t\t//assign transform by your ui framework\n\t\t//transform = ; \n";
        #endregion

        #endregion

        #region lua代码格式
        public const string regionStartFmtLua = "\n--region {0} \n";
        public const string regionEndLua = "--endregion \n";

        public static string eventRegionLua = string.Format(regionStartFmtLua, "UI Event Register");
        public static string assignRegionLua = string.Format(regionStartFmtLua, "UI Variable Assignment");

        public const string methodStartFmtLua = "function Class:{0}()\n";
        public const string methodEndLua = "\nend\n";

        public const string codeAnnotationLua = @"--this file is auto created by QuickCode,you can edit it 
--do not need to care initialization of ui widget any more 
--------------------------------------------------------------------------------
--/**
--* @author :
--* date    :
--* purpose :
--*/
--------------------------------------------------------------------------------";

        public const string assignCodeFmtLua = "\tself.{0} = self.transform:Find(\"{1}\"):GetComponent(\"{2}\"); \n";
        public const string assignGameObjectCodeFmtLua = "\t\tself.{0} = self.transform.Find(\"{1}\").gameObject; \n";
        //根物体上挂载的控件
        public const string assignRootCodeFmtLua = "\tself.{0} = self.transform:GetComponent(\"{1}\"); \n";

        public const string onClickSerilCodeLua = "\tself.{0}.onClick:AddListener(function () self:On{1}Clicked(); end); \n";
        public const string onValueChangeSerilCodeLua = "\n\tself.{0}.onValueChanged:AddListener(function (args) self:On{1}ValueChanged(args); end);";
        public const string btnCallbackSerilCodeLua = "\nfunction Class:On{0}Clicked()\n\nend\n";
        public const string eventCallbackSerilCodeLua = "\nfunction Class:On{0}ValueChanged(args)\n\nend\n";

        //文件和类
        public const string requireCode = "\nrequire \"class\"\n\n";
        public const string classStartLua = "\nlocal {0} = class(\"{1}\");\n";
        public const string classEndLua = "\nreturn {0};\n";
        public const string classCtorLua = "\nfunction {0}:ctor(...)\n\t--assign transform by your ui framework\n\tself.transform = nil;\nend\n";
        #endregion
    }
}
