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
    public class ComboCharacterShow : Anim_BaseShow
    {
        private Image m_GenusIcon;
        private Text m_AttrText;
        private GameObject m_UpGo;
        private GameObject m_DownGo;
        public override void Construct(AnimData animData, CSVDamageShowConfig.Data cSVDamageShowConfigData, Action<Anim_BaseShow> _onPlayCompleted = null)
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
            mText.font = FontManager.GetFont(GlobalAssets.sFont_Normal);// "Font/Normal_font.fontsettings"
            SetCrystal();
            base.Show(finnaldamage, floatingdamage);
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

            // uint attackGenusInfo = 0;
            // uint hitGenusInfo = 0;
            // //attack
            // if (mAnimData.attackType == CombatUnitType.Hero || mAnimData.attackType == CombatUnitType.Partner)
            // {
            //     attackGenusInfo = 7;
            // }
            // else if (mAnimData.attackType == CombatUnitType.Monster)
            // {
            //     CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(mAnimData.attackInfoId);
            //     if (cSVMonsterData != null)
            //     {
            //         attackGenusInfo = cSVMonsterData.genus;
            //     }
            //     else
            //     {
            //         DebugUtil.LogErrorFormat("找不到monsterInfoId: {0}", mAnimData.attackInfoId);
            //     }
            // }
            // else if (mAnimData.attackType == CombatUnitType.Pet)
            // {
            //     CSVPetNew.Data cSVPet = CSVPetNew.Instance.GetConfData(mAnimData.attackInfoId);
            //     if (cSVPet != null)
            //     {
            //         attackGenusInfo = cSVPet.race;
            //     }
            //     else
            //     {
            //         DebugUtil.LogErrorFormat("找不到petInfoId: {0}", mAnimData.attackInfoId);
            //     }
            // }
            // //hit
            // if (mAnimData.hitType == CombatUnitType.Hero || mAnimData.hitType == CombatUnitType.Partner)
            // {
            //     hitGenusInfo = 7;
            // }
            // else if (mAnimData.hitType == CombatUnitType.Monster)
            // {
            //     CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(mAnimData.hitInfoId);
            //     if (cSVMonsterData != null)
            //     {
            //         hitGenusInfo = cSVMonsterData.genus;
            //     }
            //     else
            //     {
            //         DebugUtil.LogErrorFormat("找不到monsterInfoId: {0}", mAnimData.hitInfoId);
            //     }
            // }
            // else if (mAnimData.hitType == CombatUnitType.Pet)
            // {
            //     CSVPetNew.Data cSVPet = CSVPetNew.Instance.GetConfData(mAnimData.hitInfoId);
            //     if (cSVPet != null)
            //     {
            //         hitGenusInfo = cSVPet.race;
            //     }
            //     else
            //     {
            //         DebugUtil.LogErrorFormat("找不到petInfoId: {0}", mAnimData.hitInfoId);
            //     }
            // }

            CSVGenus.Data cSVGenus = CSVGenus.Instance.GetConfData(mAnimData.race_attack);
            if (null == cSVGenus)
            {
                DebugUtil.Log(ELogType.eBattleCommand, string.Format("找不到攻击方种族id{0}", mAnimData.race_attack.ToString()));
                m_GenusIcon.gameObject.SetActive(false);
                return;
            }
            else
            {
                uint res = cSVGenus.damageRate[(int)mAnimData.race_hit];

                uint iconId = 0;
                bool showIcon = false;
                if (res == 8000)//全被克
                {
                    iconId = 994113;
                    showIcon = true;
                }
                else if (res == 9000)//半被克
                {
                    iconId = 994114;
                    showIcon = true;
                }
                else if (res == 10000)//正常
                {
                    showIcon = false;
                }
                else if (res == 11000)//半克
                {
                    iconId = 994112;
                    showIcon = true;
                }
                else if (res == 12000)//全克
                {
                    iconId = 994111;
                    showIcon = true;
                }
                if (showIcon)
                {
                    m_GenusIcon.gameObject.SetActive(true);
                    ImageHelper.SetIcon(m_GenusIcon, iconId);
                }
                else
                {
                    m_GenusIcon.gameObject.SetActive(false);
                }
            }
        }

        //public override void InitPos_Vec3(Vector3 vector3)
        //{
        //    if (CameraManager.mSkillPlayCamera == null)
        //    {
        //        CameraManager.World2UI(mRootGameObject, vector3, CameraManager.mCamera, UIManager.mUICamera);
        //        rect.position = new Vector3(rect.position.x, rect.position.y, UIManager.mUICamera.transform.position.z + 100);
        //    }
        //    else
        //    {
        //        CameraManager.World2UI(mRootGameObject, vector3, CameraManager.mSkillPlayCamera, UIManager.mUICamera, 800, 480,
        //              CameraManager.relativePos.x, CameraManager.relativePos.y);
        //        rect.position = new Vector3(rect.position.x, rect.position.y, UIManager.mUICamera.transform.position.z + 100);
        //    }
        //}
    }


    public class ComboData
    {
        public AnimType animType;
        public int finnaldamage;
        public int floatingdamage;
        public CombatUnitType attackType; //攻击方类型
        public CombatUnitType hitType;  //被击方类型
        public uint attackInfoId;       //攻击方配置id
        public uint hitInfoId;          //受击方id
        public uint race_hit;
        public uint race_attack;

        public void Push()
        {
            CombatObjectPool.Instance.Push<ComboData>(this);
        }
    }

    public class ComboGroup
    {
        private List<Vector2> vector2s = new List<Vector2>();
        private Transform target;
        private GameObject rootGo;
        public List<Anim_BaseShow> hudComponents = new List<Anim_BaseShow>();

        Queue<ComboData> comboDatas = new Queue<ComboData>();
        private CSVDamageShowConfig.Data configData;

        public float mPlayTimer;
        public float mShowTimer;
        public float mHideTimer;
        public float mRateTimer;
        public float mUpdis;
        public float mUpTimer;
        public float mStartScale;
        public float mMaxScale;
        public float mStart2MaxScaleTimer;
        public float mMax2NormalTimer;
        public float mMinScale;
        private float mRateTime;

        public bool Inited = false;
        private int index;
        private Func<GameObject> getAnim;
        private Action<GameObject> pushAnim;

        public ComboGroup(GameObject _go, CSVDamageShowConfig.Data _configData, Func<GameObject> _getAnim, Action<GameObject> _pushAnim)
        {
            rootGo = _go;
            configData = _configData;
            getAnim = _getAnim;
            pushAnim = _pushAnim;
        }

        private void Initizal()
        {
            if (Inited)
            {
                return;
            }
            Inited = true;

            CSVParam.Data cSVParamData2 = CSVParam.Instance.GetConfData(107);
            string[] str1 = cSVParamData2.str_value.Split('|');
            for (int i = 0; i < str1.Length; i++)
            {
                string[] str2 = str1[i].Split(',');
                vector2s.Add(new Vector2(Convert.ToSingle(str2[0]), Convert.ToSingle(str2[1])));
            }
        }

        public void SetTarget(Transform _transform)
        {
            if (_transform == null)
            {
                DebugUtil.LogErrorFormat("ComboGroup.SetTarget====>_transform=null");
            }
            target = _transform;
            Initizal();
        }

        public void Update()
        {
            for (int i = hudComponents.Count - 1; i >= 0; --i)
            {
                hudComponents[i].Update();
            }
            if (comboDatas.Count == 0)
                return;
            mRateTimer -= Time.deltaTime;
            if (mRateTimer <= 0)
            {
                mRateTimer = mRateTime;
                PopCombo();
            }
        }

        private void PopCombo()
        {
            ComboData comboData = comboDatas.Dequeue();
            if (target == null)
            {
                DebugUtil.LogErrorFormat("PopCombo====>target=null");
                return;
            }
            GameObject go;
            go = getAnim();
            go.SetActive(true);
            Anim_BaseShow hUDComponent = null;
            AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
            Vector3 targetPos = target.position + (Vector3)vector2s[index % vector2s.Count];
            switch (comboData.animType)
            {
                case AnimType.e_Normal:
                    //animData = new AnimData(go, null, comboData.finnaldamage, comboData.floatingdamage, "", false, targetPos);
                    animData.animType = AnimType.e_Normal;
                    animData.uiObj = go;
                    animData.battleObj = null;
                    animData.finnaldamage = comboData.finnaldamage;
                    animData.floatingdamage = comboData.floatingdamage;
                    animData.content = string.Empty;
                    animData.bUseTrans = false;
                    animData.pos = targetPos;
                    animData.attackType = comboData.attackType;
                    animData.hitType = comboData.hitType;
                    animData.attackInfoId = comboData.attackInfoId;
                    animData.hitInfoId = comboData.hitInfoId;
                    animData.race_hit = comboData.race_hit;
                    animData.race_attack = comboData.race_attack;
                    hUDComponent = HUDFactory.Get<ComboCharacterShow>();
                    break;
                case AnimType.e_Crit:
                    //animData = new AnimData(go, null, comboData.finnaldamage, comboData.floatingdamage, "", false, targetPos);
                    animData.animType = AnimType.e_Crit;
                    animData.uiObj = go;
                    animData.battleObj = null;
                    animData.finnaldamage = comboData.finnaldamage;
                    animData.floatingdamage = comboData.floatingdamage;
                    animData.content = string.Empty;
                    animData.bUseTrans = false;
                    animData.pos = targetPos;
                    animData.attackType = comboData.attackType;
                    animData.hitType = comboData.hitType;
                    animData.attackInfoId = comboData.attackInfoId;
                    animData.hitInfoId = comboData.hitInfoId;
                    animData.race_hit = comboData.race_hit;
                    animData.race_attack = comboData.race_attack;
                    hUDComponent = HUDFactory.Get<CritCharacterShow>();
                    break;
                case AnimType.e_Miss:
                    //animData = new AnimData(go, null, comboData.finnaldamage, comboData.floatingdamage, "闪避", false, targetPos);
                    animData.animType = AnimType.e_Miss;
                    animData.uiObj = go;
                    animData.battleObj = null;
                    animData.finnaldamage = comboData.finnaldamage;
                    animData.floatingdamage = comboData.floatingdamage;
                    animData.content = "闪避";
                    animData.bUseTrans = false;
                    animData.pos = targetPos;
                    animData.attackType = comboData.attackType;
                    animData.hitType = comboData.hitType;
                    animData.attackInfoId = comboData.attackInfoId;
                    animData.hitInfoId = comboData.hitInfoId;
                    animData.race_hit = comboData.race_hit;
                    animData.race_attack = comboData.race_attack;
                    hUDComponent = HUDFactory.Get<MissCharacterShow>();
                    break;
                case AnimType.e_PassiveDamage:
                case AnimType.e_ConverAttack | AnimType.e_Normal:
                case AnimType.e_ConverAttack | AnimType.e_PassiveDamage:
                    animData.animType = AnimType.e_PassiveDamage;
                    animData.uiObj = go;
                    animData.battleObj = null;
                    animData.finnaldamage = comboData.finnaldamage;
                    animData.floatingdamage = comboData.floatingdamage;
                    animData.content = string.Empty;
                    animData.bUseTrans = false;
                    animData.pos = targetPos;
                    hUDComponent = HUDFactory.Get<PassiveDamageShow>();
                    break;
                default:
                    break;
            }
            hUDComponent.mShowTimer = mShowTimer;
            hUDComponent.mHideTimer = mHideTimer;
            hUDComponent.mPlayTimer = mPlayTimer;
            hUDComponent.mUpTimer = mUpTimer;
            hUDComponent.mUpDis = mUpdis;
            hUDComponent.mStartScale = mStartScale;
            hUDComponent.mMaxScale = mMaxScale;
            hUDComponent.mStart2MaxScaleTimer = mStart2MaxScaleTimer;
            hUDComponent.mMax2NormalTimer = mMax2NormalTimer;
            hUDComponent.mMinScale = mMinScale;
            hUDComponent.Construct(animData, configData, RemoveHud);
            hudComponents.Add(hUDComponent);
            index++;
            comboData.Push();
            //animData.Dispose();
            //CombatObjectPool.Instance.Push(animData);
        }


        public void RemoveHud(Anim_BaseShow anim_BaseShow)
        {
            pushAnim.Invoke(anim_BaseShow.mRootGameObject);
            anim_BaseShow.Dispose();
            HUDFactory.Recycle(anim_BaseShow);
            hudComponents.Remove(anim_BaseShow);
        }

        public void PushCombData(ComboData comboData)
        {
            comboDatas.Enqueue(comboData);
        }
    }
}




