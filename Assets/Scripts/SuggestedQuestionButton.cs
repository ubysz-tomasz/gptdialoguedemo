using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SuggestedQuestionButton : MonoBehaviour
{
    public string Text
    {
        get => _buttonText == null ? "" : _buttonText.text;
        set
        {
            if (_buttonText == null) return;
            
            _buttonText.text = value;
        }
    }

    private Button _button;
    private ColorBlock _normalColorBlock;
    private TMP_Text _buttonText;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _buttonText = GetComponentInChildren<TMP_Text>();

        _normalColorBlock = _button.colors;
        
        if (_buttonText == null) Debug.LogError($"No button text found in button: \n{name}\n");
    }

    public void ResetButton()
    {
        _button.colors = _normalColorBlock;
        _button.interactable = false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ActivateButton()
    {
        _button.interactable = true;
    }
}
