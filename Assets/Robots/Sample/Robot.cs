using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Robots.Samples.Unity
{
    public class Robot : MonoBehaviour
    {
        [SerializeField]
        #nullable enable
        private Material? _material;
        private Program? _program;
        private UnityMeshPoser? _meshPoser;

        void Update()
        {
            if (_program is null)
                return;

            var time = Mathf.PingPong(Time.time, (float)_program.Duration);
            _program.Animate(time, false);
        }

        public async void CreateProgramFromJSON(string json)
        {
            _program = await GrasshopperSyncProgram.CreateAsync(json);

            if (_material == null)
                throw new ArgumentNullException(nameof(_material));

            if(_meshPoser == null)
                _meshPoser = new UnityMeshPoser(_program.RobotSystem, _material);
            
            _program.MeshPoser = _meshPoser;
        }
    }
}