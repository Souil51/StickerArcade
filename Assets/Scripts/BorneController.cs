using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorneController : MonoBehaviour
{
    private List<Rect> lstSpawnableAreas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitBorne()
    {
        lstSpawnableAreas = new List<Rect>();

        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform tChild = this.transform.GetChild(i);
            BoxCollider2D col2D = tChild.GetComponent<BoxCollider2D>();

            if (col2D != null)
            {
                Rect r = new Rect();

                float top = col2D.offset.y + (col2D.size.y / 2f);
                float btm = col2D.offset.y - (col2D.size.y / 2f);
                float left = col2D.offset.x - (col2D.size.x / 2f);
                float right = col2D.offset.x + (col2D.size.x / 2f);

                Vector3 topRight = transform.TransformPoint(new Vector3(right, top, 0f));
                Vector3 btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));

                r.x = btmLeft.x + tChild.position.x;
                r.y = btmLeft.y + tChild.position.y;
                r.width = topRight.x + tChild.position.x;
                r.height = topRight.y + tChild.position.y;

                lstSpawnableAreas.Add(r);
            }
        }
    }

    public List<Rect> GetSpawnableAreas()
    {
        return this.lstSpawnableAreas;
    }
}
