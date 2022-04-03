using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject cratePrefab;
    public GameObject balloonPrefab;
    public GameObject player;
    private int currentLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        createClouds(0, 3.0f);
        createClouds(1, 2.5f);
        createClouds(2, 2.0f);
        //createClouds(3, 1.5f);
        //createClouds(4, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y / 100 > currentLevel + 1){
            currentLevel++;
            if(currentLevel <= 2){
                createClouds(currentLevel + 2, 2.0f - (0.5f * currentLevel));
            }
            else if(currentLevel <= 11){
                createClouds(currentLevel + 2, 1.0f - (0.1f * (currentLevel - 2)));
            }
            else{
                createClouds(currentLevel + 2, 0.1f);
            }
        }
    }

    //every "level" is a 100 units high chunk, input refers to midpoint
    void createClouds(int level, float density)
    {
        for(int i=0; i < 1000 * density; i++){
            Vector3 coord = new Vector3(Random.Range(-300.0f, 300.0f), Random.Range((100 * level) - 50, (100 * level) + 50), Random.Range(-300.0f, 300.0f));
            Instantiate(cloudPrefab, coord, Quaternion.identity);

            float spawning = Random.value;
            if(spawning > .9f){
                Instantiate(cratePrefab, coord + new Vector3(0, 1, 0), Quaternion.identity);
            }
            else if(spawning < .1f){
                Instantiate(balloonPrefab, coord + new Vector3(0, 3, 0), Quaternion.identity);
            }

            while(Random.value > .3f){
                float dir = Random.Range(-1.0f, 1.0f);
                if(dir > 0){
                    if(dir > .5f){
                        coord += new Vector3(5, 0, 0);
                    }
                    else{
                        coord += new Vector3(-5, 0, 0);
                    }
                }else{
                    if(dir < -.5f){
                        coord += new Vector3(0, 0, 5);
                    }
                    else{
                        coord += new Vector3(0, 0, -5);
                    }
                }

                dir = Random.Range(-1.0f, 1.0f);
                if(dir > 0){
                    if(dir > .5f){
                        coord += new Vector3(0, 1, 0);
                    }
                    else{
                        coord += new Vector3(0, -1, 0);
                    }
                }

                if(!Physics.CheckSphere(coord, 1f)){
                    Instantiate(cloudPrefab, coord, Quaternion.identity);
                }
                
            }
        }
    }
}
