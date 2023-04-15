using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static SatSys.SatRecord;
using System.IO;
using SFB;

public class FileManager : MonoBehaviour
{
    void Start()
    {
        EventManager.OpenButtonClicked += OpenTask;
        EventManager.SaveButtonClicked += SaveTask;
    }

    private void OpenTask()
    {
        StandaloneFileBrowser.OpenFilePanelAsync(
            "Open File",
            "",
            "json",
            false,
            (paths) =>
            {
                if (paths.Length == 0)
                    return;
                using (StreamReader file = File.OpenText(paths[0]))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    SatTask task = (SatTask)serializer.Deserialize(file, typeof(SatTask));
                }
            }
        );
    }

    private void SaveTask()
    {
        var task = new SatTask();
        StandaloneFileBrowser.SaveFilePanelAsync(
            "Save File",
            "",
            "test",
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
}