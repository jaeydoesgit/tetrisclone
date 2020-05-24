using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTetromino : MonoBehaviour
{
    private int startNumber = 20;
    // Start is called before the first frame update
    void Start()
    {
        tag = "currentGhostTetromino";

        foreach (Transform mino in transform) {

            mino.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .2f);
        }

        startNumber = 20;
        //transform.position = new Vector3(5, 20, 0);
    }

    // Update is called once per frame
    void Update()
    {
        FollowActiveTetromino();
    }

    void FollowActiveTetromino () {

        Transform currentActiveTetrominoTransform = GameObject.FindGameObjectWithTag("currentActiveTetromino").transform;

        int my_pos_y = (int)transform.position.y;

        int pos_x = (int)currentActiveTetrominoTransform.position.x;
        int pos_y = startNumber - my_pos_y;

        if (my_pos_y == 11) {
            startNumber = 10;
        }

        //if (my_pos_y == 6) {
        //    startNumber = 5;
        //}

        //transform.position += new Vector3(0, 10, 0);

        transform.position = GameObject.FindGameObjectWithTag("currentActiveTetromino").transform.position; // new Vector3(pos_x, 10, 0); // currentActiveTetrominoTransform.position;
        transform.rotation = currentActiveTetrominoTransform.rotation;

        MoveDown();
    }

    void MoveDown () {

        while (CheckIsValidPosition ()) {

            int y_pos = (int)transform.position.y;

            transform.position += new Vector3(0, -1, 0);
        }

        if (!CheckIsValidPosition ()) {

            transform.position += new Vector3(0, 1, 0);
        }
    }

    bool CheckIsValidPosition () {

        foreach (Transform mino in transform) {

            Vector2 pos = FindObjectOfType<Game> ().Round (mino.position);

            if (FindObjectOfType<Game>().CheckIsInsideGrid(pos) == false)
                return false;

            //if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null)
            //    return false;

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null &&
                FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent.tag == "currentActiveTetromino")
                return true;

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null &&
                FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent.tag == "currentGhostTetromino")
                return true;

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null &&
                FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
                return false;
        }

        return true;
    }
}
