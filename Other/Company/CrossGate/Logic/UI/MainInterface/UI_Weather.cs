using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using UnityEngine.ResourceManagement.AsyncOperations;
using Lib.Core;

namespace Logic
{
    public class UI_Weather_Layout
    {
        public Transform transform;
        public Image icon;
        public Text state;
        public Text time;
        public Text next;
        public Button closeBtn;

        public void Init(Transform transform)
        {
            this.transform = transform;
            icon = transform.Find("Animator/Image_Icon/Image_Weather").GetComponent<Image>();
            state = transform.Find("Animator/Text_State").GetComponent<Text>();
            time = transform.Find("Animator/Text_Tips").GetComponent<Text>();
            next = transform.Find("Animator/Text_Next").GetComponent<Text>();
            closeBtn = transform.Find("Animator/Button_Close").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
        }
    }

    public class UI_Weather : UIBase, UI_Weather_Layout.IListener
    {
        private uint weatherid;
        private uint nextweatherid;
        private uint seasonid;
        private uint specialWeatherId;
        private UI_Weather_Layout layout = new UI_Weather_Layout();
        private AsyncOperationHandle<GameObject> mHandle;

        private string season;
        private string day;
        private uint nextDayOrNightTime;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnShow()
        {
            SetMessage();
        }

        protected override void OnHide()
        {
            AddressablesUtil.ReleaseInstance(ref mHandle, MHandle_Completed);
        }

        protected override void ProcessEvents(bool toRegister)
        {            
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnWeatherChange, OnWeatherChange, toRegister);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnDayNightChange, OnDayNightChange, toRegister);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnSeasonChange, OnSeasonChange, toRegister);
        }

        private void OnSeasonChange()
        {
            seasonid = (uint)Sys_Weather.Instance.GetESeasonStage()+1;
            OnWeatherChange();
            OnDayNightChange();
        }

        private void OnDayNightChange()
        {
            seasonid = (uint)Sys_Weather.Instance.GetESeasonStage() + 1;
            specialWeatherId = Sys_Weather.Instance.isSpecialWeather();
            if (specialWeatherId!=0)
            {
                season = LanguageHelper.GetTextContent(CSVWeatherSpecial.Instance.GetConfData(specialWeatherId).name);
            }
            else
            {
                season = LanguageHelper.GetTextContent(CSVWeatherSeason.Instance.GetConfData(seasonid).name);
            }
            if (Sys_Weather.Instance.isDay)
            {
                day = LanguageHelper.GetTextContent(1007005);
            }
            else
            {
                day = LanguageHelper.GetTextContent(1007006);
            }
                layout.state.text = LanguageHelper.GetTextContent(1007014, season,day, LanguageHelper.GetTextContent(CSVWeather.Instance.GetConfData(weatherid).name));
                layout.time.text = LanguageHelper.GetTextContent(1007007, day, (Sys_Weather.Instance.GetNextDayOrNightTime() / 60).ToString(), (Sys_Weather.Instance.GetNextDayOrNightTime() % 60).ToString());
        }

        private void OnWeatherChange()
        {
            weatherid = Sys_Weather.Instance.curWeather;
            if (CSVWeather.Instance.ContainsKey(weatherid))
            {
                OnDayNightChange();
                for (int i = 0; i < layout.icon.transform.childCount; i++)
                {
                    GameObject.Destroy(layout.icon.transform.GetChild(i).gameObject);
                }
                AddressablesUtil.InstantiateAsync(ref mHandle, CSVWeather.Instance.GetConfData(weatherid).icon, MHandle_Completed, true, layout.icon.transform);
            }
            nextweatherid = Sys_Weather.Instance.nextWeather;
            if (CSVWeather.Instance.ContainsKey(nextweatherid))
            {
                layout.next.text = LanguageHelper.GetTextContent(1007008, LanguageHelper.GetTextContent(CSVWeather.Instance.GetConfData(nextweatherid).name));
            }
            else{ DebugUtil.LogErrorFormat("天气列表不存在该下一个天气");}
        }

        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            Transform trans = handle.Result.transform;
            trans.localScale = Vector3.one;
        }

        protected override void OnUpdate()
        {
            nextDayOrNightTime = Sys_Weather.Instance.GetNextDayOrNightTime();
            if (Sys_Weather.Instance.isDay)
            {
                layout.time.text = LanguageHelper.GetTextContent(1007007, LanguageHelper.GetTextContent(1007006), (nextDayOrNightTime / 60).ToString(), (nextDayOrNightTime % 60).ToString());
            }
            else
            {
                layout.time.text = LanguageHelper.GetTextContent(1007007, LanguageHelper.GetTextContent(1007005), (nextDayOrNightTime / 60).ToString(), (nextDayOrNightTime % 60).ToString());
            } 
        }

        private void SetMessage()
        {
            OnWeatherChange();
            OnDayNightChange();
            OnSeasonChange();
        }

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Weather);
        }
    }
}
