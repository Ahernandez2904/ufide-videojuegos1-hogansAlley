﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class level1 : MonoBehaviour {
    
//////////////////////////////////////////////////////////////////////////////////////////////
// VARIABLES PRINCIPALES                                                                    //
//////////////////////////////////////////////////////////////////////////////////////////////

    AudioManagerController audioManager; AnimatorClipInfo[] currentClipInfo;

    //Se esta usando public en vez de SerializedField ya que según mi investigación, sirven para lo mismo
    //Aparte que con public se pueden ver desde otros lados del código
    //Static permite conservar el valor durante la sesión de juego

    /*Valores principales*/ public int round = 1, score = 0, missed = 0, timetime; public static int top;

    /*Aleatorios*/ int randomNumber = 0, randomNumberGuy; int[] randomNumberGuyArr; 
    /*Animaciones*/ string[] guysAnimations = {"error","error","error"}; float moveSpeed = 2F; string stageAnimation = "error";
    /*Cronómetro*/ float timeAfterShootGoodGuy = 0, timerStart = 0; bool isTimerRunning = false, wasAGoodGuyShot = false;
    /*Estados*/ bool areEntering = false, areLeaving = false, areInvincible = true;
    /*Guys*/ int totalBadGuys = 0, badGuysMissed = 0; bool[] isBadGuy = {false, false, false}, isShot; 
    /*Score*/ bool[] wasFastShot = {false, false, false};
    /*Sharpshooter*/ public int sharpshooterLevel = 15, superSharpshooterLevel = 30;
    /*Tiempo*/ float maxTime = 2.50f, minMaxTime = 1.30f, minTime = 0.60f, extraTime = 0.5f, randomTime = 0.00f;
    /*x round y miss digits*/ float r1x = 0.35f, r2x = -0.35f; float m1x = 1.1f, m2x = 0.7f;
    /*x score digits*/ float s3x = -3.55f, s4x = -3.95f, s5x = -4.35f, s6x = -4.75f;
    /*x time digits*/ float time1x = 0.4f, time2x = -0.4f;
    /*x top digits*/ float top3x = 3.95f, top4x = 3.55f, top5x = 3.15f, top6x = 2.75f;
    /*x/y goodguys/badguys*/ float[] xPos = {-3.3f, 0f, 3.3f}; float yPosGuy =0.43f;
    /*x/y miss gameover*/ float missYGO = 0.6f, m2xGO = 0.8f, m1xGO = 1.2f;
    /*y todos*/ float scoreY = -4.3f, topY = -4.3f, roundY = -4.15f, missY = -4.77f, timeY = 4.15f, missLabelY = -2f;

//////////////////////////////////////////////////////////////////////////////////////////////
// GAMEOBJECTS ORIGINALES Y CLONES                                                          //
//////////////////////////////////////////////////////////////////////////////////////////////    

    /*Edge*/ public GameObject leftEdge, rightEdge; GameObject leftEdgeClone, rightEdgeClone;
    /*Guys*/ public GameObject GangA, GangB, GangC, Lady, Professor, Police; GameObject[] guys; 
    /*Miss Digit*/ GameObject missDigit1, missDigit2, missPatchClone; 
    /*Miss Digit (Game Over)*/ GameObject missDigit1GO, missDigit2GO;
    /*Numbers Clone*/ public GameObject greenNumber, whiteNumber, missPatch, roundPatch;
    /*Round*/ GameObject roundDigit1, roundDigit2, roundPatchClone;
    /*Scenary*/ public GameObject missLabel, stage; GameObject[] missLabelArr = { null, null, null };
    /*Score*/ GameObject scoreDigit3, scoreDigit4, scoreDigit5, scoreDigit6; 
    /*Sharpshooter*/ public GameObject sharpshooterText, superSharphooterText, blueScreen; 
    /*Sharpshooter Clone*/ GameObject sharpshooterTextClone, superSharphooterTextClone, blueScreenClone;
    /*Time*/ GameObject timeDigit1, timeDigit2;
    /*Top*/ GameObject topDigit3, topDigit4, topDigit5, topDigit6;    

//////////////////////////////////////////////////////////////////////////////////////////////
// FUNCIONES DE INICIO                                                                      //
//////////////////////////////////////////////////////////////////////////////////////////////

    void Start() { createEdge(); createNumbers(); StartCoroutine (showRound1()); }

    void Update() {
        updateScore(); updateTopScore(); updateRound(); updateMissed(); updateTime();
        guysEnter(); guysLeave(); timer(); shot();
    }

    void Awake() {
        audioManager = FindObjectOfType<AudioManagerController>();
        randomNumberGuyArr = new int[300];
        guys = new GameObject[300];
        isShot = new bool[300];
    }

//////////////////////////////////////////////////////////////////////////////////////////////
// FUNCIONES DE DISPARO                                                                     //
//////////////////////////////////////////////////////////////////////////////////////////////

    public void shot () {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 raycastPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(raycastPos, Vector2.zero);
            if(hit.collider!=null && areInvincible == false) { 
                if (hit.collider.gameObject.tag == "Good") { StartCoroutine(goodGuyShot(hit)); return; }
                if (hit.collider.gameObject.tag == "Bad")  { badGuyShot(hit);  return; }
            } audioManager.Play("shot");
        }
    }

    public void badGuyShot (RaycastHit2D hit) {
        hit.collider.gameObject.GetComponent<Animator>().Play("isShot");
        audioManager.Play("hit");
    }

    IEnumerator goodGuyShot (RaycastHit2D hit) {
        hit.collider.gameObject.GetComponent<Animator>().Play("isShot");
        audioManager.Play("fail");
        stage.GetComponent<Animator>().Play("isRed");
        wasAGoodGuyShot = true; areInvincible = true;
        destroyEdge(); stopTimer();
        yield return new WaitForSeconds(0.01f);
        updateGuysAnimations();
        int i = (round-1)*3;
        if(guysAnimations[0] == "isShot" && guys[i+0].tag == "Good") { missed++; createMissLabel(0); }
        if(guysAnimations[1] == "isShot" && guys[i+1].tag == "Good") { missed++; createMissLabel(1); }
        if(guysAnimations[2] == "isShot" && guys[i+2].tag == "Good") { missed++; createMissLabel(2); }
    }

    /*Listo*/ IEnumerator fastShot(){
        yield return new WaitForSeconds(1);
        updateGuysAnimations();
        int i = (round-1)*3;
        if(guysAnimations[0] == "isShot" && guys[i+0].tag == "Bad")  { wasFastShot[0] = true; }
        if(guysAnimations[1] == "isShot" && guys[i+1].tag == "Bad")  { wasFastShot[1] = true; }
        if(guysAnimations[2] == "isShot" && guys[i+2].tag == "Bad")  { wasFastShot[2] = true; }
    } 

    public void resetWasFastShot() { wasFastShot[0] = false; wasFastShot[1] = false; wasFastShot[2] = false; }

