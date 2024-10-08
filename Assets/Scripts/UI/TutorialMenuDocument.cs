using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialMenuDocument : MonoBehaviour
{
    public UIDocument tutorialMenuDocument;
    public List<ChapterData> chapterData;
    VisualElement _chapterGroup;
    Label _sectionTitle;
    Label _sectionDescription;
    Button _startLessonButton;
    
    public void OnEnable()
    {
        _chapterGroup = tutorialMenuDocument.rootVisualElement.Q<VisualElement>("ChapterGroup");
        _sectionTitle = tutorialMenuDocument.rootVisualElement.Q<Label>("SectionTitle");
        _sectionDescription = tutorialMenuDocument.rootVisualElement.Q<Label>("SectionDescription");
        _startLessonButton = tutorialMenuDocument.rootVisualElement.Q<Button>("StartLessonButton");

        _startLessonButton.clicked += OnStartLessonButtonClicked;
        
        _chapterGroup.Clear(); // Clear previous elements to avoid duplication
            
        foreach (var chapterData in chapterData)
        {
            var serializedObject = new SerializedObject(chapterData);
            var chapterNameProperty = serializedObject.FindProperty("chapterName");

            var chapterFoldout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/TutorialMenu/ChapterFoldout.uxml").CloneTree();
            var foldout = chapterFoldout.Q<Foldout>("ChapterFoldout");
            var foldoutContent = chapterFoldout.Q<VisualElement>("unity-content");
            
            foldout.text = chapterNameProperty.stringValue;
            
            _chapterGroup.Add(chapterFoldout);

            foreach (var sectionData in chapterData.sections)
            {
                var sectionButton = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/TutorialMenu/SectionButton.uxml").CloneTree();
                var sectionSerializedObject = new SerializedObject(sectionData);
                var sectionNameProperty = sectionSerializedObject.FindProperty("title");
                var sectionDescriptionProperty = sectionSerializedObject.FindProperty("description");
                var sectionCompletedProperty = sectionSerializedObject.FindProperty("completed");

                var sectionButtonElement = sectionButton.Q<Button>("SectionButton");
                var sectionToggleElement = sectionButtonElement.Q<Toggle>("SectionToggle");
                
                sectionButtonElement.text = sectionNameProperty.stringValue;
                sectionToggleElement.BindProperty(sectionCompletedProperty);
                
                sectionToggleElement.RegisterValueChangedCallback(evt =>
                {
                    sectionData.completed = evt.newValue;
                    UpdateProgressBar();
                });
                
                sectionButtonElement.clicked += () =>
                {
                    MenuController.Instance.OnChapterSelected?.Invoke(chapterData);
                    MenuController.Instance.OnSectionSelected?.Invoke(sectionData);
                    _sectionTitle.text = sectionNameProperty.stringValue;
                    _sectionDescription.text = sectionDescriptionProperty.stringValue;
                };
                
                foldoutContent.Add(sectionButton);
            }
        }
    }

    private void OnDisable()
    {
        _startLessonButton.clicked -= OnStartLessonButtonClicked;
    }
    
    private void OnStartLessonButtonClicked()
    {
        if (MenuController.Instance.currentSelectedSection != null)
        {
            Debug.Log($"Starting lesson for {MenuController.Instance.currentSelectedSection.title}");
            MenuController.Instance.OnLessonStarted?.Invoke();
        }
    }

    private void UpdateProgressBar()
    {
        // Find the total number of sections and the number of completed sections
        float totalSections = chapterData.Sum(chapter => chapter.sections.Count);
        float completedSections = chapterData.Sum(chapter => chapter.sections.Count(section => section.completed));
        
        var progressBar = tutorialMenuDocument.rootVisualElement.Q<ProgressBar>("ChapterProgress");
        var progressValue = completedSections / totalSections * 100;
        progressBar.value = progressValue;
        progressBar.title = $"{completedSections}/{totalSections} Sections Completed";
        
        // Set visibility based on progress value
        progressBar.style.display = progressValue == 0 ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
