using System;
using UnityEngine;

[Serializable]
public class FaceManager
{
    [SerializeField] private Sprite[] faces;
    [SerializeField] private Sprite def; //Default Sprite face

    public Sprite GetFace(int index)
    {
        if(index == -1)
        {
            return def; 
        }
        else if (index < faces.Length)
        {
            return faces[index];
        }
        else
        {
            return def;
        }
    }
}
