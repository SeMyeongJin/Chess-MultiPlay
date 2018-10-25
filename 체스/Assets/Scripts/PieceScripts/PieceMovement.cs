using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovement : MonoBehaviour {

	public static PieceMovement Instance { get; set; }

    public GameObject posiblePosition;
    private List<GameObject> positions;

    private void Start()
    {
        Instance = this;
        positions = new List<GameObject>();
    }

    private GameObject GetPositions()
    {
        GameObject obj = positions.Find(g => !g.activeSelf);

        if (obj == null)
        {
            obj = Instantiate(posiblePosition);
            positions.Add(obj);
        }

        return obj;
    }

    public void PosibleMovePosition(bool[,] move)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (move [i, j])
                {
                    GameObject obj = GetPositions();
                    obj.SetActive(true);
                    obj.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
                }
            }
        }
    }

    public void ExcludePosition()
    {
        foreach (GameObject obj in positions)
            obj.SetActive(false);
    }
}
