#if UNITY_EDITOR_NO_USE
using Google.Protobuf.Collections;
using Logic;
using Logic.Core;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Table;
using UnityEditor;
using UnityEngine;

public class CombatSimulateEditorWindow : EditorWindow
{
    public class CommandInfoInCombatEditor
    {
        public int ServerNum;
        public int SrcServerNum;

        public int SelectCommandIndex;
        public bool IsSelecting;
        public string[] CommandStrs;
        public CommandEditor[] CommandEditors;
        
        public FightControl m_FightControl;
    }

    public class CommandEditor
    {
        public int BattleCommandType;
        public uint SrcUnitId;
        public int TargetPos;
        public uint SkillId;

        public int SelectTargetServerNumIndex;
        public bool IsSelecting;
        public string[] TargetServerNumStrs;
        public int[] TargetServerNums;
    }

    private GUIStyle _buttonSt;
    private GUIStyle _popupSt;
    private GUIStyle _labelSt;
    private GUIStyle _labelSt02;
    private GUIStyle _labelSt03;
    private GUIStyle _miniButtonMidSt;
    private GUIStyle _combatUnitButtonSt;

    private DragAreaEditor _dragAreaEditor = new DragAreaEditor(0f, 0f, 200f);
    
    private DragAreaEditor _dragAreaEditor02 = new DragAreaEditor(0, 0f, 250f);
    private Vector2 _scrollPos;

    private int _selectServerNum;
    private bool _isSetRole;
    private List<uint> _pets;

    /// <summary>
    /// =0单人模式， =1双人模式， =2单人pve模式，  =3双人pve模式
    /// </summary>
    private int _menuStyle;

    private Dictionary<int, CombatSimulateUnitInfo> _singleCombatUnitInfoDic = new Dictionary<int, CombatSimulateUnitInfo>();

    private Dictionary<int, CombatSimulateUnitInfo> _multiCombatUnitInfoDic = new Dictionary<int, CombatSimulateUnitInfo>();

    private Dictionary<int, CombatSimulateUnitInfo> _pveSingleCombatUnitInfoDic = new Dictionary<int, CombatSimulateUnitInfo>();

    private Dictionary<int, CombatSimulateUnitInfo> _pveMultiCombatUnitInfoDic = new Dictionary<int, CombatSimulateUnitInfo>();

    private int _fightMenuType;
    private Vector2 _fightScrollPos;
    private Vector2 _fightScrollPos02;
    private Dictionary<int, CommandInfoInCombatEditor> _commandInfoInCombatEditor = new Dictionary<int, CommandInfoInCombatEditor>();

    private int _enemyGroupId;

