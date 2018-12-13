using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class XmlController
{
    private static readonly string filename = "Results.xml";

    public static ResultsXmlContainer GetResults()
    {
        Stream reader = File.OpenWrite(filename);
        reader.Close();
        reader = new FileStream(filename, FileMode.Open);
        XmlSerializer xs = new XmlSerializer(typeof(List<Result>));

        var container = new ResultsXmlContainer();

        try
        {
            var results = (List<Result>)xs.Deserialize(reader);
            container.Results = results;
        }
        catch
        {
            Debug.Log("Results.xml was rewritten due to: \n1) File was corrupted\n2) File is empty\n3) Some other reason");
        }

        reader.Close();

        return container;
    }
    
    public static void UpdateResults(Result result)
    {
        var results = GetResults();
        results.Results.Add(result);
        //var results = new ResultsXmlContainer();
        //results.Results.Add(result);
        XmlSerializer xs = new XmlSerializer(typeof(List<Result>));

        Stream writer = new FileStream(filename, FileMode.Create);
        xs.Serialize(writer, results.Results);
        writer.Close();
    }
}
