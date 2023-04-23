using System.Text;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldWrapper : MonoBehaviour
{
    [SerializeField] private int _maxLineLength;
    
    private TMP_InputField _inputField;
    private string _currentLine;

    private void Start()
    {
        _inputField = GetComponent<TMP_InputField>();

        if (_inputField == null) return;
        
        _inputField.onValueChanged.AddListener(delegate { UpdateWrappedText(); });
        _currentLine = "";
    }

    private void OnValidate()
    {
        if (_inputField != null) return;
        
        _inputField = GetComponent<TMP_InputField>();
    }

    private void UpdateWrappedText()
    {
        string lastChar = _inputField.text.Length > 0 ? _inputField.text[^1].ToString() : "";
        if (lastChar == " ")
        {
            _currentLine += lastChar;
            _inputField.text = _inputField.text[..^1] + WrapText(_currentLine, _maxLineLength);
            _currentLine = "";
        }
        else
        {
            _currentLine += lastChar;
        }
    }

    private string WrapText(string inputText, int lineLength)
    {
        string[] words = inputText.Split(' ');
        StringBuilder wrappedText = new StringBuilder();
        string currentLine = "";

        foreach (string word in words)
        {
            if ((currentLine.Length + word.Length + 1) > lineLength)
            {
                wrappedText.Append(currentLine + "\n");
                currentLine = "";
            }
            
            currentLine += word + " ";
        }

        if (currentLine.Length > 0)
        {
            wrappedText.Append(currentLine);
        }
        
        return wrappedText.ToString().TrimEnd();
    }
}
