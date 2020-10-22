using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public Transform ObjectToThrow;
    public Transform PositionToThrowFrom;
    public Vector3 ThrowDirection;
    public float ThrowPower;
    public float MomentToThrow = 0.5f;

    public void ThrowTheObject()
    {
        Transform obj = GameObject.Instantiate<Transform>(ObjectToThrow, PositionToThrowFrom.position, Quaternion.identity);
        obj.GetComponent<Rigidbody>().velocity = transform.TransformDirection(ThrowDirection).normalized * ThrowPower;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(PositionToThrowFrom.position, PositionToThrowFrom.position + transform.TransformDirection(ThrowDirection).normalized * ThrowPower);
    }
}
