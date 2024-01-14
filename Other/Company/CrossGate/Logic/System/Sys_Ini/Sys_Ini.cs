using System;
using System.Collections.Generic;
using Logic.Core;
using Table;

public class IniElement {
    protected string csvString;
    private bool hasParsed = false;

    public IniElement() { }
    public IniElement SetCSV(string csvString) {
        this.csvString = csvString;
        return this;
    }
    public void TryParse() {
        if (!this.hasParsed) {
            this.Parse();
            this.hasParsed = true;
        }
    }

    protected virtual void Parse() {
    }
}

namespace Logic {
    // 全局参数解析系统，不用在ui脚本中各处解析，统一处理
    // 其实最好是表格生成工具在生成表格bytes的时候就序列化成了特定格式的数据格式，而不是现在这样运行时解析
    public class Sys_Ini : SystemModuleBase<Sys_Ini> {
        public Dictionary<uint, IniElement> parseElements = new Dictionary<uint, IniElement>();

        public override void Init() {
            this.parseElements.Clear();
        }

        public bool Get<T>(uint key, out T value) where T : IniElement {
            value = null;
            if (this.parseElements.TryGetValue(key, out IniElement ini)) {
                value = ini as T;
                return true;
            }
            else {
                CSVParam.Data csv = CSVParam.Instance.GetConfData(key);
                if (csv != null) {
                    value = Activator.CreateInstance(typeof(T)) as T;
                    value.SetCSV(csv.str_value);
                    value.TryParse();

                    this.parseElements.Add(key, value);
                    return true;
                }
            }
            return false;
        }
    }
}