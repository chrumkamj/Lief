using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour {
  
  public string RudderAxis = "Rudder";
  public string SlackAxis = "Slack";

  public Vector3 WindDirection = Vector3.right;
  public float Speed = 100.0f;

  private const float maxRudderRotation = 45;
  private const float maxSailRotation = 90;
  
  private Quaternion rudderRotation = Quaternion.identity;
  private Quaternion sailRotation = Quaternion.identity;
  
  private float rudder = 0.0f;
  private float slack = 0.0f;

  public void Update() {
    UpdateInput();
    UpdateRudder();
    UpdateSail();
    UpdateMovement();
  }

  private void UpdateInput() {
    rudder = Mathf.Clamp(Input.GetAxis(RudderAxis), -1.0f, 1.0f);
    slack = Mathf.Clamp(slack + Input.GetAxis(SlackAxis), 0.0f, 100.0f);
  }

  private void UpdateRudder() {
    UpdateRudderRotation();
    Transform rudder = transform.FindChild("Rudder");
    rudder.transform.localRotation = rudderRotation;
  }
  
  private void UpdateRudderRotation() {

    // Rudder rotation as percentage of maximum
    Quaternion rotation = Quaternion.Euler(0.0f, -rudder * maxRudderRotation, 0.0f);

    // Update rudder orientation
    rudderRotation =
      Quaternion.Lerp(
        rudderRotation,
        rotation,
        Time.deltaTime
      );
    
  }

  private void UpdateSail() {
    UpdateSailRotation();
    Transform sail = transform.FindChild("Sail");
    sail.transform.localRotation = sailRotation;
  }

  private void UpdateSailRotation() {

    // Sail direction when taught is antiparallel to ship's direction
    Vector3 taughtSail = -transform.forward;

    // Sail direction when controlled only by wind
    Vector3 windSail = WindDirection;

    // Sail direction when not limited by slack
    Vector3 slackSail = Vector3.Cross(taughtSail, windSail).y < 0.0f ?
      Vector3.RotateTowards(taughtSail, transform.right, Mathf.Deg2Rad * maxSailRotation, Mathf.Deg2Rad * maxSailRotation) :
      Vector3.RotateTowards(taughtSail, -transform.right, Mathf.Deg2Rad * maxSailRotation, Mathf.Deg2Rad * maxSailRotation);

    /**
     * Problem with above logic...
     * Sail flips unnecessarily, should stay on same side until necessary.
     **/

    // Sail orientation when controlled only by wind
    Quaternion windSailRotation = Quaternion.FromToRotation(taughtSail, windSail);

    // Sail orientation when limited by slack
    Quaternion slackSailRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.FromToRotation(taughtSail, slackSail), slack / 100.0f);

    
    float windAngle = Quaternion.Angle(Quaternion.identity, windSailRotation);
    float slackAngle = Quaternion.Angle(Quaternion.identity, slackSailRotation);

    // Determine whether the wind or the slack is the limiting factor
    Quaternion targetSailRotation = Quaternion.identity;
    if (windAngle < slackAngle) {
      targetSailRotation = windSailRotation;
    } else {
      targetSailRotation = slackSailRotation;
    }

    // Determine new potential sail orientation
    targetSailRotation = 
      Quaternion.Lerp(
        sailRotation,
        targetSailRotation,
        Time.deltaTime
      );

    // Update sail orientation if within bounds
    if (Vector3.Angle((targetSailRotation * taughtSail), taughtSail) < maxSailRotation) {
      sailRotation = targetSailRotation;
    }

  }

  private void UpdateMovement() {

    transform.rotation = 
      Quaternion.Lerp(
        transform.rotation,
        transform.rotation * rudderRotation,
        Time.deltaTime
      );

    rigidbody.AddRelativeForce(Vector3.forward * Mathf.Abs(Vector3.Dot(sailRotation * transform.right, WindDirection)) * Speed * Time.deltaTime);

  }

}
