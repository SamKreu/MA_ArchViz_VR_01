using UnityEngine;

public class SineMover : MonoBehaviour
{
  public float frequency;
  public float phase;
  public Vector3 amplitude;

  private const float PI_2 = 2f * Mathf.PI;
  private Vector3 startPos;

  void Start()
  {
    startPos = transform.position;
  }

  void Update()
  {
    transform.position = startPos + Mathf.Sin(frequency * (Time.time + phase) * PI_2) * amplitude;
  }
}
