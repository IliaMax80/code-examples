using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Max: Vector3(-1.5, 2, 29)
//Min: Vector3(-74, -20, 29)


public class Taskability : MonoBehaviour
{
    //Наверно основные переменые 
    public Vector2 StartMax;
    public Vector2 StartMin;
    //public Vector2 indentUD;
    //public Vector2 indentRL;
    public float value;
    public bool BlockSide = false;
    public  bool bl, bufbool;
    public string sing;
    
    //Публичные переменые 
    public float z;
    public bool limitation = false;
    public string Name;
    public GameObject ghost;
    public bool stop;
    public Vector3 stopPosition;
    public bool distroy = false;
    [SerializeField] private GameObject prefab;
    public LineRenderer line;
    public bool NotSuit = false;

    //Непубличне переменые 
    private GameObject bufObject;
    private spavnTablet Script;
    private Ray ray;
    private RaycastHit hit;
    public Vector3 StartVector;
    [SerializeField] private TextMesh txt;
    private Vector3 buf;
    //public Vector3 buf, smesh;
    private bool l;
    private Vector2 smeh;
    private float OldTime;
    private int side = 0;
    public Vector2 Max;
    public Vector2 Min;
    public ControllerScript Controller;

    //private int x = 0;
    //private float OldTimeX;


    // Start is called before the first frame update
    void Start()
    {
        //stop = false;
        StartVector = transform.position;
        buf = transform.position;
        //z = buf.z;
        //smesh = new Vector3(transform.position.x - ghost.transform.position.x, transform.position.y - ghost.transform.position.y, transform.position.z - ghost.transform.position.z);
        line = GetComponent<LineRenderer>();
        line.positionCount = 5;
        line.SetPosition(0, new Vector3(StartMin.x, StartMax.y, z));
        line.SetPosition(1, new Vector3(StartMax.x, StartMax.y, z));
        line.SetPosition(2, new Vector3(StartMax.x, StartMin.y, z));
        line.SetPosition(3, new Vector3(StartMin.x, StartMin.y, z));
        line.SetPosition(4, new Vector3(StartMin.x, StartMax.y, z));
        line.enabled = false;
        UpdateMaxMin();
    }

