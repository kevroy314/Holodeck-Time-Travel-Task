using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCursor : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        Cursor.lockState = CursorLockMode.Locked;
	}
}
