using Packet;
using UnityEngine;
using Logic.Core;
using Lib.AssetLoader;
using System.Collections.Generic;
using UnityEngine.UI;
using Table;
using Framework;
using System;
using Lib.Core;
using UnityEngine.EventSystems;
using System.Collections;
using System.IO;
using DG.Tweening;

namespace Logic
{
    public class UI_PaintBoard : UIBase
    {
        private Shader _paintBrushShader;            //绘图shader
        private Material _paintBrushMat;             //绘图material
        private Shader _clearBrushShader;
        private Material _clearBrushMat;
        private Texture _defaultBrushTex;            //默认笔刷
        private RawImage _paintCanvas;               //绘画的画布
        private RenderTexture _renderTex;            //renderTexture
        private int _paintCanvasWidth;
        private int _paintCanvasHeight;
        private float _csvSize;                      //笔刷的配置大小
        private float _brushSize;                    //笔刷的实际大小
        private bool _vaildPaint;                    //控制是否能画画
        private bool _beginPaint;                    //是否开始画画
        private Color _defaultColor = Color.yellow;  //笔刷的默认颜色
        private float _brushLerpSize;                //笔刷的间隔大小
        private Vector2 _lastPoint;                  //默认上一次点的位置
        private Timer _timer;                        //计时器
        private int _time;                           //配置画画的时长
        private float _timeCounter;
        private int _FxTime;
        private float _fxTimeCounter;
        private bool _bStartFxCount = false;
        private bool _bStartTimeAnim = false;
        private bool _bStartTimeAnimed = false;
        private RectTransform _screenShots;          //截屏参考ui
        private Button _closeButton;
        private Button _submitButton;
        private GameObject _title;
        private GameObject _viewTime;
        private GameObject _viewShare_btn;
        private GameObject _viewShare_server;
        private GameObject _viewShare_player;
        private GameObject _viewScoreRoot;
        private GameObject _viewScore;
        private GameObject _viewScoreTips;
        private Text _text_Server;
        private Text _text_Name;
        private Text _time_Text;
        private GameObject _fxPainting;
        private GameObject _fxEndPaint;
        private GameObject _fxLimitPaint;
        private GameObject root;
        private CanvasGroup m_CanvasGroup;
        private string[] _timeconfigs;
        private uint _taskId;

        protected override void OnOpen(object arg)
        {
            Tuple<uint, object> tuple = arg as Tuple<uint, object>;
            if (tuple != null)
            {
                _taskId = tuple.Item1;
            }
        }

        protected override void OnLoaded()
        {
            _defaultBrushTex = GlobalAssets.GetAsset<Texture>(GlobalAssets.sTexture_Tt_paint);
            _paintCanvas = transform.Find("Animator/RawImage").GetComponent<RawImage>();
            root = transform.Find("Animator/GameObject").gameObject;
            m_CanvasGroup = root.GetComponent<CanvasGroup>();
            _text_Server = transform.Find("Animator/GameObject/View_Share/Text_Server/Text_Name").GetComponent<Text>();
            _text_Name = transform.Find("Animator/GameObject/View_Share/Text_Player/Text_Name").GetComponent<Text>();
            _time_Text = transform.Find("Animator/GameObject/View_Time/Text_Time").GetComponent<Text>();
            _title = transform.Find("Animator/GameObject/Image_BG04").gameObject;
            _viewTime = transform.Find("Animator/GameObject/View_Time").gameObject;
            _viewShare_btn = transform.Find("Animator/GameObject/View_Share/Btn_01").gameObject;
            _viewShare_server = transform.Find("Animator/GameObject/View_Share/Text_Server").gameObject;
            _viewShare_player = transform.Find("Animator/GameObject/View_Share/Text_Player").gameObject;
            _viewScoreRoot = transform.Find("Animator/GameObject/View_Share/Evaluation").gameObject;
            _viewScore = _viewScoreRoot.transform.Find("Text_Score").gameObject;
            _viewScoreTips = _viewScoreRoot.transform.Find("Text_ScoreTips").gameObject;
            _screenShots = transform.Find("Animator/GameObject/ScreenShotBG") as RectTransform;
            _closeButton = transform.Find("Animator/GameObject/View_Share/Btn_01").GetComponent<Button>();
            _submitButton = transform.Find("Animator/GameObject/View_Share/Btn_02").GetComponent<Button>();

            _closeButton.onClick.AddListener(() =>
            {
                ReleaseRT();
                _viewScoreRoot.SetActive(false);
                DestroyEndPaintFx();
                UIManager.CloseUI(EUIID.UI_Paint);
            });

            _submitButton.onClick.AddListener(() =>
            {
                //Sys_Task.Instance.ReqStepGoalFinishEx(_taskId);
                EndPaint();
                _submitButton.gameObject.SetActive(false);
            });

            //_shareButton.onClick.AddListener(() =>
            //{
            //    _viewShare_btn.SetActive(false);
            //    _viewShare_server.SetActive(true);
            //    _viewShare_player.SetActive(true);
            //    _title.SetActive(false);
            //    _screenShots.gameObject.SetActive(true);
            //    _closeButton.gameObject.SetActive(false);

            //    DestroyEndPaintFx();
            //    OnShowTexture(_screenShots);
            //});

            //_closeButton.onClick.AddListener(() =>
            // {
            //     ReleaseRT();
            //     _viewScoreRoot.SetActive(false);
            //     DestroyEndPaintFx();
            //     UIManager.CloseUI(EUIID.UI_Paint);
            // }
            //);


            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(_paintCanvas);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnPointClick);
            eventListener.onDragEnd = OnEndDrag;
            eventListener.onDragStart = OnBeginDrag;
            Lib.Core.EventTrigger eventListener1 = Lib.Core.EventTrigger.Get(transform.Find("Animator/GameObject/ClickClose").gameObject);
            eventListener1.AddEventListener(EventTriggerType.PointerClick, (_) =>
            {
                if (!_vaildPaint)
                {
                    UIManager.CloseUI(EUIID.UI_Paint);
                }
            }
            );
            _paintBrushShader = ShaderManager.Find("Unlit/PaintBrush");
            _clearBrushShader = ShaderManager.Find("Unlit/ClearBrush");
        }

