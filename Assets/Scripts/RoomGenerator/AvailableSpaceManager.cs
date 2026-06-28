using System.Collections.Generic;
using System.Linq;
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
        List<GameObject> oldObjects = AvailableSpaces.Select(s => s.gameObject).ToList();
        List<AvailableSpace> newSpaces = new List<AvailableSpace>();

        foreach(AvailableSpace space in AvailableSpaces)
        {
            newSpaces.AddRange(GetNewSpace(space));
        }

        AvailableSpaces = newSpaces;
        foreach(GameObject oldSpaceObjects in oldObjects)
        {
            Destroy(oldSpaceObjects);
        }
    }

    private List<AvailableSpace> GetNewSpace(AvailableSpace space)
    {
        List<AvailableSpace> newSpaces = new List<AvailableSpace>();
        List<(Vector3 position, Vector3 size)> newTransforms = space.GetNewSpaceTransforms();

        foreach((Vector3 position, Vector3 size) in newTransforms)
        {
            GameObject newSpaceObject = Instantiate(AvailableSpacePrefab, position, Quaternion.identity);
            newSpaceObject.transform.localScale = size;
            newSpaceObject.transform.SetParent(transform, true);

            newSpaces.Add(newSpaceObject.GetComponent<AvailableSpace>());
        }

        return newSpaces;
    }
}
