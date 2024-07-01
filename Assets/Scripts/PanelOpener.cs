using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelOpener : MonoBehaviour
{
    public GameObject panel;
    public Button button;
    public string lorepiece;
    private void Start()
    {
        button = gameObject.GetComponent<Button>();
        //PlayerPrefs.SetInt("FIRSTTIMEOPENING", 1);
        button.interactable = false;
        /*if (PlayerPrefs.GetInt("FIRSTTIMEOPENING", 1) == 1) {
            PlayerPrefs.SetInt("FIRSTTIMEOPENING", 0);
        }*/
    }
    private void Update()
    {
        if (PlayerPrefs.GetInt(lorepiece) == 1 && PlayerPrefs.GetInt("HasPlayedBefore") == 1)
        {
            button.interactable = true;
        } else {
            button.interactable = false;
        }
    }
    public void OpenPanel()
    {
        if (panel != null)
        {
            bool isactive = panel.activeSelf;
            panel.SetActive(!isactive);
        }
    }
}