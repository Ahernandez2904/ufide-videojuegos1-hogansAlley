﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class level1 : MonoBehaviour {
    
//////////////////////////////////////////////////////////////////////////////////////////////
// VARIABLES PRINCIPALES                                                                    //
//////////////////////////////////////////////////////////////////////////////////////////////

    AudioManagerController audioManager;
    int totalBadGuys = 0, randomNumberGuy; bool[] isBadGuy = {false, false, false}; string[] guy;
    public int round = 1, score = 0, missed = 0, top, time;
    public int sharpshooterLevel = 15, superSharpshooterLevel = 30;
    /*Tiempo y hasta donde puede bajar*/ public double maxTime = 2.50, minMaxTime = 1.30, minTime = 0.60; 
    /*Numeros aleatorios*/ int randomNumber = 0; int[] randomNumberGuyArr; double randomTime = 0.00;
    float[] xPos = {-3.3f, 0f, 3.3f}; int xPosGuyStart = -6, xPosGuyEnd = 6, yPosMiss = -2; float yPosGuy =0.43f;
    /*x de score digits*/ float s3x = -3.55f, s4x = -3.95f, s5x = -4.35f, s6x = -4.75f;
    /*x de top digits*/ float top3x = 3.95f, top4x = 3.55f, top5x = 3.15f, top6x = 2.75f;
    /*x de round y miss digits*/ float r1x = 0.35f, r2x = -0.35f; float m1x = 1.1f, m2x = 0.7f;
    /*x de time digits*/ float time1x = 0.4f, time2x = -0.4f;
    /*y de todos*/ float scoreY = -4.3f, topY = -4.3f, roundY = -4.15f, missY = -4.77f, timeY = 4.15f, missLabelY = -2f;
    /*x y y de miss en gameover*/ float missYGO = 0.6f, m2xGO = 0.8f, m1xGO = 1.2f;

//////////////////////////////////////////////////////////////////////////////////////////////
// MIS GAMEOBJECTS                                                                          //
//////////////////////////////////////////////////////////////////////////////////////////////    

    public GameObject GangA, GangB, GangC, Lady, Professor, Police; GameObject[] guys;
    GameObject scoreDigit3, scoreDigit4, scoreDigit5, scoreDigit6; GameObject topDigit3, topDigit4, topDigit5, topDigit6;
    GameObject missDigit1, missDigit2, missPatchClone; GameObject roundDigit1, roundDigit2, roundPatchClone; GameObject timeDigit1, timeDigit2;
    public GameObject greenNumber, whiteNumber, missPatch, roundPatch;
    public GameObject sharpshooterText, superSharphooterText; GameObject sharpshooterTextClone, superSharphooterTextClone;
    public GameObject stage, leftEdge, rightEdge, missLabel; GameObject leftEdgeClone, rightEdgeClone;
    GameObject missDigit1GO, missDigit2GO;

//////////////////////////////////////////////////////////////////////////////////////////////
// FUNCIONES DE INICIO                                                                      //
//////////////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        /*Create GameObjects*/ createEdge(); createNumbers();
        /*Start game*/ StartCoroutine (showRound1());
        //gameOver();
    }

    void Update() {
        updateScore(); updateTopScore(); updateRound(); updateMissed(); updateTime(); shot();
    }

    void Awake() {
        audioManager = FindObjectOfType<AudioManagerController>();
        randomNumberGuyArr = new int[300];
        guys = new GameObject[300];
    }

//////////////////////////////////////////////////////////////////////////////////////////////
// FUNCIONES DE DISPARO                                                                     //
//////////////////////////////////////////////////////////////////////////////////////////////

    public void missedShot () {

    }

    public void shot () {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 raycastPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(raycastPos, Vector2.zero);
            if(hit.collider!=null) { Debug.Log(hit.collider.gameObject.name); }
        }
    }

    public void successfulShot () {

    }

