using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GPTAPIClient))]
public class ConversationController : MonoBehaviour
{
    private enum Gender
    {
        Male,
        Female,
    }
    
    private enum Language
    {
        Polish,
        English,
    }
    
    [Space(20), Header("Required Components")]
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TMP_Text _outputField;
    [SerializeField] private Button _submitButton;
    [SerializeField] private List<SuggestedQuestionButton> _suggestedQuestionsButtons;

    [Space(20), Header("Player Settings")]
    [SerializeField] private Gender _playerGender;

    [Space(20), Header("AI Settings")]
    [SerializeField] private Language _aiLanguage;
    [SerializeField] private string _aiName;
    [SerializeField] private int _aiAge;
    [SerializeField] private Gender _aiGender;
    [SerializeField, Space(10), TextArea(1, 10)] private string _aiPersonality;
    [SerializeField, Space(10), TextArea(1, 10)] private string _aiStory;
    [SerializeField, Space(20), TextArea(1, 20)] private string _rulesOfEngagement;

    [Space(20), Header("Visuals")]
    [SerializeField] private Image _avatar;
    [SerializeField] private Image _background;
    [SerializeField] private List<Sprite> _avatars;
    [SerializeField] private List<Sprite> _backgrounds;

    private string _characterBuildingPrompt;
    
    private GPTAPIClient _client;

    private void Awake()
    {
        _client = GetComponent<GPTAPIClient>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return) && _inputField.text != string.Empty)
        {
            RequestResponse(_inputField.text);
        }
    }

    private void Start()
    {
        SetVisuals();
        ResetButtons();
        ResetFields();
        BuildCharacter();
        RequestResponse(_characterBuildingPrompt);
    }

    private void BuildCharacter()
    {
        StringBuilder initialPrompt = new StringBuilder();
        
        initialPrompt.AppendLine(_rulesOfEngagement);
        initialPrompt.AppendLine($"\nThe language You speak is: {_aiLanguage}\n");
        initialPrompt.AppendLine($"Your name is: {_aiName}\n");
        initialPrompt.AppendLine($"Your age is: {_aiAge.ToString()}\n");
        initialPrompt.AppendLine($"Your gender is: {_aiGender}\n");
        initialPrompt.AppendLine($"Your personality traits are: {_aiPersonality}\n");
        initialPrompt.AppendLine($"Your background story is: {_aiStory}\n");
        initialPrompt.AppendLine($"You are talking to a: {_playerGender}\n");
        initialPrompt.AppendLine("This is all. Build the character and start playing your new role NOW.");
        
        _characterBuildingPrompt = initialPrompt.ToString();
    }

    private void RequestResponse(string prompt, bool hideResponse = false)
    {
        if (prompt == string.Empty) return;
        
        ResetButtons();
        ResetFields();
        
        _client.RequestResponse(prompt, (response) =>
        {
            ProcessResponse(response, hideResponse);
            ActivateButton();
            ActivateInputField();
        });
    }

    private void ProcessResponse(string response, bool hideResponse = false)
    {
        List<string> parts = response.Split("[").ToList();
        
        if (!hideResponse) _outputField.text = parts[0].TrimEnd();

        if (parts.Count <= 1) return;
        
        for (int i = 0; i < 4; i++)
        {
            if (parts[i + 1] == null)
            {
                _suggestedQuestionsButtons[i].Text = string.Empty;
                continue;
            }
            
            int startIndex = parts[i + 1].IndexOf(' ');
            int endIndex = parts[i + 1].IndexOf('?') + 1;
            int length = endIndex - startIndex;
            
            _suggestedQuestionsButtons[i].Text = parts[i + 1].Substring(startIndex, length);
        }
    }

    private void SetVisuals()
    {
        if (_avatars is not { Count: > 0 }) return;
        if (_backgrounds is not { Count: > 0 }) return;
        
        if (_avatar == null) return;
        if (_background == null) return;
        
        _avatar.sprite = _avatars[Random.Range(0, _avatars.Count)];
        _background.sprite = _backgrounds[Random.Range(0, _backgrounds.Count)];
    }

    private void ResetFields()
    {
        _inputField.text = string.Empty;
        _outputField.text = "...";

        _inputField.interactable = false;
    }
    
    private void ActivateInputField()
    {
        _inputField.interactable = true;
        _inputField.ActivateInputField();
    }

    private void ResetButtons()
    {
        if (_suggestedQuestionsButtons is not { Count: > 0 }) return;

        foreach (SuggestedQuestionButton button in _suggestedQuestionsButtons)
        {
            button.ResetButton();
            button.Text = "...";
        }

        if (_submitButton == null) return;
        
        _submitButton.interactable = false;
    }

    private void ActivateButton()
    {
        if (_suggestedQuestionsButtons is not { Count: > 0 }) return;
        
        foreach (var button in _suggestedQuestionsButtons.Where(button => button.Text != "..."))
        {
            button.ActivateButton();
        }
        
        if (_submitButton == null) return;
        
        _submitButton.interactable = true;
    }
    
    // Used by UI buttons
    public void RequestResponseFromInputFieldSendButton() => RequestResponse(_inputField.text);
    public void RequestResponseFromSuggestedQuestionButton(SuggestedQuestionButton button) => 
        RequestResponse(button.Text);
}
