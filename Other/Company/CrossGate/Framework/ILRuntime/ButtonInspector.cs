using NaughtyAttributes;
using UnityEngine;
using System;

namespace Framework
{
    [Obsolete]
    public class ShowSceneRootInspector : MonoBehaviour
    {
        //[InfoBox()]
        public int 左边类型;
        public int 左边角色ID;
        public int 左边状态;
        public int 左边动画ID;

        public int 右边类型;
        public int 右边角色ID;
        public int 右边状态;
        public int 右边动画ID;

        public Action<int, int, int, int, int, int, int, int> CreateAction;


        [Button("Create")]
        public void Create()
        {
            CreateAction?.Invoke(左边类型, 左边角色ID, 左边状态, 左边动画ID, 右边类型, 右边角色ID, 右边状态, 右边动画ID);
        }
    }

    public class MenuDialogueShowSceneRootInspector: MonoBehaviour
    {
        public int 类型;
        public int 角色ID;
        public int 动画ID;

        public Action<int, int, int> CreateAction;

        [Button("Create")]
        public void Create()
        {
            CreateAction?.Invoke(类型, 角色ID, 动画ID);
        }
    }

    [Obsolete]
    public class ShowSceneInspector : MonoBehaviour
    {
        public Vector3 pos;
        public Vector3 rot;
        public Vector3 scale;

#if UNITY_EDITOR
        public Action<Vector3, Vector3, Vector3> SetTransformAction;

        [Button("重设位置属性")]
        public void SetTransform()
        {
            SetTransformAction?.Invoke(pos, rot, scale);
        }
#endif
    }

    [Obsolete]
    public class ButtonInspector : MonoBehaviour
    {
#if UNITY_EDITOR

        public Action EnterFightAction;
        public Action ExitFightAction;
        public Action LowSpeedAction;
        public Action HighSpeedAction;
        public Action NormalSpeedAction;

        public Action LowScaleAction;
        public Action NormalScaleAction;

        public Action ShowHoldingSkillAction;

        public Action TestAutoActionAction;

        public Action OpenNpcPanelAction;
        public Action HideNpcPanelAction;

        public Action OpenDialoguePanelAction;
        public Action HideDialoguePanelAction;

        [Button("测试进入战斗")]
        public void EnterFight()
        {
            EnterFightAction?.Invoke();
        }

        [Button("测试退出战斗")]
        public void ExitFight()
        {
            ExitFightAction?.Invoke();
        }

        [Button("半速")]
        public void LowSpeed()
        {
            LowSpeedAction?.Invoke();
        }

        [Button("2倍速")]
        public void HighSpeed()
        {
            HighSpeedAction?.Invoke();
        }

        [Button("正常速")]
        public void NormalSpeed()
        {
            NormalSpeedAction?.Invoke();
        }

        [Button("LowScale")]
        public void LowScale()
        {
            LowScaleAction?.Invoke();
        }

        [Button("NormalScale")]
        public void NormalScale()
        {
            NormalScaleAction?.Invoke();
        }

        [Button("ShowHoldingSkillAction")]
        public void ShowHoldingSkill()
        {
            ShowHoldingSkillAction?.Invoke();
        }

        [Button("TestAutoActionAction")]
        public void TestAutoAction()
        {
            TestAutoActionAction?.Invoke();
        }

        [Button("OpenNPCPanel")]
        public void OpenNPCPanel()
        {
            OpenNpcPanelAction?.Invoke();
        }

        [Button("HideNPCPanel")]
        public void HideNPCPanel()
        {
            HideNpcPanelAction?.Invoke();
        }

        [Button("OpenDialoguePanel")]
        public void OpenDialoguePanel()
        {
            OpenDialoguePanelAction?.Invoke();
        }

        [Button("HideDialoguePanel")]
        public void HideDialoguePanel()
        {
            HideDialoguePanelAction?.Invoke();
        }

#endif
    }

    [Obsolete]
    public class NPCButtonInspector : MonoBehaviour
    {
#if UNITY_EDITOR

        public Action ShowAction;
        public Action HideAction;

        public Action TestFunctionAction;
        public Action SetLookPointAction;

        [Button("测试显示")]
        public void EnterFight()
        {
            ShowAction?.Invoke();
        }

        [Button("测试隐藏")]
        public void ExitFight()
        {
            HideAction?.Invoke();
        }

        [Button("Function")]
        public void TestFunction()
        {
            TestFunctionAction?.Invoke();
        }

        [Button("SetLookPoint")]
        public void SetLookPoint()
        {
            SetLookPointAction?.Invoke();
        }

#endif
    }
}

