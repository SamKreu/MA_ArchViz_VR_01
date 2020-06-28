using UnityEngine;
using Oculus.Avatar;
using System.IO;

public class AvatarRecorder : MonoBehaviour
{
  public AvatarMotionData data;

  public float startTimeOffset { get; set; } = 0f;

  private int nPacket = 0;
  private float startTime = 0f;

  private OvrAvatar avatar;
  
  void Awake()
  {
    avatar = GetComponent<OvrAvatar>();
  }

  void OnEnable()
  {
    if (avatar) {
      avatar.RecordPackets = true;
      avatar.PacketRecorded += OnPacketRecorded;

      if (!data)
        data = ScriptableObject.CreateInstance<AvatarMotionData>();

      startTime = Time.time;
      data.Init(avatar.UseSDKPackets, avatar.oculusUserID, startTime - startTimeOffset);
    }
    else {
      Debug.Log("Can't record motion data - avatar not assigned!");
      enabled = false;
    }
  }

  void OnDisable()
  {
    if (!avatar)
      return;

    avatar.RecordPackets = false;
    avatar.PacketRecorded -= OnPacketRecorded;

    data.FinishRecording();
  }

  private void OnPacketRecorded(object sender, OvrAvatar.PacketEventArgs args)
  {
    using (MemoryStream outputStream = new MemoryStream()) {
      BinaryWriter writer = new BinaryWriter(outputStream);

      if (avatar.UseSDKPackets) {
        uint size = CAPI.ovrAvatarPacket_GetSize(args.Packet.ovrNativePacket);
        byte[] data = new byte[size];
        CAPI.ovrAvatarPacket_Write(args.Packet.ovrNativePacket, size, data);

        writer.Write(nPacket++);
        writer.Write(size);
        writer.Write(data);
      }
      else {
        writer.Write(nPacket);
        args.Packet.Write(outputStream);
      }

      data.RecordPacket(Time.time - startTime, outputStream.ToArray());
    }
  }
}
