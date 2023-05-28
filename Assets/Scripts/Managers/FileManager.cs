using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static SatSys.SatRecord;
using System.IO;
using SFB;
using One_Sgp4;

public class FileManager : MonoBehaviour
{
    public static void OpenFile()
    {
        StandaloneFileBrowser.OpenFilePanelAsync(
            "Open File",
            "",
            new[] { new ExtensionFilter("", "json"), new ExtensionFilter("", "txt") },
            false,
            (paths) =>
            {
                if (paths.Length == 0)
                    return;
                string fileName = paths[0];
                if (fileName.EndsWith(".json"))
                    OpenTask(fileName);
                else
                    OpenTleFile(fileName);
            }
        );
    }

    public static void OpenTask(string fileName)
    {
        using (StreamReader file = File.OpenText(fileName))
        {
            JsonSerializer serializer = new JsonSerializer();
            SatTask task = (SatTask)serializer.Deserialize(file, typeof(SatTask));
            ObjectManager.Instance.LoadTask(task);
        }
    }

    public static void SaveTask()
    {
        var task = ObjectManager.Instance.SaveScene() ?? new SatTask();

        StandaloneFileBrowser.SaveFilePanelAsync(
            "Save File",
            "",
            "output",
            "json",
            (path) =>
            {
                if (path == "")
                    return;

                using (StreamWriter file = File.CreateText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(file, task);
                }
            }
        );
    }

    public static void OpenTleFile(string fileName)
    {
        var tleList = ParserTLE.ParseFile(fileName);
        ObjectManager.Instance.LoadTleList(tleList);
    }
}
