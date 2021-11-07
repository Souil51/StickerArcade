using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPointsDisplayController : MonoBehaviour
{
    private float fTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
    }

    // Update is called once per frame
    void Update()
    {
        fTimer += Time.deltaTime;

        if(fTimer > GameController.TIMER_ADD_POINTS)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
