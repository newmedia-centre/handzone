using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WebClient))]
public class WebClientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WebClient webClient = (WebClient)target;

        webClient.url = EditorGUILayout.TextField("URL", webClient.url);
        
        if (GUILayout.Button("Send: interfaces:get_inverse_kin"))
        {
            webClient.SendInveseKinematicsRequest();
        }
    }
}
