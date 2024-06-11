using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class TutorialMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public Toggle playToggle;
    public Slider videoSlider;
    
    [Header("Director")]
    public PlayableDirector director;
    
    [Header("Images")]
    public GameObject playImage;
    public GameObject playBackground;
    public GameObject pauseImage;

    private bool buttonState = false;

    [Header("Tutorial list references")]
    public GameObject viewportContent;
    public GameObject listElementPrefab;

    private bool _isSliderCodeUpdate = false;

    public static bool CustomDirectorChange = false;

    private void Awake()
    {
        videoSlider.onValueChanged.AddListener(SliderChange);
        
        director.stopped += TimelineEnd;
    }

    public void Enter(TutorialData[] tutorialData)
    {
        FillTutorialScroller(tutorialData);
    }

    public void Exit()
    {
        ChangeButton(false);

        if (director.state == PlayState.Playing)
        {
            director.Stop();
        }
        
        for (int i = viewportContent.transform.childCount; i > 0; i--)
        {
            Destroy(viewportContent.transform.GetChild(i - 1).GameObject());
        }

        director.playableAsset = null;
        videoSlider.interactable = false;
        playToggle.interactable = false;
    }

    private void FillTutorialScroller(TutorialData[] tutorialData)
    {
        foreach (TutorialData _data in tutorialData)
        {
            GameObject _element = Instantiate(listElementPrefab, viewportContent.transform);
            TMP_Text _text = _element.GetComponentInChildren<TMP_Text>();
            _text.text = _data.name;

            Button _button = _element.GetComponentInChildren<Button>();
            _button.onClick.AddListener(() => PrepareTutorial(_data));
        }
    }

    private void PrepareTutorial(TutorialData data)
    {
        director.playableAsset = data.timeline;
        director.time = 0;

        _isSliderCodeUpdate = true;
        videoSlider.value = 0;
        _isSliderCodeUpdate = false;
        
        if(videoSlider.interactable == false)
        {
            videoSlider.interactable = true;
            playToggle.interactable = true;
        }
        
        ChangeButton(false);
    }

    public void SliderChange(float value)
    {
        if(!_isSliderCodeUpdate)
        {
            director.time = director.duration * value;

            CustomDirectorChange = true;
            director.Evaluate();
            CustomDirectorChange = false;
        }
    }

    public void TimelineEnd(PlayableDirector playableDirector)
    {
        ChangeButton(false);
    }
    
    public void ChangeButton(bool play)
    {
        playImage.SetActive(!play);
        playBackground.SetActive(play);
        pauseImage.SetActive(play);

        buttonState = play;
    }

    public void OnButtonClick()
    {
        if (buttonState)
        {
            ChangeButton(false);
            director.Pause();
        }
        else
        {
            ChangeButton(true);
            director.Play();
        }
    }

    public void FixedUpdate()
    {
        if (director.state == PlayState.Playing)
        {
            _isSliderCodeUpdate = true;
            videoSlider.value = (float) (director.time / director.duration);
            _isSliderCodeUpdate = false;
        }
    }
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TutorialMenuData", order = 1)]
public class TutorialMenuData : ScriptableObject
{
    public TutorialData[] data;
}

[System.Serializable]
public struct TutorialData
{
    public string name;
    public TimelineAsset timeline;
}