//////////////////////////////////////////////////////////////////////////////////////////////
// FUNCIONES DE ACTUALIZAR DATOS                                                         u  //
//////////////////////////////////////////////////////////////////////////////////////////////

    /*Listo*/ public void updateScore() {
        updateNumber(scoreDigit3, (score / 100) % 10);   updateNumber(scoreDigit4, (score / 1000) % 10); 
        updateNumber(scoreDigit5, (score / 10000) % 10); updateNumber(scoreDigit6, (score / 100000) % 10);
    }

    /*Listo*/ public void updateTopScore() {
        if (score > top) { top = score; }
        updateNumber(topDigit3, (top / 100) % 10);   updateNumber(topDigit4, (top / 1000) % 10);
        updateNumber(topDigit5, (top / 10000) % 10); updateNumber(topDigit6, (top / 100000) % 10);
    }

    /*Listo*/ public void updateRound() {
        updateNumber(roundDigit1, round % 10);
        if(round > 9 && roundDigit2 == null) { createSecondRoundNumber(); } if(round > 9) { updateNumber(roundDigit2, (round / 10) % 10); }
    }

    /*Listo*/ public void updateMissed() { 
        updateNumber(missDigit1, missed % 10);
        if(missed > 9 && missDigit2 == null) { createSecondRoundNumber(); } if(missed > 9) { updateNumber(missDigit2, (missed / 10) % 10); }
    }

    /*Listo*/ public void updateTime() { updateNumber(timeDigit1, time % 10); updateNumber(timeDigit2, (time / 10) % 10); }

    /*Listo*/ public void updateNumber(GameObject go, int i) {
        if (i == 0) { go.GetComponent<Animator>().Play("is0"); }
        else if (i == 1) { go.GetComponent<Animator>().Play("is1"); }
        else if (i == 2) { go.GetComponent<Animator>().Play("is2"); }
        else if (i == 3) { go.GetComponent<Animator>().Play("is3"); } 
        else if (i == 4) { go.GetComponent<Animator>().Play("is4"); }
        else if (i == 5) { go.GetComponent<Animator>().Play("is5"); }
        else if (i == 6) { go.GetComponent<Animator>().Play("is6"); }
        else if (i == 7) { go.GetComponent<Animator>().Play("is7"); }
        else if (i == 8) { go.GetComponent<Animator>().Play("is8"); }
        else if (i == 9) { go.GetComponent<Animator>().Play("is9"); }
        else { go.GetComponent<Animator>().Play("isEmpty"); }
    }

