using Schema.Socket.Unity;
using TMPro;
using UnityEngine;

public class NetworkVNCCursor : MonoBehaviour
{
    [SerializeField] private Renderer coloredCursor;
    [SerializeField] private TMP_Text playerName;
    
    public Color Color
    { 
        set => coloredCursor.material.color = value;
    }

    public string PlayerNameLabel
    {
        set => playerName.SetText(value);
    }
}
