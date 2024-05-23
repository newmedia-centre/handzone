using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SessionClient))]
public class WebClientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SessionClient sessionClient = (SessionClient)target;

        sessionClient.url = EditorGUILayout.TextField("URL", sessionClient.url);
        
        if (GUILayout.Button("Send: interfaces:get_inverse_kin"))
        {
            //webClient.SendInverseKinematicsRequest();
        }
    }
}
