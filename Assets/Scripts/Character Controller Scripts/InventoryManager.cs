using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Chronos;

public class InventoryManager : MonoBehaviour
{
    public GameObject fallPrefabItem;
    public GameObject flyPrefabItem;
    public GameObject foilPrefabItem;

    public AudioClip soundEffect;
    public AudioClip multiSoundEffect;
    private AudioSource audioSrc;

    public Image displayImage;
    public Material emptyMaterial;
    public Timeline time;
    public float placeDistance = 3f;

    public KeyCode placeKeyCode = KeyCode.Space;
    public string placeButtonString = "a";
    public KeyCode nextKeyCode = KeyCode.Q;
    public string nextButtonString = "y";
    public KeyCode pickUpAllCode = KeyCode.P;
    public string pickUpAllButtonString = "back";
    public KeyCode nextItemTypeKeyCode = KeyCode.E;
    public string nextItemTypeButtonString = "b";

    public Material upMaterial;
    public Material downMaterial;
    public Material infinityMaterial;

    public Image typeDisplayImage;

    private bool previousInputState = false;
    private bool previousNextInputState = false;
    private bool previousPickUpAllInputState = false;
    private bool previousNextItemTypeState = false;

    public ItemGenerator generator;
    private ClickableObject[] clickableObjects;
    private LinkedList<int> objectHeldList;

    public int closestIndex = -1;
    public float closestDist = float.MaxValue;

    private int currentInventoryIndex = 0;
    private int currentItemTypeIndex = 0;

    private bool firstCall = true;

    // Use this for initialization
    void Start()
    {
        displayImage.material = emptyMaterial;
        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.clip = soundEffect;
    }

    private static System.Random rng = new System.Random();

    public static List<T> Shuffle<T>(IList<T> list)
    {
        List<T> newList = new List<T>(list);
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return newList;
    }

    void Update()
    {
        if (firstCall)
        {
            //Needs to be run on first update because Start() order isn't promised
            int numChildren = generator.gameObject.transform.childCount;
            List<ClickableObject> clickable = new List<ClickableObject>();
            objectHeldList = new LinkedList<int>();
            for (int i = 0; i < numChildren; i++)
            {
                ClickableObject tmp = generator.gameObject.transform.GetChild(i).GetComponentInChildren<ClickableObject>();
                if (tmp != null)
                {
                    clickable.Add(tmp);
                    tmp.gameObject.transform.parent.gameObject.SetActive(false);
                }
            }
            Shuffle<ClickableObject>(clickable);
            clickableObjects = clickable.ToArray();
            for (int i = 0; i < clickableObjects.Length; i++)
                objectHeldList.AddFirst(i);
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
            if (closestIndex != -1 && clickableObjects[closestIndex].gameObject.GetComponent<MeshRenderer>().isVisible && closestDist <= placeDistance) //And it is visible, within the min distance, and clickable
            {
                clickableObjects[closestIndex].gameObject.transform.parent.gameObject.SetActive(false);
                objectHeldList.AddFirst(closestIndex);

                audioSrc.pitch = 1f;
                audioSrc.clip = soundEffect;
                audioSrc.Play();
            }
            else //No object is being picked up, drop the current item
            {
                if (objectHeldList.Count > 0)
                {
                    int index = objectHeldList.First.Value;
                    objectHeldList.RemoveFirst();
                    Texture2D prevTexture = clickableObjects[index].mainTexture;
                    Texture2D prevClickTexture = clickableObjects[index].clickTexture;
                    Transform prevParent = clickableObjects[index].gameObject.transform.parent.transform.parent;
                    FallFromSky fallScript = clickableObjects[index].gameObject.GetComponent<FallFromSky>();
                    FlyToSky flyScript = clickableObjects[index].gameObject.GetComponent<FlyToSky>();
                    int prevItemNum = int.Parse(clickableObjects[index].gameObject.transform.parent.gameObject.name.Substring(4));
                    Debug.Log(prevItemNum);
                    GameObject obj;
                    if (currentItemTypeIndex == 2)
                        obj = ItemGenerator.GenerateFall(fallPrefabItem, prevParent, transform.position + (transform.forward * placeDistance), prevTexture, prevClickTexture, time.time, time, prevItemNum);
                    else if (currentItemTypeIndex == 1)
                        obj = ItemGenerator.GenerateFly(flyPrefabItem, prevParent, transform.position + (transform.forward * placeDistance), prevTexture, prevClickTexture, time.time, time, prevItemNum);
                    else
                        obj = ItemGenerator.GenerateFoil(foilPrefabItem, prevParent, transform.position + (transform.forward * placeDistance), prevTexture, prevClickTexture, time, prevItemNum);
                    GameObject oldObj = clickableObjects[index].gameObject.transform.parent.gameObject;
                    clickableObjects[index] = obj.GetComponentInChildren<ClickableObject>();
                    DestroyImmediate(oldObj);

                    audioSrc.pitch = 1f;
                    audioSrc.clip = soundEffect;
                    audioSrc.Play();
                }
            }
        }
        previousInputState = inputState;

        bool nextInputState = Input.GetKey(nextKeyCode) || Input.GetButton(nextButtonString);
        if (nextInputState && !previousNextInputState && objectHeldList.Count != 0) {
            objectHeldList.AddLast(objectHeldList.First.Value);
            objectHeldList.RemoveFirst();
        }
        previousNextInputState = nextInputState;

        if (objectHeldList.Count != 0)
            displayImage.material = clickableObjects[objectHeldList.First.Value].gameObject.GetComponent<MeshRenderer>().material;
        else
            displayImage.material = emptyMaterial;

        bool pickUpAllInputState = Input.GetKey(pickUpAllCode) || Input.GetButton(pickUpAllButtonString);
        if (pickUpAllInputState && !previousPickUpAllInputState)
            for (int i = 0; i < clickableObjects.Length; i++)
            {
                if (clickableObjects[i].gameObject.transform.parent.gameObject.activeSelf)
                {
                    clickableObjects[i].gameObject.transform.parent.gameObject.SetActive(false);
                    objectHeldList.AddFirst(i);
                    
                    audioSrc.pitch = 1f;
                    audioSrc.clip = multiSoundEffect;
                    audioSrc.Play();
                }
            }
        previousPickUpAllInputState = pickUpAllInputState;

        bool nextItemTypeState = Input.GetKey(nextItemTypeKeyCode) || Input.GetButton(nextItemTypeButtonString);
        if(nextItemTypeState && !previousNextItemTypeState)
        {
            currentItemTypeIndex = (currentItemTypeIndex + 1) % 3;
            switch (currentItemTypeIndex)
            {
                case 0:
                    typeDisplayImage.material = infinityMaterial;
                    break;
                case 1:
                    typeDisplayImage.material = upMaterial;
                    break;
                case 2:
                    typeDisplayImage.material = downMaterial;
                    break;
                default:
                    typeDisplayImage.material = null;
                    break;
            }
        }
        previousNextItemTypeState = nextItemTypeState;
    }
}
