using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackWaypoints : MonoBehaviour
{
    public static Transform[] waypoints;
    public static Transform[] points1;
    public static Transform[] points2;
    public static Transform[] points3;
    public static Transform[] points4;
    public static Transform[] points5;
    public static Transform[] points6;

    private void Awake()
    {
        int i = 1;
        foreach(Transform child in transform)
        {
            if (i == 1)
            {
                points1 = new Transform[child.childCount];
                for(int j = 0; j < points1.Length; j++)
                {
                    points1[j] = child.GetChild(j);
                }
            }

            if (i == 2)
            {
                points2 = new Transform[child.childCount];
                for(int j = 0; j < points2.Length; j++)
                {
                    points2[j] = child.GetChild(j);
                }
            } 

            if (i == 3)
            {
                points3 = new Transform[child.childCount];
                for(int j = 0; j < points3.Length; j++)
                {
                    points3[j] = child.GetChild(j);
                }
            } 

            if (i == 4)
            {
                points4 = new Transform[child.childCount];
                for(int j = 0; j < points4.Length; j++)
                {
                    points4[j] = child.GetChild(j);
                }
            } 

            if (i == 5)
            {
                points5 = new Transform[child.childCount];
                for(int j = 0; j < points5.Length; j++)
                {
                    points5[j] = child.GetChild(j);
                }
            } 

            if (i == 6)
            {
                points6 = new Transform[child.childCount];
                for(int j = 0; j < points6.Length; j++)
                {
                    points6[j] = child.GetChild(j);
                }
            } 
            i++;
        }
    }
}
