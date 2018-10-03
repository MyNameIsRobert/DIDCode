using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class OnSceneLoad : MonoBehaviour {

    public float timer;

  

     void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}