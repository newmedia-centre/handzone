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
    
    public void Start()
    {
        var chapterGroup = tutorialMenuDocument.rootVisualElement.Q<VisualElement>("ChapterGroup");

        foreach (var chapterData in chapterData)
        {
            var chapterFoldout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/ChapterFoldout.uxml").CloneTree();
            var serializedObject = new SerializedObject(chapterData);
            var chapterNameProperty = serializedObject.FindProperty("chapterName");

            var foldout = chapterFoldout.Q<Foldout>("ChapterFoldout");
            foldout.text = chapterNameProperty.stringValue;
            
            chapterGroup.Add(chapterFoldout);
            
            var contentGroup = chapterFoldout.Q<VisualElement>("unity-content");

            foreach (var sectionData in chapterData.sections)
            {
                var sectionButton = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/SectionButton.uxml").CloneTree();
                var sectionSerializedObject = new SerializedObject(sectionData);
                var sectionNameProperty = sectionSerializedObject.FindProperty("sectionName");
                var sectionCompletedProperty = sectionSerializedObject.FindProperty("sectionCompleted");

                var sectionButtonElement = sectionButton.Q<Button>("SectionButton");
                var sectionToggleElement = sectionButtonElement.Q<Toggle>("SectionToggle");
                
                sectionButtonElement.text = sectionNameProperty.stringValue;
                sectionToggleElement.BindProperty(sectionCompletedProperty);
                
                sectionToggleElement.RegisterValueChangedCallback(evt =>
                {
                    sectionData.sectionCompleted = evt.newValue;
                    UpdateProgressBar();
                });
                
                sectionButtonElement.clicked += () =>
                {
                    // TODO: Set current section and load the data
                };
                
                contentGroup.Add(sectionButton);
            }
        }
        
    }

    private void UpdateProgressBar()
    {
        // Find the total number of sections and the number of completed sections
        float totalSections = chapterData.Sum(chapter => chapter.sections.Count);
        float completedSections = chapterData.Sum(chapter => chapter.sections.Count(section => section.sectionCompleted));
        
        var progressBar = tutorialMenuDocument.rootVisualElement.Q<ProgressBar>("ChapterProgress");
        var progressValue = completedSections / totalSections * 100;
        progressBar.value = progressValue;
        progressBar.title = $"{completedSections}/{totalSections} Sections Completed";
        
        // Set visibility based on progress value
        progressBar.style.display = progressValue == 0 ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
