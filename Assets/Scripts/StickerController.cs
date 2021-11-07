using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerController : MonoBehaviour
{
    private GameController m_gameCtrl;
    private bool m_bEnabled = true;
    private bool m_bDragging = false;
    private SpriteRenderer m_sprtRenderder;
    private bool m_bHasBeenCounted = false;

    private float m_fDistanceToMouse;

    // Start is called before the first frame update
    void Start()
    {
        this.m_sprtRenderder = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bDragging && m_bEnabled)
        {
            UpdatePosition();
        }

        if (m_gameCtrl.IsGameEnded())
        {
            m_bDragging = false;
        }
    }

    void OnMouseEnter()
    {
        //m_sprtRenderder.material.color = mouseOverColor;
    }

    void OnMouseExit()
    {
        //m_sprtRenderder.material.color = originalColor;
    }

    void OnMouseDown()
    {
        if (m_gameCtrl.IsGameEnded()) return;

        if (m_bEnabled)
        {
            m_fDistanceToMouse = Vector3.Distance(transform.position, Camera.main.transform.position);
            m_bDragging = true;

            //UpdatePosition();
        }
    }

    void OnMouseUp()
    {
        if (m_gameCtrl.IsGameEnded()) return;

        if (m_bDragging && m_bEnabled)
        {
            m_bDragging = false;

            //m_bEnabled = false;

            transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        }
    }

    public void SetGameController(GameController gameCtrl)
    {
        m_gameCtrl = gameCtrl;
    }

    private Vector3 UpdatePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(m_fDistanceToMouse);
        Vector3 vRes = new Vector3(rayPoint.x, rayPoint.y, -1);
        transform.position = vRes;

        return vRes;
    }

    public bool IsCounted()
    {
        return m_bHasBeenCounted;
    }

    public void SetCounted(bool bCounted)
    {

    }
}
