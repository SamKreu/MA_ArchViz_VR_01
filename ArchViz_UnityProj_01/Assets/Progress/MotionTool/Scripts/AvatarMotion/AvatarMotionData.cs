using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New.avatarmotiondata.asset", order = 10002)]
public class AvatarMotionData : ScriptableObject
{
  [Serializable]
  public class Packet
  {
    public float time;
    public byte[] bytes;
  }

  public bool useSDKPackets;
  public float startTime;
  public Queue<Packet> packetQueue = new Queue<Packet>();

  public List<Packet> packetList = new List<Packet>();

  void OnEnable()
  {
    packetQueue = new Queue<Packet>(packetList);
  }

  public void Init(bool useSDKPackets, string oculusUserID, float startTime)
  {
    packetQueue.Clear();
    this.useSDKPackets = useSDKPackets;
    this.startTime = startTime;
  }

  public void RecordPacket(float time, byte[] bytes)
  {
    packetQueue.Enqueue(new Packet { time = time, bytes = bytes });
  }

  public void FinishRecording()
  {
    packetList = new List<Packet>(packetQueue);

#if UNITY_EDITOR
    string path = AssetDatabase.GetAssetPath(this);
    if (path == "") {
      path = Path.Combine("Assets", "AvatarMotionData");
      Directory.CreateDirectory(path);
      path = Path.Combine(path, "New.avatarmotiondata.asset");
      AssetDatabase.CreateAsset(this, AssetDatabase.GenerateUniqueAssetPath(path));
    }

    EditorUtility.SetDirty(this);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();
#else
    //string path = Path.Combine(Application.persistentDataPath, oculusUserID + "_" + DateTime.Now.ToShortTimeString());
    //using (Stream stream = File.Open(path, FileMode.Create))
    //  new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(stream, packetQueue);

    //Debug.Log("Avatar motion saved to File " + Path.GetFileName(path));
#endif
  }
}
