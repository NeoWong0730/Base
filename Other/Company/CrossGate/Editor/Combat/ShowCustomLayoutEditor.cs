using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CombatCustomMobDataTest))]
public class ShowDisposerEditor : Editor
{
    private CustomDataLayoutEditor _customDataLayoutEditor;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_customDataLayoutEditor == null)
            _customDataLayoutEditor = new CustomDataLayoutEditor();

        CombatCustomMobDataTest combatCustomMobDataTest = (CombatCustomMobDataTest)target;

        _customDataLayoutEditor.DrawBlock(combatCustomMobDataTest.m_MobEntity, null, 0u, 0u, 0u);
    }
}
