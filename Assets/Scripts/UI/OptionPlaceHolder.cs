using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsPlaceholder : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text valueText;

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        UpdateText(volumeSlider.value);
    }

    void OnSliderChanged(float value)
    {
        Debug.Log("Volume (placeholder) : " + value);
        UpdateText(value);
    }

    void UpdateText(float value)
    {
        if (valueText != null)
            valueText.text = "Volume : " + Mathf.RoundToInt(value * 100) + "%";
    }
}