    [MenuItem("Tools/Combat/战斗模拟器")]
    public static void OpenFSM()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("提示", "要在游戏中打开！！！！", "OK");
            return;
        }

        CombatSimulateManager.Instance.InitEditor();

        CombatSimulateEditorWindow window = GetWindow<CombatSimulateEditorWindow>(false, "战斗模拟器", true);
        window.Show();

        window.Init();
    }

    private void OnEnable()
    {
        CSVTestRole.Load();
        CSVTestPet.Load();

        _selectServerNum = -1;
    }

    private void OnDisable()
    {
        if (CSVTestRole.Instance != null)
            CSVTestRole.Unload();
        if (CSVTestPet.Instance != null)
            CSVTestPet.Unload();
    }

    private void Init()
    {
        _buttonSt = new GUIStyle(EditorStyles.miniButtonMid);
        _buttonSt.fontSize = 15;
        _buttonSt.fixedHeight = 21;
        _buttonSt.fontStyle = FontStyle.Bold;
        _buttonSt.normal.textColor = new Color(0.8f, 0.8f, 0.2f, 1);

        _popupSt = new GUIStyle(EditorStyles.popup);
        _popupSt.fontSize = 15;
        _popupSt.fixedHeight = 20;
        _popupSt.fontStyle = FontStyle.Bold;
        _popupSt.normal.textColor = new Color(0.2f, 0.8f, 0.2f, 1);

        _labelSt = new GUIStyle(EditorStyles.textField);
        _labelSt.wordWrap = true;
        _labelSt.fixedHeight = 20;
        _labelSt.fontSize = 15;

        _labelSt02 = new GUIStyle(EditorStyles.textField);
        _labelSt02.wordWrap = true;
        _labelSt02.fontSize = 15;

        _labelSt03 = new GUIStyle(EditorStyles.textField);
        _labelSt03.wordWrap = true;
        _labelSt03.fontSize = 25;

        _miniButtonMidSt = new GUIStyle(EditorStyles.miniButtonMid);
        _miniButtonMidSt.fontSize = 16;
        _miniButtonMidSt.fixedHeight = 20;
        _miniButtonMidSt.padding.bottom = 2;
        _miniButtonMidSt.fontStyle = FontStyle.Bold;

        _combatUnitButtonSt = new GUIStyle(EditorStyles.miniButtonMid);
        _combatUnitButtonSt.fontSize = 16;
        _combatUnitButtonSt.fixedHeight = 60;
        _combatUnitButtonSt.fontStyle = FontStyle.Bold;
        _combatUnitButtonSt.wordWrap = true;

        CombatSimulateManager.Instance.Init();
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            Close();
            return;
        }

        if (CombatManager.Instance.m_IsFight && CombatSimulateManager.Instance.m_CombatUnitSum <= MobManager.Instance.m_MobDic.Count)
        {
            _battleId = 0u;

            DrawInFight();
        }
        else
        {
            _commandInfoInCombatEditor.Clear();
            DrawOutFight();
        }
    }

    void OnInspectorUpdate()
    {
        //开启窗口的重绘，不然窗口信息不会刷新
        Repaint();
    }

    #region OutFight
    private void DrawOutFight()
    {
        EditorGUILayout.BeginHorizontal();
        DrawLeftOutFight();
        DrawRightOutFight();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLeftOutFight()
    {
        _dragAreaEditor.BeginVerticalDragArea(false);

        using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(Screen.height - 20f)))
        {
            DrawLeftMenu("单人模式", 0);
            EditorGUILayout.Space();
            DrawLeftMenu("双人模式", 1);
            EditorGUILayout.Space();
            DrawLeftMenu("单人PVE模式", 2);
            //EditorGUILayout.Space();
            //DrawLeftMenu("双人PVE模式", 3);
        }

        _dragAreaEditor.EndVerticalDragArea(Repaint);
    }

    private void DrawLeftMenu(string context, int menuStyle, Action action = null)
    {
        _miniButtonMidSt.normal.textColor = _menuStyle == menuStyle ? Color.yellow : new Color(0.2f, 0.6f, 0.8f, 1);

        if (GUILayout.Button(context, _miniButtonMidSt))
        {
            _menuStyle = menuStyle;

            action?.Invoke();
        }
    }

    private void DrawRightOutFight()
    {
        if (_menuStyle == 0)
            DrawRightSingleStyle();
        else if (_menuStyle == 1)
            DrawRightMultiStyle();
        else if (_menuStyle == 2)
            DrawRightPveSingleStyle();
        else if (_menuStyle == 3)
            DrawRightPveMultiStyle();
    }

    private void DrawRightSingleStyle()
    {
        EditorGUILayout.BeginHorizontal();
        DrawCombatUnitMenus();

        EditorGUILayout.BeginVertical();
        DrawUpCombatUnits();

        GUILayout.Space(Screen.height * 0.15f);
        if (GUILayout.Button("准备完毕", _combatUnitButtonSt))
        {
            var dic = GetCombatUnitInfoDic(_menuStyle);
            if (dic == null || dic.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "没有选取任何角色", "OK");
            }
            else
            {
                SetSimulateDatas();

                CombatSimulateManager.Instance.SendEnterSimulateCombat(_menuStyle + 1, "0");
            }
        }

        DrawDownCombatUnits();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private uint _battleId;
    private ulong _roleId;
    private void DrawRightMultiStyle()
    {
        EditorGUILayout.BeginHorizontal();
        DrawCombatUnitMenus();

        EditorGUILayout.BeginVertical();
        DrawUpCombatUnits();

        GUILayout.Space(Screen.height * 0.1f);

        if (CombatSimulateManager.Instance.m_IsAlready)
            EditorGUILayout.LabelField($"当前战斗Id：{CombatManager.Instance.m_BattleId.ToString()}", GUILayout.Width(100f));
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextArea(Sys_Role.Instance?.Role?.RoleId.ToString(), GUILayout.Width(Screen.height * 0.3f));
        GUILayout.Space(Screen.height * 0.4f);
        EditorGUILayout.LabelField("填写战斗Id：", GUILayout.Width(100f));
        _battleId = (uint)EditorGUILayout.IntField((int)_battleId, GUILayout.Width(80f));
        if (GUILayout.Button("进入战斗", GUILayout.Width(80f)) && _battleId > 0u)
        {
            if (CombatSimulateManager.Instance.m_IsSendEnterBattle)
            {
                Debug.Log("已经点击了进入战斗");
            }
            else
            {
                var unitDic = GetCombatUnitInfoDic(_menuStyle);
                if (unitDic == null || unitDic.Count == 0)
                {
                    EditorUtility.DisplayDialog("提示", "没有选取任何角色", "OK");
                }
                else
                {
                    SetSimulateDatas();

                    CombatSimulateManager.Instance.SendBattleSimulationReq(_battleId, _menuStyle + 1);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("发起战斗填写RoleId：", GUILayout.Width(150f));
        string roleIdStr = EditorGUILayout.TextArea(_roleId.ToString(), GUILayout.Width(400f));
        if (ulong.TryParse(roleIdStr, out ulong roleId))
            _roleId = roleId;
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("发起战斗", _combatUnitButtonSt))
        {
            if (_roleId == 0ul)
            {
                Debug.LogError("先填写RoleId，再发起战斗！！！");
            }
            else if (CombatSimulateManager.Instance.m_IsAlready)
            {
                Debug.Log("发起战斗");
            }
            else
            {
                var unitDic = GetCombatUnitInfoDic(_menuStyle);
                if (unitDic == null || unitDic.Count == 0)
                {
                    EditorUtility.DisplayDialog("提示", "没有选取任何角色", "OK");
                }
                else
                {
                    SetSimulateDatas();

                    CombatSimulateManager.Instance.SendEnterSimulateCombat(_menuStyle + 1, _roleId.ToString());
                }
            }
        }

        DrawDownCombatUnits();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawRightPveSingleStyle()
    {
        EditorGUILayout.BeginHorizontal();
        DrawCombatUnitMenus();

        EditorGUILayout.BeginVertical();

        GUILayout.Space(Screen.height * 0.1f);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(Screen.width * 0.2f);
        EditorGUILayout.LabelField("遇敌组：", GUILayout.Width(50f), GUILayout.Height(25f));
        _enemyGroupId = EditorGUILayout.IntField(_enemyGroupId, _labelSt, GUILayout.Width(300f), GUILayout.Height(25f));
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(Screen.height * 0.15f);
        if (GUILayout.Button("准备完毕", _combatUnitButtonSt) && _enemyGroupId > 0)
        {
            var unitDic = GetCombatUnitInfoDic(_menuStyle);
            if (unitDic == null || unitDic.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "没有选取任何角色", "OK");
            }
            else
            {
                SetSimulateDatas();

                CombatSimulateManager.Instance.SendEnterSimulateCombat(_menuStyle + 1, _enemyGroupId.ToString());
            }
        }

        DrawDownCombatUnits();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawRightPveMultiStyle()
    {
        EditorGUILayout.BeginHorizontal();
        DrawCombatUnitMenus();

        EditorGUILayout.BeginVertical();

        GUILayout.Space(Screen.height * 0.41f);
        if (GUILayout.Button("准备完毕", _combatUnitButtonSt))
        {
            SetSimulateDatas();
        }

        DrawDownCombatUnits();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawCombatUnitMenus()
    {
        _dragAreaEditor02.StartX = _dragAreaEditor.Width;

        _dragAreaEditor02.BeginVerticalDragArea(false);

        using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(Screen.height - 20f)))
        {
            using (EditorGUILayout.ScrollViewScope svs = new EditorGUILayout.ScrollViewScope(_scrollPos))
            {
                if (_selectServerNum > -1)
                {
                    var dic = GetCombatUnitInfoDic(_menuStyle);

                    if (GUILayout.Button("空", _buttonSt))
                    {
                        dic.Remove(_selectServerNum);
                    }

                    if (_isSetRole)
                    {
                        foreach (var kv in CSVTestRole.Instance.GetAll())
                        {
                            var lanTb = CSVLanguage.Instance.GetConfData(kv.name);
                            if (lanTb == null)
                            {
                                Debug.LogError($"CSVTestRole表中Id：{kv.id.ToString()}的name：{kv.name.ToString()}在CSVLanguage表中没有Id：{kv.name.ToString()}");
                            }
                            else if (GUILayout.Button(lanTb.words, _buttonSt))
                            {
                                if (!dic.TryGetValue(_selectServerNum, out CombatSimulateUnitInfo combatUnitInfoEditor) || combatUnitInfoEditor == null)
                                {
                                    combatUnitInfoEditor = new CombatSimulateUnitInfo();

                                    dic[_selectServerNum] = combatUnitInfoEditor;
                                }

                                combatUnitInfoEditor.ServerNum = _selectServerNum;
                                combatUnitInfoEditor.TestRoleDataTb = kv;
                                combatUnitInfoEditor.TestPetDataTb = null;
                            }
                        }
                    }
                    else
                    {
                        if (_pets != null && _pets.Count > 0)
                        {
                            foreach (var petId in _pets)
                            {
                                CSVTestPet.Data petTb = CSVTestPet.Instance.GetConfData(petId);
                                if (petTb == null)
                                    continue;

                                if (GUILayout.Button(CSVLanguage.Instance.GetConfData(petTb.name).words, _buttonSt))
                                {
                                    if (!dic.TryGetValue(_selectServerNum, out CombatSimulateUnitInfo combatUnitInfoEditor) || combatUnitInfoEditor == null)
                                    {
                                        combatUnitInfoEditor = new CombatSimulateUnitInfo();

                                        dic[_selectServerNum] = combatUnitInfoEditor;
                                    }

                                    combatUnitInfoEditor.ServerNum = _selectServerNum;
                                    combatUnitInfoEditor.TestRoleDataTb = null;
                                    combatUnitInfoEditor.TestPetDataTb = petTb;
                                }
                            }
                        }
                    }
                }

                _scrollPos = svs.scrollPosition;
            }
        }

        _dragAreaEditor02.EndVerticalDragArea(Repaint);
    }

    private void DrawUpCombatUnits()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Space(Screen.height * 0.05f);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(Screen.width * 0.03f);
        DrawCombatUnitButton(30);
        GUILayout.Space(10f);
        DrawCombatUnitButton(31);
        GUILayout.Space(10f);
        DrawCombatUnitButton(32);
        GUILayout.Space(10f);
        DrawCombatUnitButton(33);
        GUILayout.Space(10f);
        DrawCombatUnitButton(34);
        GUILayout.Space(Screen.width * 0.03f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(Screen.width * 0.03f);
        DrawCombatUnitButton(20);
        GUILayout.Space(10f);
        DrawCombatUnitButton(21);
        GUILayout.Space(10f);
        DrawCombatUnitButton(22);
        GUILayout.Space(10f);
        DrawCombatUnitButton(23);
        GUILayout.Space(10f);
        DrawCombatUnitButton(24);
        GUILayout.Space(Screen.width * 0.03f);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawDownCombatUnits()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Space(Screen.height * 0.15f);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(Screen.width * 0.03f);
        DrawCombatUnitButton(10);
        GUILayout.Space(10f);
        DrawCombatUnitButton(11);
        GUILayout.Space(10f);
        DrawCombatUnitButton(12);
        GUILayout.Space(10f);
        DrawCombatUnitButton(13);
        GUILayout.Space(10f);
        DrawCombatUnitButton(14);
        GUILayout.Space(Screen.width * 0.03f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(Screen.width * 0.03f);
        DrawCombatUnitButton(0);
        GUILayout.Space(10f);
        DrawCombatUnitButton(1);
        GUILayout.Space(10f);
        DrawCombatUnitButton(2);
        GUILayout.Space(10f);
        DrawCombatUnitButton(3);
        GUILayout.Space(10f);
        DrawCombatUnitButton(4);
        GUILayout.Space(Screen.width * 0.03f);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawCombatUnitButton(int serverNum)
    {
        var dic = GetCombatUnitInfoDic(_menuStyle);

        dic.TryGetValue(serverNum, out CombatSimulateUnitInfo combatUnitInfoEditor);

        string btnStr = combatUnitInfoEditor == null ? $"选择出战单位({serverNum.ToString()}-{CombatHelp.ServerToClientNum(serverNum).ToString()})" :
            $"{(combatUnitInfoEditor.TestRoleDataTb == null ? (combatUnitInfoEditor.TestPetDataTb == null ? serverNum.ToString() : CSVLanguage.Instance.GetConfData(combatUnitInfoEditor.TestPetDataTb.name).words) : CSVLanguage.Instance.GetConfData(combatUnitInfoEditor.TestRoleDataTb.name).words)}";

        if (serverNum == _selectServerNum)
        {
            _combatUnitButtonSt.normal.textColor = Color.yellow;
            _combatUnitButtonSt.hover.textColor = Color.yellow;
        }
        else
        {
            _combatUnitButtonSt.normal.textColor = Color.white;
            _combatUnitButtonSt.hover.textColor = Color.white;
        }

        if (GUILayout.Button(btnStr, _combatUnitButtonSt, GUILayout.Width(180f)))
        {
            _selectServerNum = serverNum;

            _isSetRole = true;
            if (combatUnitInfoEditor != null)
            {
                if (combatUnitInfoEditor.TestPetDataTb != null)
                    _isSetRole = false;
                else if (combatUnitInfoEditor.TestRoleDataTb != null)
                    _pets = combatUnitInfoEditor.TestRoleDataTb.pet_list;
            }
            else
            {
                int neighborServerNum = GetNeighborServerNum(serverNum);
                if (dic.TryGetValue(neighborServerNum, out CombatSimulateUnitInfo neighborCuie) && neighborCuie != null)
                {
                    if (neighborCuie.TestRoleDataTb != null)
                    {
                        _isSetRole = false;
                        _pets = neighborCuie.TestRoleDataTb.pet_list;
                    }
                }
            }
        }
    }
    #endregion

    #region InFight
    private void DrawInFight()
    {
        EditorGUILayout.BeginHorizontal();
        DrawLeftInFight();
        DrawRightInFight();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLeftInFight()
    {
        _dragAreaEditor.BeginVerticalDragArea(false);

        using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(Screen.height - 20f)))
        {
            _miniButtonMidSt.normal.textColor = _fightMenuType == 0 ? Color.yellow : new Color(0.2f, 0.6f, 0.8f, 1);
            if (GUILayout.Button("战斗命令选择", _miniButtonMidSt))
            {
                _fightMenuType = 0;
            }

            _miniButtonMidSt.normal.textColor = _fightMenuType == 1 ? Color.yellow : new Color(0.2f, 0.6f, 0.8f, 1);
            if (GUILayout.Button("战斗统计", _miniButtonMidSt))
            {
                _fightMenuType = 1;
            }
        }

        _dragAreaEditor.EndVerticalDragArea(Repaint);
    }

    private void DrawRightInFight()
    {
        if (_fightMenuType == 0)
            DrawSelectCommands();
        else
            DrawCombatEndDatas();
    }

    private void DrawSelectCommands()
    {
        using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(Screen.height - 30f)))
        {
            using (EditorGUILayout.ScrollViewScope svs = new EditorGUILayout.ScrollViewScope(_fightScrollPos))
            {
                EditorGUILayout.Space(15f);

                var dic = GetCombatUnitInfoDic(_menuStyle);
                
                EditorGUILayout.BeginVertical();
                for (int i = 3; i > -1; i--)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < 5; j++)
                    {
                        int sn = i * 10 + j;
                        if (dic.TryGetValue(sn, out CombatSimulateUnitInfo combatUnitInfoEditor) && combatUnitInfoEditor != null
                            &&
                            (combatUnitInfoEditor.TestRoleDataTb != null || combatUnitInfoEditor.TestPetDataTb != null)
                            )
                        {
                            DrawSelectCommand(sn, combatUnitInfoEditor);
                        }
                        else
                        {
                            int neighborServerNum = GetNeighborServerNum(sn);
                            if (dic.TryGetValue(neighborServerNum, out CombatSimulateUnitInfo neighborCombatUnitInfoEditor) && neighborCombatUnitInfoEditor != null
                                &&
                                (neighborCombatUnitInfoEditor.TestRoleDataTb != null || neighborCombatUnitInfoEditor.TestPetDataTb != null)
                                )
                            {
                                DrawSelectCommand(sn, neighborCombatUnitInfoEditor, 1);
                            }
                            else
                            {
                                DrawSelectCommand(sn, null, 2);
                            }
                        }

                        EditorGUILayout.Space(2f);
                    }
                    EditorGUILayout.EndHorizontal();

                    if (i == 2)
                    {
                        EditorGUILayout.Space(50f);
                        if (GUILayout.Button($"{((Net_Combat.Instance.m_RoundOver && !CombatSimulateManager.Instance.m_IsSendSimulationCommand) ? "发送指令" : "正在执行指令中。。。")}", _buttonSt, GUILayout.Height(30f)))
                        {
                            if (Net_Combat.Instance.m_RoundOver && !CombatSimulateManager.Instance.m_IsSendSimulationCommand)
                            {
                                RepeatedField<BattleCommand> battleCommands = new RepeatedField<BattleCommand>();
                                foreach (var kv in _commandInfoInCombatEditor)
                                {
                                    var ciice = kv.Value;
                                    if (ciice == null || ciice.CommandEditors == null || ciice.CommandEditors.Length == 0 || ciice.SelectCommandIndex <= 0)
                                        continue;

                                    CommandEditor commandEditor = ciice.CommandEditors[ciice.SelectCommandIndex];
                                    battleCommands.Add(FightControl.GetBattleCommand(commandEditor.BattleCommandType, commandEditor.SrcUnitId, commandEditor.TargetPos, commandEditor.SkillId));
                                }
                                CombatSimulateManager.Instance.SendCmdBattleSimulationCommandReq(battleCommands);
                            }
                        }
                        EditorGUILayout.Space(50f);
                    }
                    else
                        EditorGUILayout.Space(10f);
                }
                EditorGUILayout.EndVertical();
                
                _fightScrollPos = svs.scrollPosition;
            }
        }
    }

    private void DrawSelectCommand(int serverNum, CombatSimulateUnitInfo combatUnitInfoEditor, int drawState = 0)
    {
        EditorGUILayout.BeginVertical(_labelSt02, GUILayout.Width(210f), GUILayout.Height(80f));

        if (drawState == 2)
        {
            EditorGUILayout.LabelField("无", GUILayout.Width(209f), GUILayout.Height(75f));
        }
        else
        {
            if (drawState == 1)
            {
                EditorGUILayout.LabelField("------------二动数据------------", GUILayout.Width(203f));
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"位置{serverNum.ToString()}({CombatHelp.ServerToClientNum(serverNum).ToString()}): ", GUILayout.Width(70f));
                EditorGUILayout.LabelField(CSVLanguage.Instance.GetConfData(combatUnitInfoEditor.TestRoleDataTb != null ? combatUnitInfoEditor.TestRoleDataTb.name : combatUnitInfoEditor.TestPetDataTb.name).words,
                    GUILayout.Width(120f));
                //EditorGUILayout.LabelField("魔法师魔法师魔法师魔", GUILayout.Width(150f));
                EditorGUILayout.EndHorizontal();
            }

            if (!_commandInfoInCombatEditor.TryGetValue(serverNum, out CommandInfoInCombatEditor commandInfoInCombatEditor) || commandInfoInCombatEditor == null)
            {
                commandInfoInCombatEditor = new CommandInfoInCombatEditor();
                //commandInfoInCombatEditor.m_FightControl = GameCenter.fightWorld.CreateActor<FightControl>((ulong)serverNum);
                //TODO FightControl销毁时机
                commandInfoInCombatEditor.m_FightControl = World.AllocActor<FightControl>((ulong)serverNum);
                commandInfoInCombatEditor.ServerNum = serverNum;
                commandInfoInCombatEditor.SrcServerNum = combatUnitInfoEditor.ServerNum;

                var mob = MobManager.Instance.GetMobByServerNum(commandInfoInCombatEditor.SrcServerNum);
                uint srcUnitId = 0u;
                if (mob != null && mob.m_MobCombatComponent != null && mob.m_MobCombatComponent.m_BattleUnit != null)
                    srcUnitId = mob.m_MobCombatComponent.m_BattleUnit.UnitId;

                int commandCount = 1;
                if (combatUnitInfoEditor.TestRoleDataTb != null)
                {
                    commandCount += 3;
                    if (combatUnitInfoEditor.TestRoleDataTb.skill_list != null)
                    {
                        commandCount += combatUnitInfoEditor.TestRoleDataTb.skill_list.Count;
                    }

                    commandInfoInCombatEditor.CommandStrs = new string[commandCount];
                    commandInfoInCombatEditor.CommandEditors = new CommandEditor[commandCount];

                    commandInfoInCombatEditor.CommandStrs[0] = "选择操作";

                    commandInfoInCombatEditor.CommandStrs[1] = "防御";
                    commandInfoInCombatEditor.CommandEditors[1] = new CommandEditor
                    {
                        BattleCommandType = 1,
                        SrcUnitId = srcUnitId,
                        TargetPos = commandInfoInCombatEditor.SrcServerNum
                    };

                    commandInfoInCombatEditor.CommandStrs[2] = "召唤宠物";
                    commandInfoInCombatEditor.CommandEditors[2] = new CommandEditor();

                    commandInfoInCombatEditor.CommandStrs[3] = "召回宠物";
                    commandInfoInCombatEditor.CommandEditors[3] = new CommandEditor();

                    if (combatUnitInfoEditor.TestRoleDataTb.skill_list != null)
                    {
                        for (int i = 0; i < combatUnitInfoEditor.TestRoleDataTb.skill_list.Count; i++)
                        {
                            var skillInfoTb = CSVActiveSkillInfo.Instance.GetConfData(combatUnitInfoEditor.TestRoleDataTb.skill_list[i]);
                            if (skillInfoTb == null)
                                continue;

                            commandInfoInCombatEditor.CommandStrs[i + 4] = CSVLanguage.Instance.GetConfData(skillInfoTb.name).words;

                            CommandEditor commandEditor = new CommandEditor
                            {
                                BattleCommandType = 4,
                                SrcUnitId = srcUnitId,
                                SkillId = skillInfoTb.active_skillid
                            };
                            commandInfoInCombatEditor.CommandEditors[i + 4] = commandEditor;
                        }
                    }
                }
                else if (combatUnitInfoEditor.TestPetDataTb != null)
                {
                    commandCount += 1;
                    if (combatUnitInfoEditor.TestPetDataTb.skill_list != null)
                    {
                        commandCount += combatUnitInfoEditor.TestPetDataTb.skill_list.Count;
                    }

                    commandInfoInCombatEditor.CommandStrs = new string[commandCount];
                    commandInfoInCombatEditor.CommandEditors = new CommandEditor[commandCount];

                    commandInfoInCombatEditor.CommandStrs[0] = "选择操作";

                    commandInfoInCombatEditor.CommandStrs[1] = "防御";
                    commandInfoInCombatEditor.CommandEditors[1] = new CommandEditor
                    {
                        BattleCommandType = 1,
                        SrcUnitId = srcUnitId,
                        TargetPos = commandInfoInCombatEditor.SrcServerNum
                    };

                    if (combatUnitInfoEditor.TestPetDataTb.skill_list != null)
                    {
                        for (int i = 0; i < combatUnitInfoEditor.TestPetDataTb.skill_list.Count; i++)
                        {
                            var skillInfoTb = CSVActiveSkillInfo.Instance.GetConfData(combatUnitInfoEditor.TestPetDataTb.skill_list[i]);
                            if (skillInfoTb == null)
                                continue;

                            commandInfoInCombatEditor.CommandStrs[i + 2] = CSVLanguage.Instance.GetConfData(skillInfoTb.name).words;

                            CommandEditor commandEditor = new CommandEditor
                            {
                                BattleCommandType = 4,
                                SrcUnitId = srcUnitId,
                                SkillId = skillInfoTb.active_skillid
                            };
                            commandInfoInCombatEditor.CommandEditors[i + 2] = commandEditor;
                        }
                    }
                }
                else
                {
                    commandCount += 3;

                    commandInfoInCombatEditor.CommandStrs = new string[commandCount];
                    commandInfoInCombatEditor.CommandEditors = new CommandEditor[commandCount];

                    commandInfoInCombatEditor.CommandStrs[0] = "选择操作";

                    commandInfoInCombatEditor.CommandStrs[1] = "防御";
                    commandInfoInCombatEditor.CommandEditors[1] = new CommandEditor
                    {
                        BattleCommandType = 1,
                        SrcUnitId = srcUnitId,
                        TargetPos = commandInfoInCombatEditor.SrcServerNum
                    };

                    commandInfoInCombatEditor.CommandStrs[2] = "召唤宠物";
                    commandInfoInCombatEditor.CommandEditors[2] = new CommandEditor();

                    commandInfoInCombatEditor.CommandStrs[3] = "召回宠物";
                    commandInfoInCombatEditor.CommandEditors[3] = new CommandEditor();
                }
                _commandInfoInCombatEditor[serverNum] = commandInfoInCombatEditor;

                foreach (var ceItem in commandInfoInCombatEditor.CommandEditors)
                {
                    SetSelectList(commandInfoInCombatEditor, ceItem);
                }
            }

            //int selectCommandIndex = EditorGUILayout.Popup(commandInfoInCombatEditor.SelectCommandIndex, commandInfoInCombatEditor.CommandStrs, _popupSt);
            //if (selectCommandIndex != commandInfoInCombatEditor.SelectCommandIndex)
            //{
            //    commandInfoInCombatEditor.SelectCommandIndex = selectCommandIndex;
            //}

            int selectCommandIndex = commandInfoInCombatEditor.SelectCommandIndex;
            if (GUILayout.Button(commandInfoInCombatEditor.CommandStrs[commandInfoInCombatEditor.SelectCommandIndex]))
            {
                commandInfoInCombatEditor.IsSelecting = !commandInfoInCombatEditor.IsSelecting;
            }
            if (commandInfoInCombatEditor.IsSelecting)
            {
                EditorGUILayout.Space(1f);
                for (int commandStrIndex = 0; commandStrIndex < commandInfoInCombatEditor.CommandStrs.Length; commandStrIndex++)
                {
                    var commandStr = commandInfoInCombatEditor.CommandStrs[commandStrIndex];
                    if (GUILayout.Button(commandStr))
                    {
                        commandInfoInCombatEditor.IsSelecting = false;
                        commandInfoInCombatEditor.SelectCommandIndex = commandStrIndex;
                        selectCommandIndex = commandStrIndex;
                    }
                }
            }

            EditorGUILayout.Space(5f);

            CommandEditor selectCommandEditor = commandInfoInCombatEditor.CommandEditors[selectCommandIndex];
            if (selectCommandEditor != null && selectCommandEditor.TargetServerNumStrs != null)
            {
                //selectCommandEditor.SelectTargetServerNumIndex = EditorGUILayout.Popup(selectCommandEditor.SelectTargetServerNumIndex, selectCommandEditor.TargetServerNumStrs, _popupSt);
                
                if (GUILayout.Button(selectCommandEditor.TargetServerNumStrs[selectCommandEditor.SelectTargetServerNumIndex]))
                {
                    selectCommandEditor.IsSelecting = !selectCommandEditor.IsSelecting;
                }
                if (selectCommandEditor.IsSelecting)
                {
                    EditorGUILayout.Space(1f);
                    for (int selectTSNIndex = 0; selectTSNIndex < selectCommandEditor.TargetServerNumStrs.Length; selectTSNIndex++)
                    {
                        var targetServerNumStr = selectCommandEditor.TargetServerNumStrs[selectTSNIndex];
                        if (GUILayout.Button(targetServerNumStr))
                        {
                            selectCommandEditor.IsSelecting = false;
                            selectCommandEditor.SelectTargetServerNumIndex = selectTSNIndex;
                        }
                    }
                }
                
                CommandEditor ce = commandInfoInCombatEditor.CommandEditors[selectCommandIndex];
                if (ce != null)
                {
                    if (selectCommandEditor.TargetServerNums != null && selectCommandEditor.TargetServerNums.Length > 1)
                        ce.TargetPos = selectCommandEditor.TargetServerNums[selectCommandEditor.SelectTargetServerNumIndex];
                    else
                        ce.TargetPos = combatUnitInfoEditor.ServerNum;
                }
            }
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawCombatEndDatas()
    {
        using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(Screen.height - 30f)))
        {
            using (EditorGUILayout.ScrollViewScope svs = new EditorGUILayout.ScrollViewScope(_fightScrollPos02))
            {
                EditorGUILayout.BeginHorizontal(_labelSt02, GUILayout.Height(40f));
                if (GUILayout.Button("刷新", GUILayout.Height(30f), GUILayout.Width(180f)))
                {
                    CombatSimulateManager.Instance.RefreshUnitDataReq();
                }
                EditorGUILayout.LabelField("伤害量", _labelSt03, GUILayout.Height(40f), GUILayout.Width(120f));
                EditorGUILayout.LabelField("击杀", _labelSt03, GUILayout.Height(40f), GUILayout.Width(80f));
                EditorGUILayout.LabelField("治疗量", _labelSt03, GUILayout.Height(40f), GUILayout.Width(120f));
                EditorGUILayout.LabelField("承受伤害量", _labelSt03, GUILayout.Height(40f), GUILayout.Width(200f));
                EditorGUILayout.LabelField("被控行动数", _labelSt03, GUILayout.Height(40f), GUILayout.Width(200f));
                EditorGUILayout.EndHorizontal();
                
                for (int i = 3; i > -1; i--)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        EditorGUILayout.BeginHorizontal(_labelSt02, GUILayout.Height(40f));

                        int sn = i * 10 + j;
                        if (_commandInfoInCombatEditor.TryGetValue(sn, out CommandInfoInCombatEditor commandInfoInCombatEditor) && commandInfoInCombatEditor != null && commandInfoInCombatEditor.ServerNum == commandInfoInCombatEditor.SrcServerNum)
                        {
                            DrawCombatEndUnitData(commandInfoInCombatEditor);
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                _fightScrollPos02 = svs.scrollPosition;
            }
        }
    }

    private void DrawCombatEndUnitData(CommandInfoInCombatEditor commandInfoInCombatEditor)
    {
        var cssi = CombatSimulateManager.Instance.GetCombatSimulateStatisticsInfo(commandInfoInCombatEditor.SrcServerNum);
        if (cssi == null)
            return;

        EditorGUILayout.LabelField($"位置{commandInfoInCombatEditor.ServerNum.ToString()}", GUILayout.Height(40f), GUILayout.Width(180f));
        EditorGUILayout.LabelField(cssi.Dmg.ToString(), _labelSt02, GUILayout.Height(40f), GUILayout.Width(120f));
        EditorGUILayout.LabelField(cssi.KillNum.ToString(), _labelSt02, GUILayout.Height(40f), GUILayout.Width(80f));
        EditorGUILayout.LabelField(cssi.AddHp.ToString(), _labelSt02, GUILayout.Height(40f), GUILayout.Width(120f));
        EditorGUILayout.LabelField(cssi.BeDmg.ToString(), _labelSt02, GUILayout.Height(40f), GUILayout.Width(200f));
        EditorGUILayout.LabelField(cssi.BeControlledNum.ToString(), _labelSt02, GUILayout.Height(40f), GUILayout.Width(200f));
    }
    #endregion

    #region 逻辑处理
    private void SetSimulateDatas()
    {
        var dic = GetCombatUnitInfoDic(_menuStyle);
        foreach (var kv in dic)
        {
            if (kv.Value == null || (kv.Value.TestRoleDataTb == null && kv.Value.TestPetDataTb == null))
                continue;

            CombatSimulateManager.Instance.AddSimulateUnit(kv.Value.TestRoleDataTb != null ? (uint)UnitType.Hero : (uint)UnitType.Pet,
                kv.Value.TestRoleDataTb != null ? kv.Value.TestRoleDataTb.id : kv.Value.TestPetDataTb.id, (uint)kv.Value.ServerNum);
        }
    }

    public Dictionary<int, CombatSimulateUnitInfo> GetCombatUnitInfoDic(int menuStyle)
    {
        Dictionary<int, CombatSimulateUnitInfo> dic = null;

        if (menuStyle == 0)
            dic = _singleCombatUnitInfoDic;
        else if (menuStyle == 1)
            dic = _multiCombatUnitInfoDic;
        else if (menuStyle == 2)
            dic = _pveSingleCombatUnitInfoDic;
        else if (menuStyle == 3)
            dic = _pveMultiCombatUnitInfoDic;
        
        return dic;
    }

    public int GetNeighborServerNum(int serverNum)
    {
        int tbNum = serverNum / 10;
        int lNum = serverNum % 10;

        if (tbNum == 0)
            return 10 + lNum;
        else if (tbNum == 1)
            return lNum;
        else if (tbNum == 2)
            return 30 + lNum;
        else if (tbNum == 3)
            return 20 + lNum;

        return -1;
    }

    public void SetSelectList(CommandInfoInCombatEditor commandInfoInCombatEditor, CommandEditor commandEditor)
    {
        if (commandInfoInCombatEditor == null)
            return;
        
        if (commandEditor == null || commandEditor.SkillId == 0u)
            return;

        var activeSkillTb = CSVActiveSkill.Instance.GetConfData(commandEditor.SkillId);
        if (activeSkillTb == null)
            return;

        commandEditor.TargetServerNumStrs = new string[0];
        commandEditor.TargetServerNums = new int[0];
        
        MobEntity mobEntity = MobManager.Instance.GetMobByServerNum(commandInfoInCombatEditor.SrcServerNum);

        List<MobEntity> selectList = new List<MobEntity>(); //MobManager.Instance.m_MobDic.Values.ToList();
        commandInfoInCombatEditor.m_FightControl.CalShowSelect(activeSkillTb.choose_type, activeSkillTb.choose_req, 
            mobEntity, selectList, mobEntity.m_MobCombatComponent.m_BattleUnit);

        if (selectList != null && selectList.Count > 0)
        {
            selectList.Sort(SortSelectList);

            commandEditor.TargetServerNumStrs = new string[selectList.Count + 1];
            commandEditor.TargetServerNumStrs[0] = "操作目标";
            commandEditor.TargetServerNums = new int[selectList.Count + 1];
            commandEditor.TargetServerNums[0] = 0;
            for (int i = 0; i < selectList.Count; i++)
            {
                var selectMob = selectList[i];
                if (selectMob == null)
                    continue;

                commandEditor.TargetServerNumStrs[i + 1] = selectMob.m_MobCombatComponent.m_BattleUnit.Pos.ToString();
                commandEditor.TargetServerNums[i + 1] = selectMob.m_MobCombatComponent.m_BattleUnit.Pos;
            }
        }
    }

    public int SortSelectList(MobEntity a, MobEntity b)
    {
        if (a.m_MobCombatComponent.m_BattleUnit.Pos > b.m_MobCombatComponent.m_BattleUnit.Pos)
            return -1;
        else if (a.m_MobCombatComponent.m_BattleUnit.Pos < b.m_MobCombatComponent.m_BattleUnit.Pos)
            return 1;
        else
            return 0;
    }
    #endregion
}
#endif