using System;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // 对话
    public class UI_FavorabilityDialogue : UI_Dialogue {
        protected override void OnClose() {
            base.OnClose();

            OnClickSkipButton();
        }

        protected override void ProcessEvents(bool toRegister) {
            base.ProcessEvents(toRegister);

            Sys_Dialogue.Instance.eventEmitter.Handle<bool>(Sys_Dialogue.EEvents.OnShowContent, OnShowContent, toRegister);
        }
        private void OnShowContent(bool toShow) {
            if (gameObject != null && gameObject.activeInHierarchy && dialogueRoot != null) {
                dialogueRoot.SetActive(toShow);
            }
        }
    }
}