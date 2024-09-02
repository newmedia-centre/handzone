using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class NetworkVNCCursor : MonoBehaviour
{
    [SerializeField] private Image coloredCursor;
    [SerializeField] private TMP_Text playerName;

    public Color Color
    { 
        set => coloredCursor.color = value;
    }

    public string PlayerNameLabel
    {
        set => playerName.text = value;
    }
}
