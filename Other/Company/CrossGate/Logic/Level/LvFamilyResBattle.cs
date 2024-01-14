using Lib.AssetLoader;
using Logic.Core;
using UnityEngine;
using Framework;
using Table;
using System;
using System.Collections.Generic;

namespace Logic {
    public class LvFamilyResBattle : LevelBase {
        public override void OnEnter(LevelParams param, Type fromLevelType) {
            base.OnEnter(param, fromLevelType);

            //mLevelPreload.PreOpenScene("chuangjue_01", UnityEngine.SceneManagement.LoadSceneMode.Single);

            mLevelPreload.StartLoad();
        }

        public override void OnLoaded() {
            base.OnLoaded();

            UIManager.OpenUI(EUIID.UI_MainInterface);
        }

        public override void OnExit(Type toLevelType) {
        }
    }
}