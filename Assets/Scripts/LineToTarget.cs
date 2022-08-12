using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineToTarget : MonoBehaviour
{

    public GameObject Line;
    GameObject lineAnchor;
    public bool displayLineToTarget;
    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        lineAnchor = Line.transform.Find("Anchor").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (EditingManager.Instance.GetLastEdited() != null)
        {
            target = EditingManager.Instance.GetLastEdited().transform;
            Line.SetActive(true);
            if (lineAnchor.transform.position != target.position)
            {
                lineAnchor.transform.position = target.position;
            }
        }
        else
        {
            target = null;
            Line.SetActive(false);
        }
        Line.SetActive(displayLineToTarget);
    }
}
