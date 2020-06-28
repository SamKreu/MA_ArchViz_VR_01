using UnityEngine;
using System.IO;
using System.Collections;
using Oculus.Avatar;
using System;

public class AvatarReplayer : MonoBehaviour
{
  public AvatarMotionData data;

  public float startTimeOffset { get; set; } = 0f;

  private OvrAvatarRemoteDriver remoteDriver;

  private OvrAvatar avatar;
  private Vector3 origPos;

  void Awake()
  {
    avatar = GetComponent<OvrAvatar>();
  }

  void OnEnable()
  {
    if (!avatar) {
      Debug.Log("Can't replay motion data - remoteDriver not assigned!");
      enabled = false;
      return;
    }

    remoteDriver = avatar.GetComponent<OvrAvatarRemoteDriver>();
    if (!remoteDriver)
      remoteDriver = avatar.gameObject.AddComponent<OvrAvatarRemoteDriver>();

    if (avatar.GetComponent<OvrAvatarLocalDriver>())
      avatar.GetComponent<OvrAvatarLocalDriver>().enabled = false;

    avatar.gameObject.SetActive(true);
    avatar.enabled = true;

    StartCoroutine(ProcessRecordedPackages());
  }

  void OnDisable()
  {
    StopAllCoroutines();
  }

  // little hacky --->
  public void HideAvatar()
  {
    origPos = transform.position;
    transform.position = 1000f * Vector3.up;
  }

  public void RestoreAvatar()
  {
    transform.position = origPos;
  }
  // <--- little hacky

  private IEnumerator ProcessRecordedPackages()
  {
    if (data.packetQueue.Count == 0)
      yield break;

    AvatarMotionData.Packet packet = data.packetQueue.Dequeue();
    yield return new WaitForSeconds(data.startTime - startTimeOffset + packet.time);

    RestoreAvatar();
    ReceivePacketData(packet.bytes);

    float replayStartTime = Time.time;
    while (data.packetQueue.Count > 0) {
      packet = data.packetQueue.Dequeue();
      yield return new WaitForSeconds(packet.time - (Time.time - replayStartTime));
      ReceivePacketData(packet.bytes);
    }
  }

  private void ReceivePacketData(byte[] packetData)
  {
    using (MemoryStream inputStream = new MemoryStream(packetData)) {
      BinaryReader reader = new BinaryReader(inputStream);
      int sequence = reader.ReadInt32();

      OvrAvatarPacket packet;
      if (data.useSDKPackets) {
        int size = reader.ReadInt32();
        byte[] sdkData = reader.ReadBytes(size);

        IntPtr ptr = CAPI.ovrAvatarPacket_Read((UInt32)packetData.Length, sdkData);
        packet = new OvrAvatarPacket { ovrNativePacket = ptr };
      }
      else {
        packet = OvrAvatarPacket.Read(inputStream);
      }

      remoteDriver.QueuePacket(sequence, packet);
    }
  }

  //void ReadFile()
  //{
  //  string path = Application.isEditor ? filename : Path.Combine(Application.persistentDataPath, filename);
  //  using (Stream stream = File.Open(filename, FileMode.Open))
  //    packetQueue = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(stream) as Queue<byte[]>;

  //  Debug.Log("Avatar motion loaded from File " + filename);
  //}
}
