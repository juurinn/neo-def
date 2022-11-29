using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para> Object pooling for better bullet spawning performace. </para>
/// <para> Generates the pool to the specified size when turret is first created. </para>
/// <para> Pool resizing should be avoided if possible. </para>
/// </summary>
public class BulletPool : MonoBehaviour {

    [SerializeField] private Turret turret;
    [SerializeField] private int poolSize = 32; // This can't be changed during runtime
    [SerializeField] private bool resizeable; // Resizes the pool size *= 2

    private Queue<GameObject> bulletQueue;

    private const int kGrowthFactor = 2;
    public int Capacity { get; private set; }

    private void Awake() {
        Capacity = poolSize;
        bulletQueue = new Queue<GameObject>(poolSize);
        FillQueue();
    }

    /// <summary>
    /// Get GameObject from the pool.
    /// If pool is empty, resize it if resizing is allowed.
    /// </summary>
    /// <returns> Free GameObject from the pool. If none is found and pool is not resizeable returns null instead. </returns>
    public GameObject GetGameObjectFromPool() {
        int queueCount = bulletQueue.Count;
        if (queueCount == 0 && !resizeable) {
            Debug.LogWarning("[BulletPool]: Trying to get object from a pool but the pool is empty and resizing is disabled. Consider a higher starting pool size.");
            return null;
        } else if (queueCount == 0) {
            ResizePool();
        }
        return bulletQueue.Dequeue();
    }

    /// <summary>
    /// Return used GameObject to queue.
    /// </summary>
    /// <param name="m_GameObject"> GameObject to return. </param>
    public void ReturnGameObjectToPool(GameObject m_GameObject) {
        m_GameObject.SetActive(false);
        bulletQueue.Enqueue(m_GameObject);
    }

    /// <summary>
    /// Fills up the entire queue with GameObjects.
    /// </summary>
    private void FillQueue() {
        for (int i = 0; i < Capacity; i++) {
            GenerateNewObject();
        }
    }

    /// <summary>
    /// <para> Apparently microsoft is bunch of mongoloids and they decided that nobody needs to access Queue capacity. </para>
    /// <para> So, i just have to guess the current capacity. *= 2 The old capacity when the old capacity is met. </para>
    /// <para> Bonus: Unity uses .NET version that does not support Queue.EnsureCapacity which is also major bs. </para>
    /// </summary>
    private void ResizePool() {
        Capacity *= kGrowthFactor;
        Debug.LogWarning("[BulletPool]: The pool was resized to: [" + Capacity + "]. Consider a higher starting pool size.");
        for (int i = 0; i < Capacity / 2; i++) {
            GenerateNewObject();
        }
    }

    /// <summary>
    /// Instantiates a new GameObject to the pool and enqueues it.
    /// </summary>
    private void GenerateNewObject() {
        GameObject obj = Instantiate(turret.projectileToSpawn);
        obj.transform.parent = transform;

        // Not sure if this is better than having the refrence set in bullet start
        // Causes spike when placing a turret but will not cause spikes after the fact
        if (obj.TryGetComponent(out Bullet bullet)) {
            bullet.SetPoolRef(this);
            bullet.SetTurretRef(turret);
        } else {
            Debug.LogWarning("[BulletPool]: Could not get bullet component while creating a bullet!");
        }

        obj.SetActive(false);
        bulletQueue.Enqueue(obj);
    }

}
