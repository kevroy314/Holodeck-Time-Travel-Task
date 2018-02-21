using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockCursor : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Confined;
    }
}
