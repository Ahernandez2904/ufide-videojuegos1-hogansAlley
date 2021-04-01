using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour {

    [SerializeField]
    Transform selector;

    void Update() {
        changePos(position());
        selectGame(position());
    }

    public int position() {
        if (Input.mousePosition.y >= 160) {return 0; }
        else if (Input.mousePosition.y <= 120) {return 2; }
        else { return 1; }
    }

    public void changePos(int pos) {
        if (pos == 0) { selector.transform.position = new Vector3(selector.transform.position.x, -0.6f, selector.transform.position.z); }
        if (pos == 1) { selector.transform.position = new Vector3(selector.transform.position.x, -1.4f, selector.transform.position.z); }
        if (pos == 2) { selector.transform.position = new Vector3(selector.transform.position.x, -2.2f, selector.transform.position.z); }
    }

    public void selectGame(int pos){
        if (Input.GetMouseButtonDown(0) && pos == 0){
            SceneManager.LoadScene("Level1");
        } 
        return;
    }
}
