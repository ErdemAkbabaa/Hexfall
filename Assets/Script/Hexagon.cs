using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
public class Hexagon : MonoBehaviour
{
    private Vector3 fp;   //First touch position
    private Vector3 lp;   //Last touch position
    private float dragDistance;
    public List<Hexagon> checkList = new List<Hexagon>();
    public Vector2 coordinate;
    [SerializeField] public Material outline,witoutOutline;
    private LevelGenerator _levelGenerator;
    private bool isMoving,isFalling;

    public struct neighbourCoordinate
    {
        public Vector2 up;
        public Vector2 down;
        public Vector2 left;
        public Vector2 right;
        public Vector2 upLeft;
        public Vector2 upRight;
        public Vector2 downLeft;
        public Vector2 downRight;
    }

    public neighbourCoordinate currentNeighbourCoordinate;
    [NonSerialized]public int colorIndex;

    private void Start()
    {            
        DOTween.Init();
        _levelGenerator = LevelGenerator.instante;
        colorIndex = Random.Range(0, _levelGenerator.colorCount);
        this.GetComponent<SpriteRenderer>().color = LevelGenerator.instante.Colors[colorIndex];           
    }

    private void Update() 
    {
        if(isMoving)
        {
            Explosion();
        }          
        Fall();

        SwipeCheck();
    }

    public void SetPositions(int x, int y)
    {
        coordinate = new Vector2(x, y);
    }

    private void OnMouseUp()
    {
        FindNeighbour();
        _levelGenerator.activeHexagon = this;
    }

    private void OnMouseOver() 
    {
        if(Input.GetKeyDown(KeyCode.P) && isMoving==false) //testing
        {
            StartCoroutine(Rotate(true,3));
        }

        if(Input.GetKeyDown(KeyCode.O) && isMoving==false) //testing
        {
            StartCoroutine(Rotate(false,3));           
        }

        if(Input.GetKeyDown(KeyCode.U)) //testing
        {
            Explosion();           
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
          
        }
    }

    public void FindNeighbour()
    {
        GetCurrentCoordinate();
        _levelGenerator.ClearSelectedHexaons();

        Vector2 first,second;
        int selectionStatus = Random.Range(0,6);
        bool breakLoop = false;

        do {
            switch (selectionStatus) {
                case 0: first = currentNeighbourCoordinate.up; second = currentNeighbourCoordinate.upRight; break;
                case 1: first = currentNeighbourCoordinate.upRight; second = currentNeighbourCoordinate.downRight; break;
                case 2: first = currentNeighbourCoordinate.downRight; second = currentNeighbourCoordinate.down; break;
                case 3: first = currentNeighbourCoordinate.down; second = currentNeighbourCoordinate.downLeft; break;
                case 4: first = currentNeighbourCoordinate.downLeft; second = currentNeighbourCoordinate.upLeft; break;
                case 5: first = currentNeighbourCoordinate.upLeft; second = currentNeighbourCoordinate.up; break;
                default: first = Vector2.zero; second = Vector2.zero; break;
            }

            if (first.x < 0 || first.x >= _levelGenerator.levelHorizontalSize+1 || first.y < 0 || first.y >= _levelGenerator.levelVerticalSize || second.x < 0 || second.x >= _levelGenerator.levelHorizontalSize+1 || second.y < 0 || second.y >= _levelGenerator.levelVerticalSize) {
                selectionStatus = Random.Range(0,6);
            }
            else {
                breakLoop = true;
            }
        } while (!breakLoop);


        for (int i = 0; i < LevelGenerator.instante.hexagons.Count; i++)
        {
            switch (_levelGenerator.hexagons[i].coordinate)
            {
                case Vector2 v when v.Equals(first):
                case Vector2 v0 when v0.Equals(second):
                SelectNeighbour(i);
                    break;
            }
        }
    }

