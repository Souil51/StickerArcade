using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPickedController : MonoBehaviour
{
    private List<GameObject> lstColliding;

    // Start is called before the first frame update
    void Start()
    {
        InitObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitObject()
    {
        if (lstColliding == null)
            lstColliding = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        InitObject();

        if (!lstColliding.Contains(collision.gameObject))
            lstColliding.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        lstColliding.Remove(collision.gameObject);
    }

    public List<GameObject> GetCollidingList()
    {
        return this.lstColliding;
    }
}
