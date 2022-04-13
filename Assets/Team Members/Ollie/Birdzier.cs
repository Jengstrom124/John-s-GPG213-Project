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
    public OllieTerrain ollieTerrain;
    private Vector3 pos1, pos2, pos3, pos4, originPos;
    private Vector3 spawnPos1, spawnPos2, spawnPos3, spawnPos4;
    private Vector3 a, b, c, d, e, point;
    private float tWidth, tHeight;
    private float speed, timer, timeScale;
    public GameObject birdPrefab;
    private GameObject bird;
    void Start()
    {
        //adds modularity so spawner takes into account terrain width/height
        tWidth = ollieTerrain.width;
        tHeight = ollieTerrain.height;
        positions = new List<Vector3>();
        spawnPosList = new List<Vector3>();
        pos1 = new Vector3(0, 20, 0);
        pos2 = new Vector3(tWidth, 20, 0);
        pos3 = new Vector3(tWidth, 20, tHeight);
        pos4 = new Vector3(0, 20, tHeight);
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
        timeScale = Time.deltaTime/10;
    }

    private void Update()
    {
        
        speed = timer;
        print(pos1);
        print(positions.Capacity);
        print(spawnPosList.Capacity);
        BezierCurve();
        UpdateSpawnPos();
        SpawnBird();
        TimeFlip();
    }

    private void FixedUpdate()
    {
        if (bird != null)
        {
            Vector3 tempPoint = bird.transform.position;
            bird.transform.position = point;
            Vector3 dir = bird.transform.InverseTransformDirection(point - tempPoint);
            if (dir.x > 0)
            {
                bird.transform.eulerAngles = new Vector3(bird.transform.eulerAngles.x, bird.transform.eulerAngles.y + 2,
                    bird.transform.eulerAngles.z);
            }
            else if (dir.x < 0)
            {
                bird.transform.eulerAngles = new Vector3(bird.transform.eulerAngles.x, bird.transform.eulerAngles.y - 2,
                    bird.transform.eulerAngles.z);
            }
            
            //could try and use lerp/dotween to make the movement a bit smoother


            // float singleStep = 10.0f * Time.deltaTime;
            // Vector3 newDirection = Vector3.RotateTowards(transform.forward, dir, singleStep, 0.0f);
            //bird.transform.localRotation = Quaternion.LookRotation(newDirection);

            //try and get dir as local coords
            //neg = right
            //pos = left
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
        spawnPos1 = new Vector3((UnityEngine.Random.Range(0, tWidth)), 20, 0);
        spawnPos2 = new Vector3((UnityEngine.Random.Range(0, tWidth)), 20, tHeight);
        spawnPos3 = new Vector3(tWidth, 20, (UnityEngine.Random.Range(0, tHeight)));
        spawnPos4 = new Vector3(0, 20, (UnityEngine.Random.Range(0, tHeight)));
        
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
