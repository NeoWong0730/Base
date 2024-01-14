using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using System;
using Lib.Core;
using NaughtyAttributes;

namespace Framework
{
    [RequireComponent(typeof(Animator))]
    public partial class SimpleAnimation : MonoBehaviour
    {
        public string animStr = string.Empty;

        [Button("PlayTest")]
        public void PlayTest()
        {
            Play(animStr);
        }

        public interface State
        {
            bool enabled { get; set; }
            bool isValid { get; }
            float time { get; set; }
            float normalizedTime { get; set; }
            float speed { get; set; }
            string name { get; set; }
            float weight { get; set; }
            float length { get; }
            AnimationClip clip { get; }
            WrapMode wrapMode { get; set; }

        }
        public Animator animator
        {
            get
            {
                if (m_Animator == null)
                {
                    m_Animator = GetComponent<Animator>();
                }
                return m_Animator;
            }
        }

        public bool animatePhysics
        {
            get { return m_AnimatePhysics; }
            set { m_AnimatePhysics = value; animator.updateMode = m_AnimatePhysics ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal; }
        }

        public AnimatorCullingMode cullingMode
        {
            get { return animator.cullingMode; }
            set { m_CullingMode = value; animator.cullingMode = m_CullingMode; }
        }

        public bool isPlaying { get { return m_Playable.IsPlaying(); } }

        public bool playAutomatically
        {
            get { return m_PlayAutomatically; }
            set { m_PlayAutomatically = value; }
        }

        public AnimationClip clip
        {
            get { return m_Clip; }
            set
            {
                LegacyClipCheck(value);
                m_Clip = value;
            }
        }

        public WrapMode wrapMode
        {
            get { return m_WrapMode; }
            set { m_WrapMode = value; }
        }

        private Timer playOverTime = null;
        private Action _playOverAction;

        public void AddClip(AnimationClip clip, string newName)
        {
            LegacyClipCheck(clip);
            AddState(clip, newName);
        }

        public void Blend(string stateName, float targetWeight, float fadeLength)
        {
            if (m_Animator != null)
                m_Animator.enabled = true;

            Kick();

            if (m_Playable != null)
                m_Playable.Blend(stateName, targetWeight, fadeLength);
        }

        public void SetSpeed(float speed)
        {
            if (m_Playable != null)
                m_Playable.SetSpeed(speed);
        }

        public bool Play()
        {
            if (m_Animator != null)
                m_Animator.enabled = true;

            Kick();

            if (m_Clip != null && m_PlayAutomatically)
            {
                m_Playable?.Play(kDefaultStateName);
            }

            return false;
        }

        private void OnPlayOver()
        {
            if (_playOverAction != null)
                _playOverAction?.Invoke();
        }

        public void Play(string stateName, Action playOver = null)
        {
            if (m_Animator != null)
                m_Animator.enabled = true;

            Kick();

            _playOverAction = playOver;

            if (m_Playable != null)
            {
                int index = m_Playable.Play(stateName);
                if (index >= 0)
                {
                    playOverTime?.Cancel();
                    if (_playOverAction != null)
                        playOverTime = Timer.Register(m_States[index].clip.length, OnPlayOver);
                }
            }
        }

        public bool PlaySuccess(string stateName, Action playOver = null)
        {
            if (m_Animator != null)
                m_Animator.enabled = true;

            Kick();

            _playOverAction = playOver;

            if (m_Playable != null)
            {
                int index = m_Playable.Play(stateName);

                if (index >= 0)
                {
                    playOverTime?.Cancel();
                    if (_playOverAction != null)
                        playOverTime = Timer.Register(m_States[index].clip.length, OnPlayOver);

                    return true;
                }
            }
           
            return false;
        }

        public void CrossFade(string stateName, float fadeLength, Action playOver = null)
        {
            if (m_Animator != null)
                m_Animator.enabled = true;
            
            Kick();

            _playOverAction = playOver;
            if (m_Playable != null)
            {
                int index = m_Playable.Crossfade(stateName, fadeLength);
                if (index >= 0)
                {
                    playOverTime?.Cancel();
                    if (_playOverAction != null)
                        playOverTime = Timer.Register(m_States[index].clip.length, OnPlayOver);
                }
            }
           
        }

