using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeDetection : MonoBehaviour {
    [SerializeField] private float threshold;

    [SerializeField] private Text lastSwipedText;

    private Vector2 startPosition;
    private Vector2 direction;
    private string choice = "none";

    private float lastTouch = 0f;


	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 60;
	}
	
	// Update is called once per frame
	void Update () {

        if (Time.time - lastTouch > 1f)
        {
            //Debug.Log(lastTouch.ToString() + "" + Time.time.ToString());
            lastSwipedText.text = "";
        }

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            switch(t.phase)
            {
                case TouchPhase.Began:
                    startPosition = t.position;
                    break;

                case TouchPhase.Moved:
                    direction = t.position - startPosition;
                    break;


                case TouchPhase.Ended:
                    if (Mathf.Abs(direction.x) > threshold)
                    {
                        if (direction.x < 0)
                        {
                            choice = "left";
                        }
                        else
                        {
                            choice = "right";
                        }
                        lastSwipedText.text = choice;
                    }

                    lastTouch = Time.time;
                    break;
            }
        }
		
	}

    void SetXPos(float x)
    {
        transform.localPosition = new Vector3(x, 0, 0);
    }
}
