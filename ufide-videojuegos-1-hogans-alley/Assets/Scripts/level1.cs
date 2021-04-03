using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class level1 : MonoBehaviour {
    
//////////////////////////////////////////////////////////////////////////////////////////////
// VARIABLES PRINCIPALES                                                                    //
//////////////////////////////////////////////////////////////////////////////////////////////

    AudioManagerController audioManager;
    int totalBadGuys = 0, randomNumberGuy; bool[] isBadGuy; string[] guy;
    public int round = 1, score = 0, missed = 0;
    public int sharpshooterLevel = 15, superSharpshooterLevel = 30;
    public double maxTime = 2.50, minMaxTime = 1.30, minTime = 0.60, randomTime = 0.00; int randomNumber = 0; int[] randomNumberGuyArr;
    public float[] xPos = {-3.3f, 0f, 3.3f}; public int xPosGuyStart = -6, xPosGuyEnd = 6, yPosMiss = -2; public float yPosGuy =0.43f;
    /*x de score digits*/ float s3x = -3.55f, s4x = -3.95f, s5x = -4.35f, s6x = -4.75f;
    /*x de top digits*/ float top3x = 3.95f, top4x = 3.55f, top5x = 3.15f, top6x = 2.75f;
    /*x de round y miss digits*/ float r1x = 0.35f, r2x = -0.35f; float m1x = 1.1f, m2x = 0.7f;
    /*x de time digits*/ float time1x = 0.4f, time2x = -0.4f;
    /*y de todos*/float scoreY = -4.3f, topY = -4.3f, roundY = -4.15f, missY = -4.77f, timeY = 4.15f;

//////////////////////////////////////////////////////////////////////////////////////////////
// MIS GAMEOBJECTS                                                                          //
//////////////////////////////////////////////////////////////////////////////////////////////    

    public GameObject GangA, GangB, GangC, Lady, Professor, Police; GameObject[] guys;
    public GameObject scoreDigit3, scoreDigit4, scoreDigit5, scoreDigit6;
    public GameObject topDigit3, topDigit4, topDigit5, topDigit6;
    public GameObject missDigit1, missDigit2, missPatch;
    public GameObject roundDigit1, roundDigit2, roundPatch;
    public GameObject timeDigit1, timeDigit2;
    public GameObject sharpshooterText, superSharphooterText; GameObject sharpshooterTextClone, superSharphooterTextClone;
    public GameObject stage, leftEdge, rightEdge;

//////////////////////////////////////////////////////////////////////////////////////////////
// FUNCIONES DE INICIO                                                                      //
//////////////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        StartCoroutine (showRound1());
    }

    void Update() {
        updateScore(); 
        updateTopScore();
        updateRound();
        updateMissed();
        updateTime();
    }

    void Awake() {
        audioManager = FindObjectOfType<AudioManagerController>();
    }

//////////////////////////////////////////////////////////////////////////////////////////////
// FUNCIONES DE DISPARO                                                                     //
//////////////////////////////////////////////////////////////////////////////////////////////

    public void missedShot () {

    }

    public void shot () {

    }

    public void successfulShot () {

    }

//////////////////////////////////////////////////////////////////////////////////////////////
// FUNCIONES DE ACTUALIZAR DATOS                                                         u  //
//////////////////////////////////////////////////////////////////////////////////////////////

    public void updateScore() {

    }

    public void updateRound() {

    }

    public void updateMissed() {

    }

    public void updateTopScore() {

    }

    public void updateTime() {

    }

