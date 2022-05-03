using System.IO;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;

public static class SavePlanet
{
    public static void Save(GlobalFacilitator globalFacilitator)
    {
        //this is the data that will be saved
        KeyPlanetData planetData = new KeyPlanetData(globalFacilitator);

        //this is the formatter for saving the file
        BinaryFormatter bFormatter = new BinaryFormatter();

        //this is the file location that the data will be saved to
        string savePath = Application.persistentDataPath + GlobalSettings.saveDataSuffix + GlobalSettings.currentPlanetIndex.ToString();

        //check if this is a directory otherwise create it
        if(!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        //this is the stream that will be used
        FileStream dataStream = new FileStream(savePath + GlobalSettings.saveDataFileName, FileMode.Create);

        //This serializes (or saves) the data to the specified path
        bFormatter.Serialize(dataStream, planetData);

        //now the file stream must be closed
        dataStream.Close();
    }

    public static void DeleteCurrentSave()
    {
        //this is the file location that the data will be saved to
        string savePath = Application.persistentDataPath + GlobalSettings.saveDataSuffix + GlobalSettings.currentPlanetIndex.ToString();

        if(Directory.Exists(savePath))
        {
            Directory.Delete(savePath, true);
        }
    }

    public static bool Load(out KeyPlanetData planetData, GlobalFacilitator globalFacilitator, string path)
    {
        //this is the file location that the data will be loaded from
        string loadPath = Application.persistentDataPath + GlobalSettings.saveDataSuffix + path + GlobalSettings.saveDataFileName;

        if (File.Exists(loadPath)) 
        {
            //this is the formatter for saving the file
            BinaryFormatter bFormatter = new BinaryFormatter();

            //this is the stream that will be used
            FileStream dataStream = new FileStream(loadPath, FileMode.Open);

            //This retrieves the data and saves it as the planet data
            planetData = bFormatter.Deserialize(dataStream) as KeyPlanetData;

            //now the file stream must be closed
            dataStream.Close();

            return true;

        }
        else
        {
            Debug.Log("No save file for this planet, generating new");

            planetData = new KeyPlanetData(globalFacilitator);

            return false;
        }
    }
}
