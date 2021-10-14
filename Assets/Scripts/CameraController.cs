using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CameraMode curMode = CameraMode.LerpToTarget;

    [SerializeField]
    private Vector3 offsetToFollow = new Vector3(0, 4, -3);

    [SerializeField]
    private Transform target;
    void LateUpdate()
    {
        Vector3 camPosition;
        switch (curMode)
        {
            case CameraMode.Follow:
                camPosition = target.position;
                camPosition.x = 0;
                camPosition += offsetToFollow;
                transform.position = camPosition;
                break;
            case CameraMode.RotateAround:
                transform.LookAt(target);
                transform.Translate(Vector3.right * Time.deltaTime);
                break;
            case CameraMode.LerpToTarget:
                transform.LookAt(target);
                camPosition = Vector3.Lerp(transform.position, target.transform.position + offsetToFollow, Time.deltaTime);
                transform.position = camPosition;
                break;
        }

    }

}

public enum CameraMode
{
    Free,
    Follow,
    RotateAround,
    LerpToTarget
}