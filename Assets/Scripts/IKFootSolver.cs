using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{

    [SerializeField] private IKFootSolver otherFoot = default;
    [SerializeField] private Transform rayOrigin = default;
    internal LayerMask terrainLayer = default;
    internal Transform body = default;
    internal float speed = 1;
    internal float stepDistance = 4;
    internal float stepLength = 4;
    internal float stepHeight = 1;
    internal float stepDistanceTolarance = 0.01f;
    private Vector3 oldPosition, currentPosition, newPosition;
    private Vector3 oldNormal, currentNormal, newNormal;
    private float lerp;
    private Vector3 raycastPoint;
    private float heightOffset;

    private void Start()
    {
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;
        heightOffset = transform.position.y;
        // Debug.Log(gameObject.name + ": heightOffset: " + heightOffset);
        // Debug.Log(gameObject.name + ": body.up * heightOffset: " + body.up.normalized * heightOffset);
    }

    // Update is called once per frame

    void Update()
    {
        transform.position = currentPosition;
        transform.up = currentNormal;

        raycastPoint = rayOrigin.position;
        Ray ray = new Ray(raycastPoint, body.up * -1);

        if (Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer.value))
        {
            // Debug.Log("Raycast hit " + info.point);
            bool overDistance = Vector3.Distance(newPosition, info.point) > stepDistance + stepDistanceTolarance;

            if (overDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = info.point + (body.forward * stepLength * direction);
                // newPosition = info.point + (body.forward * stepLength * direction) + body.up.normalized * heightOffset;

                // newPosition = info.point;

                newNormal = info.normal;
            }
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(newPosition, 0.01f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(raycastPoint, 0.01f);
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }
}