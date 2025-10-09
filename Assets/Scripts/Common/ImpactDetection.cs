using UnityEngine;

public class ImpactDetection : MonoBehaviour
{
    public bool IncomingCollision;
    public LayerMask CollisionLayer;
    RaycastHit hit;

    void Update()
    {
        if (Physics.Raycast(transform.localPosition, transform.forward, out hit, Mathf.Infinity, 0))
        {
            IncomingCollision = true;
        }
        else
        {
            IncomingCollision = false;
        }
    }
}
