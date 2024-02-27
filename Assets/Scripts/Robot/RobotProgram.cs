using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Plane = Rhino.Geometry.Plane;

namespace Robots.Samples.Unity
{
    public class RobotProgram : MonoBehaviour
    {
        [Serializable]
        public enum RobotState
        {
            Off = -1,
            Idle = 0,
            Normal = 1
        }

        public static RobotState State { get; set; } = RobotState.Off;
        
        public PlaybackPanel[] playbackPanels;
        
        public string gripperLoadedName;
        public string gripperUnloadedName;
        public Transform robotMeshTarget;
        public GameObject toolPrefab;

        [SerializeField]
        #nullable enable
        private Material? _toolMaterial;
        private Program? _program;
        private UnityMeshPoser? _meshPoser;
        private UnityMeshPoser? _toolMeshPoser;
        private bool _isPlaying;
        private int _programDuration;
        private int _currentTargetIndex;
        private bool _isLoaded;
        private float _currentTime;

        private void OnEnable()
        {
            RobotActions.OnToolLoaded += LoadTool;
            RobotActions.OnToolUnloaded += UnloadTool;
            WebClient.OnToolpaths += CreateProgramFromObject;
        }
        
        static async Task<RobotSystem> GetRobotAsync()
        {
            var cellName = "TUDelft-LAMA-UR5";

            try
            {
                return FileIO.LoadRobotSystem(cellName, Plane.WorldXY);
            }
            catch (ArgumentException e)
            {
                if (!e.Message.Contains("not found"))
                    throw;

                UnityEngine.Debug.Log("TUDelft-LAMA-UR5 robot library not found, installing...");
                await DownloadLibraryAsync();
                return FileIO.LoadRobotSystem(cellName, Plane.WorldXY);
            }
        }
        
        static async Task DownloadLibraryAsync()
        {
            var online = new OnlineLibrary();
            await online.UpdateLibraryAsync();
            var bartlett = online.Libraries["TUDelft"];
            await online.DownloadLibraryAsync(bartlett);
        }

        private async void CreateProgramFromObject(List<IToolpath> toolpaths)
        {
            var robot = await GetRobotAsync();
            
            _program = new Program("GrasshopperSyncProgram", robot, toolpaths);
            
            Debug.Log("Created program from object " + _program.RobotSystem.Name);
            _program.MeshPoser = new UnityMeshPoser(_program.RobotSystem, _toolMaterial, toolPrefab, robotMeshTarget);
        }

        private void OnDisable()
        {
            RobotActions.OnToolLoaded -= LoadTool;
            RobotActions.OnToolUnloaded -= UnloadTool;
        }

        private void Start()
        {
            foreach (var playbackPanel in playbackPanels)
            {
                if (playbackPanel == null)
                {
                    Debug.LogWarning("Playback panel is empty, not able to add listener to it");
                    return;
                }
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
                _currentTime += Time.deltaTime;
                if (_currentTime > (float)_program.Duration)
                    _currentTime -= (float)_program.Duration;
                // var time = Mathf.PingPong(Time.time, (float)_program.Duration);
                
                SetPlaybackTime(_currentTime);
            }
        }

        void PlayPlayback()
        {
            _isPlaying = true;
        }
        void PausePlayback()
        {
            _isPlaying = false;
            UR_EthernetIPClient.StopMoving();
        }

        /// <summary>
        /// Set's the program current playback time, and alters the target state which can trigger robot events.
        /// </summary>
        /// <param name="time"></param>
        void SetPlaybackTime(float time)
        {
            if (_program is null)
                return;


            _currentTime = time;
            _program.Animate(_currentTime, false);

            RobotActions.OnTimeUpdated(_currentTime);
            UpdateProgramTargetState(_program.CurrentSimulationPose.TargetIndex);
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
                if (_isLoaded == false)
                {
                    RobotActions.OnToolLoaded();
                }
            }
            
            if (_program.Targets[_currentTargetIndex].ProgramTargets[0].Target.Tool.Name.Contains(gripperUnloadedName))
            {
                if (_isLoaded)
                {
                    RobotActions.OnToolUnloaded();
                }
            }
        }

        void LoadTool()
        {
            _isLoaded = true;
            Debug.Log("Loaded");
        }

        void UnloadTool()
        {
            _isLoaded = false;
            Debug.Log("Unloaded");
        }
    }
}