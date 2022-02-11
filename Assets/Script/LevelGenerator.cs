using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{   
    public Hexagon activeHexagon;
    [HideInInspector] public bool hexagonsRotate;
    public static LevelGenerator instante;
    public float hexVerticalDistance;
    [NonSerialized] private float hexHorizontalDistance;
    
    private Vector3 currentPosition;
    [HideInInspector]public int colorCount;
    
    [Header("Prefabs")]
    public List<Color> Colors = new List<Color>();
    [SerializeField] public Transform hexagonsParent;
    [SerializeField] public GameObject hexagon;
    [SerializeField] public Transform startPoint;

    [Header("Level Size")]
    public int levelVerticalSize;
    public int levelHorizontalSize;

    [Header("Lists")]
    public List<Hexagon> hexagons = new  List<Hexagon>();
    public List<Hexagon> selectedHexagons = new  List<Hexagon>();
    private void Awake()
    {
        instante = this;
    }
    
    void Start()
    {
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
        colorCount = PlayerPrefs.GetInt("colorCount",5);
        levelVerticalSize = PlayerPrefs.GetInt("levelHeight",9);
        levelHorizontalSize = PlayerPrefs.GetInt("levelWidth",6);
        hexVerticalDistance = 0.48f;
        hexHorizontalDistance = 0.42f;
        currentPosition = startPoint.position;
        CreateLevel();
    }

    private void SwipeDetector_OnSwipe(SwipeData data)
    {
        if(data.Direction == SwipeDirection.Left)
        {  
            StartCoroutine(activeHexagon.Rotate(false,3));           
        }

        if(data.Direction == SwipeDirection.Right)
        {  
            StartCoroutine(activeHexagon.Rotate(true,3));           
        }
    }
    private void Update() {
        MissedHexagonsCheck();

        if(Input.touchCount>0)
        {
            Vector2 test = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            RaycastHit2D hit = Physics2D.Raycast(test, (Input.GetTouch(0).position));
            if (hit.collider && hit.collider.tag == "Hexagon")
            {
                hit.collider.gameObject.GetComponent<Hexagon>().FindNeighbour();
            }
        }

    }

    public void MissedHexagonsCheck()
    {
        for(int i = 0; i<hexagons.Count; i++)
        {
            if(hexagons[i]==null)
            {
                hexagons.Remove(hexagons[i]);
            }
        }     

         for(int j = 0; j<selectedHexagons.Count; j++)
        {
            if(selectedHexagons[j]==null)
            {
                selectedHexagons.Remove(selectedHexagons[j]);
            }
        }        
    }

    private void CreateLevel()
    {
        for (int j = 0; j < levelHorizontalSize; j++)
        {
            Vector3 nextHorizontalPosition = Vector3.zero;
            
            nextHorizontalPosition = new Vector3(startPoint.position.x + (hexHorizontalDistance * j),
                startPoint.position.y, startPoint.position.z);
            
            if (j % 2 == 0)
            {
                nextHorizontalPosition.y -= hexVerticalDistance / 2;
            }
            
            currentPosition = CreateHexagon(j,0,nextHorizontalPosition).transform.position;
            for (int i = 0; i < levelVerticalSize; i++)
            {
                currentPosition = new Vector3(currentPosition.x, currentPosition.y - hexVerticalDistance, currentPosition.z);
                CreateHexagon(j,i+1,currentPosition);
            }
        }
    }

    private GameObject CreateHexagon(int x, int y, Vector3 position)
    {
        GameObject cloneHexagon = Instantiate(hexagon, position, hexagon.transform.rotation,hexagonsParent);
        cloneHexagon.gameObject.name = x + " : " + y;
        hexagons.Add(cloneHexagon.GetComponent<Hexagon>());
        cloneHexagon.GetComponent<Hexagon>().SetPositions(x,y);
        return cloneHexagon;
    }

    public void ClearSelectedHexaons()
    {
        for (int i = 0; i < selectedHexagons.Count; i++)
        {
            selectedHexagons[i].GetComponent<SpriteRenderer>().material = selectedHexagons[i].witoutOutline;
        }
        
        selectedHexagons.Clear();
    }

    public void DestroyHexegon(Hexagon deletedHexagon)
    {
        ClearSelectedHexaons();
        Destroy(deletedHexagon.gameObject);
        int test = (int) deletedHexagon.coordinate.y;
        Vector2 targetPosition = new Vector2(deletedHexagon.transform.position.x,
            deletedHexagon.transform.position.y + (hexVerticalDistance * test));
        GameObject clone = CreateHexagon((int) deletedHexagon.coordinate.x, 0,targetPosition);
        //clone.GetComponent<Hexagon>().transform.DOMove(new Vector3(targetPosition.x,targetPosition.y - hexVerticalDistance ,clone.transform.position.z),0.2f);
    }
}
