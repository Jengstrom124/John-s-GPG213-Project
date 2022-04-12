using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.VisualScripting;
using UnityEngine;

public class Birdzier : MonoBehaviour
{
    public List<Vector3> positions;
    public List<Vector3> spawnPosList;
    public List<Vector3> newSpawnPosList;
    private Vector3 pos1, pos2, pos3, pos4, originPos;
    private Vector3 spawnPos1, spawnPos2, spawnPos3, spawnPos4;
    private Vector3 a, b, c, d, e, point;
    private float speed, timer, timeScale;
    public GameObject birdPrefab;
    private GameObject bird;
    void Start()
    {
        positions = new List<Vector3>();
        spawnPosList = new List<Vector3>();
        pos1 = new Vector3(0, 20, 0);
        pos2 = new Vector3(256, 20, 0);
        pos3 = new Vector3(256, 20, 256);
        pos4 = new Vector3(0, 20, 256);
        //lol this does NOT work
        //positions.Add(pos1 + pos2 + pos3 + pos4);
        positions.Add(pos1);
        positions.Add(pos2);
        positions.Add(pos3);
        positions.Add(pos4);
        spawnPosList.Add(spawnPos1);
        spawnPosList.Add(spawnPos2);
        spawnPosList.Add(spawnPos3);
        spawnPosList.Add(spawnPos4);
        timeScale = Time.deltaTime;
    }

    private void Update()
    {
        TimeFlip();
        SpawnBird();
        speed = timer;
        print(pos1);
        print(positions.Capacity);
        print(spawnPosList.Capacity);
        BezierCurve();
    }

    private void FixedUpdate()
    {
        if (bird != null)
        {
            bird.transform.position = point;
        }
    }

    void BezierCurve()
    {
        a = Vector3.Lerp(positions[0], positions[1], speed);
        b = Vector3.Lerp(positions[1], positions[2], speed);
        c = Vector3.Lerp(positions[2], positions[3], speed);
        d = Vector3.Lerp(a, b, speed);
        e = Vector3.Lerp(b, c, speed);
        point = Vector3.Lerp(d, e, speed);
    }
    
    void TimeFlip()
    {
        timer += timeScale;
        if (timer >= 1)
        {
            timer = 1;
            timeScale = (Time.deltaTime * -1)/10;
            DestroyBird();
        }

        if (timer <= 0)
        {
            timer = 0;
            timeScale = (Time.deltaTime)/10;
            DestroyBird();
        }
    }

    void SpawnBird()
    {
        //if no bird present
        //pick random spot between 0-1, 1-2, 2-3, 3-4
            //^^^ how do I do this?
        //set pos1 to this spot
        //spawn bird
        //move bird via point
        //destroy bird when TimeFlip timer reaches peak/trough
        if (bird == null && (UnityEngine.Random.Range(0,10) <= 1))
        {
            originPos = spawnPosList[UnityEngine.Random.Range(0, spawnPosList.Capacity-1)];
            bird = Instantiate(birdPrefab);
            bird.transform.position = originPos;
            positions[0] = originPos;
        }
    }

    void DestroyBird()
    {
        if (bird != null)
        {
            UpdateSpawnPos();
            print("yeet that bird");
            Destroy(bird);
        }
    }

    void UpdateSpawnPos()
    {
        newSpawnPosList.Clear();
        
        
        GenerateSpawnPos();
    }
    
    void GenerateSpawnPos()
    {
        spawnPos1 = new Vector3((UnityEngine.Random.Range(0, 256)), 20, 0);
        spawnPos2 = new Vector3((UnityEngine.Random.Range(0, 256)), 20, 256);
        spawnPos3 = new Vector3(256, 20, (UnityEngine.Random.Range(0, 256)));
        spawnPos4 = new Vector3(0, 20, (UnityEngine.Random.Range(0, 256)));
        
        newSpawnPosList.Add(spawnPos1);
        newSpawnPosList.Add(spawnPos2);
        newSpawnPosList.Add(spawnPos3);
        newSpawnPosList.Add(spawnPos4);

        spawnPosList = newSpawnPosList;
    }
    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(point,10f);
    }
}
