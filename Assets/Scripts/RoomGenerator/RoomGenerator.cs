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

    private const float PlayingFieldSize = 100f;
    private const float PlayingFieldHeight = 50f;

    private const int TotalRooms = 5;
    private int CurrentRooms = 0;
    

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
        CreateRooms();
    }

    private void CreateRooms()
    {
        if (CurrentRooms >= TotalRooms)
        {
            return;
        }

        if (RoomCarvingInstance == null)
        {
            GenerateCarving();
        }
        else
        {
            GenerateRoom();
            CurrentRooms++;
        }
    }

    private void GenerateCarving()
    {
        if (UsedSpaceManager.CurrentState == UsedSpaceManagerState.Waiting)
        {
            UsedSpaceManager.StartComputingSizeAndPosition(AvailableSpaceManager.GetAvailableSpaces());
            return;
        }
        if (UsedSpaceManager.CurrentState != UsedSpaceManagerState.Finished)
        {
            return;
        }
        UsedSpaceManager.ResetState();

        RoomCarvingInstance = Instantiate(RoomCarvingPrefab, UsedSpaceManager.NewPosition, Quaternion.identity);
        RoomCarvingInstance.transform.localScale = UsedSpaceManager.NewSize;
        RoomCarvingInstance.transform.SetParent(transform);
        Debug.Break();

        return;
    }

    private void GenerateRoom()
    {
        Debug.Break();
        AvailableSpaceManager.CarveSpaces();

        Destroy(RoomCarvingInstance);
        RoomCarvingInstance = null;

        AvailableSpaceManager.ResetSpaces();
    }
}
