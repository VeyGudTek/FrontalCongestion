using UnityEngine;

public class RoomGenerator : Singleton<RoomGenerator>
{
    [SerializeField]
    private GameObject AvailableSpaceManager;

    private AvailableSpaceManager RoomGenerationSpaceManager;
    const float PlayingFieldSize = 100f;
    const float PlayingFieldHeight = 50f;

    private void Awake()
    {
        InitializeSingleton(this);
    }

    private void Start()
    {
        InitializeSpace();
    }

    private void InitializeSpace()
    {
        GameObject spaceManagerObject = Instantiate(AvailableSpaceManager, transform);
        RoomGenerationSpaceManager = spaceManagerObject.GetComponent<AvailableSpaceManager>();
        RoomGenerationSpaceManager.InitializeSpace(new Vector3(PlayingFieldSize, PlayingFieldHeight, PlayingFieldSize), Vector3.zero);
    }
}
