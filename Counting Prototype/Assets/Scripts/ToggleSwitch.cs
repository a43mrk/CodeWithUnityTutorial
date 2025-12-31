using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    [Header("Slider setup")]
    [SerializeField, Range(0, 1f)]
    private float sliderValue;
    public bool CurrentValue {get; private set;}

    private Slider _slider;

    [Header("Animation")]
    [SerializeField, Range(0, 1f)]
    private float animationDuration = 0.5f;
    [SerializeField]
    private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine _animateSliderCoroutine;

    [Header("Events")]
    [SerializeField]
    private UnityEvent<bool> onToggle;

    private ToggleSwitchGroupManager _toggleSwitchManager;


    void OnValidate()
    {
        SetupToggleComponent();
        _slider.value = sliderValue;
    }

    void Awake()
    {
        SetupToggleComponent();
    }

    private void SetupToggleComponent()
    {
        if(_slider != null)
            return;
        SetupSliderComponent();
    }

    private void SetupSliderComponent()
    {
        _slider = GetComponent<Slider>();

        if(_slider == null)
        {
            Debug.LogError("No slider found!", this);
            return;
        }

        _slider.interactable = false;
        var sliderColors = _slider.colors;
        sliderColors.disabledColor = Color.white;
        _slider.colors = sliderColors;
        _slider.transition = Selectable.Transition.None;
    }

    public void SetupForManager(ToggleSwitchGroupManager manager)
    {
        _toggleSwitchManager = manager;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }

    private void Toggle()
    {
        if(_toggleSwitchManager != null)
            _toggleSwitchManager.ToggleGroup(this);
        else
            SetStateAndStartAnimation(!CurrentValue);
    }

    public void ToggleByGroupManager(bool value)
    {
        SetStateAndStartAnimation(value);
    }

    private void SetStateAndStartAnimation(bool v)
    {
        CurrentValue = v;

        // publish the current value
        onToggle?.Invoke(CurrentValue);

        if(_animateSliderCoroutine != null)
            StopCoroutine(_animateSliderCoroutine);
        
        _animateSliderCoroutine = StartCoroutine(AnimateSlider());
    }

    private System.Collections.IEnumerator AnimateSlider()
    {
        float startValue = _slider.value;
        float endValue = CurrentValue ?1 :0;

        float time = 0f;

        while(animationDuration >0 && time < animationDuration)
        {
            time += Time.deltaTime;

            float lerpFactor = slideEase.Evaluate(time / animationDuration);
            _slider.value = sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);

            yield return null;
        }

        _slider.value = endValue;
    }
}
