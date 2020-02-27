using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeEmail : MonoBehaviour
{
    public GameObject Panel;

    public void UpdateEmail()
    {
        if (Panel != null)
        {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }
}
