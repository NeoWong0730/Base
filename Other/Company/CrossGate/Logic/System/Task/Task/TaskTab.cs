using Table;

namespace Logic {
    public class TaskTab {
        public int taskCategory;

        public CSVTaskCategory.Data _csv;
        public CSVTaskCategory.Data csv {
            get {
                if (this._csv == null) {
                    this._csv = CSVTaskCategory.Instance.GetConfData((uint)this.taskCategory);
                }
                return this._csv;
            }
        }

        public uint contentId => this.csv.simpleName;
        public string content => LanguageHelper.GetTextContent(this.contentId);
        public int traceLilit => (int)this.csv.traceLimit;

        public uint iconId => this.csv.iconId;
        public uint lightIconId => this.csv.lightIconId;
        public string taskTabDetailName => LanguageHelper.GetTextContent(this.csv.name);
        public uint funcOpenId => this.csv.funcOpenId;
        public int priority => this.csv.priority;

        public TaskTab(ETaskCategory taskCategory) {
            this.taskCategory = (int)taskCategory;
        }

        public bool IsOpen() {
            if (this.funcOpenId == 0) {
                return true;
            }
            return Sys_FunctionOpen.Instance.IsOpen(this.funcOpenId, false);
        }
    }
}