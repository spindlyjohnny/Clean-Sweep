using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    public GameObject popupbox;
    public void PopUpFunc() {
        if (popupbox != null) {
            bool isactive = popupbox.activeSelf;
            popupbox.SetActive(!isactive);
        }
        
    }
}
