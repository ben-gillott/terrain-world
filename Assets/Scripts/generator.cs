using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class generator : MonoBehaviour
{
    // private int dim = 100;
    public GameObject cubeFab;
    private int genType = 1;

    private const int scale = 50;                //Chunk width in world
    private const int poly = 1;                 //Number of polygons per 1x1 area
    private const int smoothness = 7;           //jagged <= low val, high val => flat
    private const int heightBound = 10;         //height values range from 0 to this value
    private const int cornerX = -25;
    private const int cornerZ = -25;

    void Awake(){
        List<float[]> points;

        if(genType == 0){
            points = setupPerlin();
        }
        else{
            points = setupFlat();
        }

        drawPoints(points);
    }

    List<float[]> setupFlat(){
        List<float[]> tempPoints = new List<float[]>();
        int dim  = (int)scale*poly;

        for(int x = 0; x < dim; x++){
            for(int z = 0; z < dim; z++){
                float[] point = new float[3];
                point[0] = (float)x/poly + cornerX;     //Pass X and Z in world coords
                point[1] = 0f;
                point[2] = (float)z/poly + cornerZ;
                tempPoints.Add(point);
            }
        }
        return tempPoints;
    }


    List<float[]> setupPerlin(){
        List<float[]> tempPoints = new List<float[]>();
        
        float freq = scale/smoothness;    //Rate of change in the perlin noise
        int dim  = (int)scale*poly;           //Chunk width in model

        //X and Z are in model coords, where model is the chunk
        for(int x = 0; x < dim; x++){
            for(int z = 0; z < dim; z++){
                float[] point = new float[3];
                point[0] = (float)x/poly + cornerX;     //Pass X and Z in world coords
                point[1] = getHeight((float)x/poly, (float)z/poly, freq);   //Pass XZ to perlin in worl coords, but not shifted
                point[2] = (float)z/poly + cornerZ;
                tempPoints.Add(point);
            }
        }
        return tempPoints;
    }

    //INPUT: two coords in world space
    //OUTPUT: perlin noise in a scale size chunk, affected by freq
    float getHeight(float x, float z, float freq){
        //scale/2 is a hacky fix by adding half of scale to undo centering around 0
        //transform coords to 0-1 range floats
        float nx = (float)x/scale;
        float nz = (float)z/scale;
        float num = Mathf.PerlinNoise(nx*freq, nz*freq);
        // Debug.Log(nx);
        num = Mathf.Min(heightBound, num*heightBound);//Stretch the 0_1 output into 0_heightBound
        return num;
    }

    void drawPoints(List<float[]> points){

        Debug.Log("At draw");
        // GameObject cube = Instantiate(cubeFab, this.transform, true);
        // cube.transform.position = new Vector3(0, 0, 2);
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }

        foreach(float[] point in points){
            
            float x = point[0];
            float y = point[1];
            float z = point[2];

            GameObject cube = Instantiate(cubeFab, transform, true);
            cube.transform.position = new Vector3(x, y, z);
            Vector3 cubesize = cubeFab.transform.localScale;
            cube.transform.localScale = cubesize/(poly);
        }
    }
}
