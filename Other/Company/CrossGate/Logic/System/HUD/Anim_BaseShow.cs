using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Table;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Lib.Core;

namespace Logic
{
    public class AnimData
    {
        public AnimType animType;
        public GameObject uiObj;
        public GameObject battleObj;
        public int finnaldamage;
        public int floatingdamage;
        public string content;
        public bool bUseTrans;
        public Vector3 pos;
        public CombatUnitType attackType; //攻击方类型
        public CombatUnitType hitType;  //被击方类型
        public uint race_hit;
        public uint race_attack;
        public uint attackInfoId;       //攻击方配置id
        public uint hitInfoId;          //受击方id
        public int clientNum;

        public AnimData() { }

        public AnimData(AnimType _animType, GameObject _uiObj, GameObject _battleObj, int _finnaldamage, int _floatingdamage, string _content, bool _bUseTrans = true, Vector3 _pos = default(Vector3))
        {
            animType = _animType;
            uiObj = _uiObj;
            battleObj = _battleObj;
            finnaldamage = _finnaldamage;
            floatingdamage = _floatingdamage;
            content = _content;
            bUseTrans = _bUseTrans;
            pos = _pos;
        }

        public void Dispose()
        {
            animType = AnimType.e_None;
            uiObj = null;
            battleObj = null;
            content = string.Empty;
            bUseTrans = true;
        }
    }

    public abstract class Anim_BaseShow : IHUDComponent
    {
        protected AnimData mAnimData;

        public AnimType animType
        {
            get
            {
                return mAnimData.animType;
            }
        }

        public GameObject mRootGameObject
        {
            get
            {
                return mAnimData.uiObj;
            }
        }

        protected RectTransform rect
        {
            get
            {
                return mRootGameObject.transform as RectTransform;
            }
        }

        protected Transform mTarget
        {
            get
            {
                if (mAnimData.battleObj == null)
                {
                    return null;
                }
                return mAnimData.battleObj.transform;
            }
        }

        protected Vector3 mTargetPos;

        protected CanvasGroup mCanvasGroup;

        protected Text mText
        {
            get
            {
                return mRootGameObject.transform.Find("Text").GetComponent<Text>();
            }
        }

        private CSVDamageShowConfig.Data configData;
        public float mShowTimer;
        public float mHideTimer;
        public float mPlayTimer;
        public float mUpDis;
        public float mUpTimer;
        public float mStartScale;
        public float mMaxScale;
        public float mStart2MaxScaleTimer;
        public float mMax2NormalTimer;
        public float mMinScale;
        protected Sequence sequence;

        protected bool bStartShow;
        private bool bStartHide;

        public bool LogicOver = false;//逻辑生命周期

        protected Action<Anim_BaseShow> onPlayCompleted = null;

        TweenerCore<float, float, FloatOptions> alphaShower1;
        TweenerCore<float, float, FloatOptions> alphaShower2;
        TweenerCore<float, float, FloatOptions> alphaHider;

        protected float offsetX;
        protected float offsetY;

        protected HUDPositionCorrect positionCorrect;

#if UNITY_EDITOR
        ModifyCombatHud _ModifyHudTest;
#endif

        public virtual void Construct(AnimData animData, CSVDamageShowConfig.Data _configData, Action<Anim_BaseShow> action = null)
        {
            mAnimData = animData;
            configData = _configData;
            onPlayCompleted = action;
            mCanvasGroup = mRootGameObject.GetComponent<CanvasGroup>();
            positionCorrect = mRootGameObject.GetNeedComponent<HUDPositionCorrect>();
            sequence = DOTween.Sequence();
            Initizal();
        }

