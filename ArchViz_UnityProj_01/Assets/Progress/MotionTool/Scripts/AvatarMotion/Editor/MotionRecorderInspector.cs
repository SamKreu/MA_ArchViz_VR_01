using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MotionRecorder))]
public class MotionRecorderInspector : Editor
{
  private MotionRecorder recorder;

  void OnEnable()
  {
    recorder = (MotionRecorder)target;
  }

  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    if (!recorder.motionData)
      return;

    EditorGUILayout.Space();
    EditorUtils.DrawLine(Color.white);

    if (GUILayout.Button("Set Nodes From Children"))
      recorder.SetNodesFromChildren();

    EditorGUILayout.Space();

    if (GUILayout.Button("Init MotionData"))
      recorder.motionData.Init(recorder.nodes, 0f);
  }
}
