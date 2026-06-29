using UnityEngine;

public class RoomGenerator : Singleton<RoomGenerator>
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject AvailableSpaceManager;
    [SerializeField] 
    private GameObject RoomCarvingPrefab;

    [Header("Instances")]
    [SerializeField]
    private AvailableSpaceManager RoomGenerationSpaceManager;
    [SerializeField]
    private GameObject RoomCarvingInstance;

    const float PlayingFieldSize = 100f;
    const float PlayingFieldHeight = 50f;

    const float MaxRoomSize = 10f;
    const float MinRoomSize = 5f;
    const float MaxRoomHeight = 5f;
    const float MinRoomHeight = 2.5f;

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

    private void Update()
    {
        if (InputManager.GetInstance().IsJump)
        {
            CarveSpace();
        }
    }

    private void CarveSpace()
    {
        if (RoomCarvingInstance == null)
        {
            RoomCarvingInstance = Instantiate(RoomCarvingPrefab, GetRandomPosition(), Quaternion.identity);
            RoomCarvingInstance.transform.localScale = GetRandomSize();
            RoomCarvingInstance.transform.SetParent(transform);

            return;
        }
        else
        {
            RoomGenerationSpaceManager.CarveSpaces();

            Destroy(RoomCarvingInstance);
            RoomCarvingInstance = null;

            RoomGenerationSpaceManager.ResetSpaces();
        }
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(-PlayingFieldSize / 2f, PlayingFieldSize / 2f),
            Random.Range(-PlayingFieldHeight / 2f, PlayingFieldHeight / 2f),
            Random.Range(-PlayingFieldSize / 2f, PlayingFieldSize / 2f)
        );
    }

    private Vector3 GetRandomSize()
    {
        return new Vector3(
            Random.Range(MinRoomSize, MaxRoomSize),
            Random.Range(MinRoomHeight, MaxRoomHeight),
            Random.Range(MinRoomSize, MaxRoomSize)
        );
    }
}
