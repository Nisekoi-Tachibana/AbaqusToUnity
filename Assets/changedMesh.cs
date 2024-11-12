using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.Windows;
using UnityEditor;

public class changedMesh : MonoBehaviour
{
    void Start()
    {
        CreateCube();
    }

    private void CreateCube()
    {
        //导入顶点数据
        Vector3[] vertices = new Vector3[34371];
        string filePath = "D:\\abaqusFinalNode.txt";
        int index = 0;
        //读取顶点数据文件
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 3)
                    {
                        float a = float.Parse(parts[0]);
                        float b = float.Parse(parts[1]);
                        float c = float.Parse(parts[2]);
                        vertices[index] = new Vector3(a, b, c);
                        index++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
        }
        //放大形变程度
        Vector3[] verticesPre = new Vector3[34371];
        readPreNode(verticesPre);
        for (int i = 0; i < 34371; i++)
        {
            vertices[i] = verticesPre[i] + 8000 * (vertices[i] - verticesPre[i]);
            print(vertices[i].x - verticesPre[i].x);
            print(vertices[i].y - verticesPre[i].y);
            print(vertices[i].z - verticesPre[i].z);
        }
        //读取每个六面体的三角形面顶点坐标顺序
        int[] triangles = new int[806400];
        readRec(triangles);
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        float[] physicsDatas = new float[34371];
        string filePath_ = "D:\\abaqusNodeStress.txt";
        int index_ = 0;
        try
        {
            using (StreamReader reader = new StreamReader(filePath_))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        physicsDatas[index_] = float.Parse(parts[1]);//parts[0]为序号，去除
                        index_++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
        }
        //将应力值转化为color值
        Color[] colorDatas = new Color[physicsDatas.Length];
        float[] hueColorH = new float[physicsDatas.Length];
        float max = 1.22149f;
        float min = 13.2991E-03f;
        float range = max - min;
        for (int i = 0; i < physicsDatas.Length; i++)
        {
            if (physicsDatas[i] >= min - 0.1 && physicsDatas[i] < 1.140E-01f)
            {
                colorDatas[i] = new Color(0 / 255, 0 / 255, 255 / 255, 0);
            }
            else if (physicsDatas[i] >= 1.140E-01f && physicsDatas[i] < 2.147E-01f)
            {
                colorDatas[i] = new Color(65.0f / 255, 105.0f / 255, 225.0f / 255, 0);
            }
            else if (physicsDatas[i] >= 2.147E-01f && physicsDatas[i] < 3.153E-01f)
            {
                colorDatas[i] = new Color(135.0f / 255, 106.0f / 255, 235.0f / 255, 0);
            }
            else if (physicsDatas[i] >= 3.153E-01f && physicsDatas[i] < 4.160E-01f)
            {
                colorDatas[i] = new Color(0 / 255, 255 / 255, 255 / 255, 0);
            }
            else if (physicsDatas[i] >= 4.160E-01f && physicsDatas[i] < 5.167E-01f)
            {
                colorDatas[i] = new Color(0 / 255, 255 / 255, 127.0f / 255, 0);
            }
            else if (physicsDatas[i] >= 5.167E-01f && physicsDatas[i] < 6.174E-01f)
            {
                colorDatas[i] = new Color(0 / 255, 255 / 255, 0 / 255, 0);
            }
            else if (physicsDatas[i] >= 6.174E-01f && physicsDatas[i] < 7.181E-01f)
            {
                //0.67约为百分之二十五分位点
                if (physicsDatas[i] >= 0.67)
                {
                    colorDatas[i] = new Color(0 / 255, 255 / 255, 0 / 255, 1);
                }
                else
                {
                    colorDatas[i] = new Color(0 / 255, 255 / 255, 0 / 255, 0);
                }
            }
            else if (physicsDatas[i] >= 7.181E-01f && physicsDatas[i] < 8.188E-01f)
            {
                colorDatas[i] = new Color(127.0f / 255, 255 / 255, 0 / 255, 1);
            }
            else if (physicsDatas[i] >= 8.188E-01f && physicsDatas[i] < 9.194E-01f)
            {
                colorDatas[i] = new Color(255 / 255, 255 / 255, 0 / 255, 1);
            }
            else if (physicsDatas[i] >= 9.194E-01f && physicsDatas[i] < 10.20E-01f)
            {
                colorDatas[i] = new Color(255 / 255, 215.0f / 255, 0 / 255, 1);
            }
            else if (physicsDatas[i] >= 10.20E-01f && physicsDatas[i] < 11.21E-01f)
            {
                colorDatas[i] = new Color(255 / 255, 97.0f / 255, 0 / 255, 1);
            }
            else if (physicsDatas[i] >= 11.21E-01f && physicsDatas[i] <= max + 0.1)
            {
                colorDatas[i] = new Color(255 / 255, 0 / 255, 0 / 255, 1);
            }
            else { }
        }
        //导入color值
        mesh.colors = colorDatas;
        mesh.Optimize();
        mesh.RecalculateNormals();
    }

    //读取原坐标，用于放大形变
    public void readPreNode(Vector3[] vertices)
    {
        string filePath = "D:\\abaqusData\\data_3.txt";
        int index = 0;
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 3)
                    {
                        float a = float.Parse(parts[0]);
                        float b = float.Parse(parts[1]);
                        float c = float.Parse(parts[2]);
                        vertices[index] = new Vector3(a, b, c);
                        index++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
        }
    }
    //读取三角形的顶点坐标顺序
    public void readRec(int[] triangles)
    {
        string filePath = "D:\\abaqusData\\data_2.txt";
        int index = 0;
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 8)
                    {
                        int[] temp = new int[8];
                        for (int i = 0; i < 8; i++)
                        {
                            temp[i] = int.Parse(parts[i]);
                        }
                        //int[] t = {
                        //         0, 2, 1, 0, 3, 2,   2, 3, 4, 2, 4, 5,   1, 2, 5,1, 5, 6,    0, 7, 4, 0, 4, 3,    5, 4, 7, 5, 7, 6 ,   0, 6, 7, 0, 1, 6
                        //}; 
                        triangles[index++] = temp[0] - 1; triangles[index++] = temp[1] - 1; triangles[index++] = temp[2] - 1;
                        triangles[index++] = temp[0] - 1; triangles[index++] = temp[2] - 1; triangles[index++] = temp[3] - 1;
                        triangles[index++] = temp[1] - 1; triangles[index++] = temp[6] - 1; triangles[index++] = temp[2] - 1;
                        triangles[index++] = temp[1] - 1; triangles[index++] = temp[5] - 1; triangles[index++] = temp[6] - 1;
                        triangles[index++] = temp[4] - 1; triangles[index++] = temp[6] - 1; triangles[index++] = temp[5] - 1;
                        triangles[index++] = temp[4] - 1; triangles[index++] = temp[7] - 1; triangles[index++] = temp[6] - 1;
                        triangles[index++] = temp[0] - 1; triangles[index++] = temp[7] - 1; triangles[index++] = temp[4] - 1;
                        triangles[index++] = temp[0] - 1; triangles[index++] = temp[3] - 1; triangles[index++] = temp[7] - 1;
                        triangles[index++] = temp[0] - 1; triangles[index++] = temp[4] - 1; triangles[index++] = temp[1] - 1;
                        triangles[index++] = temp[1] - 1; triangles[index++] = temp[4] - 1; triangles[index++] = temp[5] - 1;
                        triangles[index++] = temp[2] - 1; triangles[index++] = temp[6] - 1; triangles[index++] = temp[7] - 1;
                        triangles[index++] = temp[2] - 1; triangles[index++] = temp[7] - 1; triangles[index++] = temp[3] - 1;

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
        }
    }
    void Update()
    {

    }
}