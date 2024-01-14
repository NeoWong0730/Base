using Framework;
using Lib.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public static class ImageHelper {
        // 灰化
        public static void SetGray(Component image, bool toGray, bool isChild = false) {
            if (image != null) {
                GraphicGrayer grayer = image.GetOrAddComponent<GraphicGrayer>();
                grayer.Status = !toGray;
            }
        }

        // RawImage设置
        public static void SetTexture(RawImage image, string textureName, bool isSetNativeSize = false) {
            if (image == null) {
                return;
            }

            RawImageLoader rawImageLoader = image.GetOrAddComponent<RawImageLoader>();
            rawImageLoader.Set(textureName, isSetNativeSize);
        }

        public static void SetIcon(Image image, string atlasName, string spriteName, bool isSetNativeSize = false) {
            if (image == null) {
                return;
            }

            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            ImageLoader imageSpriteLoader = image.GetOrAddComponent<ImageLoader>();
            imageSpriteLoader.LoadSprite(atlasName, spriteName, isSetNativeSize);
        }

        // 设置Image
        public static void SetIcon(Image image, uint csvIconId, bool isSetNativeSize = false) {
            string atlas = null;
            string icon = null;
            CSVIcon.Data csvIcon = CSVIcon.Instance.GetConfData(csvIconId);
            if (csvIcon != null) {
                icon = csvIcon.icon;

                CSVAtlas.Data csvAtlas = CSVAtlas.Instance.GetConfData(csvIcon.atlas);
                if (csvAtlas != null) {
                    atlas = csvAtlas.atlas;
                }
                else {
                    DebugUtil.LogWarningFormat("CSVAtlas not find id {0}", csvIcon.atlas.ToString());
                }
            }
            else {
                DebugUtil.LogErrorFormat("CSVIcon not find id {0}", csvIconId.ToString());
                return;
            }

            SetIcon(image, atlas, icon, isSetNativeSize);
        }
    }
}