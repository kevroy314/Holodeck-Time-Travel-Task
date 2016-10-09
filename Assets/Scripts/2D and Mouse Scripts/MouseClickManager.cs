using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MouseClickManager : MonoBehaviour
{
    public ItemGenerator itemGen;
    private Transform itemsParent;
    private Vector3 down = new Vector3(0, -1, 0);
    private bool prevMouseButtonState = false;
    public Text itemsRemainingText;
    public AudioClip completeSound;
    private bool complete = false;
    // Use this for initialization
    void Start()
    {
        itemsParent = itemGen.transform;
    }

    // Update is called once per frame
    void Update()
    {
        int remaining = 0;
        List<Transform> children = new List<Transform>(itemsParent.childCount);
        for (int i = 0; i < children.Capacity; i++)
        {
            children.Add(itemsParent.GetChild(i));
            if (children[i].GetComponent<BoxCollider>() == null)
            {
                BoxCollider collider = children[i].gameObject.AddComponent<BoxCollider>();
                collider.center = new Vector3(0f, 0f, 0f);
            }
            if (!children[i].gameObject.GetComponentInChildren<ClickableObject>().HasBeenClicked())
                remaining++;
        }
        if (remaining == 0 && !complete)
        {
            gameObject.AddComponent<AudioSource>().PlayOneShot(completeSound);
            complete = true;
        }
        itemsRemainingText.text = remaining + " Items Remaining";
        RaycastHit hit;
        bool mouseButtonState = Input.GetMouseButtonDown(0);
        if (mouseButtonState && !prevMouseButtonState)
        {
            if (Physics.Raycast(Camera.allCameras[0].ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (children.Contains(hit.collider.gameObject.transform.parent))
                {
                    ClickableObject clickableObj = hit.collider.gameObject.GetComponentInChildren<ClickableObject>();
                    if (clickableObj.clickable)
                        clickableObj.Click();
                }
            }
        }
        prevMouseButtonState = mouseButtonState;
    }
}