//////////////////////////////////////////////////////////////////////////////////////////////
// ACTUALIZAR ANIMACIONES DE NÚMEROS                                                        //
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
        if(round > 9 && roundDigit2 == null) { createSecondRoundNumber(); updateNumber(roundDigit2, (round / 10) % 10); } 
        if(round > 9) { updateNumber(roundDigit2, (round / 10) % 10); }
    }

    /*Listo*/ public void updateMissed() { 
        updateNumber(missDigit1, missed % 10);
        if(missed > 9 && missDigit2 == null) { createSecondMissNumber(); updateNumber(missDigit2, (missed / 10) % 10); } 
        if(missed > 9) { updateNumber(missDigit2, (missed / 10) % 10); }
    }

    /*Listo*/ public void updateTime() { updateNumber(timeDigit1, timetime % 10); updateNumber(timeDigit2, (timetime / 10) % 10); }

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
// GENERADOR DE TIEMPO ALEATORIO Y CRONÓMETRO                                               //
//////////////////////////////////////////////////////////////////////////////////////////////

    /*Listo*/ public int generateRandomTime() {
        timetime = UnityEngine.Random.Range( Convert.ToInt32((maxTime*10)-10), Convert.ToInt32(maxTime*10) );
        randomTime = (float) timetime/10;
        if (randomTime < minTime ) { randomTime = minTime; timetime = Convert.ToInt32(randomTime)*10; }
        if (maxTime > minMaxTime) { maxTime = maxTime - 0.05f; }
        return timetime;
    }

    public void timer() {
        if(isTimerRunning) { timeAfterShootGoodGuy = timeAfterShootGoodGuy + Time.deltaTime;  }
    }

    public void restartTimer() { timeAfterShootGoodGuy = 0; isTimerRunning = true; wasAGoodGuyShot = false; }
    
    public void stopTimer() { isTimerRunning = false; }

