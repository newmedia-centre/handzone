// Copyright 2024 NewMedia Centre - Delft University of Technology
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#region

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#endregion

public class TutorialMenuDocument : MonoBehaviour
{
    public UIDocument tutorialMenuDocument;
    public List<ChapterData> chapterData;
    private VisualElement _chapterGroup;
    private Label _sectionTitle;
    private Label _sectionDescription;
    private Button _startLessonButton;
    private Button _returnButton;

    public void OnEnable()
    {
        _chapterGroup = tutorialMenuDocument.rootVisualElement.Q<VisualElement>("ChapterGroup");
        _sectionTitle = tutorialMenuDocument.rootVisualElement.Q<Label>("SectionTitle");
        _sectionDescription = tutorialMenuDocument.rootVisualElement.Q<Label>("SectionDescription");
        _startLessonButton = tutorialMenuDocument.rootVisualElement.Q<Button>("StartLessonButton");
        _returnButton = tutorialMenuDocument.rootVisualElement.Q<Button>("ReturnButton");

        if (_chapterGroup == null || _sectionTitle == null || _sectionDescription == null ||
            _startLessonButton == null ||
            _returnButton == null) Debug.LogError("TutorialMenuDocument: One or more UI elements are not found.");

        _startLessonButton.clicked += OnStartLessonButtonClicked;
        _returnButton.clicked += () => OnReturnButtonClicked();

        _chapterGroup.Clear(); // Clear previous elements to avoid duplication

        foreach (var chapterData in chapterData)
        {
            var serializedObject = new SerializedObject(chapterData);
            var chapterNameProperty = serializedObject.FindProperty("chapterName");

            var chapterFoldout = AssetDatabase
                .LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/TutorialMenu/ChapterFoldout.uxml").CloneTree();
            var foldout = chapterFoldout.Q<Foldout>("ChapterFoldout");
            var foldoutContent = chapterFoldout.Q<VisualElement>("unity-content");

            foldout.text = chapterNameProperty.stringValue;

            _chapterGroup.Add(chapterFoldout);

            foreach (var sectionData in chapterData.sections)
            {
                var sectionButton = AssetDatabase
                    .LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/TutorialMenu/SectionButton.uxml").CloneTree();
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

    private void OnReturnButtonClicked()
    {
        MenuController.Instance.ChangeMenu(MenuName.Main);
    }

    private void OnDisable()
    {
        _startLessonButton.clicked -= OnStartLessonButtonClicked;
        _returnButton.clicked -= OnReturnButtonClicked;
    }

    private void OnStartLessonButtonClicked()
    {
        if (MenuController.Instance.currentSelectedSection != null)
        {
            Debug.Log($"Starting lesson for {MenuController.Instance.currentSelectedSection.title}");
            StartCoroutine(LoadTutorialSceneAndChangeMenu());
        }
    }

    private IEnumerator LoadTutorialSceneAndChangeMenu()
    {
        yield return StartCoroutine(Utility.LoadSceneCoroutine("Tutorial"));

        // Change the menu to Playback after the scene has loaded
        MenuController.Instance.ChangeMenu(MenuName.Playback);
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