//////////////////////////////////////////////////////////////////////////////////////////////
// GENERACIÓN DE TIEMPO DE RESPUESTA Y ENEMIGOS                                             //
//////////////////////////////////////////////////////////////////////////////////////////////

    /*Listo*/ public int generateRandomTime() {
        /*Genera número aleatorio*/ time = UnityEngine.Random.Range( Convert.ToInt32((maxTime*10)-10), Convert.ToInt32(maxTime*10) );
        /*Genera tiempo en segundos*/ randomTime = time/10;
        /*Tiempo debe ser mayor al mínimo*/ if (randomTime < minTime ) { randomTime = minTime; time = Convert.ToInt32(randomTime)*10; }
        /*Menor tiempo progresivamente*/ if (maxTime > minMaxTime) {maxTime = maxTime - 0.05; }
        /*Retorna tiempo x10*/ return time;
    }

    /*Listo*/ public void generateGoodGuy(int i) {
        /*Genera aleatorios*/ randomNumberGuy = UnityEngine.Random.Range(0,3); randomNumberGuyArr[i] = randomNumberGuy;
        /*No se puede repetir en tanda*/ if ((i%3==2 && ((isBadGuy[(i%3)-1] == false && randomNumberGuyArr[i-1] == randomNumberGuy) || 
        (isBadGuy[(i%3)-2] == false && randomNumberGuyArr[i-2] == randomNumberGuy))) || 
        (i%3==1 && (isBadGuy[(i%3)-1] == false && randomNumberGuyArr[i-1] == randomNumberGuy))) { generateBadGuy(i); return; }
        /*Generate lady*/ if (randomNumberGuy == 0) { guys[i] = Instantiate(Lady, new Vector3( xPos[i%3]-12, yPosGuy, 0.1f), Quaternion.identity); return; }
        /*Generate professor*/ if (randomNumberGuy == 1) { guys[i] = Instantiate(Professor, new Vector3( xPos[i%3]-12, yPosGuy, 0.1f), Quaternion.identity); return; }
        /*Generate police*/ if (randomNumberGuy == 2) { guys[i] = Instantiate(Police, new Vector3( xPos[i%3]-12, yPosGuy, 0.1f), Quaternion.identity); return; }
    }

    /*Listo*/ public void generateBadGuy(int i) {
        /*Genera aleatorios*/ randomNumberGuy = UnityEngine.Random.Range(0,3); randomNumberGuyArr[i] = randomNumberGuy;
        /*No se puede repetir en tanda*/ if ((i%3==2 && ((isBadGuy[(i%3)-1] && randomNumberGuyArr[i-1] == randomNumberGuy) || 
        (isBadGuy[(i%3)-2] && randomNumberGuyArr[i-2] == randomNumberGuy))) || 
        (i%3==1 && (isBadGuy[(i%3)-1] && randomNumberGuyArr[i-1] == randomNumberGuy))) { generateBadGuy(i); return; }
        /*Generate Gang A*/ if (randomNumberGuy == 0) { guys[i] = Instantiate(GangA, new Vector3( xPos[i%3]-12, yPosGuy, 0.1f), Quaternion.identity); return; }
        /*Generate Gang B*/ if (randomNumberGuy == 1) { guys[i] = Instantiate(GangB, new Vector3( xPos[i%3]-12, yPosGuy, 0.1f), Quaternion.identity); return; }
        /*Generate Gang C*/ if (randomNumberGuy == 2) { guys[i] = Instantiate(GangC, new Vector3( xPos[i%3]-12, yPosGuy, 0.1f), Quaternion.identity); return; }
    }

    /*Listo*/ public void generateGuys() {
        /*Genera al primero*/ if (isBadGuy[0]) { generateBadGuy(((round-1)*3) + 0); } else { generateGoodGuy(((round-1)*3) + 0); }
        /*Genera al segundo*/ if (isBadGuy[1]) { generateBadGuy(((round-1)*3) + 1); } else { generateGoodGuy(((round-1)*3) + 1); }
        /*Genera al tercero*/ if (isBadGuy[2]) { generateBadGuy(((round-1)*3) + 2); } else { generateGoodGuy(((round-1)*3) + 2); }
    }

    /*Listo*/ public void guysPositions () { 
        /*Decide quienes son bad guys*/ for (int i = 0; i < 3; i++){ if (UnityEngine.Random.Range(0,2) == 0) { totalBadGuys++; isBadGuy[i] = true; } else { isBadGuy[i] = false; } }
        /*Si no hay bad guys, genera uno*/ if (totalBadGuys == 0) { isBadGuy[UnityEngine.Random.Range(0,3)] = true; totalBadGuys++; }
        /*Si hay tres bad guys, elimina uno*/ if (totalBadGuys == 3) { isBadGuy[UnityEngine.Random.Range(0,3)] = false; totalBadGuys--; }
    }

