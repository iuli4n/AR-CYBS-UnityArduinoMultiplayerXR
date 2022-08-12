using UnityEngine;

public class GazePointerPrefabView : MonoBehaviour
{
    public GazePointerModel pointerModel;
    //public GameObject prefab;

    bool isPrefabCreated = false;
    public GameObject newPrefab;

    void Start()
    {
        pointerModel.onPositionUpdate += ChangePos;
        pointerModel.onDirectionUpdate += ChangeDirection;

        if (!isPrefabCreated)
        {
            //newPrefab = Instantiate(prefab, pointerModel.StartPos, Quaternion.identity);
            newPrefab.name = "PointerPrefab";
            isPrefabCreated = true;
        }

    }

    public void ChangePos()
    {


        newPrefab.transform.position = pointerModel.StartPos;

    }

    public void ChangeDirection()
    {
        //transform.rotation = pointerModel.Direction;
        newPrefab.transform.rotation = pointerModel.Direction;
        newPrefab.transform.Rotate(0, -90, 0);

    }

    private void OnDestroy()
    {
        pointerModel.onPositionUpdate -= ChangePos;
        pointerModel.onPositionUpdate -= ChangeDirection;

    }
}
