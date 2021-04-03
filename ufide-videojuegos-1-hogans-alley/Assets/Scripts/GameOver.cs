using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    void Start() {
       StartCoroutine (Esperar(5));
    }

    IEnumerator Esperar(int s) { 
        yield return new WaitForSeconds(s);
        SceneManager.LoadScene("MainMenu");
    }

}
