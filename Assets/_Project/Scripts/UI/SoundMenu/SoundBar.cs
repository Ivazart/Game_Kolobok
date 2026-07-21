using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Project.UI
{
    public class SoundBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text percentText;

        public float Value
        {
            get => slider.value;
            set
            {
                slider.value = Mathf.Clamp01(value);
                UpdatePercentText();
            }
        }

        public System.Action<float> OnValueChanged { get; set; }

        private void Awake()
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            UpdatePercentText();
        }

        private void OnSliderValueChanged(float value)
        {
            UpdatePercentText();
            OnValueChanged?.Invoke(value);
        }

        private void UpdatePercentText()
        {
            if (percentText == null) return;
            float val = slider.value;
            int percent = val <= 0.0002f ? 0 : Mathf.RoundToInt(val * 100f);
            percentText.text = $"{percent}%";
        }

        public void Init(float initialValue, System.Action<float> callback)
        {
            OnValueChanged = callback;
            slider.value = Mathf.Clamp01(initialValue);
            UpdatePercentText();
        }
    }
}