        protected override void OnShow()
        {
            Sys_LittleGame_SeekItem.Instance.EnterPaint(_taskId);
            ResetData();
            ParseData();
            InitData();
            ResetComponent();
        }

        protected override void OnShowEnd()
        {
            PlayPaintingFx();
            PlayLimitFx();
            _bStartFxCount = true;
        }

        private void ParseData()
        {
            string color = CSVParam.Instance.GetConfData(292).str_value;
            string[] colors = color.Split('|');
            _defaultColor = new Color(float.Parse(colors[0]) / 255, float.Parse(colors[1]) / 255, float.Parse(colors[2]) / 255, 1);
            _time = int.Parse(CSVParam.Instance.GetConfData(293).str_value);
            _FxTime = int.Parse(CSVParam.Instance.GetConfData(290).str_value);
            _timeCounter = (float)_time;
            _fxTimeCounter = (float)_FxTime;
            _csvSize = float.Parse(CSVParam.Instance.GetConfData(291).str_value);
            _timeconfigs = CSVParam.Instance.GetConfData(673).str_value.Split(',');
        }


        //初始化数据
        void InitData()
        {
            _brushSize = _csvSize * 50f;
            _brushLerpSize = (_defaultBrushTex.width + _defaultBrushTex.height) / 2.0f / _brushSize;
            _lastPoint = Vector2.zero;

            if (_paintBrushMat == null)
            {
                UpdateBrushMaterial();
            }
            if (_clearBrushMat == null)
                _clearBrushMat = new Material(_clearBrushShader);
            if (_renderTex == null)
            {
                _paintCanvasWidth = (int)(_paintCanvas.transform as RectTransform).rect.width;
                _paintCanvasHeight = (int)(_paintCanvas.transform as RectTransform).rect.height;
                _renderTex = RenderTexture.GetTemporary(_paintCanvasWidth, _paintCanvasHeight, 24);
                _paintCanvas.texture = _renderTex;
            }
            Graphics.Blit(null, _renderTex, _clearBrushMat);
            //ShowRawImgae();
        }

        //更新笔刷材质
        private void UpdateBrushMaterial()
        {
            _paintBrushMat = new Material(_paintBrushShader);
            _paintBrushMat.SetTexture("_BrushTex", _defaultBrushTex);
            _paintBrushMat.SetColor("_Color", _defaultColor);
            _paintBrushMat.SetFloat("_Size", _brushSize);
        }

        //插点
        private void LerpPaint(Vector2 point)
        {
            Paint(point);

            if (_lastPoint == Vector2.zero)
            {
                _lastPoint = point;
                return;
            }

            float dis = Vector2.Distance(point, _lastPoint);
            if (dis > _brushLerpSize)
            {
                Vector2 dir = (point - _lastPoint).normalized;
                int num = (int)(dis / _brushLerpSize);
                for (int i = 0; i < num; i++)
                {
                    Vector2 newPoint = _lastPoint + dir * (i + 1) * _brushLerpSize;
                    Paint(newPoint);
                }
            }
            _lastPoint = point;
        }

