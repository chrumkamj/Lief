using UnityEngine;
using System.Collections;

public class FlagController : MonoBehaviour {

  public Vector3 WindDirection = Vector3.right;

  public void Update() {

    transform.forward = -WindDirection;

  }

}