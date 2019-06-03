using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : SnakeNode
{
    public float velocity;
    public float timeNodeGrow;
    private float timeLastNode;
    public int maxNodes;
    public int nodes;
    public float nodeGap;
    public SnakeNode prefabNode;

    public void UpdateSnake()
    {
        // Calculate movement
        float netRotation = 0;
        foreach (Planet planet in Helper.planets)
        {
            netRotation += GetRotationBy(planet);
        }
        netRotation *= Mathf.Rad2Deg;
        transform.Rotate(0, 0, netRotation * Time.deltaTime);
        transform.Translate(0, velocity * Time.deltaTime, 0);

        if (Time.time - timeLastNode >= timeNodeGrow)
        {
            timeLastNode = Time.time;

            Debug.Log("Increasing nodes: " + (nodes > 0 && nodes < maxNodes));

            if (nodes > 0 && nodes < maxNodes)
            {
                IncreaseNodes(1);
            }
        }

        SnakeNode node = after;
        while (node != null)
        {
            Transform trans = node.transform;
            Vector3 difference = node.before.transform.position - trans.position;
            float distanceToMove = difference.magnitude - nodeGap;
            trans.position = Vector3.MoveTowards(trans.position, node.before.transform.position, distanceToMove);
            float z = Mathf.Rad2Deg * Mathf.Atan2(-difference.x, difference.y);
            trans.rotation = Quaternion.Euler(0, 0, z);
            node = node.after;
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
        if (coll.tag == "SpaceObject")
        {
            Collide(coll.GetComponent<Planet>());
        }
    }

    private void Collide(Planet planet)
    {
        planet.Destroy();

        SnakeNode node = this;
        while (node.after != null) node = node.after;
        for (int i = 0; i < 4; i++)
        {
            if (node == this)
            {
                Crash();
                return;
            }

            //Instantiate(prefabEffect, node.transform.position, Quaternion.identity);
            node = node.before;
            Destroy(node.after.gameObject);
            node.after = null;
            nodes--;
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
        while (lastNode.after != null)
        {
            lastNode = lastNode.after;
        }

        for (int i = 0; i < num; i++)
        {
            SnakeNode newNode = Instantiate(prefabNode, Vector3.zero, Quaternion.identity);
            newNode.gameObject.name = "Snake " + nodes;
            newNode.transform.position = lastNode.transform.position + lastNode.transform.up * -nodeGap;
            newNode.before = lastNode;
            lastNode.after = newNode;
            lastNode = newNode;
            nodes++;
        }
    }

    private new void Start()
    {
        base.Start();

        SnakeNode mostRecent = this;
        before = this;

        for (int i = 1; i < maxNodes; i++)
        {
            // Create node
            Vector3 position = mostRecent.transform.position;
            position += Vector3.down * nodeGap;
            SnakeNode node = Instantiate(prefabNode, position, Quaternion.identity);
            node.gameObject.name = "Snake " + i;

            // Set varibles
            node.before = mostRecent;
            mostRecent.after = node;
            mostRecent = node;
        }

        nodes = maxNodes;
    }
}
