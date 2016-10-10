using UnityEngine;
using System.Collections;

public class MouseOcculusion : MonoBehaviour {
    public BoundaryManager boundaries;
    public bool changeColorWithBoundaries = true;
    private MeshRenderer render;
    public KeyCode keyboardInvisibilityBubbleButton = KeyCode.None;
    public float distance = 10f;
    private bool prevInputState = false;

	// Use this for initialization
	void Start () {
        render = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.localScale = new Vector3(distance, distance, distance);
        Vector3 mouse = Input.mousePosition;
        Vector3 screenMouse = Camera.allCameras[0].ScreenToWorldPoint(mouse);
        transform.position = new Vector3(screenMouse.x, transform.position.y, screenMouse.z);
        if (changeColorWithBoundaries)
            render.material.color = boundaries.renderers[0].material.color;
        if (keyboardInvisibilityBubbleButton != KeyCode.None)
        {
            bool inputState = Input.GetKey(keyboardInvisibilityBubbleButton);
            if (inputState && !prevInputState)
                render.enabled = !render.enabled;
            prevInputState = inputState;
        }
	}
}