    // Update is called once per frame
    void Update()
    {
        //Тык?
        if (Input.GetMouseButtonDown(0)) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                //Debug.Log("oK");
                if (hit.collider.gameObject == gameObject) {
                    Debug.Log(Name + " tap");
                    Controller.SetActiveGiven(true);
                    if (!limitation)
                    {
                        smeh.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
                        smeh.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;

                        bl = true;
                        if (!distroy)
                        {
                            line.enabled = true;
                        }
                        if (GetComponent<OperatorScript>())
                        {
                            //Debug.Log("Ok&&&&");
                            GetComponent<OperatorScript>().SetFixing(true);
                            //GetComponent<OperatorScript>().ActiveOn();
                        }
                    }
                }
            }
        }
        //Не тык
        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log(Time.time - OldTime);

            bl = false;
            line.enabled = false;

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Controller.SetActiveGiven(false);
                    if (GetComponent<OperatorScript>())
                    {
                        GetComponent<OperatorScript>().SetFixing(false);
                    }  

                    if (Time.time - OldTime < 0.5f)
                    {
                        ScipSuit();
                        Debug.Log("Start SetSuit()");
                    }
                    OldTime = Time.time;
                }
            }
        }

        if (bl)
        {
            l = true;
            //transform.Translate(Input.GetAxis("Mouse X") * speed, Input.GetAxis("Mouse Y") * speed, 0);
            //Таскабильность
            buf = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            buf.x -= smeh.x;
            buf.y -= smeh.y;
            buf.z = z;
            //Debug.Log("position: " + buf);
            //Debug.Log(buf);

            if (stop)
            {
                StartVector = stopPosition;
            }
            else
            {
                StartVector = buf;
                //Ограничители
                if (StartVector.x < Min.x)
                {
                    StartVector = new Vector3(Min.x, StartVector.y, StartVector.z);
                }
                else if (StartVector.x > Max.x)
                {
                    StartVector = new Vector3(Max.x, StartVector.y, StartVector.z);
                }
                if (StartVector.y < Min.y)
                {
                    StartVector = new Vector3(StartVector.x, Min.y, StartVector.z);
                }
                else if (StartVector.y > Max.y)
                {
                    StartVector = new Vector3(StartVector.x, Max.y, StartVector.z);
                }

            } 
            transform.position = buf;
            if (GetComponent<OperatorScript>())
            {
                GetComponent<OperatorScript>().transizeLate();
                ghost.transform.position = new Vector3(StartVector.x, StartVector.y, StartVector.z);

            }
            else
            {
                if (GetComponent<NoteScript>())
                {
                    GetComponent<NoteScript>().translate();
                }
                ghost.transform.position = new Vector3(StartVector.x, StartVector.y, StartVector.z);
            }
            
        }
        else
        {
            if (l)
            {
                l = false;
                if (distroy & transform.position.y < -40)
                {
                    delet();
                }
                transform.position = StartVector;
                if (GetComponent<OperatorScript>())
                {
                    GetComponent<OperatorScript>().transizeLate();
                    GetComponent<OperatorScript>().SetPosition();
                    ghost.transform.position = new Vector3(StartVector.x, StartVector.y, StartVector.z);
                }
                else
                {
                    if (GetComponent<NoteScript>())
                    {
                        GetComponent<NoteScript>().translate();
                    }
                    ghost.transform.position = new Vector3(StartVector.x, StartVector.y, StartVector.z);
                }
            }                

        }
     }
    public void ScipSuit()
    {
        if (NotSuit)
        {
            return;
        }
        int buf = side;
        int a = 0;
        do
        {
            a++;
            buf++;
            if (buf > 2)
            {
                buf = 0;
            }
            if(a > 3)
            {
                break;
            }
        } while (!SetSuit(buf));
        side = buf;
    }
    public bool SetSuit(int a)
    {
        if (NotSuit)
        {
            return false;
        }
        if (GetComponent<OperatorScript>() | Name == "Answer" | Name == "")
        {
            return false;
        }
        switch (a)
        {
            case 0:
                UpdateValue(value);
                break;

            case 1:
                if(sing == "")
                {
                    return false;
                }
                txt.text = sing;
                break;

            case 2:
                if (BlockSide)
                {
                    return false;
                }
                txt.text = Name;
                break;
            default:
                break;
        }
        StartCoroutine(StartSetColour(a));
        return true;

    }
    private IEnumerator StartSetColour(int a)
    {
        
        yield return 0;
        GetComponent<ScalingTablet>().SetColour(a);

    }
    public void UpdateValue(float v)
    {
        value = v;
        int a;
        a = Mathf.RoundToInt(v * 10);
        v = a / 10.0f;
        txt.text = v.ToString();
    }
    

    //Карутина для авторастоновки 

    public void Scip()
    {
        StartCoroutine(S());
    }
    private IEnumerator S()
    {   
        stop = true;
        bl = true;
        yield return 0;
        bl = false;
        stop = false;
    } 
    public void ActiveСursor()
    {
        bl = true;
    }
    public GameObject Transformation()
    {
        bufObject = Instantiate(prefab) as GameObject;
        bufObject.transform.position = transform.position;
        Script = bufObject.GetComponent<spavnTablet>();
        bufObject.GetComponent<ScalingTablet>().length = GetComponent<ScalingTablet>().length;
        Script.z = z;
        Script.Name = Name;
        Script.Controller = Controller;
        Script.updateValue(value);
        Script.sing = sing;
        Controller.Tablet.delet(gameObject);
        Destroy(gameObject);
        return bufObject;

    }
    public bool Distation()
    {
        if (transform.position.x < Min.x)
        {
            return true;
        }
        if (transform.position.y < Min.y)
        {
            return true;
        }
        if (transform.position.x > Max.x)
        {
            return true;
        }
        if (transform.position.y > Max.y)
        {
            return true;
        }
        return false;
    } 
    public void Translate(Vector3 a)
    {
        //x++;
        //if(Time.time - OldTimeX > 3)
        //{
        //    x = 0;
        //    OldTimeX = Time.time;
        //}
        //if(x > 10)
        //{
        //    return;
        //}
        //Debug.Log("new position:" + a);
        StartVector = a;
        if (GetComponent<OperatorScript>())
        {
            GetComponent<OperatorScript>().transizeLate();
            transform.position = new Vector3(StartVector.x, StartVector.y, StartVector.z);
            // не нужен так как given и так под размер подстраиваеться:  + GetComponent<ScalingTablet>().smeshX
        }
        else
        {
            transform.position = StartVector;
        }
        ghost.transform.position = new Vector3(StartVector.x, StartVector.y, StartVector.z);
    }

    public void delet()
    {
        if (GetComponent<OperatorScript>())
        {
            GetComponent<OperatorScript>().delet();
        }
        if (GetComponent<NoteScript>())
        {
            Destroy(GetComponent<NoteScript>().given.GetComponent<GivenNote>().Table);
        }
        if (Controller != null)
        {
            Controller.Tablet.delet(gameObject);
            Controller.Operator.delet(gameObject);
        }
        Destroy(gameObject);
    }
    //Мелочные функции
    public void InputSmeh(Vector2 a)
    {
        smeh = a;
    }
    public void UpdateMaxMin()
    {
        Max.x = StartMax.x - GetComponent<ScalingTablet>().size.x / 2 - 0.15f;
        Max.y = StartMax.y - GetComponent<ScalingTablet>().size.y / 2;

        Min.x = StartMin.x + GetComponent<ScalingTablet>().size.x / 2 + 0.15f;
        Min.y = StartMin.y + GetComponent<ScalingTablet>().size.y / 2;

    }
}