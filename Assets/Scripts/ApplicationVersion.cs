using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationVersion : MonoBehaviour {
    public Text versiontext;// Start is called before the first frame update
    void Start() {
        versiontext.text = "Version Number: " + Application.version;
    }
}