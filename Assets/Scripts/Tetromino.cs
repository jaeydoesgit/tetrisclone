﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    float fall = 0;
    private float fallSpeed;

    public bool allowRotation = true;
    public bool limitRotation = false;
    public string prefabName;


    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;

    private float continuousVerticalSpeed = 0.05f;
    private float continuousHorizontalSpeed = 0.1f;
    private float buttonDownWaitMax = 0.2f;

    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownWaitTimerHorizontal = 0;
    private float buttonDownWaitTimerVertical = 0;

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;


    public int individualScore = 100;

    private AudioSource audioSource;

    private float individualScoreTime;

    private int touchSensitivityHorizontal = 8;
    private int touchSensitivityVertical = 4;

    Vector2 previousUnitPosition = Vector2.zero;
    Vector2 direction = Vector2.zero;

    bool moved = false;

    void Start () {

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Game.isPaused) {

            CheckUserInput();

            UpdateIndividualScore();

            UpdateFallSpeed();
        }                   
    }

    void UpdateFallSpeed () {

        fallSpeed = Game.fallSpeed;
    }

    void UpdateIndividualScore () {

        if (individualScoreTime < 1) {

            individualScoreTime += Time.deltaTime;

        } else {

            individualScoreTime = 0;

            individualScore = Mathf.Max(individualScore - 10, 0);
        }
    }

    void CheckUserInput()
    {

#if UNITY_IOS

        if (Input.touchCount > 0) {

            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began) {

                previousUnitPosition = new Vector2(t.position.x, t.position.y);

            } else if (t.phase == TouchPhase.Moved) {

                Vector2 touchDeltaPosition = t.deltaPosition;
                direction = touchDeltaPosition.normalized;

                if (Mathf.Abs(t.position.x - previousUnitPosition.x) >= touchSensitivityHorizontal && direction.x < 0 && touchDeltaPosition.y > -10 && t.deltaPosition.y < 10) {

                    MoveLeft();
                    previousUnitPosition = t.position;
                    moved = true;

                } else if (Mathf.Abs(t.position.x - previousUnitPosition.x) >= touchSensitivityHorizontal && direction.x > 0 && t.deltaPosition.y > -10 && t.deltaPosition.y < 10) {

                    MoveRight();
                    previousUnitPosition = t.position;
                    moved = true;

                } else if (Mathf.Abs(t.position.y - previousUnitPosition.y) >= touchSensitivityVertical && direction.y > 0 && t.deltaPosition.x > -10 && t.deltaPosition.x < 10) {

                    MoveDown();
                    previousUnitPosition = t.position;
                    moved = true;
                }

            } else if (t.phase == TouchPhase.Ended) {

                if (!moved && t.position.x > Screen.width / 4) {

                    Rotate();
                }

                moved = false;
            }
        }

        if (Time.time - fall >= fallSpeed) {

            MoveDown();
        }

#else

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow)) {

            movedImmediateHorizontal = false;          
            horizontalTimer = 0;           
            buttonDownWaitTimerHorizontal = 0;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow)) {

            movedImmediateVertical = false;
            verticalTimer = 0;
            buttonDownWaitTimerVertical = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow)) {

            MoveRight ();
        }

        if (Input.GetKey(KeyCode.LeftArrow)) {

            MoveLeft ();           
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {

            Rotate ();
        }

        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed) {

            MoveDown ();
        }

        if (Input.GetKeyUp (KeyCode.Space)) {

            SlamDown();
        }

#endif
    }


    public void SlamDown () {

        while (CheckIsValidPosition ()) {

            transform.position += new Vector3(0, -1, 0);
        }

        if (!CheckIsValidPosition ()) {

            transform.position += new Vector3(0, 1, 0);
            FindObjectOfType<Game>().UpdateGrid(this);

            if (Input.GetKeyDown(KeyCode.DownArrow)) {

                PlayMoveAudio();
            }

        } else {


            transform.position += new Vector3(0, 1, 0);

            FindObjectOfType<Game>().DeleteRow();

            if (FindObjectOfType<Game>().CheckIsAboveGrid(this)) {

                FindObjectOfType<Game>().GameOver();
            }

            PlayLandAudio();
            FindObjectOfType<Game>().SpawnNextTetromino();

            Game.currentScore += individualScore;

            enabled = false;
            tag = "Untagged";
        }
    }


    void MoveLeft () {

        if (movedImmediateHorizontal) {

            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax) {

                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed) {

                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
            movedImmediateHorizontal = true;

        horizontalTimer = 0;

        transform.position += new Vector3(-1, 0, 0);

        if (CheckIsValidPosition()) {

            FindObjectOfType<Game>().UpdateGrid(this);
            PlayMoveAudio();

        } else {

            transform.position += new Vector3(1, 0, 0);
        }
    }

    void MoveRight () {

        if (movedImmediateHorizontal) {

            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax) {

                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed) {

                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
            movedImmediateHorizontal = true;

        horizontalTimer = 0;

        transform.position += new Vector3(1, 0, 0);

        if (CheckIsValidPosition()) {

            FindObjectOfType<Game>().UpdateGrid(this);
            PlayMoveAudio();

        } else {

            transform.position += new Vector3(-1, 0, 0);
        }

    }

    void MoveDown () {


        if (movedImmediateVertical) {

            if (buttonDownWaitTimerVertical < buttonDownWaitMax) {

                buttonDownWaitTimerVertical += Time.deltaTime;
                return;
            }

            if (verticalTimer < continuousVerticalSpeed) {

                verticalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateVertical)
            movedImmediateVertical = true;

        verticalTimer = 0;

        transform.position += new Vector3(0, -1, 0);

        if (CheckIsValidPosition()) {

            FindObjectOfType<Game>().UpdateGrid(this);

            if (Input.GetKeyDown(KeyCode.DownArrow)) {

                PlayMoveAudio();
            }

        } else {

            transform.position += new Vector3(0, 1, 0);

            FindObjectOfType<Game>().DeleteRow();

            if (FindObjectOfType<Game>().CheckIsAboveGrid(this)) {

                FindObjectOfType<Game>().GameOver();
            }

            PlayLandAudio();
            FindObjectOfType<Game>().SpawnNextTetromino();

            Game.currentScore += individualScore;

            enabled = false;
            tag = "Untagged";
        }

        fall = Time.time;
    }

    void Rotate () {

        if (allowRotation) {

            if (limitRotation) {

                if (transform.rotation.eulerAngles.z >= 90) {

                    transform.Rotate(0, 0, -90);

                } else {

                    transform.Rotate(0, 0, 90);
                }
            } else {

                transform.Rotate(0, 0, 90);
            }

            if (CheckIsValidPosition()) {

                FindObjectOfType<Game>().UpdateGrid(this);
                PlayRotateAudio();

            } else {

                if (limitRotation) {

                    if (transform.rotation.eulerAngles.z >= 90) {

                        transform.Rotate(0, 0, -90);

                    } else {

                        transform.Rotate(0, 0, 90);
                    }
                } else {

                    transform.Rotate(0, 0, -90);

                }
            }
        }
    }

    void PlayMoveAudio () {

        audioSource.PlayOneShot(moveSound);
    }

    void PlayRotateAudio () {

        audioSource.PlayOneShot(rotateSound);
    }

    void PlayLandAudio () {

        audioSource.PlayOneShot(landSound);
    }

    bool CheckIsValidPosition () {

        foreach (Transform mino in transform)  {

            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);

            if (FindObjectOfType<Game>().CheckIsInsideGrid(pos) == false) {

                return false;
            }

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && 
                FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform) {

                return false;
            }
        }

        return true;
    }
}