        public virtual void Initizal()
        {
            mCanvasGroup.alpha = 0;
            rect.localScale = new Vector3(1, 1, 1);
            mPlayTimer = (float)configData.dynamiceffect_parameter[0] / 10000f;
            mShowTimer = configData.dynamiceffect_parameter[1] / 10000f;
            mHideTimer = configData.dynamiceffect_parameter[2] / 10000f;
            mUpTimer = configData.dynamiceffect_parameter[3] / 10000f;
            mUpDis = configData.dynamiceffect_parameter[4] / 100f;
            mStartScale = configData.dynamiceffect_parameter[5] / 10000f;
            mMaxScale = configData.dynamiceffect_parameter[6] / 10000f;
            mStart2MaxScaleTimer = configData.dynamiceffect_parameter[7] / 10000f;
            mMax2NormalTimer = configData.dynamiceffect_parameter[8] / 10000f;
            mMinScale = configData.dynamiceffect_parameter[9] / 10000f;
            offsetX = configData.dynamiceffect_parameter[10] / 100f;
            offsetY = configData.dynamiceffect_parameter[11] / 100f;

#if UNITY_EDITOR&&DEBUG_MODE
            _ModifyHudTest = UnityEngine.Object.FindObjectOfType<ModifyCombatHud>();

            if (_ModifyHudTest && !_ModifyHudTest.correct)
            {
                _ModifyHudTest.mPlayTimer = mPlayTimer;
                _ModifyHudTest.mShowTimer = mShowTimer;
                _ModifyHudTest.mHideTimer = mHideTimer;
                _ModifyHudTest.mUpTimer = mUpTimer;
                _ModifyHudTest.mUpDis = mUpDis;
                _ModifyHudTest.mStartScale = mStartScale;
                _ModifyHudTest.mMaxScale = mMaxScale;
                _ModifyHudTest.mStart2MaxScaleTimer = mStart2MaxScaleTimer;
                _ModifyHudTest.mMax2NormalTimer = mMax2NormalTimer;
                _ModifyHudTest.mMinScale = mMinScale;
                _ModifyHudTest.Correct();
            }

            if (ModifyCombatHud.UseTestData)
            {
                mPlayTimer = _ModifyHudTest.mPlayTimer;
                mShowTimer = _ModifyHudTest.mShowTimer;
                mHideTimer = _ModifyHudTest.mHideTimer;
                mUpTimer = _ModifyHudTest.mUpTimer;
                mUpDis = _ModifyHudTest.mUpDis;
                mStartScale = _ModifyHudTest.mStartScale;
                mMaxScale = _ModifyHudTest.mMaxScale;
                mStart2MaxScaleTimer = _ModifyHudTest.mStart2MaxScaleTimer;
                mMax2NormalTimer = _ModifyHudTest.mMax2NormalTimer;
                mMinScale = _ModifyHudTest.mMinScale;
            }
#endif
        }

        public virtual void Show(int finnaldamage, int floatingdamage)
        {
            bStartShow = true;
            mText.text = finnaldamage.ToString();
            //alphaShower1 = DOTween.To(() => mCanvasGroup.alpha, x => mCanvasGroup.alpha = x, 1, mShowTimer);
        }

        public virtual void DoAction()
        {
            rect.localScale = new Vector3(mStartScale, mStartScale, mStartScale);
            sequence.Append(rect.DOScale(mMaxScale, mStart2MaxScaleTimer))
                    .Insert(0, rect.DOMoveY(rect.position.y + mUpDis, mUpTimer))
                    .Append(rect.DOScale(mStartScale, mMax2NormalTimer))
                    .Append(rect.DOScale(mStartScale, mPlayTimer - mStart2MaxScaleTimer - mMax2NormalTimer - mHideTimer))
                    .Append(rect.DOScale(mMinScale, mHideTimer));
        }

        public virtual void Show(string str)
        {
            bStartShow = true;
            mText.text = str;
            //alphaShower2 = DOTween.To(() => mCanvasGroup.alpha, x => mCanvasGroup.alpha = x, 1, mShowTimer);
        }

        public virtual void Update()
        {
            float dt = Sys_HUD.Instance.DeltaTime;
            if (bStartShow)
            {
                float showAlpha = Mathf.Lerp(0, 1, dt / mShowTimer);
                mCanvasGroup.alpha = showAlpha;
                mPlayTimer -= dt;
                if (mPlayTimer <= mHideTimer)
                {
                    bStartShow = false;
                    bStartHide = true;
                }
            }
            if (bStartHide)
            {
                mPlayTimer -= dt;
                float hideAlpha = Mathf.Lerp(1, 0, dt / mHideTimer);
                mCanvasGroup.alpha = hideAlpha;
                if (mPlayTimer <= 0)
                {
                    bStartHide = false;
                    mCanvasGroup.alpha = 0;
                    OnPlayOver();
                }
            }
        }

        public virtual void OnPlayOver()
        {
            LogicOver = true;
            onPlayCompleted?.Invoke(this);
        }

        public virtual void InitPos_Trans() { }

        public virtual void InitPos_Vec3(Vector3 pos) { }


        public virtual void Dispose()
        {
            mRootGameObject.SetActive(false);
            rect.localScale = new Vector3(1, 1, 1);
            sequence.Kill();
            alphaShower1.Kill();
            alphaShower2.Kill();
            alphaHider.Kill();
            configData = null;
            mShowTimer = 0;
            mHideTimer = 0;
            mPlayTimer = 0;
            mUpDis = 0;
            mUpTimer = 0;
            mStartScale = 0;
            mMaxScale = 0;
            mStart2MaxScaleTimer = 0;
            mMax2NormalTimer = 0;
            mMinScale = 0;
            mAnimData.Dispose();
            CombatObjectPool.Instance.Push(mAnimData);
        }
    }
}


