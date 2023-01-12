using System;
using System.Runtime.InteropServices;
using Rhino;
using Rhino.Geometry;
using UnityEngine;
using RhinoInside.Unity;
using Robots.Samples.Unity;
using Quaternion = UnityEngine.Quaternion;

public class UnityInGrasshopper : MonoBehaviour
{
    public static UnityInGrasshopper Instance;
    
    public GameObject grasshopperObjectPrefab;
    public GameObject[] uiParents;
    public GameObject sliderPanelPrefab;
    public GameObject togglePanelPrefab;
    
    #region MonoBehaviour

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!Startup.isLoaded)
        {
            Startup.Init();
        }
        
        Rhino.Runtime.HostUtils.RegisterNamedCallback("FromGHData", FromGHData);
        Rhino.Runtime.HostUtils.RegisterNamedCallback("FromGHTransform", FromGHTransform);
        
        Rhino.Runtime.HostUtils.RegisterNamedCallback("FromGHCreateSlider", FromGHCreateSlider);
        Rhino.Runtime.HostUtils.RegisterNamedCallback("FromGHCreateToggle", FromGHCreateToggle);
        Rhino.Runtime.HostUtils.RegisterNamedCallback("FromGHClearUI", FromGHClearUI);
    }

    #endregion
    
    #region From GH functions

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

    void FromGHTransform(object sender, Rhino.Runtime.NamedParametersEventArgs args)
    {
        if (Application.isPlaying)
        {
            string id = "";
            if (args.TryGetString("id", out id))
            {
                Point3d pos;
                args.TryGetPoint("pos", out pos);
                Vector3 position = pos.ToHost();
                
                if (!GameObject.Find(id))
                {
                    if (grasshopperObjectPrefab == null)
                    {
                        Debug.LogWarning("GrasshopperObjectPrefab not selected!");
                        return;
                    }
                    
                    var go = Instantiate(grasshopperObjectPrefab, position, Quaternion.identity);
                    go.name = id;
                }
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
    
    void FromGHCreateSlider(object sender, Rhino.Runtime.NamedParametersEventArgs args)
    {
        if (Application.isPlaying)
        {
            string id = "";
            string sliderName = "";
            double minVal = 0f;
            double maxVal = 0f;
            double val = 0f;
            int type = 0;
            if (args.TryGetString("id", out id))
            {
                args.TryGetString("name", out sliderName);
                args.TryGetDouble("min", out minVal);
                args.TryGetDouble("max", out maxVal);
                args.TryGetDouble("value", out val);
                args.TryGetInt("type", out type);

                foreach (var uiParent in uiParents)
                {
                    var sliderPanelObj = (GameObject)Instantiate(sliderPanelPrefab, uiParent.transform);
                    sliderPanelObj.name = id;
                    SliderPanel sliderPanel = sliderPanelObj.GetComponent<SliderPanel>();
                    sliderPanel.text.text = sliderName;
                    sliderPanel.slider.minValue = (float)minVal;
                    sliderPanel.slider.maxValue = (float)maxVal;
                    sliderPanel.slider.value = (float)val;
                    if (type > 0) {
                        sliderPanel.slider.wholeNumbers = true;
                    }
                    else
                    {
                        sliderPanel.slider.wholeNumbers = false;
                    }
                    sliderPanel.slider.onValueChanged.AddListener(value =>
                    {
                        SendSliderValue(value, id);
                    });
                }

            }
        }
    }

    void FromGHCreateToggle(object sender, Rhino.Runtime.NamedParametersEventArgs args)
    {
        if (Application.isPlaying)
        {
            string id = "";
            string toggleName = "";
            bool val = false;
            if (args.TryGetString("id", out id))
            {
                args.TryGetString("name", out toggleName);
                args.TryGetBool("value", out val);

                foreach (var uiParent in uiParents)
                {
                    var togglePanelObj = (GameObject)Instantiate(togglePanelPrefab, uiParent.transform);
                    togglePanelObj.name = id;
                    TogglePanel togglePanel = togglePanelObj.GetComponent<TogglePanel>();
                    togglePanel.text.text = toggleName;
                    togglePanel.toggle.isOn = val;

                    togglePanel.toggle.onValueChanged.AddListener(value => { SendToggleValue(value, id); });
                }
            }
        }
    }

    #endregion

    #region To GH Functions

    /// <summary>
    /// Send position as a Vector3d to a callback event with a scaling property to match Grasshopper virtual space.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="id"></param>
    public void SendPosition(Vector3 position, string id)
    {
        using (var args = new Rhino.Runtime.NamedParametersEventArgs())
        {
            Vector3d vector3d = new Vector3d(position.x, position.y, position.z);
            args.Set("position", vector3d);
            Rhino.Runtime.HostUtils.ExecuteNamedCallback("ToGH_Position_" + id, args);
        }
    }
    
    public void SendRotationEuler(UnityEngine.Quaternion rotation, string id)
    {
        using (var args = new Rhino.Runtime.NamedParametersEventArgs())
        {
            Vector3 eulerAngles = rotation.eulerAngles;
            Vector3d vector3d = new Vector3d(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            args.Set("rotationEuler", vector3d);
            Rhino.Runtime.HostUtils.ExecuteNamedCallback("ToGH_Rotation_" + id, args);
        }
    }

    public void SendRotationQuaternion(UnityEngine.Quaternion rotation, string id)
    {
        using (var args = new Rhino.Runtime.NamedParametersEventArgs())
        {
            Quaternion quaternion = new Quaternion(rotation.y, -rotation.z, rotation.x, rotation.w);
            string quatValue = quaternion.ToString();
            args.Set("rotationQuaternion", quatValue);
            Rhino.Runtime.HostUtils.ExecuteNamedCallback("ToGH_Rotation_" + id, args);
        }
    }

    public void SendPositionRotation(Vector3 position, Quaternion rotation, string id)
    {
        SendPosition(position, id);
        SendRotationEuler(rotation, id);
    }
    
    public void SendSliderValue(float val, string id)
    {
        using (var args = new Rhino.Runtime.NamedParametersEventArgs())
        {
            args.Set("sliderValue", val);
            Rhino.Runtime.HostUtils.ExecuteNamedCallback("ToGH_Slider_" + id, args);
        }
    }

    public void SendToggleValue(bool val, string id)
    {
        using (var args = new Rhino.Runtime.NamedParametersEventArgs())
        {
            args.Set("toggleValue", val);
            Rhino.Runtime.HostUtils.ExecuteNamedCallback("ToGH_Toggle_" + id, args);
        }
    }
    
    #endregion
    

    #region Rhino / GH window

    public void OpenGrasshopper()
    {
        string script = "!_-Grasshopper _W _S ENTER";
        RhinoApp.RunScript(script, false);
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
