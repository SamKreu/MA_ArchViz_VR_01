using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New.motiondata.asset", order = 10001)]
public class MotionData : ScriptableObject
{
  public float startTime;
  public float fps = 30f;
  public string[] nodeNames = new string[0];
  public Frame[] keyframes = new Frame[0];

  private List<Frame> keyframeList = new List<Frame>();

  private int minKeyframe = 0;
  private float length { get { return keyframes.Length > 0 ? keyframes[keyframes.Length - 1].time : 0f; } }

  [System.Serializable]
  public class Frame
  {
    public float time;
    public Vector3[] positions;
    public Quaternion[] rotations;
  }

  public void Init(Transform[] nodes, float startTime)
  {
    this.startTime = startTime;
    if (nodeNames == null)
      nodeNames = new string[nodes.Length];
    else
      Array.Resize(ref nodeNames, nodes.Length);

    for (int i = 0; i < nodes.Length; ++i)
      nodeNames[i] = nodes[i].name;

    keyframeList.Clear();
  }

  public void RecordKeyframe(Frame frame)
  {
    keyframeList.Add(frame);
  }

  public void FinishRecording()
  {
    keyframes = keyframeList.ToArray();
    Frame[] evenKeyframes = new Frame[(int)(length * fps)];
    for (int i = 0; i < evenKeyframes.Length; i++)
      evenKeyframes[i] = GetFrame(i / fps);

    keyframes = evenKeyframes;

#if UNITY_EDITOR
    string path = AssetDatabase.GetAssetPath(this);
    if (path == "") {
      path = Path.Combine("Assets", "MotionData");
      Directory.CreateDirectory(path);
      path = Path.Combine(path, "NewData.motiondata.asset");
      AssetDatabase.CreateAsset(this, AssetDatabase.GenerateUniqueAssetPath(path));
    }

    EditorUtility.SetDirty(this);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();
#else
    string s = JsonUtility.ToJson(this);
    string path = Path.Combine(Application.persistentDataPath, "pipi.json");
    File.WriteAllText(path, s);
#endif
  }

  public Frame GetFrame(float time)
  {
    if (keyframes.Length == 0 || time < 0f || time > length)
      return null;

    minKeyframe = Mathf.Clamp(minKeyframe, 0, keyframes.Length - 2);
    if (keyframes[minKeyframe].time > time)
      minKeyframe = 0;

    while (keyframes[minKeyframe + 1].time < time)
      ++minKeyframe;

    float t = Mathf.InverseLerp(keyframes[minKeyframe].time, keyframes[minKeyframe + 1].time, time);

    Frame frame = new Frame();
    frame.time = time;
    frame.positions = new Vector3[nodeNames.Length];
    frame.rotations = new Quaternion[nodeNames.Length];
    for (int i = 0; i < nodeNames.Length; ++i) {
      frame.positions[i] = Vector3.Lerp(keyframes[minKeyframe].positions[i], keyframes[minKeyframe + 1].positions[i], t);
      frame.rotations[i] = Quaternion.Slerp(keyframes[minKeyframe].rotations[i], keyframes[minKeyframe + 1].rotations[i], t);
    }

    return frame;
  }
}
