using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(MotionReplayer))]
public class MotionReplayerInspector : Editor
{
  private MotionReplayer replayer;

  private bool nodesToggle { get; set; }

  void OnEnable()
  {
    replayer = (MotionReplayer)target;
    nodesToggle = EditorPrefs.GetBool("MotionReplayerNodesToggle");
  }

  void OnDisable()
  {
    EditorPrefs.SetBool("MotionReplayerNodesToggle", nodesToggle);
  }

  public override void OnInspectorGUI()
  {
    GUI.enabled = false;
    EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(replayer), typeof(MotionReplayer), false);
    GUI.enabled = true;

    replayer.customOrigin = EditorGUILayout.ObjectField("Custom Origin", replayer.customOrigin, typeof(Transform), true) as Transform;
    replayer.motionData = EditorGUILayout.ObjectField("Motion Data", replayer.motionData, typeof(MotionData), true) as MotionData;
    if (!replayer.motionData) {
      GUILayout.Label("No Data");
      return;
    }

    if (replayer.nodes.Length != replayer.motionData.nodeNames.Length)
      Array.Resize(ref replayer.nodes, replayer.motionData.nodeNames.Length);

    if (replayer.motionData.nodeNames.Length == 0) {
      GUILayout.Label("Nodes Empty");
      return;
    }

    EditorGUILayout.Space();

    if (GUILayout.Button("Try Auto-Assign"))
      replayer.TryAutoAssign();

    if (replayer.motionData) {
      nodesToggle = EditorGUILayout.Foldout(nodesToggle, "Nodes");
      if (nodesToggle)
        for (int i = 0; i < replayer.motionData.nodeNames.Length; ++i)
          if (!string.IsNullOrEmpty(replayer.motionData.nodeNames[i]))
            replayer.nodes[i] = EditorGUILayout.ObjectField(replayer.motionData.nodeNames[i], replayer.nodes[i], typeof(Transform), true) as Transform;
          else {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Unassigned", replayer.nodes[i], typeof(Transform), true);
            GUI.enabled = false;
          }
    }

    replayer.pause = EditorGUILayout.Toggle("Pause", replayer.pause);
  }
}
