using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemyList;
    [SerializeField] private float spawnPosXMax = 7.5f;
    [SerializeField] private float spawnPosXMin = -7.5f;
    [SerializeField] private float spawnPosYMax = 1.6f;
    [SerializeField] private float spawnPosYMin = -4.8f;
    [SerializeField] private float distanceAwayFromPlayer = 2.4f;

    private void Start()
    {
        RoomManager.Instance.OnEnterNewRoom += RoomManager_OnEnterNewRoom;
    }

    private void RoomManager_OnEnterNewRoom(object sender, System.EventArgs e)
    {
        if (RoomManager.Instance.GetCurRoom().GetRoomType() == RoomManager.RoomType.Regular)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        int spawnCount = Random.Range(2, 5);

        List<Enemy> spawnableEnemyList = new List<Enemy>();

        for (int i = 0; i < enemyList.Count; i++)
        {
            spawnableEnemyList.Add(enemyList[i]);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Enemy randomEnemy = spawnableEnemyList[Random.Range(0, spawnableEnemyList.Count)];
            spawnableEnemyList.Remove(randomEnemy);
            Enemy enemy = Instantiate(randomEnemy, transform);
            enemy.transform.position = GetRandomPosAwayFromPlayer();
        }
    }

    private Vector3 GetRandomPosAwayFromPlayer()
    {
        Vector3 posAwayFromPlayer = Vector3.zero;
        float randomPosX, randomPosY = 0;
        System.Random random = new System.Random();
        if (Random.Range(0, 2) == 0)
        {
            randomPosX = (float) random.NextDouble() * (-distanceAwayFromPlayer - spawnPosXMin) + spawnPosXMin;
            randomPosY = (float) random.NextDouble() * (-distanceAwayFromPlayer - spawnPosYMin) + spawnPosYMin;
        } else
        {
            randomPosX = (float) random.NextDouble() * (spawnPosXMax - distanceAwayFromPlayer) + distanceAwayFromPlayer;
            randomPosY = (float)random.NextDouble() * (spawnPosYMax - distanceAwayFromPlayer) + distanceAwayFromPlayer;
        }
        posAwayFromPlayer.x = randomPosX;
        posAwayFromPlayer.y = randomPosY;
        return posAwayFromPlayer;
    }
}
