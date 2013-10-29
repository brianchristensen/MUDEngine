using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace MUDInterface.Entities.Locations
{
    public class MapLoader
    {
        private MapLoader() { }
        private static MapLoader _instance = new MapLoader();
        public static MapLoader Instance { get { return _instance; } }

        public void LoadMap(string xmlMapFile)
        {
            try
            {
                string file = getXMLResource(xmlMapFile);
                XDocument doc = XDocument.Parse(file);

                List<XElement> rooms = doc.Root.Descendants("Location").ToList();
                Room room = null;

                foreach (XElement r in rooms)
                {
                    room = new Room(r.Element("Name").Value, r.Element("Description").Value, Convert.ToInt32(r.Element("ID").Value));
                    List<XElement> exits = r.Element("Exits").Descendants().ToList();
                    foreach (XElement exit in exits)
                    {
                        room.Exits.Add(exit.Name.ToString(), Convert.ToInt32(exit.Value));
                    }

                    EntityManager.Instance.RegisterLocation(room);
                }                
            }
            catch (Exception ex)
            {
                GameOutput.Client.GlobalMessage(ex.Message);
            }
        }

        private string getXMLResource(string xmlFile)
        {
            string result = string.Empty;

            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("MUDInterface.Entities.Locations." + xmlFile))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }
    }
}