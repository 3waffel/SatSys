using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static SatSys.SatRecord;
using System.IO;
using SFB;

public class FileManager : MonoBehaviour
{
    public static void OpenTask()
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
                    ObjectManager.OM.ClearScene();
                }
            }
        );
    }

    public static void SaveTask()
    {
        var collection = ObjectManager.OM.objectCollection;
        SatTask task;
        if (collection.Count == 0)
        {
            Debug.Log("Object Collection is empty");
            task = new SatTask();
        }
        else
        {
            var stations = new List<Station>();
            var satellites = new List<Satellite>();
            foreach (var so in collection)
            {
                switch (so)
                {
                    case (StationSO stt):
                        stations.Add(new Station(stt));
                        break;
                    case (SatelliteSO sat):
                        satellites.Add(
                            new Satellite(
                                sat,
                                Timeline.startDate,
                                Timeline.endDate,
                                Timeline.timeStep
                            )
                        );
                        break;
                }
            }
            task = new SatTask(
                stations,
                satellites,
                Timeline.startDate,
                Timeline.endDate,
                Timeline.timeStep
            );
        }

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
}
