using System;
using System.Collections;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;

public enum MiniGameCompletion { None = -1, Correct, Incorrect }

public interface IMiniGame
{
    event Action<MiniGameCompletion> OnCompleted;

    void Activate();
    void Deactivate();
}

public class BaseMiniGame : MonoBehaviour, IMiniGame
{
    [SerializeField] protected float _miniGameTimeDuration = 5f;

    public event Action<MiniGameCompletion> OnCompleted;

    protected MiniGameCompletion _completionType = MiniGameCompletion.None;

    protected bool _gameTimeIsOver = false;

    protected virtual void OnMiniGameCompleted(MiniGameCompletion type) => OnCompleted?.Invoke(type);

    public virtual void Activate() { }
    protected virtual void StartMiniGameTimer() { }

    protected virtual void CheckMiniGameCompletion() { }

    public virtual void Deactivate() { }
    protected virtual void ProcessSelfDeactivation() { }
}

public class SliderMiniGame : BaseMiniGame
{
    [SerializeField] private RectTransform _handle = null;
    [SerializeField] private RectTransform _positiveField = null;
    [SerializeField] private float _timeDuration = 5f;

    private Slider _slider = null;
    private float _minValue = 0f;
    private float _maxValue = 0f;

    private bool _isCanPlayed = true;

    private Coroutine _cycleRoutine = null;
    private Coroutine _forwardToRoutine = null;
    private Coroutine _miniGameTimerRoutine = null;

    private RectTransform _sliderTransform = null;

    private float _positiveFieldRangeMin = 0f;
    private float _positiveFieldRangeMax = 0f;

    private void Awake()
    {
        _slider = GetComponent<Slider>();

        _minValue = _slider.minValue;
        _maxValue = _slider.maxValue;

        _sliderTransform = GetComponent<RectTransform>();
    }

    private void Start() => Activate();

    public override void Activate()
    {
        CalculatePositionOfPositiveField();
        StartMiniGameTimer();

        _isCanPlayed = true;
        _gameTimeIsOver = false;
        _cycleRoutine = StartCoroutine(CycleRoutine());
    }

    protected override void StartMiniGameTimer() => _miniGameTimerRoutine = StartCoroutine(MiniGameTimer());
    private IEnumerator MiniGameTimer()
    {
        yield return new WaitForSeconds(_miniGameTimeDuration);

        _gameTimeIsOver = true;

        CheckMiniGameCompletion();
        ProcessSelfDeactivation();
    }

    private void CalculatePositionOfPositiveField()
    {
        float fieldWidth = _positiveField.sizeDelta.x;
        float sliderWidth = _sliderTransform.sizeDelta.x;

        float freeRangeBorder = (sliderWidth - fieldWidth) / 2f;

        float xPosition = Random.Range(-freeRangeBorder, freeRangeBorder);

        InitializePositiveFieldRange(xPosition);
        SetPositiveFiledPosition(new Vector2(xPosition, 0f));
    }
    private void InitializePositiveFieldRange(float positiveFieldCenter)
    {
        float halfFieldWidth = _positiveField.sizeDelta.x / 2f;

        _positiveFieldRangeMin = positiveFieldCenter - halfFieldWidth;
        _positiveFieldRangeMax = positiveFieldCenter + halfFieldWidth;
    }
    private void SetPositiveFiledPosition(Vector2 position)
    {
        _positiveField.anchoredPosition = position;
    }

    private IEnumerator CycleRoutine()
    {
        float from = _minValue;
        float to = _maxValue;

        while (_isCanPlayed)
        {
            _forwardToRoutine = StartCoroutine(Forward(from, to));

            yield return _forwardToRoutine;

            SwitchDirection(ref from, ref to);
        }
    }
    private IEnumerator Forward(float from, float to)
    {
        float timeLeft = 0f;

        while (timeLeft < _timeDuration)
        {
            _slider.value = Mathf.Lerp(from, to, timeLeft / _timeDuration);

            timeLeft += Time.deltaTime;

            yield return null;
        }

        _slider.value = to;
    }
    private void SwitchDirection(ref float from, ref float to)
    {
        float temple = from;
              from = to;
              to = temple;
    }

    protected override void CheckMiniGameCompletion()
    {
        _completionType = (_gameTimeIsOver == false && CheckHandleInRangeOfPositiveField()) 
                                    ? MiniGameCompletion.Correct 
                                    : MiniGameCompletion.Incorrect;
    }

    private bool CheckHandleInRangeOfPositiveField()
    {
        return _handle.localPosition.x >= _positiveFieldRangeMin && _handle.localPosition.x <= _positiveFieldRangeMax;
    }

    public override void Deactivate() => ProcessSelfDeactivation();
    protected override void ProcessSelfDeactivation()
    {
        StopGameCycleRoutines();
        NullifyMarkerPostion();
        SetPositiveFiledPosition(Vector2.zero);

        OnMiniGameCompleted(_completionType); 
    }
    private void StopGameCycleRoutines()
    {
        _isCanPlayed = false;

        if (_miniGameTimerRoutine != null)
            StopCoroutine(_miniGameTimerRoutine);

        if (_forwardToRoutine != null)
            StopCoroutine(_forwardToRoutine);

        if (_cycleRoutine != null)
            StopCoroutine(_cycleRoutine);
    }
    private void NullifyMarkerPostion()
    {
        _handle.anchorMin = Vector2.zero;
        _handle.anchorMax = Vector2.zero;
    }

    //Test
    private void Update()
    {
        if (_gameTimeIsOver == false && Input.GetMouseButtonDown(0))
        {
            CheckMiniGameCompletion();
            ProcessSelfDeactivation();
        }
    }
}
