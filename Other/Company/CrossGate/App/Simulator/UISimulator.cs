#if UNITY_STANDALONE_WIN
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using System.IO;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using UnityEngine.EventSystems;

class UISimulator : MonoBehaviour
{
    [Serializable]
    public class AppData
    {
        public string app_icon;
        public int app_type;
        public string app_link;
        public int app_qucik;
        public string app_name;
        public int app_close;
        public Sprite sprite;
    }
    [Serializable]
    public class SimulatorData
    {
        public string search;
        public List<AppData> app = new List<AppData>();
    }

    public GameObject go_Loading, go_Main;
    static Transform transUI;

    private InputField inputField;
    private Button btn_Search;
    private Text text_Time;
    private Transform pageGridParent;
    private Transform bottomGridParent;

    #region Tips
    private Button menuItem;
    private Button item;
    private GameObject panelTips;
    #endregion

    SimulatorData simulatorConfigData;
    bool isLoading;
    bool IsLoading {
        get => isLoading;
        set {
            isLoading = value;
            go_Loading.SetActive(isLoading);
            go_Main.SetActive(!isLoading);
        }
    }

    private void Awake()
    {
        Transform tran = transform.Find("UI_Simulator/Animator");
        go_Loading = tran.Find("Image_Loading").gameObject;
        go_Main = tran.Find("Image_bg").gameObject;
        IsLoading = true;
        inputField = go_Main.transform.Find("Input_Word").GetComponent<InputField>();
        btn_Search = go_Main.transform.Find("Btn_Search").GetComponent<Button>();
        btn_Search.onClick.AddListener(OnClick_Search);
        text_Time = go_Main.transform.Find("Text_Time").GetComponent<Text>();
        pageGridParent = go_Main.transform.Find("Content/View_Page/View_Island/Grid");
        bottomGridParent = go_Main.transform.Find("Bottom/Grid");

        #region Tips
        panelTips = transform.Find("UI_Simulator_Tips").gameObject;
        panelTips.transform.GetComponent<Canvas>().sortingLayerID = 10;
        panelTips.SetActive(false);
        menuItem = go_Main.transform.Find("Btn_Setting").GetComponent<Button>();//PopupList/DropDownList/MainButton
        item = go_Main.transform.Find("PopupList/DropDownList/ItemTemplate").GetComponent<Button>();
        menuItem.onClick.AddListener(() => { item.gameObject.SetActive(!item.gameObject.activeSelf); });
        item.onClick.AddListener(() => { panelTips.SetActive(true); item.gameObject.SetActive(false); });
        Button close = panelTips.transform.Find("Animator/Image_bg/Btn_Close").GetComponent<Button>();
        close.onClick.AddListener(() =>
        {
            panelTips.SetActive(false);
        });
        #endregion
    }

    private void Start()
    {
        Debug.LogError("Start");
        Timer.Register(2, () => { IsLoading = false; InitAppData(); });
        ReadAppConfig();
        DownLoadAppIcon();
    }

    private void OnClick_Search()
    {
        if (string.IsNullOrEmpty(inputField.text)) {
            Application.OpenURL(simulatorConfigData.search);
        }
        else
        {
            string url = simulatorConfigData.search.Split('=')[0];
            string content = inputField.text;
            url = url + "=" + content;
            Application.OpenURL(url);
        }
    }

    private void InitAppData()
    {
        if (simulatorConfigData == null && simulatorConfigData.app.Count > 0)
            return;
        int pagenum = (simulatorConfigData.app.Count / 15) + 1;
        FrameworkTool.CreateChildList(pageGridParent, pagenum);

        Transform itemParent = pageGridParent.Find("Item/Grid");
        FrameworkTool.CreateChildList(itemParent, simulatorConfigData.app.Count);
        int quickCount = 0;
        for (int i = 0; i < simulatorConfigData.app.Count; i++)
        {
            Transform tran = itemParent.transform.GetChild(i);
            AppItemCeil ceil = PoolManager.Fetch<AppItemCeil>();
            ceil.OnInit(tran);
            ceil.RefreshData(simulatorConfigData.app[i]);
            if (simulatorConfigData.app[i].app_qucik == 1)
                quickCount++;
        }
        FrameworkTool.CreateChildList(bottomGridParent, quickCount);
        int index = 0;
        for (int i = 0; i < simulatorConfigData.app.Count; i++)
        {
            if (simulatorConfigData.app[i].app_qucik == 0)
                continue;
            Transform tran = bottomGridParent.transform.GetChild(index);
            AppItemCeil ceil = PoolManager.Fetch<AppItemCeil>();
            ceil.OnInit(tran);
            ceil.RefreshData(simulatorConfigData.app[i]);
            index++;
        }
    }

