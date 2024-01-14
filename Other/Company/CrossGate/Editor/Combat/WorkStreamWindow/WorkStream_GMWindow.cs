using Net;
using Packet;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class WorkStreamWindow
{
    public class GMInfoBlock
    {
        public List<GMInfo> GMInfoList = new List<GMInfo>();
    }
    public class GMInfo
    {
        public string Cmd;
        public string Param;
    }

    private List<GMInfoBlock> _gmBlockList = new List<GMInfoBlock>();
    private string _gmFileName = "Normal";

    private Vector2 _gmScrollPos;

    private void DoGM()
    {
        EditorGUILayout.BeginVertical();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加GM命令"))
        {
            _gmBlockList.Add(new GMInfoBlock());
        }
        if (GUILayout.Button("Save"))
        {
            if (!Directory.Exists(_workStreamEnumContentNotePath))
                Directory.CreateDirectory(_workStreamEnumContentNotePath);

            string prefsKey = $"WS_CombatBehaveAIWindow_GM_Cmd";
            if (PlayerPrefs.HasKey(prefsKey))
            {
                _gmFileName = PlayerPrefs.GetString(prefsKey);
            }

            string filePath = $"{_workStreamEnumContentNotePath}/{WorkStreamTypeKey}_{_gmFileName}_GM_Cmd.txt";

            List<string> strs = new List<string>();
            foreach (var block in _gmBlockList)
            {
                if (block.GMInfoList.Count > 0)
                {
                    string blockStr = string.Empty;
                    for (int blockIndex = 0; blockIndex < block.GMInfoList.Count; blockIndex++)
                    {
                        var gmInfo = block.GMInfoList[blockIndex];
                        blockStr += $"{gmInfo.Cmd}#{gmInfo.Param}{(blockIndex < block.GMInfoList.Count - 1 ? "|" : null)}";
                    }

                    if (!string.IsNullOrWhiteSpace(blockStr))
                        strs.Add(blockStr);
                }
            }

            EditorToolHelp.ExportTxtFileEx(filePath, strs);

            AssetDatabase.Refresh();
        }
        string gmFileName = GUILayout.TextField(_gmFileName, GUILayout.Width(100));
        if (gmFileName != _gmFileName)
        {
            _gmFileName = gmFileName;
            string prefsKey = $"WS_CombatBehaveAIWindow_GM_Cmd";
            PlayerPrefs.SetString(prefsKey, _gmFileName);
        }
        if (GUILayout.Button("刷新"))
        {
            ParseGMCmdContentNote();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(15);

        using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(Screen.height - 30f)))
        {
            using (EditorGUILayout.ScrollViewScope svs = new EditorGUILayout.ScrollViewScope(_gmScrollPos))
            {
                EditorGUILayout.Space(10f);

                for (int blockIndex = 0, count = _gmBlockList.Count; blockIndex < count; blockIndex++)
                {
                    GMInfoBlock block = _gmBlockList[blockIndex];

                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("+", GUILayout.Width(80)))
                    {
                        block.GMInfoList.Add(new GMInfo());
                    }
                    if (GUILayout.Button("-", GUILayout.Width(80)))
                    {
                        _gmBlockList.RemoveAt(blockIndex);
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);

                    if (block.GMInfoList.Count > 0)
                    {
                        for (int gmIndex = 0; gmIndex < block.GMInfoList.Count; gmIndex++)
                        {
                            var gMInfo = block.GMInfoList[gmIndex];

                            EditorGUILayout.BeginHorizontal();
                            gMInfo.Cmd = GUILayout.TextField(gMInfo.Cmd, GUILayout.Width(200));
                            GUILayout.Space(5);
                            gMInfo.Param = GUILayout.TextField(gMInfo.Param, GUILayout.Width(200));
                            if (GUILayout.Button("发送GM命令"))
                            {
                                CmdGmReq req = new CmdGmReq();
                                req.Cmd = Google.Protobuf.ByteString.CopyFrom(gMInfo.Cmd, System.Text.Encoding.UTF8); ;
                                req.Param = Google.Protobuf.ByteString.CopyFrom(gMInfo.Param, System.Text.Encoding.UTF8);

                                NetClient.Instance.SendMessage((ushort)CmdGm.Req, req);
                            }
                            if (GUILayout.Button("-"))
                            {
                                block.GMInfoList.RemoveAt(gmIndex);
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space(10);
                        }

                        if (GUILayout.Button("一键发送"))
                        {
                            for (int gmIndex = 0; gmIndex < block.GMInfoList.Count; gmIndex++)
                            {
                                var gMInfo = block.GMInfoList[gmIndex];

                                CmdGmReq req = new CmdGmReq();
                                req.Cmd = Google.Protobuf.ByteString.CopyFrom(gMInfo.Cmd, System.Text.Encoding.UTF8); ;
                                req.Param = Google.Protobuf.ByteString.CopyFrom(gMInfo.Param, System.Text.Encoding.UTF8);

                                NetClient.Instance.SendMessage((ushort)CmdGm.Req, req);
                            }
                        }
                    }

                    EditorGUILayout.EndVertical();


                    GUILayout.Space(15);
                }

                EditorGUILayout.Space(40f);

                _gmScrollPos = svs.scrollPosition;
            }
        }

        EditorGUILayout.EndVertical();
    }

    protected void ParseGMCmdContentNote()
    {
        if (!Directory.Exists(_workStreamEnumContentNotePath))
            return;

        string prefsKey = $"WS_CombatBehaveAIWindow_GM_Cmd";
        if (PlayerPrefs.HasKey(prefsKey))
        {
            _gmFileName = PlayerPrefs.GetString(prefsKey);
        }

        string filePath = $"{_workStreamEnumContentNotePath}/{WorkStreamTypeKey}_{_gmFileName}_GM_Cmd.txt";
        if (!File.Exists(filePath))
            return;

        _gmBlockList.Clear();

        EditorToolHelp.ParseTxtInLine(filePath, (string line) =>
        {
            if (!string.IsNullOrEmpty(line))
            {
                string[] strs0 = line.Split('|');
                if (strs0 != null && strs0.Length > 0)
                {
                    GMInfoBlock gMInfoBlock = new GMInfoBlock();
                    foreach (var item in strs0)
                    {
                        string[] strs1 = item.Split('#');
                        if (strs1 != null && strs1.Length > 1)
                        {
                            GMInfo gMInfo = new GMInfo();
                            gMInfo.Cmd = strs1[0];
                            gMInfo.Param = strs1[1];

                            gMInfoBlock.GMInfoList.Add(gMInfo);
                        }
                    }

                    if (gMInfoBlock.GMInfoList.Count > 0)
                        _gmBlockList.Add(gMInfoBlock);
                }
            }
        });
    }
}
