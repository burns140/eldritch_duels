using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePsw : MonoBehaviour
{

    public GameObject Panel;

    public void ChangePassword()
    {
        if (Panel != null)
        {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }
}
