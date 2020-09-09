using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    BadBlock,
    GoodBlock
}
public enum GenerationType
{
    Ground,
    Air
}
public class BlockGenerator : MonoBehaviour
{
    public GameObject badBlockPrefab, goodBlockPrefab;
    public PhysicsManager collectorPM;
    public float runwaySpeed;
    
    public float minX, maxX;
    public float airBlockGenHeight;
    public float groundBlockGenVerticalVelocity;
    public float goodBlockProbability;
    //private List<GameObject> m_Children;
    //private int m_CurrentGeneratedBlocks;
    //private Vector3 m_GenVelocity;
    private float m_TimeOfLastGeneration;
    private float m_TimeBetweenGenerations;
    private Vector3 m_Acceleration;
    private PhysicsEngine m_CurrentPhysEngine;
    private bool m_LevelStarted;
    private int m_CurrentLevel;

    private void Awake()
    {
        m_Acceleration = new Vector3(0.0f, -9.81f, 0.0f);
        m_CurrentPhysEngine = PhysicsEngine.NativeLibrary;
        m_LevelStarted = false;
        m_CurrentLevel = 1;
        m_TimeBetweenGenerations = 3.0f;
        //m_Children = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.OnClicked += OnPhysicsChange;
        EventManager.OnGameStarted += StartGenerating;
        EventManager.OnLevelIncreased += OnLevelIncreased;
    }


    // Update is called once per frame
    void Update()
    {
        if (m_LevelStarted && Time.time > m_TimeOfLastGeneration + m_TimeBetweenGenerations)
        {
            m_TimeOfLastGeneration = Time.time;
            GenerationType gtype = (GenerationType)Random.Range(0, System.Enum.GetValues(typeof(GenerationType)).Length);
            if (gtype == GenerationType.Ground)
                GenerateGroundBlocks();
            else
                GenerateAirBlock();
        }
    }

    private void GenerateBlocks(bool ground, int numBlocks)
    {
        float originY = ground ? 0.0f : airBlockGenHeight;   // elevation of generation: on ground or in air.
        float velocityY = ground ? groundBlockGenVerticalVelocity : 0.0f; // if generated on ground, bounce up, if not, fall down.
        Vector3 genVelocity = new Vector3(0.0f, velocityY, -runwaySpeed);

        float originX = Random.Range(minX, maxX);
        float increment = originX >= 0 ? -1.0f : 1.0f;


        //BlockType bt = (BlockType)Random.Range(0, System.Enum.GetValues(typeof(GenerationType)).Length); //TODO: implement different types.
        

        for (int i = 0; i <= numBlocks - 1; i++)
        {
            BlockType btype = Random.Range(0.0f, 1.0f) > goodBlockProbability ? BlockType.BadBlock : BlockType.GoodBlock;
            float instanceX = originX + i * increment * 1.1f;
            Vector3 instancePos = transform.position + new Vector3(instanceX, originY, 0.0f);
            GameObject instance;
            switch (btype)
            {
                case BlockType.GoodBlock:
                    instance = Instantiate(goodBlockPrefab, instancePos, Quaternion.identity, transform);
                    break;
                case BlockType.BadBlock:
                    instance = Instantiate(badBlockPrefab, instancePos, Quaternion.identity, transform);
                    break;
                default:
                    Debug.LogError("Unknown block type");
                    instance = new GameObject();
                    break;
            }

            PhysicsManager pm = instance.GetComponent<PhysicsManager>();
            pm.SetPhysicsEngine(m_CurrentPhysEngine);
            pm.SetVelocity(genVelocity);
            pm.SetAcceleration(m_Acceleration);
            collectorPM.AddOtherObject(pm);
            pm.AddOtherObject(collectorPM);
            
        }
        //m_Children.Add(instance);
    }

    private void GenerateGroundBlocks()
    {
        int currentGenBlocks = Mathf.FloorToInt(Random.Range(1.0f, 5.99f));
        GenerateBlocks(ground: true, numBlocks: currentGenBlocks);
    }

    private void GenerateAirBlock()
    {
        GenerateBlocks(ground: false, numBlocks: 1);
    }


    private void OnPhysicsChange()
    {
        switch (m_CurrentPhysEngine)
        {
            case PhysicsEngine.NativeLibrary:
                m_CurrentPhysEngine = PhysicsEngine.BuiltIn;
                break;
            case PhysicsEngine.BuiltIn:
                m_CurrentPhysEngine = PhysicsEngine.NativeLibrary;
                break;
        }
    }

    private void StartGenerating()
    {
        m_LevelStarted = true;
    }

    private void OnLevelIncreased()
    {
        m_CurrentLevel++;
        if(m_CurrentLevel <= 10)
        {
            m_TimeBetweenGenerations -= 0.3f;
        }
            
    }
}
