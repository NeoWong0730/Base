using DG.Tweening;
using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    [HUDAnim(AnimType.e_Crit)]
    public class CritCharacterShow : Anim_BaseShow
    {
        private Image m_GenusIcon;
        private Text m_AttrText;
        private GameObject m_UpGo;
        private GameObject m_DownGo;

        public override void Construct(AnimData animData, CSVDamageShowConfig.Data cSVDamageShowConfigData , Action<Anim_BaseShow> _onPlayCompleted = null)
        {
            base.Construct(animData, cSVDamageShowConfigData, _onPlayCompleted);

            positionCorrect.NeedCorrectAtSkillPlay = true;
            positionCorrect.NeedFollow = false;
            positionCorrect.SetbaseOffest(new Vector3(offsetX, offsetY, 0));

            m_GenusIcon = mRootGameObject.transform.Find("Text/Image_Restrain").GetComponent<Image>();
            m_AttrText = mRootGameObject.transform.Find("Text/Text_Element").GetComponent<Text>();
            m_UpGo = m_AttrText.transform.Find("Up").gameObject;
            m_DownGo = m_AttrText.transform.Find("Down").gameObject;
            m_GenusIcon.gameObject.SetActive(false);
            m_AttrText.gameObject.SetActive(false);

            if (animData.bUseTrans)
            {
                //InitPos_Trans();
                positionCorrect.SetTarget(animData.battleObj.transform);
                positionCorrect.CalPos_Trans();
            }
            else
            {
                //InitPos_Vec3(animData.pos);
                positionCorrect.CalPos_Vec(animData.pos);
            }
            Show(mAnimData.finnaldamage, mAnimData.floatingdamage);
            DoAction();
        }

        public override void Initizal()
        {
            base.Initizal();
        }

        public override void Show(int finnaldamage, int floatingdamage)
        {
            mText.font = FontManager.GetFont(GlobalAssets.sFont_Crit);// "Font/Crit_font.fontsettings"
            bStartShow = true;
            if (finnaldamage<0)
            {
                finnaldamage = -finnaldamage;
            }
            mText.text = "必杀"+finnaldamage.ToString();
            SetCrystal();
            DOTween.To(() => mCanvasGroup.alpha, x => mCanvasGroup.alpha = x, 1, mShowTimer);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Dispose()
        {
            base.Dispose();
            positionCorrect.Dispose();
            m_GenusIcon.gameObject.SetActive(false);
            m_AttrText.gameObject.SetActive(false);
        }

        public void SetCrystal()
        {
            m_GenusIcon.gameObject.SetActive(false);
            //crystal
            if (mAnimData.floatingdamage != 0)
            {
                m_AttrText.gameObject.SetActive(true);
                if (mAnimData.floatingdamage > 0)
                {
                    TextHelper.SetText(m_AttrText, 2007271, mAnimData.floatingdamage.ToString());
                    m_UpGo.SetActive(true);
                    m_DownGo.SetActive(false);
                }
                else
                {
                    TextHelper.SetText(m_AttrText, 2007272, (-mAnimData.floatingdamage).ToString());
                    m_UpGo.SetActive(false);
                    m_DownGo.SetActive(true);
                }
            }
            else
            {
                m_AttrText.gameObject.SetActive(false);
            }

            if (mAnimData.attackType == CombatUnitType.Zero)
            {
                m_GenusIcon.gameObject.SetActive(false);
                return;
            }

            //uint attackGenusInfo = 0;
            //uint hitGenusInfo = 0;
            ////attack
            //if (mAnimData.attackType == CombatUnitType.Hero || mAnimData.attackType == CombatUnitType.Partner)
            //{
            //    attackGenusInfo = 7;
            //}
            //else if (mAnimData.attackType == CombatUnitType.Monster)
            //{
            //    CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(mAnimData.attackInfoId);
            //    if (cSVMonsterData != null)
            //    {
            //        attackGenusInfo = cSVMonsterData.genus;
            //    }
            //    else
            //    {
            //        DebugUtil.LogErrorFormat("找不到monsterInfoId: {0}", mAnimData.attackInfoId);
            //    }
            //}
            //else if (mAnimData.attackType == CombatUnitType.Pet)
            //{
            //    CSVPetNew.Data cSVPet = CSVPetNew.Instance.GetConfData(mAnimData.attackInfoId);
            //    if (cSVPet != null)
            //    {
            //        attackGenusInfo = cSVPet.race;
            //    }
            //    else
            //    {
            //        DebugUtil.LogErrorFormat("找不到petInfoId: {0}", mAnimData.attackInfoId);
            //    }
            //}
            ////hit
            //if (mAnimData.hitType == CombatUnitType.Hero || mAnimData.hitType == CombatUnitType.Partner)
            //{
            //    hitGenusInfo = 7;
            //}
            //else if (mAnimData.hitType == CombatUnitType.Monster)
            //{
            //    CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(mAnimData.hitInfoId);
            //    if (cSVMonsterData != null)
            //    {
            //        hitGenusInfo = cSVMonsterData.genus;
            //    }
            //    else
            //    {
            //        DebugUtil.LogErrorFormat("找不到monsterInfoId: {0}", mAnimData.hitInfoId);
            //    }
            //}
            //else if (mAnimData.hitType == CombatUnitType.Pet)
            //{
            //    CSVPetNew.Data cSVPet = CSVPetNew.Instance.GetConfData(mAnimData.hitInfoId);
            //    if (cSVPet != null)
            //    {
            //        hitGenusInfo = cSVPet.race;
            //    }
            //    else
            //    {
            //        DebugUtil.LogErrorFormat("找不到petInfoId: {0}", mAnimData.hitInfoId);
            //    }
            //}

            //CSVGenus.Data cSVGenus = CSVGenus.Instance.GetConfData(attackGenusInfo);
            //if (null == cSVGenus)
            //{
            //    DebugUtil.LogErrorFormat("找不到种族id{0}", attackGenusInfo);
            //    m_GenusIcon.gameObject.SetActive(false);
            //    return;
            //}
            //else
            //{
            //    uint res = cSVGenus.damageRate[(int)hitGenusInfo];

            //    DebugUtil.Log(ELogType.eHUD, string.Format("攻击方:{0}  受击方:{1}  攻击方种族id:{2} 受击方种族id:{3}  最终值{4}",
            //        mAnimData.attackType, mAnimData.hitType, attackGenusInfo, hitGenusInfo, res));

            //    uint iconId = 0;
            //    bool showIcon = false;
            //    if (res == 8000)//全被克
            //    {
            //        iconId = 994113;
            //        showIcon = true;
            //    }
            //    else if (res == 9000)//半被克
            //    {
            //        iconId = 994114;
            //        showIcon = true;
            //    }
            //    else if (res == 10000)//正常
            //    {
            //        showIcon = false;
            //    }
            //    else if (res == 11000)//半克
            //    {
            //        iconId = 994112;
            //        showIcon = true;
            //    }
            //    else if (res == 12000)//全克
            //    {
            //        iconId = 994111;
            //        showIcon = true;
            //    }
            //    if (showIcon)
            //    {
            //        m_GenusIcon.gameObject.SetActive(true);
            //        ImageHelper.SetIcon(m_GenusIcon, iconId);
            //    }
            //    else
            //    {
            //        m_GenusIcon.gameObject.SetActive(false);
            //    }
            //}
        }

        //public override void InitPos_Trans()
        //{
        //    if (mTarget != null)
        //    {
        //        if (CameraManager.mSkillPlayCamera == null)
        //        {
        //            CameraManager.World2UI(mRootGameObject, mTarget.position + new Vector3(offsetX, offsetY, 0), CameraManager.mCamera, UIManager.mUICamera);
        //            rect.position = new Vector3(rect.position.x, rect.position.y, UIManager.mUICamera.transform.position.z + 100);
        //        }
        //        else
        //        {
        //            CameraManager.World2UI(mRootGameObject, mTarget.position + new Vector3(0, 1.5f, 0), CameraManager.mSkillPlayCamera, UIManager.mUICamera);
        //            rect.position = new Vector3(rect.position.x, rect.position.y,
        //                UIManager.mUICamera.transform.position.z + 100);
        //            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x * Screen.width / 800 * 800 / 1280 + CameraManager.relativePos.x,
        //               rect.anchoredPosition.y * Screen.height / 480 * 480 / 720 + CameraManager.relativePos.y);
        //        }
        //    }
        //}
        //public override void InitPos_Vec3( Vector3 vector3 )
        //{
        //    if (CameraManager.mSkillPlayCamera == null)
        //    {
        //        CameraManager.World2UI(mRootGameObject, vector3, CameraManager.mCamera, UIManager.mUICamera);
        //        rect.position = new Vector3(rect.position.x, rect.position.y, UIManager.mUICamera.transform.position.z + 100);
        //    }
        //    else
        //    {
        //        CameraManager.World2UI(mRootGameObject, vector3, CameraManager.mSkillPlayCamera, UIManager.mUICamera, 800, 480,
        //             CameraManager.relativePos.x, CameraManager.relativePos.y);
        //        rect.position = new Vector3(rect.position.x, rect.position.y, UIManager.mUICamera.transform.position.z + 100);
        //    }
        //}
    }
}

