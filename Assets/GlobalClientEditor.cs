using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalClient))]
public class GlobalClientEditor : Editor
{
    private string _pin = "";
    public override void OnInspectorGUI()
    {
        GlobalClient globalClient = (GlobalClient)target;

        globalClient.url = EditorGUILayout.TextField("URL", globalClient.url);
        _pin = EditorGUILayout.TextField("Pin", _pin);
        
        if(GUILayout.Button("Connect"))
        {
            Task.Run(() => globalClient.TryConnectToGlobalServer(_pin));
        }
        
        if (GUILayout.Button("Request virtual robot"))
        {
            globalClient.RequestVirtual();
        }
    }
}
