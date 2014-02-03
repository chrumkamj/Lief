using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

  public GameObject Target = null;

  public float Height = 2.0f;
  public float Length = 4.0f;

  public float Dampness = 1.0f;

  public void Update() {
    float radius = Mathf.Sqrt(Mathf.Pow(Length, 2.0f) - Mathf.Pow(Height, 2.0f));

    float aX = Target.transform.position.x;
    float bX = transform.position.x;

    float aZ = Target.transform.position.z;
    float bZ = transform.position.z;

    float x = aX + radius * (bX - aX) / Mathf.Sqrt(Mathf.Pow(bX - aX, 2.0f) + Mathf.Pow(bZ - aZ, 2.0f));
    float z = aZ + radius * (bZ - aZ) / Mathf.Sqrt(Mathf.Pow(bX - aX, 2.0f) + Mathf.Pow(bZ - aZ, 2.0f));

    Vector3 target = new Vector3(x, Target.transform.position.y + Height, z);
    transform.position = Vector3.Slerp(transform.position, target, Dampness);

    transform.LookAt(Target.transform.position);
  }

}
