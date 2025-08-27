using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpawner : MonoBehaviour
{
    private const float WIND_FORCE_LIMIT = 4f;

    [SerializeField] private WindManager windPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float windForce;

    private ObjectsPool<WindManager> windPool;

    private void Awake()
    {
        windPool = new ObjectsPool<WindManager>(windPrefab);
    }

    public void SpawnWindsAtRandomPoints(int windForceMultiplier)
    {
        int randomPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);

        SpawnWind(randomPointIndex, windForceMultiplier);
    }

    public void SpawnWind(int pointIndex, int windForceMultiplier = 0)
    {
        if(pointIndex < spawnPoints.Length)
        {
            WindManager newWind = windPool.GetObject(spawnPoints[pointIndex].position);

            float windForce = this.windForce + (0.75f * windForceMultiplier);

            if(windForce > WIND_FORCE_LIMIT)
            {
                windForce = WIND_FORCE_LIMIT;
            }

            newWind.Blow(windForce);
        }
    }
}
