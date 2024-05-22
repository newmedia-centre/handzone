using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RobotClient))]
public class WebClientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RobotClient robotClient = (RobotClient)target;

        robotClient.url = EditorGUILayout.TextField("URL", robotClient.url);
        
        if (GUILayout.Button("Send: interfaces:get_inverse_kin"))
        {
            //webClient.SendInverseKinematicsRequest();
        }
    }
}
