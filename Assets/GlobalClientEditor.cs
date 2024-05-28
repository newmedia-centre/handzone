using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalClient))]
public class GlobalClientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GlobalClient globalClient = (GlobalClient)target;

        globalClient.url = EditorGUILayout.TextField("URL", globalClient.url);
        
        if (GUILayout.Button("Request virtual robot"))
        {
            globalClient.RequestVirtual();
        }
    }
}
