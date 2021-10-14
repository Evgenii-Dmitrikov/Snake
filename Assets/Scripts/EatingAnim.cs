using System.Collections;
using UnityEngine;

public class EatingAnim : MonoBehaviour
{
    public void Eaten(Transform target, float duration)
    {
        StartCoroutine(DrawIn(target, duration));
    }


    private IEnumerator DrawIn(Transform target, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, t / duration);
            transform.localScale *= 0.9f;
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        Destroy(gameObject);
        yield break;
    }
}
