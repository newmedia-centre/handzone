using Schema.Socket.Index;
using UnityEngine;
using UnityEngine.UI;

public class SessionButton : MonoBehaviour
{
    private Button _button;
    private string _sessionAddress;
    private Color _originalColor;
    
    public void Start()
    {
        _button = GetComponent<Button>();
        
        // Store the original color of the button
        _originalColor = _button.colors.normalColor;
        
        // Add listener to button to send selected session
        _button.onClick.AddListener(() =>
        {
            // Set selected session
            SessionMenu.OnSessionSelected.Invoke(transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text);
            Select();
        });
    }
    
    public void SetButton(RobotSession session)
    {
        _sessionAddress = session.Address;
        
        // Set button text to session name
        transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = _sessionAddress;
    }

    public void Select()
    {
        // Make the button appear selected
        _button.GetComponent<Image>().color = _originalColor * 0.74f;
    }
    
    public void Deselect()
    {
        _button.GetComponent<Image>().color = _originalColor;
    }
}
