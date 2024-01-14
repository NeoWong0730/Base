//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

using System.Collections.Generic;
using Framework;
using Logic;
using Logic.Core;
using Table;

public static class AudioUtil {
    public enum EAudioType {
        SkillSound = 1, //
        NPCSound = 2,   //NPC对话
        BGM = 3,        //
        UISound = 4,    //
        SceneSound = 5, //新类型在 Normal前添加
        VideoSound = 6, // cutscene视屏音效
    }

    public static AudioEntry PlayAudio(uint audioId) {
        AudioEntry audioEntry = null;
        CSVSound.Data data = CSVSound.Instance.GetConfData(audioId);
        if (data != null) {
            bool isBGM = data.Type == (int)EAudioType.BGM;
            bool isLoop = isBGM;
            bool isMulti = !isBGM;

            audioEntry = AudioManager.Instance.Play(data.Audio_Path, data.Type, isMulti, isLoop, isBGM ? AudioEntry.DEFAULT_BGMVOLUE_SCALE : 1f);
        }
        return audioEntry;
    }

    public static AudioEntry PlayAudioEx(uint audioId, bool isLoop, bool isMulti) {
        AudioEntry audioEntry = null;
        CSVSound.Data data = CSVSound.Instance.GetConfData(audioId);
        if (data != null) {
            audioEntry = AudioManager.Instance.Play(data.Audio_Path, data.Type, isMulti, isLoop, data.Type == (int)EAudioType.BGM ? AudioEntry.DEFAULT_BGMVOLUE_SCALE : 1f);
        }
        return audioEntry;
    }
    
    // 播放配音
    public static AudioEntry PlayDubbing(uint audioId, EAudioType audioType = EAudioType.NPCSound, bool isMulti = true, bool isLoop = false) {
        AudioEntry audioEntry = null;
        CSVDubbing.Data data = CSVDubbing.Instance.GetConfData(audioId);
        if (data != null) {
            int voiceType = OptionManager.Instance.GetInt(OptionManager.EOptionID.VoiceLanguage);
            string dir = data.MainLand;
            string path = $"Audio/Dubbing/MainLand/{dir}";
            if (voiceType == 1) {
                dir = data.TW;
            }
            else if (voiceType == 2) {
                dir = data.HK;
            }

            if (!string.IsNullOrWhiteSpace(dir)) {
                if (voiceType == 1) {
                    path = $"Audio/Dubbing/TW/{dir}";
                }
                else if (voiceType == 2) {
                    path = $"Audio/Dubbing/HK/{dir}";
                }
                
                audioEntry = AudioManager.Instance.Play(path, (uint)audioType, isMulti, isLoop);
            }
        }
        return audioEntry;
    }

    public static void StopAudio(AudioEntry audioEntry) {
        audioEntry?.Stop();
    }
    public static void StopAll() {
        AudioManager.Instance.StopAll();
    }
    public static void StopSingle(AudioUtil.EAudioType audioType) {
        AudioManager.Instance.StopSingle((uint)audioType);
    }

    public static void PauseSingle(AudioUtil.EAudioType audioType, bool toPause) {
        AudioManager.Instance.PauseSingle((uint)audioType, toPause);
    }

    public static AudioEntry PlayMapBGM() {
        CSVMapInfo.Data data = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
        if (data != null) {
            return PlayAudio(data.sound_bgm);
        }
        return null;
    }
    /*
    public static bool Probability(float probability)
    {
        bool result = false;
        if (probability <= 0f)
        {
            result = false;
        }
        else if (probability >= 1f)
        {
            result = true;
        }
        else
        {
            float random = UnityEngine.Random.Range(0, 10000f);
            if (random > probability * 10000f)
            {
                result = false;
            }
            else
            {
                result = true;
            }
        }
        return result;
    }*/
}
