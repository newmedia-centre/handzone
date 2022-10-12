using System;
using System.Runtime.InteropServices;
using Rhino;
using Rhino.Geometry;
using UnityEngine;
using RhinoInside.Unity;
using Robots.Samples.Unity;

public class UnityInGrasshopper : MonoBehaviour
{
    public static UnityInGrasshopper instance;
    
    #region MonoBehaviour

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!Startup.isLoaded)
        {
            Startup.Init();
        }
        
        Rhino.Runtime.HostUtils.RegisterNamedCallback("FromGHData", FromGHData);
        
        Rhino.Runtime.HostUtils.RegisterNamedCallback("FromGHClearUI", FromGHClearUI);
    }

    #endregion
    
    #region To GH functions
    
    void FromGHData(object sender, Rhino.Runtime.NamedParametersEventArgs args)
    {
        if (Application.isPlaying)
        {
            string id = "";
            string json = "";
            if (args.TryGetString("id", out id))
            {
                if (!GameObject.Find(id))
                {
                    var go = new GameObject(id);
                    go.AddComponent<GrassHopperObject>();
                }
            }

            if (args.TryGetString("json", out json))
            {
                // // DEBUG TIMER START
                // Stopwatch st = new Stopwatch();
                // st.Start();

                var robot = GameObject.Find("Robot").GetComponent<Robot>();
                robot.CreateProgramFromJSON(json);

                // // DEBUG TIMER STOP
                // st.Stop();
                // Debug.Log(string.Format("MyMethod took {0} ms to complete", st.ElapsedMilliseconds));
            }
        }
    }
    
    void FromGHClearUI(object sender, Rhino.Runtime.NamedParametersEventArgs args)
    {
        string id = "";
        if (args.TryGetString("id", out id))
        {
            var gb = GameObject.Find(id);
            if (gb != null)
            {
                Destroy(gb);
            }
        }
    }

    public void SendPosition(Vector3 position, string id)
    {
        using (var args = new Rhino.Runtime.NamedParametersEventArgs())
        {
            Vector3d vector3d = new Vector3d(position.x, position.y, position.z);
            args.Set("position", vector3d);
            Rhino.Runtime.HostUtils.ExecuteNamedCallback("ToGH_Position_" + id, args);
        }
    }
    
    public void SendRotation(Vector3 rotation, string id)
    {
        using (var args = new Rhino.Runtime.NamedParametersEventArgs())
        {
            Vector3d vector3d = new Vector3d(rotation.x, rotation.y, rotation.z);
            args.Set("rotation", vector3d);
            Rhino.Runtime.HostUtils.ExecuteNamedCallback("ToGH_Rotation_" + id, args);
        }
    }

    public void SendPositionRotation(Vector3 position, Vector3 rotation, string id)
    {
        SendPosition(position, id);
        SendRotation(rotation, id);
    }

    #endregion

    #region Rhino / GH window

    public void OpenGH()
    {
        string script = "!_-Grasshopper _W _S ENTER";
        Rhino.RhinoApp.RunScript(script, false);
    }

    public void OpenRhino()
    {
        ShowWindow(RhinoApp.MainWindowHandle(), 1);
        BringWindowToTop(RhinoApp.MainWindowHandle());
    }

    [DllImport("USER32", SetLastError = true)]
    static extern IntPtr BringWindowToTop(IntPtr hWnd);
    
    [DllImport("USER32", SetLastError = true)]
    static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

    #endregion
}
