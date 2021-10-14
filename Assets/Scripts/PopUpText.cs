using UnityEngine;

public class PopUpText : MonoBehaviour
{
    [SerializeField]
    private float destroyTime = 0.5f;
    [SerializeField]
    private Vector3 offset = new Vector3(0, 1, 0), maxRandomOffset = new Vector3(0.25f, 0.1f, 0.25f);
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);

        transform.localPosition += offset;
        transform.localPosition += new Vector3
            (
            Random.Range(-maxRandomOffset.x, maxRandomOffset.x),
            Random.Range(-maxRandomOffset.y, maxRandomOffset.y),
            Random.Range(-maxRandomOffset.z, maxRandomOffset.z)
            );
    }

    private void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime, Space.World);
    }
}
