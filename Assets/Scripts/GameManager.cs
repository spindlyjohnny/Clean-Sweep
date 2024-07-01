using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake() {
        DontDestroyOnLoad(gameObject);
        print(PlayerPrefs.GetInt("Tutorial Lore"));
        print(PlayerPrefs.GetInt("Level 1 Lore"));
        print(PlayerPrefs.GetInt("Level 2 Lore"));
        print(PlayerPrefs.GetInt("HasPlayedBefore"));
        //if (PlayerPrefs.GetInt("HasPlayedBefore") != 1)PlayerPrefs.SetInt("HasPlayedBefore", 0);
        if (!Application.isEditor && PlayerPrefs.GetInt("HasPlayedBefore") != 1) { // check if in build
            PlayerPrefs.SetInt("HasPlayedBefore", 0);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("HasPlayedBefore", 0) == 0) { // if it is the first time playing, set all lore values to 0
            // First time playing the game
            PlayerPrefs.SetInt("Tutorial Lore", 0);
            PlayerPrefs.SetInt("Level 1 Lore", 0);
            PlayerPrefs.SetInt("Level 2 Lore", 0);
            PlayerPrefs.SetInt("HasPlayedBefore", 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