        public void CrossFade(string stateName, float fadeLength, Action playOver, float reduceTime)
        {
            if (m_Animator != null)
                m_Animator.enabled = true;

            Kick();

            _playOverAction = playOver;

            if (m_Playable != null)
            {
                int index = m_Playable.Crossfade(stateName, fadeLength);
                if (index >= 0)
                {
                    playOverTime?.Cancel();
                    if (playOver != null)
                    {
                        float clipLen = m_States[index].clip.length;
                        if (reduceTime >= clipLen)
                            clipLen = 0f;
                        else
                            clipLen -= reduceTime;

                        if (clipLen <= 0f)
                            playOver?.Invoke();
                        else if (_playOverAction != null)
                            playOverTime = Timer.Register(clipLen, OnPlayOver);
                    }
                }
            }
        }

        public bool CrossFadeSuccess(string stateName, float fadeLength, Action playOver = null, float reduceTime = 0f)
        {
            if (m_Animator)
                m_Animator.enabled = true;

            Kick();

            if (m_Playable != null)
            {
                _playOverAction = playOver;
                int index = m_Playable.Crossfade(stateName, fadeLength);
                if (index >= 0)
                {
                    playOverTime?.Cancel();
                    if (playOver != null)
                    {
                        float clipLen = m_States[index].clip.length;
                        if (reduceTime >= clipLen)
                            clipLen = 0f;
                        else
                            clipLen -= reduceTime;

                        if (clipLen <= 0f)
                            _playOverAction?.Invoke();
                        else if (_playOverAction != null)
                            playOverTime = Timer.Register(clipLen, OnPlayOver);
                    }

                    return true;
                }
            }
            
            return false;
        }

        public int GetClipCount()
        {
            return m_Playable.GetClipCount();
        }

        public bool IsPlaying(string stateName)
        {
            if (m_Playable == null)
                return false;
            return m_Playable.IsPlaying(stateName);
        }

        public void Pause(string stateName)
        {
            if (m_Animator != null)
                m_Animator.enabled = true;

            Kick();
            
            if (m_Playable != null)
            {
                m_Playable.Pause(stateName);
            }
        }

        public void RemovePause(string stateName)
        {
            if (m_Animator != null)
                m_Animator.enabled = true;

            Kick();

            if (m_Playable != null)
            {
                m_Playable.RemovePause(stateName);
            }
        }

        public void PauseAll()
        {
            if (m_Animator != null)
                m_Animator.enabled = true;

            Kick();

            if (m_Playable != null)
            {
                m_Playable.PauseAll();
            }
        }

        public void RemovePauseAll()
        {
            if (m_Animator != null)
                m_Animator.enabled = true;

            Kick();

            if (m_Playable != null)
            {
                m_Playable.RemovePauseAll();
            }
        }

        public void Stop()
        {
            m_Playable?.StopAll();
            playOverTime?.Cancel();
            playOverTime = null;
        }

        public void Stop(string stateName)
        {
            m_Playable?.Stop(stateName);
            playOverTime?.Cancel();
            playOverTime = null;
        }

        public void StopPlayOverTime()
        {
            playOverTime?.Cancel();
            playOverTime = null;
        }

        public void Sample()
        {
            m_Graph.Evaluate();
        }

        public void AddState(AnimationClip clip, string name)
        {
            LegacyClipCheck(clip);
            Kick();
            if (m_Playable.AddClip(clip, name))
            {
                RebuildStates();
            }            
        }

        public void RemoveState(string name)
        {
            if (m_Playable.RemoveClip(name))
            {
                RebuildStates();
            }
        }     

        public void RemoveClip(AnimationClip clip)
        {
            if (clip == null)
                throw new System.NullReferenceException("clip");

            if (m_Playable.RemoveClip(clip))
            {
                RebuildStates();
            }
        }

        public void Rewind()
        {
            Kick();
            m_Playable.Rewind();
        }

        public void Rewind(string stateName)
        {
            Kick();
            m_Playable.Rewind(stateName);
        }

        public State GetState(string stateName)
        {
            SimpleAnimationPlayable.IState state = m_Playable.GetState(stateName);
            if (state == null)
                return null;

            return new StateImpl(state, this);
        }

        public IEnumerable<State> GetStates()
        {
            return new StateEnumerable(this);
        }

        public State this[string name]
        {
            get { return GetState(name); }
        }


        public void UpdateAni(float deltaTime)
        {
            animator?.Update(deltaTime);
        }       
    }
}