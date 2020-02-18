using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class generator : MonoBehaviour
{
    // private int dim = 100;
    public GameObject cubeFab;


    private int scale = 35;                 //Size of the output in XZ
    private int poly = 1;                  //Number of polygons per 1x1 area
    private int smoothness = 7;             //jagged <= low val, high val => flat
    private int heightBound = 10;           //height values range from 0 to this value
    private int centerOffsetX = (scale/2);
    private int centerOffsetZ = (scale/2);
    private int freq = scale/smoothness;    //Rate of change in the perlin noise
    private int dim = scale*poly; 

    void Awake(){
        //Initialize the first chunk
        drawPoints(setupPoints());
    }

    List<float[]> setupPoints(){
        Debug.Log("setup start");
        List<float[]> tempPoints = new List<float[]>();
        
        Debug.Log("pre for loop");

        for(int x = 0; x < dim; x++){
            for(int z = 0; z < dim; z++){
                float[] point = new float[3];
                point[0] = (float)x;
                point[1] = getHeight(x,z);
                point[2] = (float)z;
                tempPoints.Add(point);
            }
        }
        Debug.Log("Done setup");
        return tempPoints;
        
    }

    float getHeight(int x, int z){
        // return (x*x)/6+z/4;
        return Mathf.PerlinNoise(x/100f, z/100f) * 100;
    }

    // float getHeight(float ix, float iz){
    //     //transform coords to 0-1 range floats
    //     //scale/2 is a hacky fix by adding half of scale to undo centering around 0
    //     var nx = (coordsInWorld(ix, centerOffsetX) + scale/2) /scale;  
    //     var nz = (coordsInWorld(iz, centerOffsetZ) + scale/2) /scale;

    //     num = p5.noise(nx*freq, nz*freq);
    //     num = Math.min(heightBound, num*heightBound);//Stretch the 0_1 output into 0_heightBound
    //     return num;
    // }

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
            // cube.transform.localScale = new Vector3(sizeMod*slideVal, sizeMod*slideVal, sizeMod*slideVal);
        }
    }
    
    function coordsInWorld(i, center){
        var throwaway = center;
        var centerIn_i = dim/2; //TODO: This is only valid for center 0
        return (i - centerIn_i) * (1/poly);
    }
    // void ResizeCubes(float modSize){
    //     foreach (Transform child in transform) {
    //         child.transform.localScale = new Vector3(sizeMod*modSize, sizeMod*modSize, sizeMod*modSize);
    //     }
    // }
}
