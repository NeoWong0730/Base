using Framework;
using Lib.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

//#if USE_ADDRESSABLE_ASSET
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//#else
//using Lib.AssetLoader;
//#endif

namespace Logic
{
    public static class ImageHelper
    {
        public static void SetSprite(SpriteRenderer image, string atlasName, string spriteName, bool isSetNativeSize = false)
        {
            if (image == null) { return; }
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            SpriteRendererLoader imageSpriteLoader = image.GetNeedComponent<SpriteRendererLoader>();
            imageSpriteLoader.LoadSprite(atlasName, spriteName, isSetNativeSize);
        }

        public static void SetSprite(SpriteRenderer image, uint iconID, bool isSetNativeSize = false)
        {
            string iconAtlas = null;
            string icon = null;
            CSVIconConfigure.Data iconConfigureData = CSVIconConfigure.Instance.GetConfData(iconID);
            if (iconConfigureData != null)
            {
                iconAtlas = iconConfigureData.iconAtlas;
                icon = iconConfigureData.icon;
            }
            else
            {
                DebugUtil.LogErrorFormat(string.Format("IconConfigure not find id {0}", iconID.ToString()));
            }

            SetSprite(image, iconAtlas, icon, isSetNativeSize);
        }

        public static void SetIcon(Image image, string spriteName, bool isSetNativeSize = false)
        {
            if (image == null) { return; }
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            ImageLoader imageSpriteLoader = image.GetNeedComponent<ImageLoader>();
            imageSpriteLoader.LoadSprite(null, spriteName, isSetNativeSize);
        }

        public static void SetIcon(Image image, string atlasName, string spriteName, bool isSetNativeSize = false)
        {
            if (image == null) { return; }
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            ImageLoader imageSpriteLoader = image.GetNeedComponent<ImageLoader>();
            imageSpriteLoader.LoadSprite(atlasName, spriteName, isSetNativeSize);
        }
        public static void SetTexture(RawImage image, string textureName, bool isSetNativeSize = false)
        {
            if (image == null) { return; }
            RawImageLoader rawImageLoader = image.GetNeedComponent<RawImageLoader>();
            rawImageLoader.Set(textureName, isSetNativeSize);
        }
        public static void SetImageGray(Component image, bool isGray, bool isChild = false)
        {
            if (image == null) { return; }
            Animator[] animators = image.gameObject.GetComponentsInChildren<Animator>(true);
            foreach (Animator animator in animators)
            {
                animator.enabled = !isGray;
            }
            if (isGray)
            {
                SetGraphicGray(image, GlobalAssets.GetAsset<Material>(GlobalAssets.sMat_Gray), isChild);
            }
            else
            {
                SetGraphicGray(image, null, isChild);
            }
        }
        public static void SetGraphicGray(Component root, Material meterial, bool isChild = false)
        {
            if (root == null) { return; }

            if (root is MaskableGraphic)
            {
                var graphic = root as MaskableGraphic;
                graphic.material = meterial;
            }
            if (isChild)
            {
                var graphicArray = root.GetComponentsInChildren<MaskableGraphic>(true);
                for (int i = 0; i < graphicArray.Length; i++)
                {
                    graphicArray[i].material = meterial;
                }
            }
        }
        public static void SetIcon(Image image, uint iconID, bool isSetNativeSize = false)
        {
            string iconAtlas = null;
            string icon = null;
            CSVIconConfigure.Data iconConfigureData = CSVIconConfigure.Instance.GetConfData(iconID);
            if (iconConfigureData != null)
            {
                iconAtlas = iconConfigureData.iconAtlas;
                icon = iconConfigureData.icon;
            }
            else
            {
                DebugUtil.LogErrorFormat(string.Format("IconConfigure not find id {0}", iconID.ToString()));
            }

            SetIcon(image, iconAtlas, icon, isSetNativeSize);
        }
        public static void GetQualityColor_Frame(Image image, int quality)
        {
            uint id = 0;
            switch (quality)
            {
                case 1: { id = 231; } break;
                case 2: { id = 232; } break;
                case 3: { id = 233; } break;
                case 4: { id = 234; } break;
                case 5: { id = 235; } break;
                case 6: { id = 236; } break;
                default:
                    return;
            }
            uint iconId = uint.Parse(CSVParam.Instance.GetConfData(id).str_value);
            SetIcon(image, iconId);
        }

        public static string GetColorHexByQuality(int quality)
        {
            uint id = 0;
            switch (quality)
            {
                case 1: { id = 221; } break;
                case 2: { id = 222; } break;
                case 3: { id = 223; } break;
                case 4: { id = 224; } break;
                case 5: { id = 225; } break;
                case 6: { id = 226; } break;
                default:
                    break;
            }
            string[] _strs1 = CSVParam.Instance.GetConfData(id).str_value.Split('|');
            Color32 color32 = new Color32(byte.Parse(_strs1[0]), byte.Parse(_strs1[1]), byte.Parse(_strs1[2]), 0);
            string str= ColorUtility.ToHtmlStringRGB(color32);
            return str;
        }

