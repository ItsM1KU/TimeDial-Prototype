using UnityEngine;
using System.Collections;

public class StunDomeVisual : MonoBehaviour
{
    [SerializeField] private float expandDuration = 0.25f;
    [SerializeField] private float holdDuration = 0.3f;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private Color domeColor = new Color(0.4f, 0.7f, 1f, 0.35f);

    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propBlock;

    public void Play(float finalRadius)
    {
        meshRenderer = GetComponent<MeshRenderer>();
        propBlock = new MaterialPropertyBlock();

        SetAlpha(domeColor.a);
        SetColorRGB(domeColor);

        transform.localScale = Vector3.zero;
        StartCoroutine(AnimateDome(finalRadius));
    }

    private void SetColorRGB(Color c)
    {
        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_BaseColor", c); 
        meshRenderer.SetPropertyBlock(propBlock);
    }

    private void SetAlpha(float alpha)
    {
        meshRenderer.GetPropertyBlock(propBlock);
        Color c = domeColor;
        c.a = alpha;
        propBlock.SetColor("_BaseColor", c);
        meshRenderer.SetPropertyBlock(propBlock);
    }

    private IEnumerator AnimateDome(float finalRadius)
    {
        float diameter = finalRadius * 2f;

        float t = 0f;
        while (t < expandDuration)
        {
            t += Time.deltaTime;
            float progress = t / expandDuration;
            float eased = 1f - Mathf.Pow(1f - progress, 3f);
            transform.localScale = Vector3.one * diameter * eased;
            yield return null;
        }
        transform.localScale = Vector3.one * diameter;

        yield return new WaitForSeconds(holdDuration);

        t = 0f;
        float startAlpha = domeColor.a;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}
