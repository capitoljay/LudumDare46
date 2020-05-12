using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace FishTank.SaveState
{
    [Serializable]
    public class SaveState
    {
        public DateTime SavedOn { get; set; }
        public string SaveName { get; set; }

        public float Budget { get; set; }
        public int FoodCount { get; set; }
        public int MedicineCount { get; set; }
        public List<FoodSave> Food { get; set; }
        public List<PoopSave> FishPoop { get; set; }
        public List<FishSave> Fish { get; set; }
        public string ScreenshotPng { get; set; }
        public float TankDirtiness { get; set; }

        #region "Save and Load"

        /// <summary>
        /// Serializes the SaveState and save it to the provided path.
        /// </summary>
        /// <param name="savePath">Path of the file to save to.</param>
        /// <param name="state">The SaveState to save.</param>
        public static void Save(string savePath, SaveState state)
        {
            FileStream stream = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Save(stream, state);
            stream.Flush();
            stream.Close();
        }

        /// <summary>
        /// Serializes the SaveState and save it to the provided stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        /// <param name="state">The SaveState to save.</param>
        public static void Save(Stream stream, SaveState state)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveState));
            if (stream.CanSeek)
                stream.Position = 0;

            stream.SetLength(0);
            stream.Flush();

            serializer.Serialize(stream, state);
        }

        /// <summary>
        /// Loads the provided path into a new SaveState and returns it.
        /// </summary>
        /// <param name="loadPath">Path of the file to load from</param>
        /// <returns>A deserialized SaveState object</returns>
        public static SaveState Load(string loadPath)
        {
            FileStream stream = new FileStream(loadPath, FileMode.OpenOrCreate, FileAccess.Read);
            return Load(stream);
        }

        /// <summary>
        /// Loads the provided stream into a new SaveState and returns it.
        /// </summary>
        /// <param name="stream">The stream to load from</param>
        /// <returns></returns>
        public static SaveState Load(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveState));
            if (stream.CanSeek)
                stream.Position = 0;

            return (SaveState)serializer.Deserialize(stream);
        }

        public static void ConvertCommon<T, S>(S source, ref T destination)
        {
            var SourceProps = typeof(S).GetProperties();
            var DestinationProps = typeof(T).GetProperties();

            foreach (var prop in SourceProps)
            {
                try
                {
                    var destProp = DestinationProps.FirstOrDefault(p => p.Name.ToUpper() == prop.Name.ToUpper());
                    if (destProp != null)
                    {
                        var val = prop.GetValue(source);
                        if (val != null)
                            destProp.SetValue(destination, val);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }

            //Manually set the ObjectKey properties if both objects implement IKeyedObject
            if (typeof(IKeyedObject).IsAssignableFrom(typeof(S)) && typeof(IKeyedObject).IsAssignableFrom(typeof(S)))
            {
                (destination as IKeyedObject).ObjectKey = (source as IKeyedObject).ObjectKey;
            }
        }
        public static T ConvertCommon<T, S>(S source)
        {
            T retObj = (T)typeof(T).Assembly.CreateInstance(typeof(T).FullName);

            ConvertCommon<T, S>(source, ref retObj);

            return retObj;
        }

        #endregion "Save and Load"
    }
}
