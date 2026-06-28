using System.Collections.Generic;
using UnityEngine;

public class AvailableSpaceManager : MonoBehaviour
{
    [SerializeField]
    private GameObject AvailableSpacePrefab;

    [SerializeField]
    private List<AvailableSpace> AvailableSpaces = new List<AvailableSpace>();

    public void InitializeSpace(Vector3 size, Vector3 position)
    {
        GameObject newSpaceObject = Instantiate(AvailableSpacePrefab, position, Quaternion.identity);
        newSpaceObject.transform.localScale = size;
        newSpaceObject.transform.SetParent(transform, true);

        AvailableSpaces.Add(newSpaceObject.GetComponent<AvailableSpace>());
    }

    public void UpdateSpace()
    {
        List<AvailableSpace> newSpaces = new List<AvailableSpace>();

        foreach(AvailableSpace space in AvailableSpaces)
        {
            newSpaces.AddRange(CalculateSpace(space));
        }

        AvailableSpaces = newSpaces;
    }

    private List<AvailableSpace> CalculateSpace(AvailableSpace space)
    {
        return new List<AvailableSpace> { space };
    }
}
