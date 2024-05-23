using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalClient))]
public class GlobalClientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GlobalClient globalClient = (GlobalClient)target;

        globalClient.url = EditorGUILayout.TextField("URL", globalClient.url);
        string pin = EditorGUILayout.TextField("");
        
        if(GUILayout.Button("Connect"))
        {
            Task.Run(() => globalClient.TryConnectToGlobalServer(pin));
        }
        
        if (GUILayout.Button("Request virtual robot"))
        {
            globalClient.RequestVirtual();
        }
    }
}
