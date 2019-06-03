using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    public float rotateSpeed;
    public ParticleSystem prefabEffect;
    private bool vanishing;

    public abstract void Effect();

    private void Update()
    {
        GameManager gm = GameManager.Instance;

        if (gm.playing)
        {
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
            Vector3 snake = gm.snake.transform.position;
            float distance = Vector2.Distance(transform.position, snake);
            transform.position = Vector3.MoveTowards(transform.position, snake, 3F / Mathf.Pow(distance, 2) * Time.deltaTime);

            if (!vanishing)
            {
                if (Vector2.Distance(transform.position, snake) <= 1)
                {
                    AssetManager.Instance.PlaySound("Coin");
                    Effect();
                    Destroy();
                }
            }
        }
    }

    private void Destroy()
    {
        vanishing = true;
        StartCoroutine(Vanish());
    }

    private IEnumerator Vanish()
    {
        for (float t = 0F; t <= 0.2; t += Time.deltaTime)
        {
            float percent = t / 0.2F;
            transform.localScale = Vector3.one * (1 + Mathf.Pow(percent, 2) * 0.5F);
            yield return null;
        }

        ParticleSystem ps = Instantiate(prefabEffect, transform.position, transform.rotation);
        Destroy(ps.gameObject, 2F);

        for (float t = 0.2F; t >= 0; t -= Time.deltaTime)
        {
            float percent = t / 0.2F;
            transform.localScale = Vector3.one * Mathf.Pow(percent, 2);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        Invoke("Destroy", 30F);
    }
}