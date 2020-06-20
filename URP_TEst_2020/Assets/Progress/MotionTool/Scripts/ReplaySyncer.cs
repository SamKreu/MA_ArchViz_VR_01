using System.Collections;
using UnityEngine;

public class ReplaySyncer : MonoBehaviour
{
  public MotionReplayer motionReplayer;
  public AvatarReplayer[] avatarReplayers;

  public float startDelay = 3f;

  void OnEnable()
  {
    StartCoroutine(DelayedStart());
  }

  private void StartReplay()
  {
    foreach (AvatarReplayer ar in avatarReplayers) {
      ar.startTimeOffset = 0f;
      ar.enabled = true;
    }

    motionReplayer.startTimeOffset = 0f;
    motionReplayer.enabled = true;
  }

  void OnDisable()
  {
    foreach (AvatarReplayer ar in avatarReplayers)
      if (ar)
        ar.enabled = false;

    if (motionReplayer)
      motionReplayer.enabled = false;
  }

  private IEnumerator DelayedStart()
  {
    yield return new WaitForSeconds(startDelay);
    StartReplay();
  }
}
