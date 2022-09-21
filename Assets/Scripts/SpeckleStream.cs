using System.Collections.Generic;
using Speckle.ConnectorUnity;
using Speckle.Core.Credentials;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Stream = Speckle.Core.Api.Stream;
using Streams = Speckle.ConnectorUnity.Streams;

public class SpeckleStream : MonoBehaviour
{
    
    public TMP_Text selectStreamText;
    public TMP_Text detailsStreamText;
    public TMP_Dropdown streamSelectionDropdown;
    public Button addReceiverBtn;
    public Toggle autoReceiveToggle;
    public Button addSenderBtn;
    public GameObject streamPanel;
    public Canvas streamsCanvas;
    
    private List<Stream> _streamList = null;
    private Stream _selectedStream = null;
    private List<GameObject> _streamPanels = new List<GameObject>();
    
    // Start is called before the first frame update
    async void Start()
    {
        if (selectStreamText == null || streamSelectionDropdown == null)
        {
            Debug.Log("Please set all input fields on _SpeckleStream");
            return;
        }

        var defaultAccount = AccountManager.GetDefaultAccount();
        if (defaultAccount == null)
        {
            Debug.Log("Please set a default account in SpeckleManager");
            return;
        }

        selectStreamText.text = $"Select a stream on {defaultAccount.serverInfo.name}:";

        _streamList = await Streams.List(30);
        if (!_streamList.Any())
        {
            Debug.Log("There are no streams in your account, please create one online.");
            return;
        }

        streamSelectionDropdown.options.Clear();
        foreach (var stream in _streamList)
        {
            streamSelectionDropdown.options.Add(new TMP_Dropdown.OptionData(stream.name + " - " + stream.id));
        }

        streamSelectionDropdown.onValueChanged.AddListener(StreamSelectionChanged);
        //trigger ui refresh, maybe there's a better method
        streamSelectionDropdown.value = -1;
        streamSelectionDropdown.value = 0;


        addReceiverBtn.onClick.AddListener(AddReceiver);
        addSenderBtn.onClick.AddListener(AddSender);
    }

    public void StreamSelectionChanged(int index)
    {
      if (index == -1)
        return;

      _selectedStream = _streamList[index];
      detailsStreamText.text =
        $"Description: {_selectedStream.description}\n" +
        $"Link sharing on: {_selectedStream.isPublic}\n" +
        $"Role: {_selectedStream.role}\n" +
        $"Collaborators: {_selectedStream.collaborators.Count}\n" +
        $"Id: {_selectedStream.id}";
    }

    // Shows how to create a new Receiver from code and then pull data manually
    // Created receivers are added to a List of Receivers for future use
    private async void AddReceiver()
    {
      var autoReceive = autoReceiveToggle.isOn;
      var stream = await Streams.Get(_selectedStream.id, 30);

      var streamPrefab = Instantiate(streamPanel, new Vector3(0, 0, 0),
        Quaternion.identity);

      //set position
      streamPrefab.transform.SetParent(streamsCanvas.transform);
      var rt = streamPrefab.GetComponent<RectTransform>();
      rt.anchoredPosition = new Vector3(-10, -260 - _streamPanels.Count * 110, 0);

      streamPrefab.AddComponent<InteractionLogic>().InitReceiver(stream, autoReceive);

      _streamPanels.Add(streamPrefab);
    }

    private async void AddSender()
    {
      var stream = await Streams.Get(_selectedStream.id, 10);

      var streamPrefab = Instantiate(streamPanel, new Vector3(0, 0, 0),
        Quaternion.identity);

      streamPrefab.transform.SetParent(streamsCanvas.transform);
      var rt = streamPrefab.GetComponent<RectTransform>();
      rt.anchoredPosition = new Vector3(-10, -260 - _streamPanels.Count * 110, 0);

      streamPrefab.AddComponent<InteractionLogic>().InitSender(stream);

      _streamPanels.Add(streamPrefab);
    }

    public void RemoveStreamPrefab(GameObject streamPrefab)
    {
      _streamPanels.RemoveAt(_streamPanels.FindIndex(x => x.name == streamPrefab.name));
      ReorderStreamPrefabs();
    }

    private void ReorderStreamPrefabs()
    {
      for (var i = 0; i < _streamPanels.Count; i++)
      {
        var rt = _streamPanels[i].GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector3(-10, -110 - i * 110, 0);
      }
    }
}
