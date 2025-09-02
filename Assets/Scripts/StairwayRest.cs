using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StairwayRest : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "ZoneTwo";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Transicionando a la segunda zona...");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