//////////////////////////////////////////////////////////////////////////////////////////////
// GENERACIÓN DE GOOD GUYS Y BAD GUYS                                                       //
//////////////////////////////////////////////////////////////////////////////////////////////

    /*Listo*/ public void generateGoodGuy(int i) {
        randomNumberGuy = UnityEngine.Random.Range(0,3); randomNumberGuyArr[i] = randomNumberGuy;
        /*No se repite*/ if ((i%3==2 && ((isBadGuy[(i%3)-1] == false && randomNumberGuyArr[i-1] == randomNumberGuy) || 
            (isBadGuy[(i%3)-2] == false && randomNumberGuyArr[i-2] == randomNumberGuy))) || 
            (i%3==1 && (isBadGuy[(i%3)-1] == false && randomNumberGuyArr[i-1] == randomNumberGuy))) { generateBadGuy(i); return; }
        /*Generate lady*/ if (randomNumberGuy == 0) { guys[i] = Instantiate(Lady, new Vector3( xPos[i%3]-10, yPosGuy, 0.1f), Quaternion.identity); return; }
        /*Generate professor*/ if (randomNumberGuy == 1) { guys[i] = Instantiate(Professor, new Vector3( xPos[i%3]-10, yPosGuy, 0.1f), Quaternion.identity); return; }
        /*Generate police*/ if (randomNumberGuy == 2) { guys[i] = Instantiate(Police, new Vector3( xPos[i%3]-10, yPosGuy, 0.1f), Quaternion.identity); return; }
    }

    /*Listo*/ public void generateBadGuy(int i) {
        randomNumberGuy = UnityEngine.Random.Range(0,3); randomNumberGuyArr[i] = randomNumberGuy;
        /*No se repite*/ if ((i%3==2 && ((isBadGuy[(i%3)-1] && randomNumberGuyArr[i-1] == randomNumberGuy) || 
            (isBadGuy[(i%3)-2] && randomNumberGuyArr[i-2] == randomNumberGuy))) || 
            (i%3==1 && (isBadGuy[(i%3)-1] && randomNumberGuyArr[i-1] == randomNumberGuy))) { generateBadGuy(i); return; }
        /*Generate Gang A*/ if (randomNumberGuy == 0) { guys[i] = Instantiate(GangA, new Vector3( xPos[i%3]-10, yPosGuy, 0.1f), Quaternion.identity); return; }
        /*Generate Gang B*/ if (randomNumberGuy == 1) { guys[i] = Instantiate(GangB, new Vector3( xPos[i%3]-10, yPosGuy, 0.1f), Quaternion.identity); return; }
        /*Generate Gang C*/ if (randomNumberGuy == 2) { guys[i] = Instantiate(GangC, new Vector3( xPos[i%3]-10, yPosGuy, 0.1f), Quaternion.identity); return; }
    }

    /*Listo*/ public void generateGuys() {
        if (isBadGuy[0]) { generateBadGuy(((round-1)*3) + 0); } else { generateGoodGuy(((round-1)*3) + 0); }
        if (isBadGuy[1]) { generateBadGuy(((round-1)*3) + 1); } else { generateGoodGuy(((round-1)*3) + 1); }
        if (isBadGuy[2]) { generateBadGuy(((round-1)*3) + 2); } else { generateGoodGuy(((round-1)*3) + 2); }
    }

    /*Listo*/ public void guysPositions () { 
        for (int i = 0; i < 3; i++){ if (UnityEngine.Random.Range(0,2) == 0) { totalBadGuys++; isBadGuy[i] = true; } else { isBadGuy[i] = false; } }
        /*Si no hay bad guys, genera uno*/ if (totalBadGuys == 0) { isBadGuy[UnityEngine.Random.Range(0,3)] = true; totalBadGuys++; }
        /*Si hay tres bad guys, elimina uno*/ if (totalBadGuys == 3) { isBadGuy[UnityEngine.Random.Range(0,3)] = false; totalBadGuys--; }
    }

    public void guysEnter() {
        /*Si no deben moverse, no lo hagan*/ if (areEntering == false) { return; }
        int i = (round-1)*3;
        guys[i+0].transform.position = Vector3.MoveTowards(guys[i+0].transform.position, new Vector3(xPos[(i+0)%3], yPosGuy, 0.1f), moveSpeed * Time.deltaTime);
        guys[i+1].transform.position = Vector3.MoveTowards(guys[i+1].transform.position, new Vector3(xPos[(i+1)%3], yPosGuy, 0.1f), moveSpeed * Time.deltaTime);
        guys[i+2].transform.position = Vector3.MoveTowards(guys[i+2].transform.position, new Vector3(xPos[(i+2)%3], yPosGuy, 0.1f), moveSpeed * Time.deltaTime);
    }

    public void guysLeave() {
        /*Si no deben moverse, no lo hagan*/ if (areLeaving == false || round == 1) { return; }
        int i = (round-2)*3;
        guys[i+0].transform.position = Vector3.MoveTowards(guys[i+0].transform.position, new Vector3(6, yPosGuy, 0.1f), moveSpeed * Time.deltaTime);
        guys[i+1].transform.position = Vector3.MoveTowards(guys[i+1].transform.position, new Vector3(6, yPosGuy, 0.1f), moveSpeed * Time.deltaTime);
        guys[i+2].transform.position = Vector3.MoveTowards(guys[i+2].transform.position, new Vector3(6, yPosGuy, 0.1f), moveSpeed * Time.deltaTime);
    }

    public void moveAnimator() {
        int i = (round-1)*3; 
        guys[i+0].GetComponent<Animator>().Play("isMoving");
        guys[i+1].GetComponent<Animator>().Play("isMoving");
        guys[i+2].GetComponent<Animator>().Play("isMoving");
        if (round == 1) { return; }
        int j = (round-2)*3;
        guys[j+0].GetComponent<Animator>().Play("isMoving");
        guys[j+1].GetComponent<Animator>().Play("isMoving");
        guys[j+2].GetComponent<Animator>().Play("isMoving");
    }

    public void idleAnimator() {
        int i = (round-1)*3; 
        guys[i+0].GetComponent<Animator>().Play("isIdle");
        guys[i+1].GetComponent<Animator>().Play("isIdle");
        guys[i+2].GetComponent<Animator>().Play("isIdle");
        if (round == 1) { return; }
        int j = (round-2)*3;
        guys[j+0].GetComponent<Animator>().Play("isIdle");
        guys[j+1].GetComponent<Animator>().Play("isIdle");
        guys[j+2].GetComponent<Animator>().Play("isIdle");
    }

    public void destroyOldGuys() {
        if(round == 1) { return; }
        int i = (round-2)*3;
        Destroy(guys[i+0]); Destroy(guys[i+1]); Destroy(guys[i+2]);
    }

    public void updateGuysAnimations () { 
        int i = (round-1)*3;
        currentClipInfo = guys[i+0].GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        guysAnimations[0] = currentClipInfo[0].clip.name;
        currentClipInfo = guys[i+1].GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        guysAnimations[1] = currentClipInfo[0].clip.name;
        currentClipInfo = guys[i+2].GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        guysAnimations[2] = currentClipInfo[0].clip.name;
    }

    public void missedBadGuy (int i) {
        if (leftEdgeClone != null && rightEdge != null) { destroyEdge(); }
        stage.GetComponent<Animator>().Play("isRed");
        createMissLabel(i);
        int j = (round-1)*3;
        guys[i+j].GetComponent<Animator>().Play("isIdle");
        audioManager.Play("fail");
    }

    public void updateAnimationInGameObject (GameObject go, string anim) { go.GetComponent<Animator>().Play(anim); }

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
        roundDigit2 = Instantiate (greenNumber, new Vector3 (r2x, roundY, 0f), Quaternion.identity);
        roundPatchClone = Instantiate(roundPatch);
        roundDigit2.GetComponent<Animator>().Play("is1");
    }

    /*Listo*/ public void createMissLabel(int i) {
        missLabelArr[i] = Instantiate (missLabel, new Vector3 (xPos[i], missLabelY, 0f), Quaternion.identity);
        missLabelArr[i].GetComponent<Animator>().Play("On");
    }

    public void destroyAllMissLabels() { 
        if (missLabelArr[0] != null) { Destroy(missLabelArr[0]); }
        if (missLabelArr[1] != null) { Destroy(missLabelArr[1]); }
        if (missLabelArr[2] != null) { Destroy(missLabelArr[2]); }
    }

    /*Listo*/ public void createGameOverMiss() {
        missDigit1GO = Instantiate (whiteNumber, new Vector3 (m1xGO, missYGO, 0f), Quaternion.identity);
        missDigit2GO = Instantiate (whiteNumber, new Vector3 (m2xGO, missYGO, 0f), Quaternion.identity);
        updateNumber(missDigit1GO, missed % 10); updateNumber(missDigit2GO, (missed / 10) % 10); 
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

    /*Listo*/ public void resetStage() { 
        currentClipInfo = stage.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        stageAnimation = currentClipInfo[0].clip.name;
        if(stageAnimation == "isRed") { stage.GetComponent<Animator>().Play("isIdle"); createEdge(); destroyAllMissLabels(); }
    }

//////////////////////////////////////////////////////////////////////////////////////////////
// EVENTOS DE RONDA                                                                         //
//////////////////////////////////////////////////////////////////////////////////////////////

    /*Lista*/ IEnumerator showRound1() {
        yield return new WaitForSeconds(5);
        //audioManager.StopAllAudio();
        Destroy(GameObject.Find("Round"));
        StartCoroutine(newRound());
    }

    IEnumerator newRound() {
        totalBadGuys = 0; badGuysMissed = 0;
        guysPositions(); generateGuys(); generateRandomTime();
        audioManager.Play("move");
        areEntering = true; areLeaving = true;
        moveAnimator();
        yield return new WaitForSeconds(6);
        areEntering = false; areLeaving = false;
        idleAnimator();
        destroyOldGuys();
        audioManager.Play("ready");
        yield return new WaitForSeconds(0.8f); /*Tiempo para ejecutar animación*/
        areInvincible = false;
        StartCoroutine(fastShot());
        float totalTime = (float) extraTime + randomTime;
        restartTimer();
        yield return new WaitForSeconds(totalTime);
        stopTimer();
        float waitMore = 3 - (totalTime - timeAfterShootGoodGuy); if (waitMore < 0) { waitMore = 0; }
        if (wasAGoodGuyShot) { yield return new WaitForSeconds(waitMore); }
        areInvincible = true;
        StartCoroutine(checkRound());
        yield return new WaitForSeconds(0.85f); /*Tiempo para que se procese badGuysMissed*/
        yield return new WaitForSeconds((3 * badGuysMissed) + 0.8f);
        round++;
        if (round == sharpshooterLevel)      { StartCoroutine(sharpshooter());      yield return new WaitForSeconds(5); }
        if (round == superSharpshooterLevel) { StartCoroutine(superSharpshooter()); yield return new WaitForSeconds(5); }
        if (missed > 9 || round == 99) { round--; updateRound(); updateMissed(); gameOver(); } else { StartCoroutine(newRound()); }
    }

    IEnumerator checkRound() {
        int i = (round-1)*3; 

        updateGuysAnimations();
        guys[i+0].GetComponent<Animator>().Play("isMoving");
        guys[i+1].GetComponent<Animator>().Play("isMoving");
        guys[i+2].GetComponent<Animator>().Play("isMoving");
        yield return new WaitForSeconds(0.8f); /*Tiempo para ejecutar animación*/

        if(guysAnimations[0] == "isShot" && guys[i+0].tag == "Bad") {
            if(wasFastShot[0] == true) { score = score + 400; } score = score + 100; }
        if(guysAnimations[1] == "isShot" && guys[i+1].tag == "Bad") {
            if(wasFastShot[1] == true) { score = score + 400; } score = score + 100; }
        if(guysAnimations[2] == "isShot" && guys[i+2].tag == "Bad") {
            if(wasFastShot[2] == true) { score = score + 400; } score = score + 100; }

        if(guysAnimations[0] == "isIdle" && guys[i+0].tag == "Bad") { badGuysMissed++; }
        if(guysAnimations[1] == "isIdle" && guys[i+1].tag == "Bad") { badGuysMissed++; }
        if(guysAnimations[2] == "isIdle" && guys[i+2].tag == "Bad") { badGuysMissed++; }

        if(guysAnimations[0] == "isIdle" && guys[i+0].tag == "Bad") {
            missed++; missedBadGuy(0); yield return new WaitForSeconds(3); updateAnimationInGameObject(guys[i+0],"isMoving"); }
        if(guysAnimations[1] == "isIdle" && guys[i+1].tag == "Bad") {
            missed++; missedBadGuy(1); yield return new WaitForSeconds(3); updateAnimationInGameObject(guys[i+1],"isMoving"); }
        if(guysAnimations[2] == "isIdle" && guys[i+2].tag == "Bad") {
            missed++; missedBadGuy(2); yield return new WaitForSeconds(3); updateAnimationInGameObject(guys[i+2],"isMoving"); }

        resetWasFastShot();
        resetStage();
    }

    /*Listo*/ IEnumerator sharpshooter() {
        sharpshooterTextClone = Instantiate(sharpshooterText); blueScreenClone = Instantiate(blueScreen);
        audioManager.Play("sharpshooter");
        yield return new WaitForSeconds(5);
        Destroy(sharpshooterTextClone); Destroy(blueScreenClone);
    }

    /*Listo*/ IEnumerator superSharpshooter() {
        sharpshooterTextClone = Instantiate(sharpshooterText);
        superSharphooterTextClone = Instantiate(superSharphooterText);
        blueScreenClone = Instantiate(blueScreen);
        audioManager.Play("sharpshooter");
        yield return new WaitForSeconds(5);
        Destroy(sharpshooterTextClone); Destroy(superSharphooterTextClone); Destroy(blueScreenClone);
    }

    public void gameOver() {
        createSecondMissNumber();
        createGameOverMiss();
        dontDestroy();
        SceneManager.LoadScene("GameOver");
    }
}
