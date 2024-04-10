using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class CsvSaver : MonoBehaviour
{
    [SerializeField] private bool openFile;
    [SerializeField] private string path;

    [ContextMenu("Save")]
    public void Save()
    {
        Save(
            new List<CsvData>()
            {
                new CsvData()
                {
                    name = "Alois",
                    score = 25
                },
                new CsvData()
                {
                name = "MZR",
                score = 50
            }
            }
            );
    }

    public void Save<T>(List<T> datas)
    {
        path = Application.persistentDataPath + "/" + typeof(T).Name;
        if (!Directory.Exists(path)) // If directory dosent exist
        {
            Directory.CreateDirectory(path);
        }
        path += "/" + typeof(T).Name + " (" + Directory.GetFiles(path).Length + ")" + ".csv";

        if (!File.Exists(path))
        {
            File.Delete(path);
        }

        StreamWriter streamWriter = File.CreateText(path);
        FieldInfo[] fields = typeof(T).GetFields();

        string dataCsv = "";

        for (int i = 0; i < fields.Length; i++)
        {
            dataCsv += UpperFirst(fields[i].Name) + ";";
        }

        for (int i = 0; i < datas.Count; i++)
        {
            dataCsv += "\r\n";
            for (int j = 0; j < fields.Length; j++)
            {
                dataCsv += datas[i].GetType().GetField(fields[j].Name).GetValue(datas[i]) + ";";
            }
        }
        
        streamWriter.WriteLine(dataCsv);

        FileInfo fileInfo = new FileInfo(path);
        fileInfo.IsReadOnly = true;
        
        streamWriter.Close();

        if (openFile)
        {
            StartCoroutine(OpenCsv());
        }
    }

    private IEnumerator OpenCsv()
    {
        yield return new WaitForSeconds(.5f);
        Application.OpenURL(path);
    }

    static string UpperFirst(string text)
    {
        return char.ToUpper(text[0]) + ((text.Length > 1) ? text.Substring(1).ToLower() : string.Empty);
    }
}

public class CsvData
{
    public string name = "name";
    public int score = 0;
}