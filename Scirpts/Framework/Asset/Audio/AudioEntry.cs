using System.Collections.Generic;
using UnityEngine;
using System;
using Lib;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework
{
    [Serializable]
    public class AudioEntry 
    {
        public readonly AudioSource mAudioSource;

        public uint eAudioType { get; private set; }
        private bool isMutli;
        public string sAudioPath { get; private set; }
        private AsyncOperationHandle<AudioClip> mHandle;
        public const float VOLUME = 1f;
        public float fVolumeFade = VOLUME;

        public static float DEFAULT_BGMVOLUE_SCALE = 0.5f;

        private readonly float FADE_LENGTH = 0.6f;

        private Timer fadeTimer;
        private Timer timer;

        private bool _isPause;

        public bool isPause
        {
            get
            {
                return _isPause;
            }
            set
            {
                _isPause = value;
                if (_isPause != value)
                {
                    if (_isPause)
                    {
                        mAudioSource.Pause();
                        timer?.Pause();
                    }
                    else
                    {
                        mAudioSource.UnPause();
                        timer?.Pause();
                    }
                }
            }
        }

        public AudioEntry(AudioSource audioSource)
        {
            mAudioSource = audioSource;
            mAudioSource.playOnAwake = false;
        }

        public void Play(string audioPath, bool loop, uint audioType, bool isMutli)
        {
            if (isMutli == false)
            {
                if (string.Equals(sAudioPath, audioPath, StringComparison.Ordinal))
                {
                    return;
                }
                else
                {
                    if (sAudioPath != null)
                    {
                        fadeTimer?.Cancel();
                        fadeTimer = Timer.Register(FADE_LENGTH * 2, null, (dt) =>
                        {
                            if (dt <= FADE_LENGTH)
                            {
                                float rate = dt / FADE_LENGTH;
                                fVolumeFade = Mathf.Lerp(VOLUME * DEFAULT_BGMVOLUE_SCALE, 0.1f, rate);
                            }
                            else
                            {
                                if (!string.Equals(sAudioPath, audioPath, StringComparison.Ordinal))
                                {
                                    TryPlay(audioPath, loop, audioType, isMutli);
                                }

                                float rate = (dt - FADE_LENGTH) / FADE_LENGTH;
                                fVolumeFade = Mathf.Lerp(0.1f, VOLUME * DEFAULT_BGMVOLUE_SCALE, rate);
                            }

                            RefreshVolume();
                        });

                        return;
                    }
                }
            }

            TryPlay(audioPath, loop, audioType, isMutli);
        }

        private void TryPlay(string audioPath, bool loop, uint audioType, bool isMutli)
        {
            if (!string.Equals(sAudioPath, audioPath, StringComparison.Ordinal))
            {
                if (mHandle.IsValid())
                {
                    AddressablesUtil.Release<AudioClip>(ref mHandle, MHandle_Completed);
                }

                mAudioSource.clip = null;
            }
            else
            {
                if (isMutli == false)
                {
                    return;
                }
            }

            sAudioPath = audioPath;
            eAudioType = audioType;
            this.isMutli = isMutli;
            mAudioSource.Stop();
            mAudioSource.loop = loop;

            RefreshVolume();

            if (mAudioSource.clip == null)
            {
                if (!mHandle.IsValid())
                {
                    AddressablesUtil.LoadAssetAsync<AudioClip>(ref mHandle, sAudioPath, MHandle_Completed);
                }
            }
            else
            {
                DoPlay();
            }
        }

        private void MHandle_Completed(AsyncOperationHandle<AudioClip> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                mAudioSource.clip = handle.Result;
                DoPlay();
            }
        }

        private void DoPlay()
        {
            if (mAudioSource.clip != null)
            {
                if (!isPause)
                {
                    mAudioSource.Play();
                }

                if (!mAudioSource.loop)
                {
                    timer?.Cancel();
                    timer = Timer.Register(mAudioSource.clip.length, Stop);
                }
            }
            else
            {
                Stop();
            }
        }

        public void RefreshVolume()
        {
            mAudioSource.volume = fVolumeFade * AudioManager.Instance.GetVolume(eAudioType);
        }

        public void Pause(bool toPause)
        {
            isPause = toPause;
        }

        public void Stop()
        {
            sAudioPath = null;
            mAudioSource.Stop();
            mAudioSource.clip = null;

            timer?.Cancel();

            AddressablesUtil.Release<AudioClip>(ref mHandle, MHandle_Completed);

            if (isMutli)
            {
                AudioManager.Instance.Recovery(this);
            }
        }
    }
}