using DG.Tweening;
using Framework;
using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    [HUDAnim(AnimType.e_ConverAttack)]
    public class ConverCharacterShow : Anim_BaseShow
    {
        private Image m_GenusIcon;
        private Text m_AttrText;
        private GameObject m_UpGo;
        private GameObject m_DownGo;

        public override void Construct(AnimData animData, CSVDamageShowConfig.Data cSVDamageShowConfigData, Action<Anim_BaseShow> _onPlayCompleted = null)
        {
            base.Construct(animData, cSVDamageShowConfigData, _onPlayCompleted);
            m_GenusIcon = mRootGameObject.transform.Find("Text/Image_Restrain").GetComponent<Image>();
            m_AttrText = mRootGameObject.transform.Find("Text/Text_Element").GetComponent<Text>();
            m_UpGo = m_AttrText.transform.Find("Up").gameObject;
            m_DownGo = m_AttrText.transform.Find("Down").gameObject;
            m_GenusIcon.gameObject.SetActive(false);
            m_AttrText.gameObject.SetActive(false);

            Show(mAnimData.content);
        }

        public override void Initizal()
        {
            base.Initizal();
        }

        public override void Show(string str)
        {
            mText.font = FontManager.GetFont(GlobalAssets.sFont_Conver);// "Font/Conver_font.fontsettings"
            base.Show(str);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Dispose()
        {
            base.Dispose();

            m_UpGo.SetActive(false);
            m_DownGo.SetActive(false);
            m_GenusIcon.gameObject.SetActive(false);
            m_AttrText.gameObject.SetActive(false);
        }

        public void SetUIPosition(float radioX, float radioY)
        {
            float width = 1280;
            float height = 720;
            rect.anchoredPosition = new Vector2(radioX * width, radioY * height);
        }
    }
}
