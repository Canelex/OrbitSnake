using UnityEngine;

public class SnakeNode : MonoBehaviour
{
    [System.NonSerialized]
    public SnakeNode next;
    [System.NonSerialized]
    public SnakeNode prev;
}