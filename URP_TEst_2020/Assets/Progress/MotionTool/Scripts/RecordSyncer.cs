using UnityEngine;

public class RecordSyncer : MonoBehaviour
{
  public MotionRecorder motionRecorder;
  public AvatarRecorder[] avatarRecorders;

  void OnEnable()
  {
    foreach (AvatarRecorder ar in avatarRecorders) {
      ar.startTimeOffset = Time.time;
      ar.enabled = true;
    }

    motionRecorder.startTimeOffset = Time.time;
    motionRecorder.enabled = true;
  }

  void OnDisable()
  {
    foreach (AvatarRecorder ar in avatarRecorders)
      ar.enabled = false;

    motionRecorder.enabled = false;
  }
}
