using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : SnakeNode
{
    public float velocity;
    public float timeNodeGrow;
    private float timeLastNode;
    public int maxNodes;
    private int nodes;
    public float nodeGap;
    public SnakeNode prefabNode;
    public ParticleSystem prefabEffect;

    public void UpdateSnake()
    {
        float netRotation = 0;

        foreach (Planet planet in Helper.planets)
        {
            netRotation += GetRotationBy(planet);
        }

        netRotation *= Mathf.Rad2Deg;

        transform.Rotate(0, 0, netRotation * Time.deltaTime);
        transform.Translate(0, velocity * Time.deltaTime, 0);

        SnakeNode node = next;
        while (node != null)
        {
            Transform me = node.transform;
            Transform he = node.prev.transform;
            float distance = Vector3.Distance(me.position, he.position);
            float distanceToRadius = distance - nodeGap;
            me.position = Vector3.MoveTowards(me.position, he.position, distanceToRadius);
            node = node.next;
        }

        if (Time.time - timeLastNode > timeNodeGrow)
        {
            timeLastNode = Time.time;

            if (nodes > 0 && nodes < maxNodes)
            {
                IncreaseNodes(1);
            }
        }
    }

    private float GetRotationBy(Planet planet)
    {
        Vector3 difference = planet.transform.position - transform.position;
        float angle = transform.rotation.eulerAngles.z;
        angle -= Mathf.Atan2(-difference.x, difference.y) * Mathf.Rad2Deg;
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        int direction = Mathf.Abs(angle) > 10 ? angle > 0 ? -1 : 1 : 0; // 0 for small angles
        return direction * planet.kConst / Mathf.Pow(difference.magnitude, 2F); //direction * planet.kConst / difference.magnitude;
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "Planet")
        {
            Collide(coll.GetComponent<Planet>());
        }
    }

    private void Collide(Planet planet)
    {
        planet.Destroy();

        SnakeNode node = this;
        while (node.next != null) node = node.next;
        for (int i = 0; i < 4; i++)
        {
            if (node == this)
            {
                Crash();
                return;
            }

            //Instantiate(prefabEffect, node.transform.position, Quaternion.identity);
            Destroy(node.gameObject);
            nodes--;
            node = node.prev;
        }

        Helper.cameraFollow.SetShaking(0.75F, 0.2F);
    }

    private void Crash()
    {
        // Stop level
        Helper.gameManager.StopGame();

        // Explosion effect
        Helper.cameraFollow.SetShaking(0.75F, 0.8F);

        // Bye bye
        transform.localScale = Vector3.zero;
    }

    public void IncreaseNodes(int num)
    {
        // Last node
        SnakeNode lastNode = this;
        while (lastNode.next != null)
        {
            lastNode = lastNode.next;
        }

        for (int i = 0; i < num; i++)
        {
            SnakeNode newNode = Instantiate(prefabNode, Vector3.zero, Quaternion.identity);
            newNode.transform.position = Vector3.MoveTowards(lastNode.transform.position, lastNode.prev.transform.position, -nodeGap);
            newNode.prev = lastNode;
            lastNode.next = newNode;
            lastNode = newNode;
            nodes++;
        }
    }

    private void Start()
    {
        SnakeNode mostRecent = this;
        prev = this;

        for (int i = 1; i < maxNodes; i++)
        {
            // Create node
            Vector3 position = mostRecent.transform.position;
            position += Vector3.down * nodeGap;
            SnakeNode node = Instantiate(prefabNode, position, Quaternion.identity);

            // Set varibles
            node.prev = mostRecent;
            mostRecent.next = node;
            mostRecent = node;
        }

        nodes = maxNodes;
    }
}