    private void SelectNeighbour(int index)
    {
        if (!_levelGenerator.selectedHexagons.Contains(this))
        {
            _levelGenerator.selectedHexagons.Add(this);
            this.GetComponent<SpriteRenderer>().material = outline;
        }

        _levelGenerator.selectedHexagons.Add(LevelGenerator.instante.hexagons[index]);
        _levelGenerator.hexagons[index].GetComponent<SpriteRenderer>().material = outline;       
    }
    
    public IEnumerator Rotate(bool reverse, int wave)
    {    
        if(_levelGenerator.hexagonsRotate == false)
        {
            _levelGenerator.hexagonsRotate=true;
            UIManager.instante.AddMove(1);
            Vector2 coordinate1, coordinate2, coordinate3;
            Vector2 pos1, pos2, pos3;
            Hexagon first, second, third;               
                
            for(int i = 0; i< wave; i++)
            {
                Debug.Log("loop " + i);


                first = _levelGenerator.selectedHexagons[0];
                second = _levelGenerator.selectedHexagons[1];
                third = _levelGenerator.selectedHexagons[2];         

                coordinate1 = first.coordinate;
                coordinate2 = second.coordinate;
                coordinate3 = third.coordinate;

                pos1 = first.gameObject.transform.position;
                pos2 = second.gameObject.transform.position;
                pos3 = third.gameObject.transform.position;

                first.transform.DORotate(new Vector3(0,0,first.transform.eulerAngles.z+120),0.2f);
                second.transform.DORotate(new Vector3(0,0,second.transform.eulerAngles.z+120),0.2f);
                third.transform.DORotate(new Vector3(0,0,third.transform.eulerAngles.z+120),0.2f);
                
                if(reverse)
                {
                    first.transform.DOMove(pos3,0.2f);
                    second.transform.DOMove(pos1,0.2f);
                    third.transform.DOMove(pos2,0.2f);

                    first.coordinate = coordinate3;
                    second.coordinate = coordinate1;
                    third.coordinate = coordinate2;
                }
                else
                {
                    first.transform.DOMove(pos2,0.2f);
                    second.transform.DOMove(pos3,0.2f);
                    third.transform.DOMove(pos1,0.2f);

                    first.coordinate = coordinate2;
                    second.coordinate = coordinate3;
                    third.coordinate = coordinate1;
                }                     
                yield return new WaitForSeconds(0.2f);

                first.Explosion();
                second.Explosion();
                third.Explosion();
            }
            
            isMoving=false;
            }
            _levelGenerator.hexagonsRotate=false;
    }

