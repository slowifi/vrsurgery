using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using System;

public class readPLY : MonoBehaviour
{
    
    // Update is called once per frame
    List<List<double>> data = new List<List<double>>();
    int start = 0;
    public GameObject asdf;

    void ReadPoly()
    {
        const string FILENAME = "C:/Users/USER/Downloads/griddebug.ply";
        
        StreamReader reader = new StreamReader(FILENAME);
        string inputLine = "";
        bool endHeader = false;
        while ((inputLine = reader.ReadLine()) != null)
        {
            inputLine = inputLine.Trim();
            if (inputLine.Length > 0)
            {
                if (endHeader)
                {
                    List<double> newRow = inputLine.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x)).ToList();
                    data.Add(newRow);
                }
                else
                {
                    if (inputLine.Contains("end_header"))
                    {
                        endHeader = true;
                    }
                }
            }
        }
    }

    void Update()
    {
        if (start == 0)
        {
            ReadPoly();
            start++;
            
            foreach (var item in data)
            {
                GameObject v_test = new GameObject();
                v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                v_test.transform.SetParent(this.transform);
                v_test.transform.position = asdf.transform.TransformPoint(new Vector3(Convert.ToSingle(item[0]), Convert.ToSingle(item[1]), Convert.ToSingle(item[2])));
            }
        }

        
    }
}
