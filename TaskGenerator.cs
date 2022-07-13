using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TaskGenerator : MonoBehaviour
{
    //Публичные перемены 
    public bool test;
    public int numberQuantities;
    public string txtTask;
    public string txtGiven;
    public bool ScipNext1 = false;
    public Vector2 max = new Vector2(-2, -5);
    public Vector2 min = new Vector2(-73, -21);
    public NextScript next;
    [SerializeField] private Text task;
    [SerializeField] private Text given;
    [SerializeField] private GameObject prefab1, prefab2;
    //public GameObject answer;
    [SerializeField] private ControllerScript Controller;

    //Условно публичные переменые
    public PhysicalQuantities U1, U2, n1, n2, K, I1, R1, I2, R2;
    public List<PhysicalQuantities> Quantities = new List<PhysicalQuantities>();
    public List<PhysicalQuantities> txtQuntities = new List<PhysicalQuantities>();

    //Приватные переменые 
    private int coin;
    private List<PhysicalQuantities> QuantitiesAll = new List<PhysicalQuantities>();
    //private List<PhysicalQuantities> bufQ = new List<PhysicalQuantities>();
    private List<GameObject> ListTablet = new List<GameObject>();
    private List<GameObject> ListPlace = new List<GameObject>();
    private string[] BufStr;
    private GameObject bufObject;
    private Taskability Script;
    private Given giv;
    private float z = 29;
    private Vector3 start;
    private int ButtonNumber;
    //private ScalingTablet st;
    //QuantitiesTablet

    // Start is called before the first frame update
    void Start()
    {
        ButtonNumber = 0;
        //35.6 = sY
        //7.89 = sX
        //Разница x = 5,49 - 5,9 (5.7?)

        //Обезательная инцилизация физический величичин(ФВ)
        I1 = new PhysicalQuantities("I1", "A", 0.1f, 10, 0, new string[][] { new string[] { "U1", "R1" } }, "сила тока на первичной обмотке", 0, 1);
        R1 = new PhysicalQuantities("R1", "Ом", 0.1f, 10, 0, new string[][] { new string[] { "U1", "I1" } }, "сопротивление на первичной обмотке", 0, 1);
        I2 = new PhysicalQuantities("I2", "A", 0.1f, 10, 0, new string[][] { new string[] { "U2", "R2" } }, "сила тока на вторичной обмотке", 0, 2);
        R2 = new PhysicalQuantities("R2", "Ом", 0.1f, 10, 0, new string[][] { new string[] { "U2", "I2" } }, "сопротивление на вторичной обмотке", 0, 2);

        U1 = new PhysicalQuantities("U1", "В", 10, 500, 0.5f, new string[][] { new string[] { "U2", "K" }, new string[] { "I1", "R1"} }, "напряжение на первичной обмотке", 0, 1);
        U2 = new PhysicalQuantities("U2", "В", 10, 500, 0.5f, new string[][] { new string[] { "U1", "K" }, new string[] { "I2", "R2" } }, "напряжение на вторичной обмотке", 0, 2);
        n1 = new PhysicalQuantities("n1", "", 10, 500, 0, new string[][] { new string[] { "n2", "K" } }, "количество витков на первичной обмотке", 0, 3);
        n2 = new PhysicalQuantities("n2", "", 10, 500, 0, new string[][] { new string[] { "n1", "K" } }, "количество витков на вторичной обмотке", 0, 4);
        K = new PhysicalQuantities("K", "", 0.01f, 10, 0.7f, new string[][] { new string[] { "U1", "U2" }, new string[] { "n1", "n2" } }, "коэффициент трансформации", 0, 5);

        //Примерный план добовлений новых ФЗ
        //1 создание переменной и инцилизация
        //2 добовление в общий массив и изменение прошлых ФЗ
        //3 создание формулы под них
        //4 работа над текстовым порядком и добавление тега
        //5 редактирование старых формул фз который от них зависят и редактируем их инцелизацию 
        //6 меняем кафицент распада на наших значения

        //Добовление их в массив всех ФВ
        QuantitiesAll.Add(U1);
        QuantitiesAll.Add(U2);
        QuantitiesAll.Add(n1);
        QuantitiesAll.Add(n2);
        QuantitiesAll.Add(K);
        QuantitiesAll.Add(I1);
        QuantitiesAll.Add(R1);
        QuantitiesAll.Add(I2);
        QuantitiesAll.Add(R2);



        //TODO: OnDrawGizmosSelected() - посмотри на метод
        if (PlayerPrefs.GetInt("Training") == 0)
        {
            //Режим тестового запуска
            if (!test)
            {
                Quantities.Add(QuantitiesAll[Random.Range(0, QuantitiesAll.Count - 1)]);
            }
            else
            {
                Quantities.Add(QuantitiesAll[numberQuantities]);
            }
            //Стандартная генерация
            //Выбор корневого значение 
            Debug.Log("Start generation");
            Quantities[0].inf();
            Quantities[0].KDecay = 1f;

            //Процесс распада корневого значение на другие ФВ и их распад
            disintegration(Quantities[0]);

            //Генерацие или вычисление значение ФВ
            for (int i = Quantities.Count - 1; i > -1; i--)
            {
                Quantities[i].generator(Quantities);
            }
            foreach (PhysicalQuantities a in Quantities)
            {
                coin += a.coin;
            }
            Controller.SetCoin(coin);
        }
        else
        {
            //Обучение             
            Quantities.Add(U1);
            Quantities.Add(I2);
            Quantities.Add(R2);
            Quantities.Add(U2);
            Quantities.Add(K);
            U1.value = 10;
            I2.value = 10;
            R2.value = 5;
            U2.value = 50;
            K.value = 5;
            U1.see = false;
            I2.see = true;
            R2.see = true;
            U2.see = false;
            K.see = true;
            Controller.SetCoin(0);
        }

        //Состовление задачи
        textTaskSort(Quantities);
        txtTask = "Задачa:\n";

        foreach (PhysicalQuantities a in txtQuntities)
        {
            txtTask = txtTask + a.txtGenerator() + " ";
        }
        txtTask = txtTask + Quantities[0].txtGenerator() + " ";
        task.text = txtTask;

        //Создание табличек значений(ТЗ) 
        foreach (PhysicalQuantities a in txtQuntities)
        {
            bufObject = Instantiate(prefab1) as GameObject;
            //bufObject.tag = "Given" + a.name;
            bufObject.transform.position = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 29);
            Script = bufObject.GetComponent<Taskability>();
            Script.StartMin = min;
            Script.StartMax = max;
            Script.z = z;
            Script.Name = a.name;
            Script.sing = a.sing;
            Script.Controller = Controller;
            //Controller.add(bufObject);
            Controller.Tablet.add(bufObject);
            Script.BlockSide = true;
            Script.limitation = false;
            Script.UpdateValue(a.value);
            ListTablet.Add(bufObject);

        }

        //Составение поля дано 
        txtGiven = "Дано:\n";
        textGivenSort(Quantities);
        foreach(PhysicalQuantities a in txtQuntities) 
        {
            txtGiven = txtGiven + a.name + " = \n";
        }
        txtGiven = txtGiven + "\nНайти нужно " + Quantities[0].name + "\nОтвет:";
        given.text = txtGiven;

        //Растовление контрольных точек для ТЗ
        //start = new Vector3(35.6f, 8.89f, z);
        //start = new Vector3(37.1f, 16.5f, 29);
        start = Controller.paperСlip.transform.position;
        foreach (PhysicalQuantities a in txtQuntities)
        {
                
            bufObject = Instantiate(prefab2) as GameObject;
            bufObject.transform.position = start;
            giv = bufObject.GetComponent<Given>();
            giv.Controller = Controller;
            giv.Name = a.name;
            ListPlace.Add(bufObject);
            start.y = start.y - 5.7f;

        }
        start.y = start.y - 5.6f * 2f;
        start.x = 41.95f;
        Controller.answerGiven.transform.position = start;
        
        //Вывод информацие о каждом ФВ
        Debug.Log("Info:");
        foreach (PhysicalQuantities a in Quantities)
        {
            a.inf();
        }

        if (PlayerPrefs.GetInt("Training") == 1)
        {
            inputPosition1();
        }


            //Скип первого этапа
            if (ScipNext1)
        {
            //Автопостоновка
            foreach(GameObject table in ListTablet)
            {
                Script = table.GetComponent<Taskability>();
                foreach(GameObject place in ListPlace)
                {
                    giv = place.GetComponent<Given>();
                    if (giv.Name == Script.Name)
                    {
                        giv.scip(table);
                    }
                            
                }
            }
            //Нажатие кнопки
            //GivenCheck();
        }
    }
    //Проверка ответа
    void Update()
    {
        //if (answer.getcomponent<given>().values() == quantities[0].value)
        //{
        //    debug.log("задача решена");
        //}
    }


    //Класс всех ФВ
    public class PhysicalQuantities //: MonoBehaviour
    {   
        //Публичне переменые
        public bool rand = true;
        public bool see = false;
        public string name;
        public string sing;
        public string txt;
        public float KDecay;        
        public int placeTxt;
        public int placeGiven;
        public int coin;

        //Приватные переменые
        private bool fl = true;
        private int mingen;
        private int maxgen;
        private float mingenf;
        private float maxgenf;
        private int numberFormul;


        //Снова публичные...
        public float value;
        public string[][] formulQ;
        public List<PhysicalQuantities> Q;

        //public List<physicalQuantities> buflist = List<physicalQuantities>();
        //Конструкторы для float и int значений 
        public PhysicalQuantities(string name_, string Sing, int Min, int Max, float K, string[][] formulQ_, string txt_, int placeTxt_, int placeGiven_)
        {
            coin = 0;
            name = name_;
            sing = Sing;
            mingen = Min;
            maxgen = Max;
            formulQ = formulQ_;
            txt = txt_;
            KDecay = K;
            placeTxt = placeTxt_;
            placeGiven = placeGiven_;
            fl = true;
        }
        public PhysicalQuantities(string name_, string Sing, float Min, float Max, float K, string[][] formulQ_, string txt_, int placeTxt_, int placeGiven_)
        {
            coin = 0;
            name = name_;
            sing = Sing;
            mingenf = Min;
            maxgenf = Max;
            formulQ = formulQ_;
            txt = txt_;
            KDecay = K;
            placeTxt = placeTxt_;
            placeGiven = placeGiven_;
            fl = false;

        }

        //Генерацие или вычисление фактического значение ФВ
        public float generator(List<PhysicalQuantities> bufQ)
        {
            Q = bufQ;
            if (rand)
            {
                if (fl)
                {
                    value = Random.Range(mingen, maxgen);
                    Debug.Log( name + " generator " + value);
                    return value;
                }
                else
                {
                    value = Random.Range(mingenf, maxgenf);
                    Debug.Log( name + " generator " + value);
                    return value;
                }
            }
            else
            {
                return formulgenerator();
            }

        }
        
        //Вывод информации
        public void inf()
        {
            Debug.Log(name + ": " + value + sing);
        }

        //Выбирате значение от которых будет зависит ФВ смотря на то не скрыти ли они 
        public string[] formul(List<PhysicalQuantities> bufQ)
        {
            bool l = true;
            int i;
            string[] bufStr;
            Q = bufQ;
            rand = false;
            i = 0;
            while (l)
            {
                if (i > 100)
                {
                    Debug.Log("что то пошло не так :(");
                    break;
                }
                numberFormul = Random.Range(0, formulQ.Length);
                bufStr = formulQ[numberFormul];
                l = false;
                foreach (PhysicalQuantities a in Q)
                {
                    foreach (string b in bufStr)
                    {
                        if (a.name == b)
                        {
                            if (!a.see)
                            {
                                l = true;
                            }
                            //Debug.Log(name + ": " + a.name + ".see = " + a.see);

                        }
                    }
                }
                i++;
            }
            Debug.Log("Я, " + name + " выбрал для себя " + numberFormul + " а именно: ");
            foreach(string str in formulQ[numberFormul])
            {
                Debug.Log(str);
            }
            return formulQ[numberFormul];

        }

        //Ищет в массиве значение ФВ по его имени 
        public float values(string v)
        {
            foreach (PhysicalQuantities a in Q)
            {
                if (a.name == v)
                {
                    return a.value;
                }

            }
            return 0;
        }

        //Рандомер с пронцетной настройкой
        public bool decay()
        {
            float r;
            r = Random.Range(0, 100) / 100f;
            if (KDecay > r)
            {
                Debug.Log(name + " decay true");
                return true;

            }
            Debug.Log(name + " decay false");
            return false;

        }

        //Вычисление значений ФВ соотвествено формулам
        public float formulgenerator()
        {

            //свитч со всеми формулами 
            if (name == "U1")
            {
                switch (numberFormul)
                {
                    case 0:
                        value = values("U2") * values("K");
                        coin = 100;
                        break;
                    case 1:
                        coin = 100;
                        value = values("I1") * values("R1");
                        break;
                }
            }
            else if (name == "U2")
            {
                switch (numberFormul)
                {
                    case 0:
                        coin = 100;
                        value = values("U1") / values("K");
                        break;
                    case 1:
                        coin = 100;
                        value = values("I2") * values("R2");
                        break;
                }
            }
            else if (name == "n1")
            {
                coin = 100;
                value = values("n2") * values("K"); 
            }
            else if (name == "n2")
            {
                coin = 100;
                value = values("n1") / values("K");
            }
            else if (name == "K")
            {
                switch (numberFormul)
                {
                    case 0:
                        coin = 100;
                        value = values("U1") / values("U2");
                        break;
                    case 1:
                        coin = 100;
                        value = values("n1") / values("n2");
                        break;
                }
            }

            else if(name == "I1")
            {
                coin = 100;
                value = values("U1") / values("R1");
            }
            else if (name == "R1")
            {
                coin = 100;
                value = values("U1") / values("I1");
            }
            else if (name == "I2")
            {
                coin = 100;
                value = values("U2") / values("R2");
            }
            else if (name == "R2")
            {
                coin = 100;
                value = values("U2") / values("I2");
            }
            else
            {
                Debug.Log("Что то пошло не так :(");
            }

            Debug.Log(name + ": formulgenerator: " + value);
            return value;
        }

        //Тексттовый кусок от ФВ
        public string txtGenerator()
        {
            string buf;
            if (see)
            {
                buf = txt + " " +  Mathf.RoundToInt(value * 10)/10.0f + sing;
            }
            else
            {
                buf = "\nНайдите " + txt + ".";
            }
            return buf;
        }
    }

    //Доп функции
    //Сортировка по важности в список задачи
    public void textTaskSort(List<PhysicalQuantities> Q)
    {

        txtQuntities = new List<PhysicalQuantities>();

        foreach (PhysicalQuantities a in Q)
        {
            if (a.name == "U1" | a.name == "I1" | a.name == "R1")
            {
                if (a.see) { txtQuntities.Add(a); }
            }
            if(a.name == "U2" | a.name == "I2" | a.name == "R2")
            {
                if (a.see) { txtQuntities.Add(a); }
            }
            if (a.name == "n1" | a.name == "n2")
            {
                if (a.see) { txtQuntities.Add(a); }
            }
            if (a.name == "K")
            {
                if (a.see) { txtQuntities.Add(a); }
            }
        }
    }
    //Сортировка по местам в список дано
    public void textGivenSort(List<PhysicalQuantities> Q)
    {
        bool n;
        int i = 0;
        txtQuntities = new List<PhysicalQuantities>();
        
        while (i < 100)
        {
            
            foreach (PhysicalQuantities a in Q)
            {
                if (a.placeGiven == i)
                {
                    if (a.see) { txtQuntities.Add(a); }
                }
                n = a.placeGiven == i;
                //Debug.Log(name + ": " + a.name + ".placeGiven = " + n);
            }

            i++;
        }
    }
    //Класс проверки дано
    public void ButtonDown()
    {
        if(PlayerPrefs.GetInt("Training") == 1)
        {
            Controller.Cursor.stopClick();
        }
        if (ButtonNumber == 0)
        {
            GivenCheck();
        }
        else if (ButtonNumber == 1)
        {
            next.chek(Quantities[0].value);
        }
        else if(ButtonNumber == -1) 
        {
            next.AnimNext();
            ButtonNumber = 1;
        }
    }
    public void ButtonDownNull()
    {
        ButtonNumber = -1;
    }
    public void GivenCheck()
    {
        bool l = true;
        foreach(GameObject a in ListPlace)
        {
            giv = a.GetComponent<Given>();
            if (!giv.check)
            {
                Debug.Log(giv.Name + ".check = false");
                l = false;
            }
        }
        if (l)
        {
            ButtonNumber = 1;
            Debug.Log("Успешно");
            next.AnimNext();
            Controller.SetSuit(0);
            foreach(GameObject a in ListTablet)
            {
                //a.GetComponent<Taskability>().limitation = true;
                
                a.GetComponent<Taskability>().Transformation();
            }
            foreach (GameObject a in ListPlace)
            {
                //a.GetComponent<Taskability>().limitation = true;
                Controller.Given.delet(a);
                Destroy(a);
            }

        }
        else
        {
            Debug.Log("НеУспешно");
            next.AnimIncorrect();
        }
        //return true;
    }

    private void disintegration(PhysicalQuantities A)
    {
        if (A.decay())
        {
            BufStr = A.formul(Quantities);
            A.see = false;
            foreach (string str in BufStr)
            {
                foreach (PhysicalQuantities a in QuantitiesAll)
                {
                    if (str == a.name)
                    {
                        Quantities.Add(a);
                        if (a.rand)
                        {
                            a.see = true;
                            disintegration(a);
                        }
                    }
                }
            }
        }
    }
    private void inputPosition1()
    {
        List<Vector3> v1 = new List<Vector3>();
        List<Vector3> v2 = new List<Vector3>();


        foreach(GameObject given in ListPlace)
        {
            v2.Add(given.transform.position);
            foreach(GameObject tablet in ListTablet)
            {
                if(tablet.GetComponent<Taskability>().Name == given.GetComponent<Given>().Name)
                {
                    v1.Add(tablet.transform.position);
                }
            }
        }
        Controller.Cursor.MovementPosition(v1, v2);
    }
}





///*Наследственые классы кждого значения