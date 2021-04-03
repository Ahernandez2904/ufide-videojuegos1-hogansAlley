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

    public void destruirGameObjectsAnteriores(){
        GameObject[] wn = GameObject.FindGameObjectsWithTag("WhiteNumber(Clone)");
        GameObject[] gn = GameObject.FindGameObjectsWithTag("GreenNumber(Clone)");
        GameObject[] rp = GameObject.FindGameObjectsWithTag("PatchRound(Clone)");
        GameObject[] mp = GameObject.FindGameObjectsWithTag("PatchMiss(Clone)");
        foreach(GameObject go in wn) { Destroy(go); }
        foreach(GameObject go in gn) { Destroy(go); }
        foreach(GameObject go in rp) { Destroy(go); }
        foreach(GameObject go in mp) { Destroy(go); }
    }

}
