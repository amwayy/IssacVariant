using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Enemy> minionList;
    [SerializeField] private List<Enemy> eliteList;
    [SerializeField] private List<Enemy> midBossList;
    [SerializeField] private float distanceAwayFromPlayer = 2.4f;
    [SerializeField] private float spawnEliteBaseProbability = .01f;
    [SerializeField] private int spawnCountMin = 2;
    [SerializeField] private int spawnCountMax = 4;

    private float spawnPosXMax;
    private float spawnPosXMin;
    private float spawnPosYMax;
    private float spawnPosYMin;

    private void Start()
    {
        RoomManager.Instance.OnEnterNewRoom += RoomManager_OnEnterNewRoom;
    }

    private void RoomManager_OnEnterNewRoom(object sender, System.EventArgs e)
    {
        spawnPosXMin = RoomManager.Instance.GetCurRoom().GetLeftLimit();
        spawnPosXMax = RoomManager.Instance.GetCurRoom().GetRightLimit();
        spawnPosYMin = RoomManager.Instance.GetCurRoom().GetDownLimit();
        spawnPosYMax = RoomManager.Instance.GetCurRoom().GetUpLimit();

        if (RoomManager.Instance.GetCurRoom().GetRoomType() == RoomManager.RoomType.Regular 
            || RoomManager.Instance.GetCurRoom().GetRoomType() == RoomManager.RoomType.Boss)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        int spawnCount = Random.Range(spawnCountMin, spawnCountMax + 1);

        float spawnEliteProbability = spawnEliteBaseProbability * GameLibrary.Instance.GetLevelCount();
        List<Enemy> spawnableMinionList = new List<Enemy>(); 
        List<Enemy> spawnableEliteList = new List<Enemy>();
        List<Enemy> spawnableMidBossList = new List<Enemy>();

        for (int i = 0; i < minionList.Count; i++)
        {
            spawnableMinionList.Add(minionList[i]);
        }
        for (int i = 0; i < eliteList.Count; i++)
        {
            spawnableEliteList.Add(eliteList[i]);
        }
        for (int i = 0; i < midBossList.Count; i++)
        {
            spawnableMidBossList.Add(midBossList[i]);
        }

        if (RoomManager.Instance.GetCurRoom().GetRoomType() == RoomManager.RoomType.Boss)
        {
            if (GameLibrary.Instance.GetLevelCount() == GameLibrary.Instance.GetMidBossLevelIndex())
            {
                Enemy randomEnemy = spawnableMidBossList[Random.Range(0, spawnableMidBossList.Count)];
                spawnableMidBossList.Remove(randomEnemy);
                Enemy enemy = Instantiate(randomEnemy, transform);
                enemy.transform.position = GetRandomPosAwayFromPlayer();
            }
        } else
        {

            for (int i = 0; i < spawnCount; i++)
            {
                System.Random random = new System.Random();
                float randomNum = (float)(random.NextDouble());
                if (randomNum < spawnEliteProbability)
                {
                    // spawn elite
                    Enemy randomEnemy = spawnableEliteList[Random.Range(0, spawnableEliteList.Count)];
                    spawnableEliteList.Remove(randomEnemy);
                    Enemy enemy = Instantiate(randomEnemy, transform);
                    enemy.transform.position = GetRandomPosAwayFromPlayer();
                }
                else
                {
                    // spawn minion
                    Enemy randomEnemy = spawnableMinionList[Random.Range(0, spawnableMinionList.Count)];
                    spawnableMinionList.Remove(randomEnemy);
                    Enemy enemy = Instantiate(randomEnemy, transform);
                    enemy.transform.position = GetRandomPosAwayFromPlayer();
                }
            }
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
