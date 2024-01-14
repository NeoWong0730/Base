using Table;
using UnityEngine.UI;

namespace Logic
{
    public class OccupationHelper
    {

        public static uint GetIconID(uint occupationId)
        {
            uint icon = 0;

            CSVCareer.Data  data = CSVCareer.Instance.GetConfData(occupationId);

            if (data == null)
                return icon;

            //if (occupationId == 100)
            //    return icon;

            icon = data.icon;

            return icon;
        }

        public static uint GetCareerLogoIcon(uint occupationId)
        {
            uint icon = 0;
            CSVCareer.Data data = CSVCareer.Instance.GetConfData(occupationId);

            if (data == null)
                return icon;

            icon = data.logo_icon;

            return icon;
        }

        public static uint GetTeamIconID(uint occupationId)
        {
            uint icon = 990055;

            CSVCareer.Data data = CSVCareer.Instance.GetConfData(occupationId);

            if (data == null)
                return icon;

            //if (occupationId == 100)
            //    return icon;

            icon = data.team_icon;

            return icon;
        }

        public static uint GetLogonIconID(uint occupationId)
        {
            uint icon = 0;

            CSVCareer.Data data = CSVCareer.Instance.GetConfData(occupationId);

            if (data == null)
                return icon;

            icon = data.logo_icon;

            return icon;
        }

        public static uint GetTextID(uint occupationId,uint careerRank=0)
        {
            uint textID = 0;

            //if (occupationId == 100)
            //    return 0;

            CSVCareer.Data data = CSVCareer.Instance.GetConfData(occupationId);

            if (data == null)
                return textID;

            if (careerRank == 0)
            {
                textID = data.name;
            }
            else
            {
                textID =CSVPromoteCareer.Instance.GetConfData(Sys_Advance.Instance.GetPromoteIdByRank(occupationId, careerRank)).professionLan;
            }

            return textID;//occupationId / 10 +1000100;
        }
    }


    public class CharacterHelper
    {
        public static uint getHeadID(uint heroID, uint headId)
        {
            uint headIconId = 0;
            CSVHead.Instance.TryGetValue(headId, out CSVHead.Data data);
            CSVCharacter.Data heroData = CSVCharacter.Instance.GetConfData(heroID);
            if (data != null && data.HeadIcon[0] != 0)
            {
                headIconId = Sys_Head.Instance.GetHeadIconIdByRoleType(data.HeadIcon, heroID);
            }
            else
            {
                if (heroData != null)
                {
                    headIconId = heroData.headid;
                }
            }
            return headIconId;
        }

        public static uint getHeadFrameID(uint headFrameId)
        {
            uint headFrameIconId = 0;
            CSVHeadframe.Instance.TryGetValue(headFrameId, out CSVHeadframe.Data headFrameData);
            if (headFrameData != null)
            {
                headFrameIconId = CSVHeadframe.Instance.GetConfData(headFrameId).HeadframeIcon;
            }
            return headFrameIconId;
        }


        public static void SetHeadAndFrameData(Image headIcon, uint roleId, uint headId, uint headFrameId)
        {
            ImageHelper.SetIcon(headIcon, getHeadID(roleId, headId));
            uint frameId = getHeadFrameID(headFrameId);
            Image headFrame = headIcon.transform.Find("Image_Before_Frame").GetComponent<Image>();
            if (frameId == 0)
            {
                headFrame.gameObject.SetActive(false);
            }
            else
            {
                headFrame.gameObject.SetActive(true);
                ImageHelper.SetIcon(headFrame, frameId);
            }
        }
    }
}
