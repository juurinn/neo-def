using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pooling for enemy GameObjects.
/// </summary>
public class EnemyPools : MonoBehaviour {

    static public EnemyPools instance;

    [Header("Objects To Pool")]
    [SerializeField] private GameObject[] enemies;

    [Header("Pool Settings")]
    [Tooltip("Size of the initial pools to generate.")]
    [SerializeField] private int poolSize = 32; // This can't be changed during runtime
    [Tooltip("Is resizing the pools allowed, this might cause lag spikes depending on initial pool size.")]
    [SerializeField] private bool resizeable; // Resizes the pool size *= 2

    [Header("Sorting")]
    [Tooltip("Do the objects have their own sorting orders. [Requires PoolObject component]")]
    [SerializeField] private bool useSortOrder;
    [Tooltip("At what sorting order does the sorting start.")]
    [SerializeField] private int startingOrder = 0;
    [Tooltip("How much does the sort order increase per object pool.")]
    [SerializeField] private int orderIncrement = 1000;

    /// <summary>
    /// Capacities of each pool.
    /// </summary>
    public int[] Capacities { get; private set; }

    /// <summary>
    /// Has the pool been generated.
    /// </summary>
    public bool Configured { get; private set; } = false;

    private const int kGrowthFactor = 2;

    private Queue<GameObject>[] enemyQueues;
    private Transform[] poolParents;
    private int m_CurrentStartingOrder;
    private int[] m_CurrentSortingOrders;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("[EnemyPools]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Initialize pools.
    /// </summary>
    public void Configure() {
        if (!GameManager.instance.isInGame) {
            Debug.LogError("[EnemyPools]: Trying to create enemy pools while the game is not started yet!");
            return;
        }
        m_CurrentStartingOrder = startingOrder;
        enemyQueues = new Queue<GameObject>[enemies.Length];
        SetDefaultSortingOrders();
        SetDefaultCapacities();
        GenerateParents();
        FillQueues();
        Configured = true;
    }

    /// <summary>
    /// Get enemy from a pool.
    /// </summary>
    /// <param name="m_PoolIndex"> The index of the enemy to get. </param>
    /// <returns> Enemy GameObject </returns>
    public GameObject GetGameObjectFromPool(int m_PoolIndex) {
        int queueCount = enemyQueues[m_PoolIndex].Count;
        if (queueCount == 0 && !resizeable) {
            Debug.LogWarning("[EnemyPools]: Trying to get object from a pool but the pool is empty and resizing is disabled. Consider a higher starting pool size.");
            return null;
        } else if (queueCount == 0) {
            ResizePool(m_PoolIndex);
        }
        return enemyQueues[m_PoolIndex].Dequeue();
    }

    /// <summary>
    /// Return enemy to a pool.
    /// </summary>
    /// <param name="m_GameObject"> Enemy GameObject to return. </param>
    /// <param name="m_PoolIndex"> The index of the pool which the enemy belongs to. </param>
    public void ReturnGameObjectToPool(GameObject m_GameObject, int m_PoolIndex) {
        m_GameObject.SetActive(false);
        enemyQueues[m_PoolIndex].Enqueue(m_GameObject);
    }

    /// <summary>
    /// Sets default sorting orders.
    /// </summary>
    private void SetDefaultSortingOrders() {
        m_CurrentSortingOrders = new int[enemies.Length];
        for (int i = 0; i < m_CurrentSortingOrders.Length; i++) {
            m_CurrentSortingOrders[i] = m_CurrentStartingOrder;
            m_CurrentStartingOrder += orderIncrement;
        }
    }

    /// <summary>
    /// Set pool capacities to the default pool size.
    /// </summary>
    private void SetDefaultCapacities() {
        Capacities = new int[enemyQueues.Length];
        for (int i = 0; i < enemyQueues.Length; i++) {
            Capacities[i] = poolSize;
        }
    }

    /// <summary>
    /// Generates parent empty GameObjects to hold the pools.
    /// </summary>
    private void GenerateParents() {
        poolParents = new Transform[enemyQueues.Length];
        for (int i = 0; i < enemyQueues.Length; i++) {
            GameObject poolParent = new GameObject(enemies[i].name + "Pool"); // Create empty object with enemy name
            poolParent.transform.parent = gameObject.transform; // Parent it to this GameObject
            poolParents[i] = poolParent.transform; // Add it to parent list
        }
    }

    /// <summary>
    /// Initialize all the queues and fills them.
    /// </summary>
    private void FillQueues() {
        for (int i = 0; i < enemyQueues.Length; i++) {
            enemyQueues[i] = new Queue<GameObject>(Capacities[i]);
            FillPool(i);
        }
    }

    /// <summary>
    /// Fills up the correct queue with GameObjects.
    /// </summary>
    /// <param name="m_PoolIndex"> Index of the queue to be filled with enemies. </param>
    private void FillPool(int m_PoolIndex) {
        for (int i = 0; i < Capacities[m_PoolIndex]; i++) {
            GenerateNewObject(m_PoolIndex);
        }
    }

    /// <summary>
    /// Instantiates a new GameObject to the correct pool and enqueues it.
    /// </summary>
    /// <param name="m_PoolIndex"> Index of the pool to generate the object to. </param>
    private void GenerateNewObject(int m_PoolIndex) {
        GameObject obj = Instantiate(enemies[m_PoolIndex]);
        obj.transform.parent = poolParents[m_PoolIndex].transform;

        if (useSortOrder) {
            if (obj.TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer)) {
                renderer.sortingOrder = m_CurrentSortingOrders[m_PoolIndex]--;
            } else {
                Debug.LogError("[EnemyPools]: Could not get SpriteRenderer component while creating an enemy!");
            }
        }

        if (obj.TryGetComponent<Enemy>(out Enemy m_Enemy)) {
            // Set enemy path and enemy index
            m_Enemy.path = AI.instance.path;
            m_Enemy.enemyIndex = m_PoolIndex;
        } else {
            Debug.LogError("[EnemyPools]: Could not get Enemy component while creating an enemy!");
        }

        obj.SetActive(false);
        enemyQueues[m_PoolIndex].Enqueue(obj);
    }

    /// <summary>
    /// Resizes a pool capazity * 2 by index.
    /// </summary>
    /// <param name="m_PoolIndex"> Pool index to resize. </param>
    private void ResizePool(int m_PoolIndex) {
        Capacities[m_PoolIndex] *= kGrowthFactor;
        Debug.LogWarning("[EnemyPools]: The [" + enemies[m_PoolIndex].name + "] pool was resized to: [" + Capacities[m_PoolIndex] + "]. Consider a higher starting pool size.");
        for (int i = 0; i < Capacities[m_PoolIndex] / 2; i++) {
            GenerateNewObject(m_PoolIndex);
        }
    }

}