        public static void GetPetSkillQuality_Frame(Image image, int quality)
        {
            if (image == null) { return; }

            uint id = 0;
            uint colorId = 0;
            switch (quality)
            {
                case 0: { id = 992901; colorId = 838; } break;
                case 1: { id = 992901; colorId = 838; } break;
                case 2: { id = 992902; colorId = 838; } break;
                case 3: { id = 992903; colorId = 838; } break;
                case 4: { id = 992904; colorId = 837; } break;
                case 5: { id = 992905; colorId = 839; } break;
                case 11: { id = 992906; colorId = 838; } break;
                case 12: { id = 992907; colorId = 838; } break;
                case 13: { id = 992908; colorId = 838; } break;
                case 14: { id = 992909; colorId = 837; } break;
                case 15: { id = 992910; colorId = 839; } break;
                default:
                    return;
            }
            string[] _strs1 = CSVParam.Instance.GetConfData(colorId).str_value.Split('|');
            image.color = new Color(float.Parse(_strs1[0]) / 255f, float.Parse(_strs1[1]) / 255f, float.Parse(_strs1[2]) / 255f, float.Parse(_strs1[3]) / 255f);
            SetIcon(image, id, true);
        }

        public static void GetPetCardLevel(Image image, int quality)
        {
            uint id = 0;
            switch (quality)
            {
                case 0: { id = 822; } break;
                case 1: { id = 823; } break;
                case 2: { id = 824; } break;
                case 3: { id = 825; } break;
                case 4: { id = 826; } break;
                case 5: { id = 827; } break;
                case 6: { id = 828; } break;
                case 7: { id = 829; } break;
                case 8: { id = 830; } break;
                case 9: { id = 831; } break;
                default:
                    return;
            }

            uint iconId = uint.Parse(CSVParam.Instance.GetConfData(id).str_value);
            SetIcon(image, iconId);
        }
        public static void SetBgQuality(RawImage image, uint quality)
        {
            if (image == null) { return; }

            string bgPath = null;
            switch ((EItemQuality)quality)
            {
                case EItemQuality.White:
                    bgPath = Constants.TipBgWhite;
                    break;
                case EItemQuality.Green:
                    bgPath = Constants.TipBgGreen;
                    break;
                case EItemQuality.Blue:
                    bgPath = Constants.TipBgBlue;
                    break;
                case EItemQuality.Purple:
                    bgPath = Constants.TipBgPurple;
                    break;
                case EItemQuality.Orange:
                    bgPath = Constants.TipBgOrange;
                    break;
                default:
                    break;
            }

            RawImageLoader imageLoader = image.GetNeedComponent<RawImageLoader>();
            imageLoader.Set(bgPath);
        }
        public static void SetImgAlpha(Image image, float alpha)
        {
            if (image == null) { return; }

            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        public static void SetChatChannelIcon(Image image, Packet.ChatType chatType, EFightActorType actorType)
        {
            switch (actorType)
            {
                case EFightActorType.Hero:
                    {
                        ImageHelper.SetIcon(image, Constants.ChatChannelIconID_Hero);
                    }
                    return;
                case EFightActorType.Monster:
                    {
                        ImageHelper.SetIcon(image, Constants.ChatChannelIconID_Monster);
                    }
                    return;
                case EFightActorType.Partner:
                    {
                        ImageHelper.SetIcon(image, Constants.ChatChannelIconID_Partner);
                    }
                    return;
                default:
                    break;
            }

            switch (chatType)
            {
                case Packet.ChatType.World:
                    ImageHelper.SetIcon(image, Constants.ChatChannelIconID_World);
                    break;
                case Packet.ChatType.Guild:
                    ImageHelper.SetIcon(image, Constants.ChatChannelIconID_Guild);
                    break;
                case Packet.ChatType.Team:
                    ImageHelper.SetIcon(image, Constants.ChatChannelIconID_Team);
                    break;
                case Packet.ChatType.LookForTeam:
                    ImageHelper.SetIcon(image, Constants.ChatChannelIconID_LookForTeam);
                    break;
                case Packet.ChatType.System:
                    ImageHelper.SetIcon(image, Constants.ChatChannelIconID_System);
                    break;
                case Packet.ChatType.Local:
                    ImageHelper.SetIcon(image, Constants.ChatChannelIconID_Local);
                    break;
                case Packet.ChatType.Person:
                    ImageHelper.SetIcon(image, Constants.ChatChannelIconID_Person);
                    break;
                case Packet.ChatType.Horn:
                    ImageHelper.SetIcon(image, Constants.ChatChannelIconID_Horn);
                    break;
                case Packet.ChatType.Notice:
                    ImageHelper.SetIcon(image, Constants.ChatChannelIconID_Notice);
                    break;
                case Packet.ChatType.Career:
                    ImageHelper.SetIcon(image, Constants.ChatChannelIconID_Career);
                    break;
                case Packet.ChatType.BraveGroup:
                    ImageHelper.SetIcon(image, Constants.ChatChannelIconID_BraveGroup);
                    break;
                default:
                    break;
            }
        }
    }
}
