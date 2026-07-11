using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UsedSpaceChecker : MonoBehaviour
{
    [SerializeField]
    private bool HasCollision = false;

    [SerializeField]
    private List<Transform> Collisions = new List<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tags.UsedSpace))
        {
            HasCollision = true;
            Collisions.Add(other.transform);
        }
    }

    public Vector3 GetClampedSize()
    {
        return transform.localScale;

        if (!HasCollision)
        {
            return transform.localScale;
        }

        float maxTop = Mathf.Min(transform.GetTopBound(), Collisions.Where(c => c.position.y > transform.position.y).Select(c => c.GetBottomBound()).DefaultIfEmpty(transform.GetTopBound()).Min());
        float maxBottom = Mathf.Max(transform.GetBottomBound(), Collisions.Where(c => c.position.y < transform.position.y).Select(c => c.GetTopBound()).DefaultIfEmpty(transform.GetBottomBound()).Max());

        float maxRight = Mathf.Min(transform.GetRightBound(), Collisions.Where(c => c.position.x > transform.position.x).Select(c => c.GetLeftBound()).DefaultIfEmpty(transform.GetRightBound()).Min());
        float maxLeft = Mathf.Max(transform.GetLeftBound(), Collisions.Where(c => c.position.x < transform.position.x).Select(c => c.GetRightBound()).DefaultIfEmpty(transform.GetLeftBound()).Max());

        float maxForward = Mathf.Min(transform.GetForwardBound(), Collisions.Where(c => c.position.z > transform.position.z).Select(c => c.GetBackBound()).DefaultIfEmpty(transform.GetForwardBound()).Min());
        float maxBack = Mathf.Max(transform.GetBackBound(), Collisions.Where(c => c.position.z < transform.position.z).Select(c => c.GetForwardBound()).DefaultIfEmpty(transform.GetBackBound()).Max());

        float y = GetDimension(transform.position.y, maxTop, maxBottom);
        float x = GetDimension(transform.position.x, maxLeft, maxRight);
        float z = GetDimension(transform.position.z, maxForward, maxBack);

        return new Vector3(x, y, z);
    }


    private float GetDimension(float center, float offsetOne, float offsetTwo)
    {
        float distanceOne = Mathf.Abs(center - offsetOne);
        float distanceTwo = Mathf.Abs(center - offsetTwo);

        return Mathf.Min(distanceOne, distanceTwo) * 2f;
    }
}
