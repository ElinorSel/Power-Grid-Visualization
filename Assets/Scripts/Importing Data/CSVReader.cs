using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class CSVReader : MonoBehaviour
{
    /// <summary>
    /// Loads and parses a CSV file from a path or URL.
    /// Works in the Unity Editor, Android, and WebGL.
    /// </summary>
    public IEnumerator ReadCSVFile(
        string filePath,
        Action<List<string[]>> onComplete)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(filePath))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    $"Failed to read CSV file:\n" +
                    $"Path: {filePath}\n" +
                    $"Error: {request.error}"
                );

                onComplete?.Invoke(null);
                yield break;
            }

            string csvText = request.downloadHandler.text;

            List<string[]> dataValues = ParseCSV(csvText);

            onComplete?.Invoke(dataValues);
        }
    }


    /// <summary>
    /// Parses CSV text into a list of string arrays.
    /// Each array represents one row.
    /// </summary>
    private List<string[]> ParseCSV(string csvText)
    {
        List<string[]> dataValues = new List<string[]>();

        string[] lines = csvText.Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.RemoveEmptyEntries
        );

        foreach (string line in lines)
        {
            dataValues.Add(line.Split(','));
        }

        return dataValues;
    }
}