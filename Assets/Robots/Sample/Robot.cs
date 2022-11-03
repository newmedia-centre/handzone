using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Robots.Samples.Unity
{
    public class Robot : MonoBehaviour
    {
        public PlaybackPanel playbackPanel;
        
        [SerializeField]
        #nullable enable
        private Material? _material;
        private Program? _program;
        private UnityMeshPoser? _meshPoser;
        private bool isPlaying = false;

        void Update()
        {
            if (_program is null)
                return;

            if (isPlaying)
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

            playbackPanel.playButton.onClick.AddListener(delegate { SetPlayback(true); });
            playbackPanel.pauseButton.onClick.AddListener(delegate { SetPlayback(false); });
            playbackPanel.slider.onValueChanged.AddListener(value => SetPlaybackTime(value));
            playbackPanel.slider.maxValue = (float)_program.Duration;
        }

        void SetPlayback(bool value)
        {
            isPlaying = value;
            Debug.Log("blup");
        }

        void SetPlaybackTime(float time)
        {
            if (_program is null)
                return;
            
            _program.Animate(time, false);
        }
    }
}