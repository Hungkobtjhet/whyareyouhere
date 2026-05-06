using UnityEngine;
using System.Collections;

public class DynamicMapGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject framePrefab;
    public GameObject[] randomCubePrefabs;

    [Header("Hierarchy Organization")]
    public Transform frameParent;
    private Transform innerCubesContainer;
    private MapMover mapMoverScript;

    [Header("Map Dimensions (Tùy chỉnh độ lớn)")]
    [Tooltip("Chiều cao của bản đồ")]
    public int heightY = 52;
    [Tooltip("Chiều sâu/rộng của bản đồ")]
    public int depthZ = 52;
    [Tooltip("Vị trí trục Y bắt đầu sinh gạch (thường để bằng 1 nửa heightY)")]
    public float startY = 26f;
    private float highestLogicalRow;

    [Header("Cài đặt Đạn")]
    public Vector3 currentAmmoPos = new Vector3(0, 1f, 26f);
    public Vector3 nextAmmoPos = new Vector3(0, 1f, 21f);

    private GameObject currentShooterBlock;
    private GameObject nextShooterBlock;
    private DVDLauncher currentLauncherScript;
    private bool isWaitingToSpawn = false;

    void Start()
    {
        GameObject containerObj = new GameObject("Inner_Cubes_Container");
        containerObj.transform.SetParent(this.transform);
        containerObj.transform.localPosition = Vector3.zero;

        mapMoverScript = containerObj.AddComponent<MapMover>();
        innerCubesContainer = containerObj.transform;

        GenerateInitialMap();
        SpawnNextAmmoOnly();
        PromoteNextToCurrent();
    }

    void GenerateInitialMap()
    {
        for (int y = 0; y < heightY; y++)
        {
            for (int z = 0; z < depthZ; z++)
            {
                bool isBorder = (y == 0 || y == heightY - 1 || z == 0 || z == depthZ - 1);
                if (isBorder && framePrefab != null)
                {
                    GameObject frameBlock = Instantiate(framePrefab, frameParent);
                    frameBlock.transform.localPosition = new Vector3(0, y, z);

                    if (y == heightY - 1) frameBlock.name = "Wall_Ceiling";
                    else if (y == 0) frameBlock.name = "Wall_Bottom";
                    else frameBlock.name = "Wall_Side";

                    frameBlock.tag = "Wall";
                }
            }
        }

        // Đã sử dụng biến public startY thay vì fix cứng 26f
        float endY = heightY - 2;
        highestLogicalRow = endY;

        for (float y = startY; y <= endY; y += 1f)
        {
            SpawnRow(y);
        }
    }

    void SpawnRow(float baseRowY)
    {
        if (randomCubePrefabs.Length == 0) return;
        for (int z = 1; z < depthZ - 1; z++)
        {
            float yOffset = (z % 2 == 0) ? 0.5f : 0f;
            float finalY = baseRowY + yOffset;
            int randomIndex = Random.Range(0, randomCubePrefabs.Length);

            GameObject selectedCube = Instantiate(randomCubePrefabs[randomIndex], innerCubesContainer);
            selectedCube.transform.localPosition = new Vector3(0, finalY, z);
        }
    }

    void SpawnNextAmmoOnly()
    {
        if (randomCubePrefabs.Length == 0) return;
        int randomIndex = Random.Range(0, randomCubePrefabs.Length);

        nextShooterBlock = Instantiate(randomCubePrefabs[randomIndex], this.transform);
        nextShooterBlock.transform.localPosition = nextAmmoPos;

        BlockShatterMechanic shatter = nextShooterBlock.GetComponent<BlockShatterMechanic>();
        if (shatter != null) shatter.enabled = false;
    }

    void PromoteNextToCurrent()
    {
        if (nextShooterBlock == null) SpawnNextAmmoOnly();

        currentShooterBlock = nextShooterBlock;
        currentShooterBlock.transform.localPosition = currentAmmoPos;

        currentLauncherScript = currentShooterBlock.AddComponent<DVDLauncher>();

        SpawnNextAmmoOnly();

        if (mapMoverScript != null)
        {
            mapMoverScript.MoveMapStepDownSmooth();
            highestLogicalRow += 1f;
            SpawnRow(highestLogicalRow);
        }
    }

    void Update()
    {
        if ((currentShooterBlock == null || currentLauncherScript == null) && !isWaitingToSpawn)
        {
            StartCoroutine(WaitAndReload());
        }
    }

    private IEnumerator WaitAndReload()
    {
        isWaitingToSpawn = true;
        yield return new WaitForSeconds(0.4f);
        PromoteNextToCurrent();
        isWaitingToSpawn = false;
    }
}