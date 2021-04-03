using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    GameObject[] wn = new GameObject[20];
    GameObject[] gn = new GameObject[20];

    void Start() {
       StartCoroutine (Esperar(5));
    }

    IEnumerator Esperar(int s) { 
        yield return new WaitForSeconds(s);
        destroyOldGameObjects();
        SceneManager.LoadScene("MainMenu");
    }

    public void destroyOldGameObjects(){
        wn = GameObject.FindGameObjectsWithTag("WhiteNumber(Clone)"); foreach(GameObject go in wn) { Destroy(go); }
        gn = GameObject.FindGameObjectsWithTag("GreenNumber(Clone)"); foreach(GameObject go in gn) { Destroy(go); }
        GameObject rp = GameObject.Find("PatchRound(Clone)"); if(rp != null) { Destroy(rp); }
        GameObject mp = GameObject.Find("PatchMiss(Clone)");  if(mp != null) { Destroy(mp); }
    }

}
