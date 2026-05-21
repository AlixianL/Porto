using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialStepUI : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text labelText;
    

    [Header("Colors")]
    [SerializeField] private Color pendingColor = Color.gray;
    [SerializeField] private Color currentColor = Color.white;
    [SerializeField] private Color validatedColor = Color.green;

    public void SetPending()
    {
        background.color = pendingColor;
        
    }

    public void SetCurrent()
    {
        background.color = currentColor;
        
    }

    public void Validate()
    {
        background.color = validatedColor;
        
    }
}