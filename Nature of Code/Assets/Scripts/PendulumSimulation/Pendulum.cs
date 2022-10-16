using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public LineRenderer line;
    public Transform ball;

    public float r;
    public float damping;

    public float gravity;

    private float velocity;
    private float acceleration;
    private Vector3 position;

    public float startingAngle;
    private float currentAngle;

    private void Awake()
    {
        currentAngle = startingAngle * Mathf.Deg2Rad;
    }

    void Update()
    {
        acceleration = gravity * Mathf.Sin(currentAngle) / r;
        velocity += acceleration * Time.deltaTime;
        velocity *= damping;

        currentAngle += velocity * Time.deltaTime;

        position = new Vector3(r * Mathf.Sin(currentAngle), r * Mathf.Cos(currentAngle), 0);
        position += line.GetPosition(0);

        ball.position = position;
        line.SetPosition(1, position);
    }
}
