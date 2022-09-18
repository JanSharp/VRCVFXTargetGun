using UnityEngine;
using UdonSharp;
using TMPro;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    public class TestScript : UdonSharpBehaviour
    {
        // // private string GetString(bool foo)
        // // {
        // //     return foo ? "yes" : "no";
        // // }

        // void Update()
        // {
        //     // // Debug.Log(GetString(true));
        //     // // Debug.Log(GetString(false));

        //     // string[] createdArray = new string[4];
        //     // Debug.Log(createdArray[1]);
        // }

        void Start()
        {
            // OnChangeTest();
            // TextMeshProTest();
            Event14();
        }



        public TextMeshPro textObj;
        public TextMeshProUGUI textUGUIObj;

        void TextMeshProTest()
        {
            // alright, so instead of using the cursed function/method SetText
            // it's a simple assignment to the text field (property, yea ignore those)
            // that's it. it's this simple
            textObj.text = "one";
            textUGUIObj.text = "two";
            textObj.text = (1).ToString();
        }



        [FieldChangeCallback(nameof(OnChangePointTotal))] // mark it for on change
        public int pointTotal;

        // the on change node
        private int OnChangePointTotal
        {
            set
            {
                // the old value is still in `bar`
                // the new value is in the special keyword `value`
                // according to the description on FieldChangeCallback this method
                // is responsible for setting bar = value
                Debug.Log("old:" + pointTotal);
                Debug.Log("new:" + value);
                pointTotal = value; // yep, tested it, this is necessary,
                // otherwise the pointTotal value will not be changed
            }
        }

        // wizardry
        private int pointTotalProxy {
            get => pointTotal;
            set => SetProgramVariable(nameof(pointTotal), value);
        }

        void OnChangeTest()
        {
            pointTotal = 100; // doesn't run OnChangePointTotal
            SetProgramVariable(nameof(pointTotal), 200); // does run OnChangePointTotal
            Debug.Log("after setting:" + pointTotal);
            pointTotalProxy = 400;
            Debug.Log("proxy: " + pointTotalProxy);
        }

        public GameObject target;
        void Event1()
        {
            // order of operation:
            // 4            1      3            2                       6               5
            ((UdonBehaviour)target.GetComponent(typeof(UdonBehaviour))).SendCustomEvent("hello");
        }

        private UdonBehaviour example;
        void Event2()
        {
            example = (UdonBehaviour)target.GetComponent(typeof(UdonBehaviour));
            example.SendCustomEvent("hello");
        }

        void Event3()
        {
            // local variables in order to reuse values without bloating fields in the entire class
            UdonBehaviour behaviour = (UdonBehaviour)target.GetComponent(typeof(UdonBehaviour));
            behaviour.SetProgramVariable("something", "some value");
            behaviour.SendCustomEvent("event");
        }

        public GameObject[] targetArray;
        // the array is basically an association of indexes to values, so
        // 0 => GameObject
        // 1 => GameObject
        // 2 => GameObject
        // 3 => GameObject
        // 4 => GameObject
        // 5 => GameObject
        // 6 => GameObject
        // 7 => GameObject
        void Event4()
        {
            GameObject obj0 = (GameObject)targetArray.GetValue(0);
            obj0.SetActive(false);
        }

        void Event5()
        {
            GameObject obj0 = targetArray[0];
            obj0.SetActive(false);
        }

        void Event6()
        {
            targetArray.SetValue(target, 1);
        }

        void Event7()
        {
            targetArray[1] = target;
        }

        void Event8()
        {
            //   make var ; loop while this is true; increment
            for (int i = 0; i < targetArray.Length; i++)
            {
                ((GameObject)targetArray.GetValue(i)).SetActive(false);
            }
        }

        void Event9()
        {
            //   make var ; loop while this is true; increment
            for (int i = 0; i < targetArray.Length; i++)
            {
                // for reference:
                // the hard way:
                //   GameObject obj0 = (GameObject)targetArray.GetValue(0);
                // the easy way:
                //   GameObject obj0 = targetArray[0];
                targetArray[i].SetActive(false);
            }
        }

        void Event10()
        {
            int theSizeWeWant = 100; // evaluated somehow
            GameObject[] myArray = new GameObject[theSizeWeWant];
            myArray[0] = target;

            string[] strings = new string[]
            {
                "one",
                "two",
                "three",
            };
        }

        public bool condition;
        void Event11()
        {
            if (condition)
            {
                Debug.Log("if");
                Debug.Log("and another statement inside the block");
            }
            else
            {
                Debug.Log("else");
                Debug.Log("and again some more stuff");
            }

            if (condition)
                Debug.Log("if");
                // putting another statement here wouldn't be apart of the if block anymore
            else
                Debug.Log("else");
                // same here

            Debug.Log("and again some more stuff"); // this is outside of the else block
        }

        public float Speed;
        public Rigidbody RB;
        public void Event12()
        {
            RB.velocity = new Vector3(Speed, RB.velocity.y);
        }

        public void Event13()
        {
            // we have to do it this way in Udon
            Component[] components = gameObject.GetComponentsInChildren(typeof(Transform));
            for (int i = 0; i < components.Length; i++)
            {
                components[i].gameObject.SetActive(true);
            }

            // unfortunately this is not exposed in Udon
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();

            // technically using an enumerator get get all children
            foreach (Transform foo in gameObject.transform)
            {
                Debug.Log(foo.name);
            }

            // UdonSharp generates this behind the scenes
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform foo = gameObject.transform.GetChild(i);
                Debug.Log(foo.name);
            }
        }

        public void Event14()
        {
            // these are currently not implemented
            // UdonBehaviour test = gameObject.GetComponent<UdonBehaviour>();
            // Debug.Log(test.name);

            // int[,] bar = new int[1, 2];
            // use "jagged" arrays for this instead. TIL what jagged arrays are
            // I just called them nested arrays, it's int[][]
        }

        public void Event15()
        {
            int[] foo = new int[100];
            foreach (int bar in foo)
            {
                Debug.Log(bar);
            }
        }

        public Transform boat;
        public Quaternion headRotation;

        public void Event16()
        {
            float vertical = Input.GetAxis("vertical");

            // nope, this also doesn't do it because looking down or up would increase the angle diff even though you're not looking further to the right nor left
            Quaternion boatRotation = boat.transform.rotation;
            float angleDiff = Quaternion.Angle(boatRotation, headRotation);
            Quaternion rotationDiff = Quaternion.AngleAxis(angleDiff, Vector3.up);
            Quaternion targetRotation = boatRotation * rotationDiff;
            boat.transform.rotation = Quaternion.RotateTowards(boatRotation, targetRotation, 20f * Time.deltaTime * Mathf.Sign(vertical));
        }

        // public float castTime;

        // private float timeOfCast;
        // [UdonSynced]
        // private float castTimePassed;
        // public override void OnPreSerialization()
        // {
        //     castTimePassed = Time.time - timeOfCast;
        // }

        // public void Cast()
        // {
        //     timeOfCast = Time.time;
        // }

        // public override void OnDeserialization()
        // {
        //     SendCustomEventDelayedSeconds("something", castTime - castTimePassed);
        // }

        public Material otherMat;

        public void Event17()
        {
            ((Renderer)GetComponent(typeof(Renderer))).material = otherMat;
            GetComponent<Renderer>().material = otherMat;
        }

        // foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        // {
        //     foreach (var module in assembly.Modules)
        //     {
        //         foreach (var type in module.GetTypes())
        //         {
        //             Debug.Log(type);
        //             if (type.BaseType == typeof(VRC_Pickup))
        //             {
        //                 Debug.Log(type);
        //             }
        //         }
        //     }
        // }
        // return;

        public void Event18()
        {
            int layer = 524288;
            Debug.Log(layer.ToString("x8"));
            for (int i = 0; i < 32; i++)
                if ((layer & (1 << i)) != 0)
                    Debug.Log($"{i}: {LayerMask.LayerToName(i)}");
        }

        public void Event19()
        {

        }
    }
}
