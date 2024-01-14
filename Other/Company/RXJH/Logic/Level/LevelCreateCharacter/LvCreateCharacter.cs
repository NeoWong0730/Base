using Logic.Core;
using UnityEngine;
using Framework;
using Table;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic {
    public class LvCreateCharacter : LevelBase {
        public AsyncOperationHandle<GameObject> humanHandler;

        public override void OnEnter(LevelParams param, Type fromLevelType) {
            void Preload() {
                var csvDict = CSVCareer.Instance.GetAll();
                foreach (var kvp in csvDict) {
                    if (kvp.active) {
                        var csvModel = CSVCareerModel.Instance.GetConfData(kvp.femaleModel);
                        if (csvModel != null) {
                            mLevelPreload.Preload<GameObject>(csvModel.Cloth);
                            mLevelPreload.Preload<GameObject>(csvModel.Hair);
                        }

                        csvModel = CSVCareerModel.Instance.GetConfData(kvp.maleModel);
                        if (csvModel != null) {
                            mLevelPreload.Preload<GameObject>(csvModel.Cloth);
                            mLevelPreload.Preload<GameObject>(csvModel.Hair);
                        }
                    }
                }
            }

            // AddressablesUtil.LoadAssetAsync<GameObject>(ref humanHandler, "human", null, false);
            // humanHandler.WaitForCompletion();
            
            // Preload();

            mLevelPreload.PreOpenScene("scene_createcharacter", UnityEngine.SceneManagement.LoadSceneMode.Single);
            mLevelPreload.StartLoad();
        }

        public override void OnLoaded() {
            var zone = Sys_Server.Instance.GetSelectedZone();
            if (zone != null) {
                EUIID aimId = EUIID.Invalid;
                if (zone.roles.Count > 0) {
                    aimId = EUIID.UI_LoginOrCreateCharacter;
                }
                else {
                    aimId = EUIID.UI_CreateCharacter;
                }

                UIManager.OpenUI(aimId);
            }
            else {
                // can't reach here
                UIManager.OpenUI(EUIID.UI_LoginOrCreateCharacter);
            }
        }

        public override void OnExit(Type toLevelType) {
            base.OnExit(toLevelType);

            if (humanHandler.IsValid()) {
                AddressablesUtil.Release<GameObject>(ref humanHandler, null);
            }

            SceneManager.UnLoadAllScene();

            if (toLevelType == typeof(LvLogin)) {
                Sys_Net.Instance.Disconnect();
            }
        }
    }
}