using Framework;
using UnityEngine.UI;

namespace Logic {
    public static class ButtonHelper {
        public static void Enable(Button button, bool toEnable, bool changeGray = false) {
            button.enabled = toEnable;
            if (changeGray) {
                GraphicGrayer grayer = button.GetOrAddComponent<GraphicGrayer>();
                grayer.Status = toEnable;
            }
        }
    }
}