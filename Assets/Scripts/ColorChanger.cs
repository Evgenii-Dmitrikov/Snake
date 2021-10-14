using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorChanger : MonoBehaviour
{
    [SerializeField]
    public float duration = 2f;
    float time = 0;

    [SerializeField]
    private MeshRenderer rend;

    private Material material;
    public Color curColor;

    private struct ShaderPropertyIDs
    {
        public int _Offset;
        public int _BaseColor;
        public int _ColorTo;
    }

    private ShaderPropertyIDs shaderIds;

    private void Awake()
    {
        curColor = Color.gray;
        rend = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        shaderIds = new ShaderPropertyIDs()
        {
            _Offset = Shader.PropertyToID("_Offset"),
            _BaseColor = Shader.PropertyToID("_BaseColor"),
            _ColorTo = Shader.PropertyToID("_ColorTo")
        };
        if (material == null)
        {
            material = Instantiate(rend.sharedMaterial);
            rend.material = material;
        }

        material.SetColor(shaderIds._BaseColor, curColor);
        material.SetColor(shaderIds._ColorTo, curColor);
        material.SetFloat(shaderIds._Offset, 1);
    }
    public void SetNewColor(Color colorToLerp)
    {
        StartCoroutine(ChangeColor(colorToLerp));
    }
    public void SetNewColorIntant(Color colorTo)
    {
        curColor = colorTo;
        if (material == null)
        {
            material = Instantiate(rend.sharedMaterial);
            rend.material = material;
        }

        material.SetColor(shaderIds._BaseColor, colorTo);
        material.SetColor(shaderIds._ColorTo, colorTo);
        material.SetFloat(shaderIds._Offset, 1);
    }

    private IEnumerator ChangeColor(Color colorToLerp)
    {
        material.SetColor(shaderIds._ColorTo, colorToLerp);

        time = 0;
        while (time < duration)
        {
            material.SetFloat(shaderIds._Offset, 1f - (time / duration) * 2 );
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        material.SetColor(shaderIds._BaseColor, colorToLerp);
        material.SetFloat(shaderIds._Offset, 0.5f);
        yield break;
    }

    private void OnDestroy()
    {
        if(material != null)
        {
            Destroy(material);
        }
    }
}
