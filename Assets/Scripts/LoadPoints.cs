using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;

public class LoadPoints : MonoBehaviour
{   
    // public InputField pathField;
    // public Slider sizeSlider;
    public GameObject cubeFab;
    float sizeMod = 1;
    // enum coordinateSystem {Cartesian, Spherical};
    // coordinateSystem mode = coordinateSystem.Spherical;


    void Start(){
        Debug.Log("Started");
        // sizeSlider.onValueChanged.AddListener(ResizeCubes);
    }


    public void renderButtonPressed()
    {
        Debug.Log("Button pressed");

        // string path = "/Users/benjamingillott/Desktop/unitytext.txt";
        string path = pathField.text;

        string text = pathToString(path);
        List<float[]> points = textToFloatList(text);
        
        consoleLog(points);
        DrawCubes(points);
    }


    string pathToString(string path)
    {
        string text = File.ReadAllText(path).Trim();
        return text;
    }


    List<float[]> textToFloatList(string text)
    {
        string[] strElements = text.Split('\n');

        List<float[]> points = new List<float[]>();

        for (int i = 0; i < strElements.Length; i++){
            points.Add(Array.ConvertAll(strElements[i].Split(' '), x => float.Parse(x)));
        }

        return points;
    }


    void consoleLog(List<float[]> points){
        foreach(float[]x in points){
            Debug.Log(x[0] + " " + x[1] + " " + x[2] + "\n");
        }
    }


    void DrawCubes(List<float[]> points){

        Debug.Log("At draw");

        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }

        foreach(float[] point in points){
            
            float x = 0;
            float y = 0;
            float z = 0;

            if(mode == coordinateSystem.Cartesian){
                x = point[0];
                y = point[1];
                z = point[2];
            }
            else if (mode == coordinateSystem.Spherical){ //Format [0] r, [1] azimuth, [2] polar angle
                x = point[0] * Mathf.Sin(point[1]) * Mathf.Cos(point[2]);
                y = point[0] * Mathf.Sin(point[1]) * Mathf.Sin(point[2]);
                z = point[0] * Mathf.Cos(point[1]);
            }

            GameObject cube = Instantiate(cubeFab, transform, true);
            cube.transform.position = new Vector3(x, y, z);
            cube.transform.localScale = new Vector3(sizeMod*sizeSlider.value, sizeMod*sizeSlider.value, sizeMod*sizeSlider.value);
        }
    }
    

    void ResizeCubes(float sliderValue){
        foreach (Transform child in transform) {
            child.transform.localScale = new Vector3(sizeMod*sliderValue, sizeMod*sliderValue, sizeMod*sliderValue);
        }
    }
}
