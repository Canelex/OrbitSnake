using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    private RectTransform canvas;
    private RectTransform rect;
    public Vector3 position;
    public string text;
    public float lifetime = 1F;
    public float popTime = 0.3F;

    public void Update()
    {
        Vector2 viewport = Camera.main.WorldToViewportPoint(position) - Vector3.one * 0.5F;
        Vector2 size = canvas.rect.size;
        Vector2 pos = new Vector2(viewport.x * size.x, viewport.y * size.y);
        Debug.Log(viewport);
        
        rect.localPosition = pos;
    }

    private IEnumerator Pop()
    {
        for (float t = 0; t < popTime; t += Time.deltaTime)
        {
            float percent = t / popTime;
            rect.localScale = Vector3.one * Mathf.Sqrt(percent);
            yield return null;
        }

        rect.localScale = Vector3.one;
        yield return new WaitForSeconds(lifetime - popTime * 2F);

        for (float t = 0; t < popTime; t += Time.deltaTime)
        {
            float percent = t / popTime;
            rect.localScale = Vector3.one * Mathf.Sqrt(1 - percent);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        rect = (RectTransform)transform;
        canvas = (RectTransform) rect.parent;
        StartCoroutine(Pop());
        GetComponent<Text>().text = text;
    }
}