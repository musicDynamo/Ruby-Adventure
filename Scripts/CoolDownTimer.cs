using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolDownTimer : MonoBehaviour
{
    public Image coolDown;
    public float rate;

    private bool coolingDown = false;

    // Start is called before the first frame update
    void Start()
    {
        coolDown.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C) && !coolingDown)
        {
            Activate();
        }

        if(coolingDown)
        {
            CoolDown();
        }
    }

    void CoolDown()
    {
        coolDown.fillAmount += rate * Time.deltaTime;
        if(coolDown.fillAmount == 1)
        {
            coolingDown = false;
        }
    }

    void Activate()
    {
        coolDown.fillAmount = 0;
        coolingDown = true;
        Debug.Log("Projectile");
    }
}
