using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterMenuController : MonoBehaviour
{
    [Header("Assets")]
    public GameObject nodePrefab;
    // public ChapterMenuData data;
    public Sprite boxSprite;
    
    [Header("GraphReference")]
    public GameObject graphContainer;
    private float _graphContainerWidth;
    private float _graphContainerHeight;

    [Header("OtherScreenControllers")]
    public TutorialMenuController tutorialController;
    public TaskMenuController taskController;
    
    [Header("ChapterDisplayReferences")]
    public TMP_Text chapterTitle;
    public TMP_Text chapterDescriptions;

    [HideInInspector]
    public ChapterData currentChapterData;

    private const float PADDING = 12.5f;
    private const float SIZE_BETWEEN_NODES = 75;
    public void Awake()
    {
        _graphContainerHeight = graphContainer.GetComponent<RectTransform>().rect.height;
        _graphContainerWidth = graphContainer.GetComponent<RectTransform>().rect.width;
        // GenerateGraph();
    }

    // public void GenerateGraph()
    // {
    //     ChapterData _initialNodeData;
    //
    //     foreach(ChapterData _chapterData in data.data)
    //     {
    //         if(_chapterData.prerequisites.Length == 0)
    //         {
    //             _initialNodeData = _chapterData;
    //             goto Found;
    //         }
    //     }
    //
    //     Debug.LogError("Cannot make graph, no initial node given.");
    //     return;
    //
    //     Found:
    //     List<ChapterData> _dataList = data.data.ToList();
    //     Dictionary<string, GameObject> _placedNodes = new Dictionary<string, GameObject>();
    //     List<ChapterData> _currentLevelData = new List<ChapterData>();
    //     int _maxNodeCount = _dataList.Count;
    //
    //     _dataList.Remove(_initialNodeData);
    //
    //     GameObject _initialNode = AddNode(_initialNodeData, 0, 1, 1);
    //
    //     _placedNodes.Add(_initialNodeData.name, _initialNode);
    //
    //     foreach (ChapterData _chapterData in _dataList)
    //     {
    //         if (PreviousChaptersDone(_chapterData, _placedNodes))
    //         {
    //             _currentLevelData.Add(_chapterData);
    //         }
    //     }
    //
    //     int _levelIndex = 0;
    //
    //     while (_placedNodes.Count < _maxNodeCount && _currentLevelData.Count > 0)
    //     {
    //         _levelIndex++;
    //
    //         int _widthIndex = 0;
    //         
    //         foreach(ChapterData _chapterData in _currentLevelData)
    //         {
    //             _widthIndex++;
    //             GameObject _node = AddNode(_chapterData, _levelIndex, _widthIndex, _currentLevelData.Count);
    //
    //             foreach(string _prerequisite in _chapterData.prerequisites)
    //             {
    //                 AddConnection(_node, _placedNodes[_prerequisite]);
    //             }
    //
    //             _placedNodes.Add(_chapterData.name, _node);
    //             _dataList.Remove(_chapterData);
    //         }
    //
    //         _currentLevelData.Clear();
    //
    //         foreach (ChapterData _chapterData in _dataList)
    //         {
    //             if (PreviousChaptersDone(_chapterData, _placedNodes))
    //             {
    //                 _currentLevelData.Add(_chapterData);
    //             }
    //         }
    //     }
    //
    //     if (_dataList.Count > 0)
    //     {
    //         Debug.LogError("Could not get show all chapters since a part of the tree is disconnected. Please check the prerequisites for each of the chapters, since you either are missing, have too many prerequisites, or misspelled a prerequisite.");
    //         Debug.LogError("The following nodes could not be placed:");
    //         
    //         foreach (ChapterData _chapterData in _dataList)
    //         {
    //             Debug.LogError(_chapterData.name);
    //         }
    //     }
    // }

    private bool PreviousChaptersDone(ChapterData chapterData, Dictionary<string, GameObject> placedNodes)
    {
        // foreach (String _prerequisite in chapterData.prerequisites)
        // {
        //     if (!placedNodes.ContainsKey(_prerequisite))
        //     {
        //         return false;
        //     }
        // }

        return true;
    }

    private GameObject AddNode(ChapterData chapterData, int level, int index, int totalLevelSize)
    {
        GameObject _node = Instantiate(nodePrefab, graphContainer.transform);

        Button _button = _node.GetComponent<Button>();
        _button.onClick.AddListener(() => GoToChapter(chapterData));
        
        TMP_Text _text = _node.GetComponentInChildren<TMP_Text>();
        _text.text = chapterData.name;

        float _height = _graphContainerHeight - PADDING - (level * SIZE_BETWEEN_NODES);
        float _width = (_graphContainerWidth / (totalLevelSize + 1)) * index;
        _node.GetComponent<RectTransform>().anchoredPosition = new Vector3(_width, _height);
        
        return _node;
    }

    private void AddConnection(GameObject to, GameObject from)
    {
        GameObject _connection = new GameObject("connection");
        
        Image _newImage = _connection.AddComponent<Image>();
        _newImage.sprite = boxSprite;
        
        RectTransform _rec = _connection.GetComponent<RectTransform>();
        _rec.SetParent(graphContainer.transform, false);
        _rec.anchoredPosition = (from.transform.localPosition + to.transform.localPosition) / 2;
        _rec.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (from.transform.localPosition - to.transform.localPosition).magnitude);
        _rec.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 5);

        _connection.transform.localRotation = Quaternion.Euler(0, 0, Vector3.SignedAngle(Vector3.up, (to.transform.localPosition - from.transform.localPosition), Vector3.forward));
        _connection.transform.SetSiblingIndex(0);
        _connection.SetActive(true);
    }

    public void GoToChapter(ChapterData chapterData)
    {
        chapterTitle.text = chapterData.name;
        // chapterDescriptions.text = chapterData.description;
        currentChapterData = chapterData;
    }

    public void GoToTasks()
    {
        
    }
}

// [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ChapterMenuData", order = 1)]
// public class ChapterMenuData : ScriptableObject
// {
//     public ChapterData[] data;
// }
//
// [System.Serializable]
// public struct ChapterData
// {
//     public string name;
//     public string description;
//     public TutorialMenuData tutorialData;
//     public TaskMenuData tasks;
//     public string[] prerequisites; 
// }
