using System.Collections.Generic;
using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    public Transform MyPlane;
    [SerializeField]
    private Colorable sprinklers;
    [SerializeField]
    private List<Transform> groupsSpawnPoints;
    [SerializeField]
    private Transform obstacleCenter; 
    [SerializeField]
    private GameObject groupPrefab;
    [SerializeField]
    private List<GameObject> obstacles;

    private SegmentColor mainColor, secondaryColor;

    private void Awake()
    {
        PickTwoColors();
    }
    void Start()
    {
        sprinklers.ChangeColor(mainColor);
        SpawnFood();
        SpawnObstacles();
    }
    private void PickTwoColors() 
    {
        mainColor = (SegmentColor)Random.Range(0, GameManager.colors.Length);

        secondaryColor = (SegmentColor)Random.Range(0, GameManager.colors.Length);

        while(mainColor == secondaryColor) // мешает нормальному распределению рандома, но на геймплее не сказалось
        {
            secondaryColor = (SegmentColor)Random.Range(0, GameManager.colors.Length);
        }
    }
    private void SpawnFood()
    {
        SegmentColor prevColor = mainColor, curGroupColor = secondaryColor;
        for (int i = 0; i < groupsSpawnPoints.Count; i++)
        {
            Vector3 positionToSpawn = groupsSpawnPoints[i].position + new Vector3(Random.Range(0, 0.1f), 0, Random.Range(0, 1.5f));
            // можно было пулом объектов, но для спавна раз в ~10 секунд излишне
            Colorable[] edibles = Instantiate(groupPrefab, positionToSpawn, Quaternion.identity, transform).GetComponentsInChildren<Colorable>();

            if (i % 2 == 0)
            {
                curGroupColor = Random.Range(0, 2) == 0 ? mainColor : secondaryColor;
                prevColor = curGroupColor;
            }
            else
            {
                curGroupColor = prevColor == mainColor ? secondaryColor : mainColor;
            }

            foreach (var food in edibles)
            {
                food.transform.localPosition += new Vector3(Random.Range(0, 0.2f), 0, Random.Range(0, 0.2f));
                food.ChangeColor(curGroupColor);
            }
        }


    }
    private void SpawnObstacles()
    {
        Instantiate(obstacles[Random.Range(0, obstacles.Count)], obstacleCenter.position, Quaternion.identity, transform);
    }
}