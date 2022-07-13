using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorScript : MonoBehaviour
{
    public int OperatorNumber;
    public Given pL, pR;
    public GameObject root;
    public ControllerScript Controller;
    [SerializeField] public TextMesh txt;
    private bool tL, tR;
    public Vector3 sizeL, sizeR, sL, sR;
  //private Vector3 sizeL, sizeR, sL, sR;
    private GameObject bufL, bufR;
    public bool ok;
    public bool active;
    public void Start()
    {
        active = true;
        ok = false;
        tL = false;
        tR = false;
        pL.root = this;
        pR.root = this;
        pL.Controller = Controller;
        pR.Controller = Controller;
        pR.side = "R";
        pL.side = "L";
        sL = pL.gameObject.transform.localPosition;
        sR = pR.gameObject.transform.localPosition;
        sizeL = new Vector3(0, 0, 10000);
        sizeR = new Vector3(0, 0, 10000);
        Controller.Operator.add(gameObject);

    }
    private void Update()
    {
    }
    public void transizeLate()
    {
        //Debug.Log("Start transizeLate");
        pL.translate();
        pR.translate();
    }
    public void delet()
    {
        pL.delet();
        pR.delet();
        Controller.Given.delet(pL.gameObject);
        Controller.Given.delet(pR.gameObject);

    }
    public void SetFixing(bool a)
    {
        Debug.Log("SetFixing: " + a);
        pL.SetFixing(a);
        pR.SetFixing(a);
    }
    public void oldTable()
    {
        Debug.Log("Start: oldTable");
        newTablet("R", pR.Table);
        newTablet("L", pL.Table);
        if (pR.Table != null)
        {
            if (pR.Table.GetComponent<OperatorScript>())
            {
                pR.Table.GetComponent<OperatorScript>().oldTable();
            }
        }
        if(pL.Table != null)
        {
            if (pL.Table.GetComponent<OperatorScript>())
            {
                pL.Table.GetComponent<OperatorScript>().oldTable();
            }   
        }
    }
    public void newTablet(string side, GameObject a)
    {
        if (a == null)
        {
            Debug.Log("newTablet: " + side + " (null)");
        }
        else
        {
            Debug.Log("newTablet: " + side + " (" + a.name + ")");
        }
        if (side == "R")
        { 
            if(a == null)
            {
                tR = false;
                sizeR = new Vector3(9, 5, 10000);
                //pR.gameObject.transform.localPosition = sR;
            }
            else
            {
                tR = true;
                sizeR.x = a.GetComponent<ScalingTablet>().size.x;
                sizeR.y = a.GetComponent<ScalingTablet>().size.y;
                sizeR.z = a.GetComponent<ScalingTablet>().size.z;
            }
        }
        else if(side == "L")
        {
            if (a == null)
            {
                tL = false;
                sizeL = new Vector3(9, 5, 10000);
                //pL.gameObject.transform.localPosition = sL;
            }
            else
            {
                tL = true;
                sizeL.x = a.GetComponent<ScalingTablet>().size.x;
                sizeL.y = a.GetComponent<ScalingTablet>().size.y;
                sizeL.z = a.GetComponent<ScalingTablet>().size.z;

            }
        }

        //if(pL.Table == root | pL.Table == root)
        //{
        //    root = rootAnswer;
        //    rootAnswer.GetComponent<Answer>().delet(gameObject);
        //    rootAnswer.GetComponent<Answer>().add(gameObject);
        //}

        newScaling();  
    }
    
    public void newScaling()
    {
        Vector3 v;
        if (tL & tR)
        {
            ok = true;
            if (pL.GetComponent<Given>().Table.GetComponent<OperatorScript>())
            {
                if (!pL.GetComponent<Given>().Table.GetComponent<OperatorScript>().ok)
                {
                    ok = false;
                }
            }
            if (pR.GetComponent<Given>().Table.GetComponent<OperatorScript>())
            {
                if (!pR.GetComponent<Given>().Table.GetComponent<OperatorScript>().ok)
                {
                    ok = false;
                }
            }
            if (ok)
            {
                gameObject.GetComponent<Taskability>().value = score();
            }
        }
        else
        {
            ok = false;
        }
        if (!tR & !tL)
        {
            Debug.Log("Defoult");
            GetComponent<ScalingTablet>().UpdateSaze();
            pR.gameObject.transform.localPosition = new Vector3(sR.x + pR.smeshX, sR.y, sR.z);
            pL.gameObject.transform.localPosition = new Vector3(sL.x + pL.smeshX, sL.y, sL.z);
            //TODO!
        }
        else
        {
            SetFixing(true);
            if (OperatorNumber != 5)
            {
                v = GetComponent<ScalingTablet>().replacementSaze(sizeR, sizeL);
                pR.gameObject.transform.localPosition = new Vector3(sizeR.x / 2 + 2.5f + pR.smeshX, sR.y, sR.z);
                pL.gameObject.transform.localPosition = new Vector3(-sizeL.x / 2  - 2.5f + pL.smeshX, sL.y, sL.z);

            }
            else
            {
                v = GetComponent<ScalingTablet>().replacementSaze(sizeR, sizeL);
                pR.gameObject.transform.localPosition = new Vector3(pR.smeshX, -v.y / 2 - 1, sR.z);
                pL.gameObject.transform.localPosition = new Vector3(pL.smeshX,  v.y / 2 + 1, sL.z);
            }
            transizeLate();
            translateRoot();
            SetFixing(false);
            Debug.Log("newScaling size:" + v);
        }

        if (root != null)
        {
            if (root.GetComponent<OperatorScript>())
            {
                root.GetComponent<OperatorScript>().oldTable();
            }        
        }
    }

    public void SetRoot(GameObject a)
    {
        
        if(a == null)
        {
            Debug.Log("new root: null");
            root = null;
            if (GetComponent<Trigger>())
            {
                GetComponent<Trigger>().block = false;
            }
        }
        else
        {
            Debug.Log("new root: " + a.name);
            root = a;
            if (GetComponent<Trigger>())
            {
                GetComponent<Trigger>().block = true;                
                GetComponent<ScalingTablet>().longForm = false;
                oldTable();
            }
        }

        //if (pL.Table == root | pL.Table == root)
        //{
        //    root = rootAnswer;
        //    rootAnswer.GetComponent<Answer>().delet(gameObject);
        //    rootAnswer.GetComponent<Answer>().add(gameObject);
        //}
    }

    public float score()
    {
        switch (OperatorNumber)
        {
            case 1:
                return pL.values() + pR.values();
            case 2:
                return pL.values() - pR.values();
            case 3:
                return pL.values() * pR.values();
            case 4:
                if (pR.values() == 0)
                {
                    return 0;
                }
                return pL.values() / pR.values();
            case 5:
                if (pR.values() == 0)
                {
                    return 0;
                }
                return pL.values() / pR.values();
        }
        return 0;
    }
    public void SetPosition()
    {
        //Debug.Log("Start SetPosition: " + gameObject.name);
        pL.SetPosition();
        pR.SetPosition();
    }

    public void SetRLP()
    {
        //yield return 0;
        pL.gameObject.transform.localPosition = new Vector3(0, 3.5f, 0);
        pR.gameObject.transform.localPosition = new Vector3(0, -3.5f, 0);
        sL = pL.gameObject.transform.localPosition;
        sR = pR.gameObject.transform.localPosition;
        GetComponent<ScalingTablet>().replacementSazeFraction(new Vector3(9, 5, 0.1f), 0, new Vector3());
        GetComponent<ScalingTablet>().SetSize();
        GetComponent<ScalingTablet>().UpdateSaze();
    }
    public void smesh(float a)
    {
        Debug.Log("new Given.smesh = " + a);
        pR.smesh(a);
        pL.smesh(a);
    }
    public void translateRoot()
    {
        if(root == null)
        {
            transizeLate(); 
        }
        else
        {
            root.GetComponent<OperatorScript>().translateRoot();
        }
    }
    public void TranslateGiven()
    {
        pR.gameObject.transform.localPosition = new Vector3(sizeR.x / 2 + 2.5f + pR.smeshX, sR.y, sR.z);
        pL.gameObject.transform.localPosition = new Vector3(-sizeL.x / 2 - 2.5f + pL.smeshX, sL.y, sL.z);
    }
}
