using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShowSkillDescription : MonoBehaviour
{
    public GameObject Button;
    //public GameObject SkillDescription; 

    public void ShowDescription()
    {
        if (Button.activeInHierarchy == true)
        {
            Button.SetActive(false);
        }
        else
        {
            Button.SetActive(true);
        }
    }
}
