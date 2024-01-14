using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;

namespace Logic
{
    public partial class UI_ElementalCrystal : UIBase
    {
        private CP_ToggleRegistry m_CP_ToggleRegistry_Magic;
        private int m_CurSelectMagic;
        private int curSelectMagic
        {
            get { return m_CurSelectMagic; }
            set
            {
                if (m_CurSelectMagic != value)
                {
                    m_CurSelectMagic = value;
                }
            }
        }

        private Button m_SimulateButton;
        private GameObject m_Normal;
        private GameObject m_BeRestraint;   //被克制
        private GameObject m_Restraint;     //克制
        private Text m_BeRestraintText;
        private Text m_RestraintText;

        private void RegisterMid()
        {
            m_Normal = transform.Find("Animator/Page02/Center/Stage/Normal").gameObject;
            m_BeRestraint = transform.Find("Animator/Page02/Center/Stage/Be_Restraint").gameObject;
            m_BeRestraintText = m_BeRestraint.transform.Find("Text").GetComponent<Text>();
            m_Restraint = transform.Find("Animator/Page02/Center/Stage/Restraint").gameObject;
            m_RestraintText = m_Restraint.transform.Find("Text").GetComponent<Text>();
            m_CP_ToggleRegistry_Magic = transform.Find("Animator/Page02/Bottom/Toggle").GetComponent<CP_ToggleRegistry>();
            m_CP_ToggleRegistry_Magic.onToggleChange = OnMagicChanged;
            m_SimulateButton = transform.Find("Animator/Page02/Btn_start").GetComponent<Button>();
            m_SimulateButton.onClick.AddListener(OnSimulateButtonClicked);
        }

        private void RefreshMid()
        {
            m_CP_ToggleRegistry_Magic.SwitchTo(m_CurSelectMagic);
        }

        private void OnMagicChanged(int curToggle, int old)
        {
            curSelectMagic = curToggle;
        }

        private void OnSimulateButtonClicked()
        {
            if (m_AllotLeft > 0 && m_AllotLeft < 10)
            {
                //请分配属性点
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000501));
                return;
            }
            if (m_AllotRight > 0 && m_AllotRight < 10)
            {
                //请分配属性点
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000502));
                return;
            }
            Simulate();
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000507));
        }

        private void Simulate()
        {
            float damage = 0;
            if (m_CurSelectMagic == 0)
            {
                damage = Sys_ElementalCrystal.Instance.GetDamage_Physical(m_ElementsLeft[0].value, m_ElementsLeft[1].value, m_ElementsLeft[2].value, m_ElementsLeft[3].value,
                    m_ElementsRight[0].value, m_ElementsRight[1].value, m_ElementsRight[2].value, m_ElementsRight[3].value);
            }
            else
            {
                int landMagic = 0;
                int waterMagic = 0;
                int fireMagic = 0;
                int windMagic = 0;
                if (m_CurSelectMagic == 201)
                {
                    windMagic = 10;
                }
                else if (m_CurSelectMagic == 210)
                {
                    landMagic = 10;
                }
                else if (m_CurSelectMagic == 207)
                {
                    waterMagic = 10;
                }
                else
                {
                    fireMagic = 10;
                }
                damage = Sys_ElementalCrystal.Instance.GetDamage_Magic(m_ElementsLeft[0].value, m_ElementsLeft[1].value, m_ElementsLeft[2].value, m_ElementsLeft[3].value,
                   m_ElementsRight[0].value, m_ElementsRight[1].value, m_ElementsRight[2].value, m_ElementsRight[3].value, landMagic, waterMagic, fireMagic, windMagic);
            }
            damage = (damage - 1) * 100;
            string str = string.Format("{0:F1}", damage);
            if (str[str.Length - 1] == '0')
            {
                str = str.Substring(0,str.IndexOf('.'));
            }

            if (damage == 0)
            {
                m_Normal.SetActive(true);
                m_BeRestraint.SetActive(false);
                m_Restraint.SetActive(false);
            }
            else if (damage > 0)
            {
                m_Normal.SetActive(false);
                m_BeRestraint.SetActive(false);
                m_Restraint.SetActive(true);
                TextHelper.SetText(m_RestraintText, LanguageHelper.GetTextContent(680000504, str));
            }
            else
            {
                m_Normal.SetActive(false);
                m_BeRestraint.SetActive(true);
                m_Restraint.SetActive(false);
                TextHelper.SetText(m_BeRestraintText, LanguageHelper.GetTextContent(680000505, str));
            }
        }
    }
}


