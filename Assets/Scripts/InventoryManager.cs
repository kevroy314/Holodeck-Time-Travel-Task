using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Chronos;

public class InventoryManager : MonoBehaviour
{
    public Image displayImage;
    public Material emptyMaterial;
    public Timeline time;
    public float placeDistance = 3f;

    public KeyCode placeKeyCode = KeyCode.Space;
    public string placeButtonString = "a";
    public KeyCode nextKeyCode = KeyCode.Q;
    public string nextButtonString = "y";

    private bool previousInputState = false;
    private bool previousNextInputState = false;

    public ItemGenerator generator;
    private ClickableObject[] clickableObjects;
    private Queue<int> objectHeldList;

    public int closestIndex = -1;
    public float closestDist = float.MaxValue;

    private int currentInventoryIndex = 0;

    private bool firstCall = true;

    // Use this for initialization
    void Start()
    {
        displayImage.material = emptyMaterial;
    }

    void Update()
    {
        if (firstCall)
        {
            //Needs to be run on first update because Start() order isn't promised
            int numChildren = generator.gameObject.transform.childCount;
            List<ClickableObject> clickable = new List<ClickableObject>();
            objectHeldList = new Queue<int>();
            for (int i = 0; i < numChildren; i++)
            {
                ClickableObject tmp = generator.gameObject.transform.GetChild(i).GetComponentInChildren<ClickableObject>();
                if (tmp != null)
                {
                    clickable.Add(tmp);
                    tmp.gameObject.transform.parent.gameObject.SetActive(false);
                }
            }
            clickableObjects = clickable.ToArray();
            for (int i = 0; i < clickableObjects.Length; i++)
                objectHeldList.Enqueue(i);
            firstCall = false;
        }
        //Find closest object not being held
        closestIndex = -1;
        closestDist = float.MaxValue;
        for (int i = 0; i < clickableObjects.Length; i++)
        {
            if (!objectHeldList.Contains(i))
            {
                float dist = Vector3.Distance(gameObject.transform.position, clickableObjects[i].gameObject.transform.position);
                if (dist < closestDist)
                {
                    closestIndex = i;
                    closestDist = dist;
                }
            }
        }

        bool inputState = Input.GetKey(placeKeyCode) || Input.GetButton(placeButtonString);
        if (inputState && !previousInputState)
        {
            //If there are objects available to potentially pick up AND the closest object is visible AND the object is within the place distance AND the object is clickable
            if (closestIndex != -1 && clickableObjects[closestIndex].gameObject.GetComponent<Renderer>().isVisible && closestDist <= placeDistance && clickableObjects[closestIndex].clickable) //And it is visible, within the min distance, and clickable
            {
                clickableObjects[closestIndex].gameObject.transform.parent.gameObject.SetActive(false);
                objectHeldList.Enqueue(closestIndex);
            }
            else //No object is being picked up, drop the current item
            {
                if (objectHeldList.Count > 0)
                {
                    int index = objectHeldList.Dequeue();
                    clickableObjects[index].gameObject.transform.parent.gameObject.transform.localPosition = transform.position + (transform.forward * placeDistance);
                    Vector3 newPos = transform.position + (transform.forward * placeDistance);
                    FallFromSky fallScript = clickableObjects[index].gameObject.GetComponent<FallFromSky>();
                    FlyToSky flyScript = clickableObjects[index].gameObject.GetComponent<FlyToSky>();
                    if (fallScript != null)
                    {
                        fallScript.transitionDelay = time.time;
                        fallScript.Start();
                    }
                    if (flyScript != null)
                    {
                        flyScript.transitionDelay = time.time;
                        flyScript.Start();
                    }
                    clickableObjects[index].clickable = true;
                    clickableObjects[index].gameObject.transform.parent.gameObject.SetActive(true);
                }
            }
        }
        previousInputState = inputState;

        bool nextInputState = Input.GetKey(nextKeyCode) || Input.GetButton(nextButtonString);
        if (nextInputState && !previousNextInputState && objectHeldList.Count != 0)
            objectHeldList.Enqueue(objectHeldList.Dequeue());
        previousNextInputState = nextInputState;

        if (objectHeldList.Count != 0)
            displayImage.material = clickableObjects[objectHeldList.Peek()].gameObject.GetComponent<MeshRenderer>().material;
        else
            displayImage.material = emptyMaterial;
    }
}