    private void GetCurrentCoordinate()
    {
        if(coordinate.x %2 != 0)
        {
            currentNeighbourCoordinate.upRight = new Vector2(coordinate.x+1,coordinate.y-1);
            currentNeighbourCoordinate.upLeft = new Vector2(coordinate.x-1,coordinate.y-1);
            currentNeighbourCoordinate.downLeft = new Vector2(coordinate.x-1,coordinate.y);
            currentNeighbourCoordinate.downRight = new Vector2(coordinate.x+1,coordinate.y);
        }
        else
        {
            currentNeighbourCoordinate.upRight = new Vector2(coordinate.x+1,coordinate.y);
            currentNeighbourCoordinate.upLeft = new Vector2(coordinate.x-1,coordinate.y);
            currentNeighbourCoordinate.downLeft = new Vector2(coordinate.x-1,coordinate.y+1);
            currentNeighbourCoordinate.downRight = new Vector2(coordinate.x+1,coordinate.y+1);
        }
        currentNeighbourCoordinate.up = new Vector2(coordinate.x,coordinate.y-1);
        currentNeighbourCoordinate.down = new Vector2(coordinate.x,coordinate.y+1);
    }
    public Hexagon firstGameobject,secondGameobject;
    private void Explosion()
    {
        Vector2 first,second;
        
        for(int i = 0; i<6; i++)
        {
             switch (i) {
                case 0: first = currentNeighbourCoordinate.up; second = currentNeighbourCoordinate.upRight; break;
                case 1: first = currentNeighbourCoordinate.upRight; second = currentNeighbourCoordinate.downRight; break;
                case 2: first = currentNeighbourCoordinate.downRight; second = currentNeighbourCoordinate.down; break;
                case 3: first = currentNeighbourCoordinate.down; second = currentNeighbourCoordinate.downLeft; break;
                case 4: first = currentNeighbourCoordinate.downLeft; second = currentNeighbourCoordinate.upLeft; break;
                case 5: first = currentNeighbourCoordinate.upLeft; second = currentNeighbourCoordinate.up; break;
                default: first = Vector2.zero; second = Vector2.zero; break;
            }

            if (first.x < 0 || first.x >= _levelGenerator.levelHorizontalSize+1 || first.y < 0 || first.y >= _levelGenerator.levelVerticalSize || second.x < 0 || second.x >= _levelGenerator.levelHorizontalSize+1 || second.y < 0 || second.y >= _levelGenerator.levelVerticalSize) {
                
            }
            else 
            {
                for (int j = 0; j < LevelGenerator.instante.hexagons.Count; j++)
                {
                    if(_levelGenerator.hexagons[j].coordinate == first)
                    {
                        firstGameobject = _levelGenerator.hexagons[j];
                    }
                    if(_levelGenerator.hexagons[j].coordinate == second)
                    {
                        secondGameobject = _levelGenerator.hexagons[j];                    
                    }
                }

                if((secondGameobject.colorIndex == firstGameobject.colorIndex) && firstGameobject.colorIndex == this.colorIndex)
                {
                    _levelGenerator.DestroyHexegon(firstGameobject);
                    _levelGenerator.DestroyHexegon(secondGameobject);
                    _levelGenerator.DestroyHexegon(this);
                    UIManager.instante.AddScore(15);
                    break;                
                }                    
            }                 
        }        
    }

    private void Fall()
    {
        if (isFalling==false)
        {
            isFalling = true;
            
            if(coordinate.y != _levelGenerator.levelVerticalSize)
            {
                GetCurrentCoordinate();            
                bool downbool = false;
                for(int i =0; i<_levelGenerator.hexagons.Count; i++)
                {
                    if(_levelGenerator.hexagons[i].coordinate == currentNeighbourCoordinate.down)
                    {
                        downbool = true;
                    }
                }

                if(downbool == false)
                {                                   
                    transform.position = new Vector3(transform.position.x,transform.position.y - _levelGenerator.hexVerticalDistance ,transform.position.z);                    
                    //this.transform.DOMove(new Vector3(transform.position.x,transform.position.y - _levelGenerator.hexVerticalDistance ,transform.position.z),0.05f);                
                    this.coordinate.y += 1;                                                                               
                }
            }

            isFalling = false;
        }
                 
    }

    private void SwipeCheck()
    {
         if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                fp = touch.position;
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                lp = touch.position;  //last touch position. Ommitted if you use list
 
                //Check if drag distance is greater than 20% of the screen height
                if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
                {//It's a drag
                 //check if the drag is vertical or horizontal
                    if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                    {   //If the horizontal movement is greater than the vertical movement...
                        if ((lp.x > fp.x))  //If the movement was to the right)
                        {   //Right swipe
                            Rotate(false,3);
                        }
                        else
                        {   //Left swipe
                            Rotate(true,3);
                        }
                    }
                    else
                    {   //the vertical movement is greater than the horizontal movement
                        if (lp.y > fp.y)  //If the movement was up
                        {   //Up swipe
                            Debug.Log("Up Swipe");
                        }
                        else
                        {   //Down swipe
                            Debug.Log("Down Swipe");
                        }
                    }
                }
                else
                {   //It's a tap as the drag distance is less than 20% of the screen height
                    Debug.Log("Tap");
                }
            }
        }
    }
}