//////////////////////////////////////////////////////////////////////////////////////////////
// GENERACIÓN DE TIEMPO DE RESPUESTA Y ENEMIGOS                                             //
//////////////////////////////////////////////////////////////////////////////////////////////

    /*Lista*/ public int generateRandomTime() {
        /*Genera número aleatorio*/ randomNumber = UnityEngine.Random.Range( Convert.ToInt32((maxTime*10)-10), Convert.ToInt32(maxTime*10) );
        /*Genera tiempo en segundos*/ randomTime = randomNumber/10;
        /*Tiempo debe ser mayor al mínimo*/ if (randomTime < minTime ) { randomTime = minTime; randomNumber = Convert.ToInt32(randomTime)*10; }
        /*Menor tiempo progresivamente*/ if (maxTime > minMaxTime) {maxTime = maxTime - 0.05; }
        /*Retorna tiempo x10*/ return randomNumber;
    }

    public void generateGoodGuy(int i) {
        randomNumberGuy = UnityEngine.Random.Range(0,3);
        /*Generate lady*/ if (randomNumberGuy == 0) { guys[i] = Instantiate(Lady, new Vector3( xPos[i%3]-12, yPosGuy, 0.1f), Quaternion.identity); return; }
        /*Generate professor*/ if (randomNumberGuy == 1) { guys[i] = Instantiate(Professor, new Vector3( xPos[i%3]-12, yPosGuy, 0.1f), Quaternion.identity); return; }
        /*Generate police*/ if (randomNumberGuy == 2) { guys[i] = Instantiate(Police, new Vector3( xPos[i%3]-12, yPosGuy, 0.1f), Quaternion.identity); return; }
    }

    public void generateBadGuy(int i) {
        randomNumberGuy = UnityEngine.Random.Range(0,3);
        /*Generate Gang A*/ if (randomNumberGuy == 0) { guys[i] = Instantiate(GangA, new Vector3( xPos[i%3]-12, yPosGuy, 0.1f), Quaternion.identity); return; }
        /*Generate Gang B*/ if (randomNumberGuy == 1) { guys[i] = Instantiate(GangB, new Vector3( xPos[i%3]-12, yPosGuy, 0.1f), Quaternion.identity); return; }
        /*Generate Gang C*/ if (randomNumberGuy == 2) { guys[i] = Instantiate(GangC, new Vector3( xPos[i%3]-12, yPosGuy, 0.1f), Quaternion.identity); return; }
    }

    public void generateGuys() {
        /*Genera al primero*/if (isBadGuy[((round-1)*3) + 0]) { generateBadGuy(((round-1)*3) + 0); } else { generateGoodGuy(((round-1)*3) + 0); }
        /*Genera al segundo*/if (isBadGuy[((round-1)*3) + 1]) { generateBadGuy(((round-1)*3) + 1); } else { generateGoodGuy(((round-1)*3) + 1); }
        /*Genera al tercero*/if (isBadGuy[((round-1)*3) + 2]) { generateBadGuy(((round-1)*3) + 2); } else { generateGoodGuy(((round-1)*3) + 2); }
    }

    /*Lista*/ public void guysPositions () {
        /*Decide quienes son bad guys*/ for (int i = 0; i < 3; i++){ if (UnityEngine.Random.Range(0,2) == 0) { totalBadGuys++; isBadGuy[i] = true; } else { isBadGuy[i] = false; } }
        /*Si no hay bad guys, genera uno*/ if (totalBadGuys == 0) { isBadGuy[UnityEngine.Random.Range(0,3)] = true; totalBadGuys++; }
        /*Si hay tres bad guys, elimina uno*/ if (totalBadGuys == 3) { isBadGuy[UnityEngine.Random.Range(0,3)] = false; totalBadGuys--; }
    }

//////////////////////////////////////////////////////////////////////////////////////////////
// EVENTOS                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////

    /*Lista*/ IEnumerator showRound1() {
        /*Esperar 3 segundos*/ yield return new WaitForSeconds(5);
        /*Destruir objetos*/ Destroy(GameObject.Find("Round"));
        /*Empieza la ronda*/ newRound();
    }

    public void newRound() {
        /*Decide las posiciones de los personajes*/ guysPositions();
        /*Genera a las tres personas*/ generateGuys();
        /*Te llama super sharpshooter en ronda 30*/ if (round == superSharpshooterLevel) { StartCoroutine(superSharpshooter()); }
        /*Te llama sharpshooter en ronda 15*/ if (round == sharpshooterLevel) { StartCoroutine(sharpshooter()); }
        /*Aumenta el número de ronda al final*/ round++;
        /*Set totalBadGuys a cero*/ totalBadGuys = 0;
        /*Nueva ronda*/ if (missed < 10) { newRound(); } else { gameOver(); }
    }

    public void gameOver() {
        /*Pasar valores score, miss, top*/
        /*Cargar Escena Game Over*/ SceneManager.LoadScene("GameOver");
    }

    /*Listo*/ IEnumerator sharpshooter() {
        /*Crea gameobjects*/ sharpshooterTextClone = Instantiate(sharpshooterText);
        /*Play sharpshooter.mp3*/ audioManager.Play("sharpshooter");
        /*Espera a que termine la canción*/ yield return new WaitForSeconds(4);
        /*Destruye el elemento*/ Destroy(sharpshooterTextClone);
    }

    /*Listo*/ IEnumerator superSharpshooter() {
        /*Crea gameobjects*/ sharpshooterTextClone = Instantiate(sharpshooterText); superSharphooterTextClone = Instantiate(superSharphooterText);
        /*Play sharpshooter.mp3*/ audioManager.Play("sharpshooter");
        /*Espera a que termine la canción*/ yield return new WaitForSeconds(4);
        /*Destruye gameobjects*/ Destroy(sharpshooterTextClone); Destroy(superSharphooterTextClone); 
    }

}
