using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Robots.Samples.Unity
{
    public class Robot : MonoBehaviour
    {
        public PlaybackPanel[] playbackPanels;
        
        [SerializeField]
        #nullable enable
        private Material? _material;
        private Program? _program;
        private UnityMeshPoser? _meshPoser;
        private bool _isPlaying;
        private int _programDuration;

        private void Start()
        {
            foreach (var playbackPanel in playbackPanels)
            {
                playbackPanel.playButton.onClick.AddListener(PlayPlayback);
                playbackPanel.pauseButton.onClick.AddListener(PausePlayback);
                playbackPanel.sliderPanel.slider.onValueChanged.AddListener(value => SetPlaybackTime(value));
                
            }
        }

        void Update()
        {
            if (_program is null)
                return;

            if (_isPlaying)
            {
                var time = Mathf.PingPong(Time.time, (float)_program.Duration);
                _program.Animate(time, false);
            }
        }

        public async void CreateProgramFromJSON(string json)
        {
            _program = await GrasshopperSyncProgram.CreateAsync(json);

            if (_material == null)
                throw new ArgumentNullException(nameof(_material));

            if(_meshPoser == null)
                _meshPoser = new UnityMeshPoser(_program.RobotSystem, _material);
            
            _program.MeshPoser = _meshPoser;

            foreach (var playbackPanel in playbackPanels)
            {
                _programDuration = (int)_program.Duration;
                playbackPanel.sliderPanel.slider.maxValue = _programDuration;
                playbackPanel.sliderPanel.value.text = "0:" + _programDuration.ToString();
            }
        }

        void PlayPlayback()
        {
            _isPlaying = true;
        }
        void PausePlayback()
        {
            _isPlaying = false;
        }

        void SetPlaybackTime(float time)
        {
            if (_program is null)
                return;

            _program.Animate(time, false);
            
            foreach (var playbackPanel in playbackPanels)
            {
                playbackPanel.sliderPanel.value.text = (int)time + ":" + _programDuration;
            }
        }
    }
}