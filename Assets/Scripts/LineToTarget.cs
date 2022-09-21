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

        EditingManager.Instance.onEditedObjectChanged += OnEditedObjectChanged;

        target = null;
        Line.SetActive(false);
    }

    void OnEditedObjectChanged(GameObject newObject)
    {
        target = EditingManager.Instance.GetLastEdited().transform;
        Line.SetActive(true);
        if (lineAnchor.transform.position != target.position)
        {
            lineAnchor.transform.position = target.position;
        }
        Line.SetActive(displayLineToTarget);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
