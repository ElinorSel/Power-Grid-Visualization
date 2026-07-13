using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CSVReader : MonoBehaviour
{
    private string filePath;
    
    private List<string[]> data_values;
    public List<string[]> ReadCSVFile(string filePath)
    {
        data_values = new List<string[]>();
        using (StreamReader strReader = new StreamReader(filePath))
        {
            bool endOfFile = false;
            while(!endOfFile)
            {
                string data_String = strReader.ReadLine();
                if(data_String == null)
            {
                endOfFile = true;
                break;
            }
            data_values.Add(data_String.Split(','));
            }
        }
        return data_values;
    }


}
