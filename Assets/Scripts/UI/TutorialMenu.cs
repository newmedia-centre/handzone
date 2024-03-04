using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialMenu : MonoBehaviour
{
    [Header("Video Player")]
    public Toggle playToggle;
    public Slider videoSlider;

    public GameObject playImage;
    public GameObject pauseImage;

    public VideoPlayer videoPlayer;

    [Header("Tutorial list references")]
    public GameObject viewportContenct;
    public GameObject tutorialElementPrefab;

    [Header("Arm animation reference")]
    public Animator armAnimator;

    [Header("Tutorial Data")]
    public TutorialData[] tutorialData;

    private bool _isSliderCodeUpdate = false;

    private void Start()
    {
        FillTutorialScroller();

        videoPlayer.loopPointReached += EndOfVideo;
        videoPlayer.prepareCompleted += PrepareComplete;

        videoSlider.onValueChanged.AddListener(SliderChange);

        armAnimator.speed = 0.0f;
    }

    private void FillTutorialScroller()
    {
        foreach(TutorialData _data in tutorialData)
        {
            GameObject _element = Instantiate(tutorialElementPrefab, viewportContenct.transform);
            Text _text = _element.GetComponentInChildren<Text>();
            _text.text = _data.name;
            
            Button _button = _element.GetComponentInChildren<Button>();
            _button.onClick.AddListener(() => PrepareTutorial(_data));
        }
    }

    private void PrepareTutorial(TutorialData data)
    {
        videoPlayer.clip = data.videoClip;
        videoPlayer.frame = 0;
        videoPlayer.Prepare();

        armAnimator.SetInteger("Tutorial", 1);
        armAnimator.runtimeAnimatorController = data.overrider;
        armAnimator.Play("PlayAnimation", 0, 0);

        _isSliderCodeUpdate = true;
        videoSlider.value = 0;
        _isSliderCodeUpdate = false;
        
        
        if(videoSlider.interactable == false)
        {
            videoSlider.interactable = true;
            playToggle.interactable = true;
        }
    }

    void PrepareComplete(VideoPlayer vp)
    {
        playToggle.isOn = true;
    }

    void EndOfVideo(VideoPlayer vp)
    {
        playToggle.isOn = false;
        videoPlayer.frame = 0;
        armAnimator.Play("PlayAnimation", 0, 0);
        armAnimator.speed = 0;
    }

    public void SliderChange(float value)
    {
        if(!_isSliderCodeUpdate)
        {
            videoPlayer.frame = Convert.ToInt64(videoPlayer.frameCount * value);
            armAnimator.Play("PlayAnimation", 0, value);
        }
    }
    public void Pause_Play(bool active)
    {
        playImage.SetActive(!active);
        pauseImage.SetActive(active);

        if (active)
        {
            videoPlayer.Play();
            armAnimator.speed = 1.0f;
        }
        else
        {
            videoPlayer.Pause();
            armAnimator.speed = 0.0f;
        }       
    }

    public void Update()
    {
        if (videoPlayer.isPlaying)
        {
            _isSliderCodeUpdate = true;
            videoSlider.value = ((float) videoPlayer.frame + 1) / videoPlayer.frameCount;
            _isSliderCodeUpdate = false;
        }
    }
}

[System.Serializable]
public struct TutorialData
{
    public string name;
    public VideoClip videoClip;
    public AnimatorOverrideController overrider;
}