    private void DownLoadAppIcon()
    {
        for (int i = 0; i < simulatorConfigData.app.Count; i++)
        {
            AppData app = simulatorConfigData.app[i];
            StartCoroutine(IEDownloadTexture(app.app_icon, app));
        }
    }

    private IEnumerator IEDownloadTexture(string filename, AppData data = null)
    {
        if (filename.StartsWith("/"))
        {
            string dir = Environment.CurrentDirectory.Replace("\\", "/");
            filename = string.Format("{0}{1}", dir, filename);
        }
        if (!File.Exists(filename))
            yield break;
        string path = filename;///*"file:///" +*/ Application.streamingAssetsPath + "/" + filename + "/" + data.app_icon;
        using (UnityWebRequest wr = UnityWebRequestTexture.GetTexture(path))
        {
            yield return wr.SendWebRequest();
            if (!wr.isNetworkError || !wr.isHttpError)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(wr);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
                if (data != null)
                    data.sprite = sprite;
            }
            else
            {
                Debug.LogError(wr.error);
            }
        }
    }

    /// <summary>
    /// 读取app配置
    /// </summary>
    private void ReadAppConfig()
    {
        if (simulatorConfigData == null)
            simulatorConfigData = new SimulatorData();
        simulatorConfigData.app.Clear();
        string dir = Environment.CurrentDirectory.Replace("\\", "/");
        string config = string.Format("{0}/{1}", Application.dataPath, "app.json");
        string content = File.ReadAllText(config);
        Debug.LogError("json:" + content);
        simulatorConfigData = LitJson.JsonMapper.ToObject<SimulatorData>(content);

        ////TextAsset configText = Resources.Load("app") as TextAsset;
        //Stream stream = Lib.AssetLoader.AssetMananger.Instance.LoadStream("Config/app.json");
        //StreamReader sr = new StreamReader(stream);
        //string content = sr.ReadToEnd();
        //Debug.LogError("json:" + content);
        //simulatorConfigData = LitJson.JsonMapper.ToObject<SimulatorData>(content);
        //sr.Close();
        //sr.Dispose();
        //stream.Close();
        //stream.Dispose();
    }

    public class AppItemCeil
    {
        Button btn_app;
        Text t_name;
        Image icon;
        AppData data;

        ButtonLeftAndRight right;
        internal void OnInit(Transform tran)
        {
            t_name = tran.Find("Text").GetComponent<Text>();
            icon = tran.Find("Btn_Icon").GetComponent<Image>();
            btn_app = tran.Find("Btn_Icon").GetComponent<Button>();

            if (btn_app.GetComponent<ButtonLeftAndRight>() == null)
                right = btn_app.gameObject.AddComponent<ButtonLeftAndRight>();
            if (right != null)
                right.Init(this);
        }

        internal void RefreshData(AppData appData)
        {
            this.data = appData;
            t_name.text = data.app_name;
            icon.sprite = data.sprite;
        }
        public void OnClick_AppBtn()
        {
            if (data.app_type == 1)
            {
                bool isSuccess = StartProcess(data.app_link);
                if (isSuccess && data.app_close == 1)
                    Application.Quit();

            }
            else if (data.app_type == 2)
            {
                string url = data.app_link;
                Application.OpenURL(url);
            }
        }

        public bool StartProcess(string filename)
        {

            if (filename.StartsWith("/"))
            {
                string dir = Environment.CurrentDirectory.Replace("\\", "/");
                filename = string.Format("{0}{1}", dir, filename);
            }
            if (!File.Exists(filename))
                return false;
            //Debug.LogErrorFormat("filename:{0}", filename);
            try
            {
                //IL2cpp还未实现C#的process类，不能直接start启动外部程序
                //Process myprocess = new Process();
                uint ptr = StartExternalProcess.StartProcess(Directory.GetCurrentDirectory(), filename);

                Debug.Log($"pid:{ptr}");
                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("出错原因：" + ex.Message);
            }
            return false;
        }
    }
}
#endif


