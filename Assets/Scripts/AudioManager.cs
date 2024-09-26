using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    void Awake()
    {
        // If there's no instance of AudioManager, set this one
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Prevent this object from being destroyed
        }
        else
        {
            // If an instance already exists, destroy the new one to avoid duplicates
            Destroy(gameObject);
        }
    }
}