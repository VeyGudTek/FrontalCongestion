using UnityEngine;

public class RoomGenerator : Singleton<RoomGenerator>
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject AvailableSpaceManagerPrefab;
    [SerializeField]
    private GameObject UsedSpaceManagerPrefab;
    [SerializeField] 
    private GameObject RoomCarvingPrefab;

    [Header("Instances")]
    [SerializeField]
    private AvailableSpaceManager AvailableSpaceManager;
    [SerializeField]
    private UsedSpaceManager UsedSpaceManager;
    [SerializeField]
    private GameObject RoomCarvingInstance;

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
        GameObject spaceManagerObject = Instantiate(AvailableSpaceManagerPrefab, transform);
        AvailableSpaceManager = spaceManagerObject.GetComponent<AvailableSpaceManager>();
        AvailableSpaceManager.InitializeSpace(new Vector3(PlayingFieldSize, PlayingFieldHeight, PlayingFieldSize), Vector3.zero);

        GameObject usedSpaceManagerObject = Instantiate(UsedSpaceManagerPrefab, transform);
        UsedSpaceManager = usedSpaceManagerObject.GetComponent<UsedSpaceManager>();
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
            (Vector3 size, Vector3 position) = UsedSpaceManager.GetNewRoomSizeAndPosition(AvailableSpaceManager.GetAvailableSpaces());

            RoomCarvingInstance = Instantiate(RoomCarvingPrefab, position, Quaternion.identity);
            RoomCarvingInstance.transform.localScale = size;
            RoomCarvingInstance.transform.SetParent(transform);

            return;
        }
        else
        {
            UsedSpaceManager.RegisterRoom(RoomCarvingInstance);
            AvailableSpaceManager.CarveSpaces();

            Destroy(RoomCarvingInstance);
            RoomCarvingInstance = null;

            AvailableSpaceManager.ResetSpaces();
        }
    }
}