//////////////////////////////////////////////////////////////////////////////////////////////
// CREACIÓN DE GAMEOBJECTS                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////

    /*Listo*/ public void createNumbers() {
        /*1 Miss*/ missDigit1 = Instantiate (whiteNumber, new Vector3 (m1x, missY, 0f), Quaternion.identity);
        /*1 Round*/ roundDigit1 = Instantiate (greenNumber, new Vector3 (r1x, roundY, 0f), Quaternion.identity);
        /*1 Time*/ timeDigit1 = Instantiate (greenNumber, new Vector3 (time1x, timeY, 0f), Quaternion.identity);
        /*2 Time*/ timeDigit2 = Instantiate (greenNumber, new Vector3 (time2x, timeY, 0f), Quaternion.identity);
        /*3 Score*/ scoreDigit3 = Instantiate (whiteNumber, new Vector3 (s3x, scoreY, 0f), Quaternion.identity);
        /*4 Score*/ scoreDigit4 = Instantiate (whiteNumber, new Vector3 (s4x, scoreY, 0f), Quaternion.identity);
        /*5 Score*/ scoreDigit5 = Instantiate (whiteNumber, new Vector3 (s5x, scoreY, 0f), Quaternion.identity);
        /*6 Score*/ scoreDigit6 = Instantiate (whiteNumber, new Vector3 (s6x, scoreY, 0f), Quaternion.identity);
        /*3 Score*/ topDigit3 = Instantiate (whiteNumber, new Vector3 (top3x, topY, 0f), Quaternion.identity);
        /*4 Score*/ topDigit4 = Instantiate (whiteNumber, new Vector3 (top4x, topY, 0f), Quaternion.identity);
        /*5 Score*/ topDigit5 = Instantiate (whiteNumber, new Vector3 (top5x, topY, 0f), Quaternion.identity);
        /*6 Score*/ topDigit6 = Instantiate (whiteNumber, new Vector3 (top6x, topY, 0f), Quaternion.identity);
    }

    /*Listo*/ public void createSecondMissNumber() {
        /*2 Miss*/ missDigit2 = Instantiate (whiteNumber, new Vector3 (m2x, missY, 0f), Quaternion.identity);
        /*Esconde "="*/ missPatchClone = Instantiate(missPatch);
        /*Lo pone en valor 1.*/  missDigit2.GetComponent<Animator>().Play("is1");
    }

    /*Listo*/ public void createSecondRoundNumber() {
        /*2 Round*/ roundDigit2 = Instantiate (greenNumber, new Vector3 (r2x, roundY, 0f), Quaternion.identity);
        /*Esconde "R="*/ roundPatchClone = Instantiate(roundPatch);
        /*Lo pone en valor 1.*/ roundDigit2.GetComponent<Animator>().Play("is1");
    }

    /*Listo*/ public void createMiss(int i) {
        /*Miss*/ GameObject missClone = Instantiate (missLabel, new Vector3 (xPos[i], missLabelY, 0f), Quaternion.identity);
    }

    /*Listo*/ public void createGameOverMiss(){
        missDigit1GO = Instantiate (whiteNumber, new Vector3 (m1xGO, missYGO, 0f), Quaternion.identity);
        missDigit2GO = Instantiate (whiteNumber, new Vector3 (m2xGO, missYGO, 0f), Quaternion.identity);
        updateNumber(missDigit1GO, missed%10); updateNumber(missDigit2GO, (missed/10) %10); 
    }

    /*Listo*/ public void dontDestroy() {
        DontDestroyOnLoad(scoreDigit3); DontDestroyOnLoad(scoreDigit4); DontDestroyOnLoad(scoreDigit5); DontDestroyOnLoad(scoreDigit6);
        DontDestroyOnLoad(topDigit3); DontDestroyOnLoad(topDigit4); DontDestroyOnLoad(topDigit5); DontDestroyOnLoad(topDigit6);
        DontDestroyOnLoad(roundDigit1); if (roundDigit2 != null) { DontDestroyOnLoad(roundDigit2); DontDestroyOnLoad(roundPatchClone); }  
        DontDestroyOnLoad(missDigit1); if (missDigit2 != null) { DontDestroyOnLoad(missDigit2); DontDestroyOnLoad(missPatchClone);  } 
        DontDestroyOnLoad(timeDigit1); DontDestroyOnLoad(timeDigit2);
        DontDestroyOnLoad(missDigit1GO); DontDestroyOnLoad(missDigit2GO);
    }

    /*Listo*/ public void createEdge() { rightEdgeClone = Instantiate(rightEdge); leftEdgeClone = Instantiate(leftEdge); }

    /*Listo*/ public void destroyEdge() { Destroy(rightEdgeClone); Destroy(leftEdgeClone); }

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
        /*Cambia el tiempo por ronda*/ generateRandomTime();
        /*Te llama super sharpshooter en ronda 30*/ if (round == superSharpshooterLevel) { StartCoroutine(superSharpshooter()); }
        /*Te llama sharpshooter en ronda 15*/ if (round == sharpshooterLevel) { StartCoroutine(sharpshooter()); }
        /*Aumenta ronda y revisa si son 10*/ round++; if(round == 10) { createSecondRoundNumber(); }
        /*Set totalBadGuys a cero*/ totalBadGuys = 0;
        /*Nueva ronda*/ if (missed > 9 || round == 100) { gameOver(); } else { newRound(); }
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

    public void gameOver() {
        /*Miss se convierte en mayor a 9*/ createSecondMissNumber();
        /*Crea miss de game over*/ createGameOverMiss();
        /*No me destruyan estos objectos*/ dontDestroy();
        /*Cargar Escena Game Over*/ SceneManager.LoadScene("GameOver");
    }

}