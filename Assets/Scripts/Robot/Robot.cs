using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Robots.Samples.Unity
{
    public class Robot : MonoBehaviour
    {
        public PlaybackPanel[] playbackPanels;
        
        public string gripperLoadedName;
        public string gripperUnloadedName;
        public GameObject toolPrefab;

        [SerializeField]
        #nullable enable
        private Material? _material;
        private Program? _program;
        private UnityMeshPoser? _meshPoser;
        private UnityMeshPoser? _toolMeshPoser;
        private bool _isPlaying;
        private int _programDuration;
        private int _currentTargetIndex;
        private bool isLoaded;

        private void OnEnable()
        {
            RobotActions.OnGripperLoaded += LoadGripper;
            RobotActions.OnGripperUnloaded += UnloadGripper;
        }
        
        private void OnDisable()
        {
            RobotActions.OnGripperLoaded -= LoadGripper;
            RobotActions.OnGripperUnloaded -= UnloadGripper;
        }

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
                UpdateProgramTargetState(_program.CurrentSimulationPose.TargetIndex);
            }
        }

        public async void CreateProgramFromJSON(string json)
        {
            
            _program = await GrasshopperSyncProgram.CreateAsync(json);

            if (_material == null)
                throw new ArgumentNullException(nameof(_material));

            if(_meshPoser == null)
                _meshPoser = new UnityMeshPoser(_program.RobotSystem, _material, toolPrefab);

            _program.MeshPoser = _meshPoser;

            foreach (var playbackPanel in playbackPanels)
            {
                _programDuration = (int)_program.Duration;
                playbackPanel.sliderPanel.slider.maxValue = _programDuration;
                playbackPanel.sliderPanel.value.text = "0-" + _programDuration.ToString();
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
            if (_program is null || _isPlaying)
                return;

            _program.Animate(time, false);
            UpdateProgramTargetState(_program.CurrentSimulationPose.TargetIndex);

            foreach (var playbackPanel in playbackPanels)
            {
                playbackPanel.sliderPanel.value.text = (int)time + "-" + _programDuration;
            }
        }

        void UpdateProgramTargetState(int targetIndex)
        {
            if (targetIndex != _currentTargetIndex)
            {
                _currentTargetIndex = targetIndex;
                UpdateLoadedState();
            }
        }

        void UpdateLoadedState()
        {
            if (_program is null)
                return;
            
            // If the current state of a tool contains the name of the defined gripperLoadedName, then it's current state is loaded
            if (_program.Targets[_currentTargetIndex].ProgramTargets[0].Target.Tool.Name.Contains(gripperLoadedName))
            {
                if (isLoaded == false)
                {
                    RobotActions.OnGripperLoaded();
                }
            }
            
            if (_program.Targets[_currentTargetIndex].ProgramTargets[0].Target.Tool.Name.Contains(gripperUnloadedName))
            {
                if (isLoaded)
                {
                    RobotActions.OnGripperUnloaded();
                }
            }
        }

        void LoadGripper()
        {
            isLoaded = true;
            Debug.Log("Loaded");
        }

        void UnloadGripper()
        {
            isLoaded = false;
            Debug.Log("Unloaded");
        }
    }
}