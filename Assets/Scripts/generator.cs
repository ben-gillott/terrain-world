using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generator : MonoBehaviour
{
    public int dim = 10;
    List<float[]> points;

    void Awake(){
        //Initialize the first chunk
        points = setupPoints();
        drawPoints(points);
    }

    List<float[]> setupPoints(){
        List<float[]> tempPoints = new List<float[]>();

        for (int x = 0; x < dim; x++){
            for(int z = 0; z < dim; z++){
                float[] point = {x, 0, z};
                tempPoints.Add(point);
            }
        }
        return tempPoints;
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

        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }

        foreach(float[] point in points){
            
            float x = point[0];
            float y = point[1];
            float z = point[2];

            GameObject cube = Instantiate(cubeFab, transform, true);
            cube.transform.position = new Vector3(x, y, z);
            cube.transform.localScale = new Vector3(sizeMod*slideVal, sizeMod*slideVal, sizeMod*slideVal);
        }
    }
    

    // void ResizeCubes(float modSize){
    //     foreach (Transform child in transform) {
    //         child.transform.localScale = new Vector3(sizeMod*modSize, sizeMod*modSize, sizeMod*modSize);
    //     }
    // }
}