        //画点
        private void Paint(Vector2 point)
        {
            if (point.x < 0 || point.x > _paintCanvasWidth || point.y < 0 || point.y > _paintCanvasHeight)
                return;

            Vector2 uv = new Vector2(point.x / (float)_paintCanvasWidth,
                point.y / (float)_paintCanvasHeight);
            _paintBrushMat.SetVector("_UV", uv);
            Graphics.Blit(_renderTex, _renderTex, _paintBrushMat);
        }

        public void OnDrag(BaseEventData eventData)
        {
            if (!_vaildPaint)
                return;
            PointerEventData pointerEventData = eventData as PointerEventData;
            Vector2 templocalPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_paintCanvas.transform as RectTransform,
                pointerEventData.position, pointerEventData.pressEventCamera, out templocalPoint);
            LerpPaint(templocalPoint);
        }

        public void OnBeginDrag(GameObject go)
        {
            if (!_vaildPaint)
                return;
            BeginPaint();
        }

        public void OnEndDrag(GameObject go)
        {
            _lastPoint = Vector2.zero;
        }


        public void OnPointClick(BaseEventData baseEventData)
        {
            if (!_vaildPaint)
                return;
            BeginPaint();
            PointerEventData pointerEventData = baseEventData as PointerEventData;
            Vector2 templocalPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_paintCanvas.transform as RectTransform,
                pointerEventData.position, pointerEventData.pressEventCamera, out templocalPoint);
            Paint(templocalPoint);
        }

        protected override void OnClose()
        {
            TriggerStone();
        }

        private void TriggerStone()
        {
            //UIManager.OpenUI(EUIID.UI_Ring);
        }


        private Rect targetRect;
        private CoroutineHandler mCoroutineHandler;

        public void OnShowTexture(RectTransform rect)
        {
            Vector3 vect = RectTransformUtility.WorldToScreenPoint(UIManager.mUICamera, rect.gameObject.transform.position);
            CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
            float radio = Screen.height / canvasScaler.referenceResolution.y;
            float x = vect.x - rect.sizeDelta.x * rect.pivot.x * radio;
            float y = vect.y - rect.sizeDelta.y * rect.pivot.x * radio;

            targetRect = new Rect(x, y, rect.sizeDelta.x * radio, rect.sizeDelta.y * radio);
            UploadPNG();
            mCoroutineHandler = CoroutineManager.Instance.StartHandler(UploadPNG());
        }

        private IEnumerator UploadPNG()
        {
            yield return new WaitForEndOfFrame();
            Texture2D tex = new Texture2D((int)targetRect.width, (int)targetRect.height, TextureFormat.RGB24, false);

            tex.ReadPixels(new Rect((int)targetRect.x, (int)targetRect.y, (int)targetRect.width, (int)targetRect.height), 0, 0);
            tex.Apply();

            string path = Application.persistentDataPath + "onMobileSavedScreen.png";
            DebugUtil.Log(ELogType.eGrabTexture, path);
            File.WriteAllBytes(path, tex.EncodeToPNG());
            _screenShots.gameObject.SetActive(false);
            UIManager.CloseUI(EUIID.UI_Paint);
            ReleaseRT();
        }

        private void ReleaseRT()
        {
            RenderTexture.ReleaseTemporary(_renderTex);
            _renderTex = null;
        }

        public int _TimeCounter
        {
            get
            {
                return (int)(_timeCounter) + 1;
            }
        }

        public int _LastTime = 0;

        protected override void OnUpdate()
        {
            if (!_vaildPaint)
                return;

            if (!_beginPaint && _bStartFxCount)
            {
                _fxTimeCounter -= deltaTime;
                if (_fxTimeCounter <= 0)
                {
                    //DestroyPaintingFx();
                    //PlayPaintingFx();
                    _fxTimeCounter = _FxTime;
                }
            }
            if (_beginPaint)
            {
                _timeCounter -= deltaTime;
                if (_timeCounter <= 0)
                {
                    EndPaint();
                    _submitButton.gameObject.SetActive(false);
                }
                if (_LastTime != _TimeCounter)
                {
                    _LastTime = _TimeCounter;
                    //_text_Time.text = _LastTime.ToString();
                    UpdateTime();
                }
            }
            if (_bStartTimeAnim)
            {
                _bStartTimeAnim = false;
                _submitButton.gameObject.SetActive(true);
                _time_Text.gameObject.SetActive(true);
            }
        }

        private void UpdateTime()
        {
            uint iconId = uint.Parse(_timeconfigs[_LastTime]);
            TextHelper.SetText(_time_Text, _LastTime.ToString());
        }

        private void BeginPaint()
        {
            _beginPaint = true;
            if (!_bStartTimeAnimed)
            {
                _bStartTimeAnimed = true;
                _bStartTimeAnim = true;
            }
        }

        private void EndPaint()
        {
            _vaildPaint = false;
            _time_Text.gameObject.SetActive(false);
            //_viewShare_btn.SetActive(true);
            //_viewShare_server.SetActive(true);
            //_viewShare_player.SetActive(true);
            //_viewScoreRoot.SetActive(true);
            //_viewScoreRoot.GetComponent<Animator>().enabled = true;
            //_text_Name.text = Sys_Role.Instance.sRoleName;
            //_text_Server.text = Sys_Login.Instance.mSelectedServer.mServerInfo.ServerName;
            DestroyPaintingFx();
            DestroyLimitFx();
            PlayEndPaintFx();
            //Score();

            DOTween.To(() => m_CanvasGroup.alpha, x => m_CanvasGroup.alpha = x, 0, 2).onComplete = OnFadeComplete;
        }

        private void OnFadeComplete()
        {
            Sys_Task.Instance.ReqStepGoalFinishEx(_taskId);
            Sys_CutScene.Instance.TryDoCutScene(5000403);
            UIManager.CloseUI(EUIID.UI_Paint);
        }

        private void Score()
        {
            string str = CSVParam.Instance.GetConfData(294).str_value;
            string[] scores = str.Split('|');
            int index = UnityEngine.Random.Range(0, scores.Length);
            int score = int.Parse(scores[index]);

            string str1 = CSVParam.Instance.GetConfData(295).str_value;
            string[] scoretips = str1.Split('|');
            int _index = UnityEngine.Random.Range(0, scoretips.Length);
            uint lanId = uint.Parse(scoretips[_index]);
            string lan = CSVLanguage.Instance.GetConfData(lanId).words;

            _viewScore.transform.Find("Text_Name").GetComponent<Text>().text = score.ToString();
            _viewScoreTips.transform.Find("Text_Name").GetComponent<Text>().text = lan.ToString();
        }


        private void PlayPaintingFx()
        {
            if (_fxPainting == null)
            {
                _fxPainting = GameObject.Instantiate<GameObject>(GlobalAssets.GetAsset<GameObject>(GlobalAssets.sPrefab_Fx_painting));
                _fxPainting.transform.SetParent(transform.Find("Animator"));
                _fxPainting.transform.localScale = Vector3.one;
                _fxPainting.transform.localPosition = Vector3.zero;
            }
        }

        private void PlayLimitFx()
        {
            if (_fxLimitPaint == null)
            {
                _fxLimitPaint = GameObject.Instantiate<GameObject>(GlobalAssets.GetAsset<GameObject>(GlobalAssets.sPrefab_Fx_limitpaint));
                _fxLimitPaint.transform.SetParent(transform.Find("Animator"));
                _fxLimitPaint.transform.localScale = Vector3.one;
                _fxLimitPaint.transform.localPosition = Vector3.zero;
            }
        }


        private void PlayEndPaintFx()
        {
            if (_fxEndPaint == null)
            {
                _fxEndPaint = GameObject.Instantiate<GameObject>(GlobalAssets.GetAsset<GameObject>(GlobalAssets.sPrefab_Fx_endpaint));
                _fxEndPaint.transform.SetParent(transform.Find("Animator"));
                _fxEndPaint.transform.localScale = Vector3.one;
                _fxEndPaint.transform.localPosition = Vector3.zero;
            }
        }

        private void DestroyPaintingFx()
        {
            if (null != _fxPainting)
            {
                GameObject.Destroy(_fxPainting);
                _fxPainting = null;
            }
        }


        private void DestroyLimitFx()
        {
            if (null != _fxLimitPaint)
            {
                GameObject.Destroy(_fxLimitPaint);
                _fxLimitPaint = null;
            }
        }

        private void DestroyEndPaintFx()
        {
            if (null != _fxEndPaint)
            {
                GameObject.Destroy(_fxEndPaint);
                _fxEndPaint = null;
            }
        }

        private void ResetData()
        {
            _vaildPaint = true;
            _beginPaint = false;
            _bStartFxCount = false;
            _bStartTimeAnimed = false;
            _bStartTimeAnim = false;
            _LastTime = 0;
            _renderTex = null;
        }

        private void ResetComponent()
        {
            _screenShots.gameObject.SetActive(false);
            _submitButton.gameObject.SetActive(false);
            _viewShare_btn.SetActive(false);
            _viewShare_server.SetActive(false);
            _viewShare_player.SetActive(false);
            _closeButton.gameObject.SetActive(false);
            _viewScoreRoot.SetActive(false);
            _time_Text.gameObject.SetActive(false);
            _viewScoreRoot.GetComponent<Animator>().enabled = false;
            _viewTime.SetActive(true);
            _title.SetActive(true);
        }
    }
}



