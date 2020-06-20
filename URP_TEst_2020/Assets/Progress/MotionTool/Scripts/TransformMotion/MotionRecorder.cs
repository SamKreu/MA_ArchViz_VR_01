using UnityEngine;
using System.Collections.Generic;

public class MotionRecorder : MonoBehaviour
{
  public Transform customOrigin;
  public MotionData motionData;
  public Transform[] nodes = new Transform[0];
  public bool pause;

  public float startTimeOffset { get; set; } = 0f;

  private float currentTime;

  void Update()
  {
    if (pause)
      return;

    RecordFrame(currentTime);
    currentTime += Time.deltaTime;
  }

  void OnEnable()
  {
    StartRecording();
  }

  void OnDisable()
  {
    StopRecording();
  }

  public string[] GetNodeNames()
  {
    string[] nodeNames = new string[nodes.Length];
    for (int i = 0; i < nodes.Length; ++i)
      nodeNames[i] = nodes[i].name;

    return nodeNames;
  }

  public void SetNodesFromChildren()
  {
    List<Transform> children = new List<Transform>();
    GetComponentsInChildren(children);
    children.Remove(transform);

    nodes = children.ToArray();
  }

  private void StartRecording()
  {
    if (nodes.Length == 0) {
      Debug.Log("Could not start recording: nodes array is empty!");
      enabled = false;
      return;
    }

    if (!motionData)
      motionData = ScriptableObject.CreateInstance<MotionData>();

    motionData.Init(nodes, Time.time - startTimeOffset);

    currentTime = 0f;
  }

  private void StopRecording()
  {
    motionData.FinishRecording();
  }

  private void RecordFrame(float time)
  {
    MotionData.Frame frame = new MotionData.Frame();
    frame.positions = new Vector3[nodes.Length];
    frame.rotations = new Quaternion[nodes.Length];

    frame.time = time;
    for (int i = 0; i < nodes.Length; ++i) {
      if (!nodes[i])
        continue;

      frame.positions[i] = customOrigin ? nodes[i].position - customOrigin.position : nodes[i].position;
      frame.rotations[i] = customOrigin ? nodes[i].rotation * Quaternion.Inverse(customOrigin.rotation) : nodes[i].rotation;
    }

    motionData.RecordKeyframe(frame);
  }
}
