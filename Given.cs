using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TaskGeneratorV2;

public class Given : MonoBehaviour
{
    public string Name;
    public bool notDistroy;
    public bool check;
    public bool fixing;
    public OperatorScript root;
    public string side;
    public ControllerScript Controller;
    public float smeshX;
    private Taskability script;
    //private GameObject Table;
    public GameObject Table;
    private bool b;
    private bool buf;
    private bool mesh;
    private float StartX;
    // Start is called before the first frame update
    public void Start()
    {
        fixing = false;
        Table = null;
        StartX = Controller.paperСlip.transform.position.x - transform.position.x;
        Controller.Given.add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
    //Вхождение в тригер
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("!");
        if (other.GetComponent<Taskability>())
        {
            if (other.GetComponent<NoteScript>() || ( notDistroy & other.GetComponent<OperatorScript>()))
            {
                return;
            }
            //Debug.Log("!!" + other.gameObject.name);
            if (!fixing)
            {
                if (Table == null)
                {
  
                    if(notDistroy)
                    {
                        transform.position = new Vector3(Controller.paperСlip.transform.position.x - StartX + (other.GetComponent<ScalingTablet>().length - 9)/2, transform.position.y, transform.position.z);
                    }
                    if (!other.GetComponent<Taskability>().bl)
                    {
                        return;
                    }

                    Table = other.gameObject;
                    GetComponent<MeshRenderer>().enabled = false;
                    if (root != null)
                    {
                        if (Table.GetComponent<OperatorScript>())
                        {
                            Table.GetComponent<OperatorScript>().SetRoot(root.gameObject);
                        }
                        root.newTablet(side, Table);
                        if (PlayerPrefs.GetInt("Training") == 1)
                        {
                            Controller.Cursor.stop1();
                        }
                    }
                    //Debug.Log("!!2");
                    //Debug.Log("Касание");
                    script = Table.GetComponent<Taskability>();
                    script.stop = true;
                    script.stopPosition = transform.position;
                    if (notDistroy)
                    {
                        if (script.distroy)
                        {
                            script.distroy = false;
                            buf = true; 
                        }
                        else
                        {   
                            buf = false;
                        }
                    }
                    if (script.Name == Name)
                    {
                        check = true;
                        if(PlayerPrefs.GetInt("Training") == 1)
                        {
                            Controller.Cursor.stop1();
                        }
                    }
                    else
                    {
                        check = false;
                    }
                }
            }


            
        }
    }
    //Выход
    private void OnTriggerExit(Collider other)
    {
        if (!fixing)
        {
            //Debug.Log("Exit " + other.name);
            if (other.GetComponent<Taskability>())
            {
                if (!other.GetComponent<Taskability>().bl)
                {
                    return;
                }
                //Debug.Log("Exit " + other.name + "!");
                if (other.gameObject == Table)
                {
                    GetComponent<MeshRenderer>().enabled = mesh;
                    //Debug.Log("Exit " + other.name + "!!");
                    if (Table.GetComponent<OperatorScript>())
                    {
                        Table.GetComponent<OperatorScript>().SetRoot(null);
                    }
                    Table = null;
                    if (root != null)
                    {
                        root.newTablet(side, Table);
                    }
                    //Debug.Log("Нет касания");
                    script = other.GetComponent<Taskability>();
                    script.stop = false;
                    check = false;
                    if (notDistroy)
                    {
                        script.distroy = buf;
                    }
                }
            }
        }

    }
    //Ручное присвоение
    public void scip(GameObject a)
    {
        Table = a;
        script = Table.GetComponent<Taskability>();
        script.stopPosition = transform.position;
        check = true;
        script.Scip();
        
    }
    public void translate()
    {
        if (Table != null)
        {
            if (Table.GetComponent<Taskability>())
            {
                Table.GetComponent<Taskability>().Translate(transform.position);
            }
            if (Table.GetComponent<OperatorScript>())
            {
                Table.GetComponent<OperatorScript>().transizeLate();
            }
        }
    }
    public void SetFixing(bool a)
    {
        // Debug.Log(name + ": " + a);
        fixing = a;
        //GetComponent<MeshCollider>().enabled = !a;
        if(Table != null)
        {
            if (Table.GetComponent<OperatorScript>())
            {
                Table.GetComponent<OperatorScript>().SetFixing(a);
            }
        }
    }
    public void delet()
    {
        if (Table != null)
        {
            if (Table.GetComponent<OperatorScript>())
            {
                Table.GetComponent<OperatorScript>().delet();
            }
            Controller.Tablet.delet(Table);
            Destroy(Table);
            Controller.Given.delet(gameObject);
        }
    }

    public float values()
    {
        if(Table != null)
        {
            return Table.GetComponent<Taskability>().value;
        }
        return 0;
    }

    public void SetMesh(bool a)
    {
        if(Table == null)
        {
            GetComponent<MeshRenderer>().enabled = a;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        mesh = a;
    }
    public void smesh(float a)
    {
        smeshX = a;
    }
    public void SetPosition()
    {
        if(Table != null)
        {
            Table.GetComponent<Taskability>().stopPosition = transform.position;
            if (Table.GetComponent<OperatorScript>())
            {
                Table.GetComponent<OperatorScript>().SetPosition();
            }
        }
    }
}