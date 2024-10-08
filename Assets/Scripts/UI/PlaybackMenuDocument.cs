using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class PlaybackMenuDocument : MonoBehaviour
{
    public UIDocument playbackMenuDocument;
    public PlayableDirector playableDirector;
    
    private Slider _slider;
    private Toggle _playButton;
    private Label _chapterTitle;
    private Label _sectionTitle;
    private Button _returnButton;

    private void Awake()
    {
        playbackMenuDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _playButton = playbackMenuDocument.rootVisualElement.Q<Toggle>("PlayButton");
        _slider = playbackMenuDocument.rootVisualElement.Q<Slider>("Slider");
        _chapterTitle = playbackMenuDocument.rootVisualElement.Q<Label>("ChapterTitle");
        _sectionTitle = playbackMenuDocument.rootVisualElement.Q<Label>("SectionTitle");
        _returnButton = playbackMenuDocument.rootVisualElement.Q<Button>("ReturnButton");
        
        _chapterTitle.text = MenuController.Instance.currentSelectedChapter.chapterName;
        _sectionTitle.text = MenuController.Instance.currentSelectedSection.title;

        _returnButton.clicked += OnReturnButtonClicked;
        _slider.RegisterValueChangedCallback(OnSliderValueChanged);
        _slider.RegisterCallback<PointerDownEvent>(OnSliderPointerDown);
        _playButton.RegisterValueChangedCallback(OnPlayButtonValueChanged);
        
        PrepareTutorial(MenuController.Instance.currentSelectedSection);
    }

    private void OnDisable()
    {
        _returnButton.clicked -= OnReturnButtonClicked;
        _slider.UnregisterValueChangedCallback(OnSliderValueChanged);
        _slider.UnregisterCallback<PointerDownEvent>(OnSliderPointerDown);
        _playButton.UnregisterValueChangedCallback(OnPlayButtonValueChanged);
        
        if(playableDirector)
            playableDirector.Stop();
    }

    public void Update()
    {
        if (playableDirector.state == PlayState.Playing)
        {
            _slider.value = (float) (playableDirector.time / playableDirector.duration);
        }
    }
    
    private void OnSliderValueChanged(ChangeEvent<float> evt)
    {
        playableDirector.time = playableDirector.duration * evt.newValue;
        playableDirector.Evaluate();
    }

    private void OnSliderPointerDown(PointerDownEvent evt)
    {
        _playButton.value = false;
        playableDirector.Pause();
    }
    
    private void OnPlayButtonValueChanged(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            _slider.pickingMode = PickingMode.Position;
            playableDirector.Play();
        }
        else
        {
            _slider.pickingMode = PickingMode.Ignore;
            playableDirector.Pause();
        }
    }
    
    void OnReturnButtonClicked()
    {
        MenuController.Instance.ShowTutorialMenu();
        playableDirector.Stop();
    }

    void PrepareTutorial(SectionData data)
    {
        playableDirector.playableAsset = data.timelineAsset;
        playableDirector.time = 0;
        
        _slider.value = 0;
        
        if(_slider.pickingMode == PickingMode.Ignore)
        {
            _slider.pickingMode = PickingMode.Position;
            _playButton.pickingMode = PickingMode.Position;
        }
    }
}
