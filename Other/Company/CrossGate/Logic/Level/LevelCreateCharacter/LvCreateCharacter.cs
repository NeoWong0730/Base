using Lib.AssetLoader;
using Logic.Core;
using UnityEngine;
using Framework;
using Table;
using System;
using System.Collections.Generic;

namespace Logic {
    public class LvCreateCharacter : LevelBase {
        public override void OnEnter(LevelParams param, Type fromLevelType) {
            base.OnEnter(param, fromLevelType);

            HitPointManager.HitPoint("game_createrole");
            //使用最低帧率
            OptionManager.Instance.SetInt(OptionManager.EOptionID.FrameRate, 0, true);
            OptionManager.Instance.SetInt(OptionManager.EOptionID.LightEffect, 1, true);

            //mLevelPreload.Preload<GameObject>(UIConfig.GetConfData(EUIID.UI_CreateCharacter)?.prefabPath);            

            var csvDict = CSVCharacter.Instance.GetAll();
            foreach (var kvp in csvDict) {
                if (kvp.active != 0) {
                    mLevelPreload.Preload<GameObject>(kvp.model_show);
                    mLevelPreload.Preload<GameObject>(kvp.create_char_timeline);
                    List<string> paths;
                    AnimationComponent.GetAnimationPaths(Hero.GetHighModelID(kvp.id), kvp.show_weapon_id, out paths, Constants.IdleAnimationClipHashSet);
                    if (paths != null) {
                        for (int index = 0, len = paths.Count; index < len; index++)
                        {
                            mLevelPreload.Preload<AnimationClip>(paths[index]);
                        }
                    }
                }
            }

            mLevelPreload.PreOpenScene("chuangjue_01", UnityEngine.SceneManagement.LoadSceneMode.Single);

            mLevelPreload.StartLoad();
        }

        public override void OnLoaded() {
            base.OnLoaded();

            //将ShaderManager.WarmUp放在进入游戏 或者 进入创角的时候 因为有加载界面能掩盖卡顿
            //ShaderManager.WarmUp();

            UIManager.OpenUI(EUIID.UI_CreateCharacter);

            //play bgm
            CSVParam.Data paramData = CSVParam.Instance.GetConfData(131);
            if (paramData != null) {
                AudioUtil.PlayAudio(System.Convert.ToUInt32(paramData.str_value));
            }
        }

        public override void OnExit(Type toLevelType) {
            UIManager.CloseUI(EUIID.UI_CreateCharacter);
            base.OnExit(toLevelType);
            OptionManager.Instance.CancelOverride(OptionManager.EOptionID.FrameRate);            
            OptionManager.Instance.CancelOverride(OptionManager.EOptionID.LightEffect);            

            SceneManager.UnLoadAllScene();

            if (toLevelType == typeof(LvLogin)) {
                Sys_Net.Instance.Disconnect();
            }
        }
    }
}