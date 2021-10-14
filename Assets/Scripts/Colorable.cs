using System.Collections.Generic;
using UnityEngine;

public class Colorable : MonoBehaviour
{
    public SegmentColor curColor;
    public List<ParticleSystem> sprinklers { get; private set; }
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        sprinklers = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
    }
    public void ChangeColor(SegmentColor segmentColor)
    {
        curColor = segmentColor;

        Color color = GameManager.colors[(int)curColor];
        rend.material.color = color;

        foreach (var item in sprinklers)
        {
            var main = item.main;
            main.startColor = color;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<SnakeHead>().SetNewColor(curColor);
    }
}
