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
    public ChapterMenuData data;
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
    public void Awake()
    {
        _graphContainerHeight = graphContainer.GetComponent<RectTransform>().rect.height;
        _graphContainerWidth = graphContainer.GetComponent<RectTransform>().rect.width;
        generateGraph();
    }

    public void generateGraph()
    {
        ChapterData _initialNodeData;

        foreach(ChapterData _chapterData in data.data)
        {
            if(_chapterData.prerequisites.Length == 0)
            {
                _initialNodeData = _chapterData;
                goto Found;
            }
        }

        Debug.LogError("Cannot make graph, no initial node given.");
        return;

        Found:
        List<ChapterData> _dataList = data.data.ToList();
        Dictionary<string, GameObject> _placedNodes = new Dictionary<string, GameObject>();
        List<ChapterData> _currentLevelData = new List<ChapterData>();
        int maxNodeCount = _dataList.Count;

        _dataList.Remove(_initialNodeData);

        GameObject _node = AddNode(_initialNodeData, 0, 1, 1);

        _placedNodes.Add(_initialNodeData.name, _node);

        foreach(ChapterData _chapterData in _dataList)
        {
            foreach(string prerequisite in _chapterData.prerequisites)
            {
                if (!_placedNodes.ContainsKey(prerequisite))
                {
                    goto outerContinue;
                }
            }

            _currentLevelData.Add(_chapterData);

        outerContinue:
            continue;
        }

        int levelIndex = 0;

        while (_placedNodes.Count < maxNodeCount && _currentLevelData.Count > 0)
        {
            levelIndex++;

            int widthIndex = 0;
            
            foreach(ChapterData _chapterData in _currentLevelData)
            {
                widthIndex++;
                GameObject node = AddNode(_chapterData, levelIndex, widthIndex, _currentLevelData.Count);

                foreach(string prerequisite in _chapterData.prerequisites)
                {
                    AddConnection(node, _placedNodes[prerequisite]);
                }

                _placedNodes.Add(_chapterData.name, node);
                _dataList.Remove(_chapterData);
            }

            _currentLevelData.Clear();

            foreach(ChapterData _chapterData in _dataList)
            {
                foreach(string prerequisite in _chapterData.prerequisites)
                {
                    if (!_placedNodes.ContainsKey(prerequisite))
                    {
                        goto outerContinue;
                    }
                }

                _currentLevelData.Add(_chapterData);

            outerContinue:
                continue;
            }
        }

        if (_dataList.Count > 0)
        {
            Debug.LogError("Could not get show all chapters since a part of the tree is disconnected. Please check the prerequisites for each of the chapters, since you either are missing, have too many prerequisites, or misspelled a prerequisite.");
            Debug.LogError("The following nodes could not be placed:");
            
            foreach (ChapterData _chapterData  in _dataList)
            {
                Debug.LogError(_chapterData.name);
            }
        }
    }

    public GameObject AddNode(ChapterData chapterData, int level, int index, int totalLevelSize)
    {
        GameObject _node = Instantiate(nodePrefab, graphContainer.transform);

        Button _button = _node.GetComponent<Button>();
        _button.onClick.AddListener(() => GoToChapter(chapterData));
        
        TMP_Text _text = _node.GetComponentInChildren<TMP_Text>();
        _text.text = chapterData.name;

        float height = _graphContainerHeight - 12.5f - (level * 75);
        float width = (_graphContainerWidth / (totalLevelSize + 1)) * index;
        _node.GetComponent<RectTransform>().anchoredPosition = new Vector3(width, height);
        
        return _node;
    }

    public void AddConnection(GameObject to, GameObject from)
    {
        GameObject connection = new GameObject("connection");
        
        Image newImage = connection.AddComponent<Image>();
        newImage.sprite = boxSprite;
        
        RectTransform rec = connection.GetComponent<RectTransform>();
        rec.SetParent(graphContainer.transform, false);
        rec.anchoredPosition = (from.transform.localPosition + to.transform.localPosition) / 2;
        rec.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (from.transform.localPosition - to.transform.localPosition).magnitude);
        rec.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 5);

        connection.transform.localRotation = Quaternion.Euler(0, 0, Vector3.SignedAngle(Vector3.up, (to.transform.localPosition - from.transform.localPosition), Vector3.forward));
        connection.transform.SetSiblingIndex(0);
        connection.SetActive(true);
    }

    public void GoToChapter(ChapterData chapterData)
    {
        chapterTitle.text = chapterData.name;
        chapterDescriptions.text = chapterData.description;
        currentChapterData = chapterData;
    }

    public void GoToTasks()
    {
        
    }
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ChapterMenuData", order = 1)]
public class ChapterMenuData : ScriptableObject
{
    public ChapterData[] data;
}

[System.Serializable]
public struct ChapterData
{
    public string name;
    public string description;
    public TutorialMenuData tutorialData;
    public TaskMenuData tasks;
    public string[] prerequisites; 
}
