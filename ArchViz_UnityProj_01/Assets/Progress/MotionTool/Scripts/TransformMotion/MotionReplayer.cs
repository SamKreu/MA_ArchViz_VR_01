using UnityEngine;
using System.Collections.Generic;

public class MotionReplayer : MonoBehaviour
{
  public Transform customOrigin;
  public MotionData motionData;
  public Transform[] nodes = new Transform[0];
  public bool pause;

  public float startTimeOffset { get; set; } = 0f;

  private float currentTime;

  void Update()
  {
    if (!motionData) {
      enabled = false;
      return;
    }

    if (pause)
      return;

    if (currentTime >= 0f)
      setFrame(currentTime);

    currentTime += Time.deltaTime;
  }

  void OnEnable()
  {
    currentTime = -motionData.startTime + startTimeOffset;
  }

  public void TryAutoAssign()
  {
    Dictionary<string, int> dicIndices = new Dictionary<string, int>();
    Dictionary<string, Transform> dicTransforms = new Dictionary<string, Transform>();
    Transform[] children = GetComponentsInChildren<Transform>(true);

    for (int i = 0; i < motionData.nodeNames.Length; ++i)
      dicIndices.Add(motionData.nodeNames[i], i);
    foreach (Transform child in children)
      dicTransforms.Add(child.name, child);

    if (dicIndices.Count > dicTransforms.Count) {
      Transform t;
      foreach (KeyValuePair<string, int> kvp in dicIndices)
        if (dicTransforms.TryGetValue(kvp.Key, out t))
          nodes[kvp.Value] = t;
    }
    else {
      int idx;
      foreach (KeyValuePair<string, Transform> kvp in dicTransforms)
        if (dicIndices.TryGetValue(kvp.Key, out idx))
          nodes[idx] = kvp.Value;
    }
  }

  private void setFrame(float time)
  {
    MotionData.Frame frame = motionData.GetFrame(time);
    if (frame == null) {
      enabled = false;
      return;
    }

    for (int i = 0; i < nodes.Length; ++i) {
      if (!nodes[i] || string.IsNullOrEmpty(motionData.nodeNames[i]))
        continue;

      nodes[i].position = customOrigin ? customOrigin.position + frame.positions[i] : frame.positions[i];
      nodes[i].rotation = customOrigin ? customOrigin.rotation * frame.rotations[i] : frame.rotations[i];
    }
  }
}
