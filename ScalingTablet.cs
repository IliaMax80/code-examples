using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingTablet : MonoBehaviour
{

    public Material StandartMaterial;
    public Material material1, material2;
    public bool Advanced;
    public bool update;
    public bool longForm = false;
    public float length;
    public float width;
    public float ScaleZ;
    public float z;
    public Vector3 size;
    public float R1;
    public float R2;
    public BoxCollider boxCollider;
    public float smeshX;
    public List<Material> colours = new List<Material>();
    public bool fraction;
    public GameObject txt;
    public GameObject equally;
    public GameObject Answer;

    public ScalingForm form;
    public ScalingFrame frame;
    public GameObject fractionObject;
    private Vector3 OldR, OldL;
    //TODO: ƒоработай деление а там за кнопки и ввод текста у намбера и нотетаблички
    //ну и по итогу можно кончено всех кнопок редезайн сделать особенно что касательно меню нашего
    public float rL, rW;
    public void Start()
    {

        smeshX = 0;
        boxCollider = GetComponent<BoxCollider>();

        rL = frame.length - form.length;
        rW = frame.width - form.width;
        foreach (GameObject part in form.parts)
        {
            part.GetComponent<MeshRenderer>().materials = new Material[] { StandartMaterial };
        }

        UpdateSaze();
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            UpdateSaze();
        }
    }
    public void updateTag(string a, GameObject root)
    {
        form.updateTag(a, root);
        frame.updateTag(a, root);

    }
    public void UpdateSaze()
    {
        if (Advanced)
        {
            replacementSaze(new Vector3(9, 5, 0.1f), new Vector3());
            return;
        }
        size = new Vector3(length, width, ScaleZ);
        form.length = length;
        form.width = width;
        form.R = R1;
        form.ScaleZ = ScaleZ;
        OldL = new Vector3(9, 5, 0.1f);
        OldR = new Vector3(9, 5, 0.1f);
        frame.length = length + rL;
        frame.width = width + rW;
        frame.R = R2;
        frame.ScaleZ = ScaleZ;
        frame.z = z;
        boxCollider.size = new Vector3(length + R2 / 2 + 0.05f, width + R2 / 2 + 0.05f, ScaleZ);
        if (fractionObject != null)
        {
            form.length = length;
        }
        form.UpdateForm();
        frame.UpdateForm();
        if (GetComponent<Taskability>())
        {
            GetComponent<Taskability>().UpdateMaxMin();
        }

    }

    public void SetSize()
    {
        length = form.length;
        width = form.width;
    }

    public Vector3 replacementSaze(Vector3 R, Vector3 L, bool check = false)
    {
        float Rp;
        Vector3 AnswerSize = new();
        if (longForm)
        {
            Answer.SetActive(true);
            Answer.GetComponent<OperatorAnswer>().update();
            AnswerSize = Answer.GetComponent<ScalingTablet>().size;
            Rp = 5 + AnswerSize.x;

        }
        else
        {
            Rp = 0;
            Answer.SetActive(false);
        }
        Debug.Log("replacementSaze: (R:" + R + ", L: " + L + ", Rp: " + Rp + ")");
        Vector3 v;

        //–абота с размерами
        if (R.x == 0)
        {
            v = L;
            R = v;
        }
        else if (L.x == 0)
        {
            v = R;
            L = v;
        }
        else
        {
            if (R.z < L.z) { v.z = R.z; }
            else { v.z = L.z; }
            if (R.x < L.x) { v.x = L.x; }
            else { v.x = R.x; }
            if (R.y < L.y) { v.y = L.y; }
            else { v.y = R.y; }

        }

        if (check)
        {
            return v;
        }
        if (fraction)
        {
            replacementSazeFraction(v, Rp, AnswerSize);
            return v;
        }

        //ѕолучение итоговых размеров 
        float l = R.x + L.x + 6.2f + Rp;
        float w = v.y + 1;
        float zA = v.z - 0.001f;
        Debug.Log("length: " + l + ", width: " + w);

        //ѕередача их в form и frame и т.д.
        size = new Vector3(l, w, zA);

        form.length = l;
        form.width = w;
        form.R = R1;
        form.ScaleZ = zA;

        frame.length = l + rL;
        frame.width = w + rW;
        frame.R = R2;
        frame.ScaleZ = zA;
        frame.z = z;

        boxCollider.size = new Vector3(l + R2 + 0.05f, w + R2 / 2 + 0.05f, zA);
        //работа со смещением
        smeshX = (L.x - R.x - Rp) / 2f;
        txt.transform.localPosition = new Vector3(smeshX, txt.transform.localPosition.y);
        
        if (GetComponent<OperatorScript>())
        {
            GetComponent<OperatorScript>().smesh(smeshX);
        }

        if (GetComponent<Trigger>())
        {
            if (longForm)
            {
                equally.GetComponent<MeshRenderer>().enabled = true;
                equally.transform.localPosition = new Vector3(R.x + 5 + smeshX, equally.transform.localPosition.y);
                Answer.transform.localPosition = new Vector3(R.x + 7.5f + AnswerSize.x / 2 + smeshX, 0, 0);
            }
            else
            {
                equally.GetComponent<MeshRenderer>().enabled = false;
                
            }
            GetComponent<Trigger>().Translate(R.x + Rp, v.y, -smeshX, longForm);
        }

        OldL = L;
        OldR = R;




        form.UpdateForm();
        frame.UpdateForm();
        if (GetComponent<Taskability>())
        {
            GetComponent<Taskability>().UpdateMaxMin();
        }
        return v;
    }
    public void replacementSazeFraction(Vector3 a, float Rp, Vector3 AnswerSize)
    {

        float l = a.x + 1 + Rp;
        float w = (a.y + 1) * 2 + 1f;
        float zA = a.z - 0.001f;
        Debug.Log("length: " + l + ", width: " + w);

        size = new Vector3(l, w, zA);

        form.length = l;
        form.width = w;
        form.R = R1;
        form.ScaleZ = zA;

        frame.length = l + rL;
        frame.width = w + rW;
        frame.R = R2;
        frame.ScaleZ = zA;
        frame.z = z;

        boxCollider.size = new Vector3(l + R2 / 2 + 0.05f, w + R2 / 2 + 0.05f, zA);

        smeshX = -Rp / 2f;
        fractionObject.transform.localScale = new Vector3(a.x, fractionObject.transform.localScale.y, fractionObject.transform.localScale.z);
        fractionObject.transform.localPosition = new Vector3(smeshX, 0, z);
        if (GetComponent<OperatorScript>())
        {
            GetComponent<OperatorScript>().smesh(smeshX);
        }

        if (GetComponent<Trigger>())
        {
            if (longForm)
            {
                equally.GetComponent<MeshRenderer>().enabled = true;
                equally.transform.localPosition = new Vector3(a.x / 2 + 2.5f + smeshX, equally.transform.localPosition.y);
                Answer.transform.localPosition = new Vector3(a.x / 2 + 5f + AnswerSize.x / 2 + smeshX, 0, 0);
            }
            else
            {
                equally.GetComponent<MeshRenderer>().enabled = false;

            }
            GetComponent<Trigger>().Translate(Rp + a.x/2f - 2.6f, a.y, -smeshX, longForm);
        }

        form.UpdateForm();
        frame.UpdateForm();
        if (GetComponent<Taskability>())
        {
            GetComponent<Taskability>().UpdateMaxMin();
        }

        //if (GetComponent<OperatorScript>())
        //{
        //    GetComponent<OperatorScript>().transizeLate();
        //}    
    }
    public void SetActiveMaterial(bool a)
    {
        Material buf;
        if (a)
        {
            buf = material2;
        }
        else
        {
            buf = material1;
        }
        foreach (GameObject b in frame.parts)
        {
            b.GetComponent<MeshRenderer>().materials = new Material[] { buf };
        }
    }
    public void SetColour(int a)
    {

        Material colour = colours[a];


        foreach (GameObject part in form.parts)
        {
            part.GetComponent<MeshRenderer>().materials = new Material[] { colour };
        }


    }
    public Vector3[] GetOldSize()
    {
        return new Vector3[] { OldR, OldL };
    }
    public void updateRwRL()
    {

        rL = frame.length - form.length;
        rW = frame.width - form.width;
    }

}
