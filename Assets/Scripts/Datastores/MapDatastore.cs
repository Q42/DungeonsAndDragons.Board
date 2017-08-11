using Firebase.Database;
using Models;
using UnityEngine;

namespace Datastores
{
    public class MapDatastore : Datastore
    {
        /// <summary>
        /// Subscribe to the changes in the maps object. For now this is just for testing purposes.
        /// </summary>
        public void SubscribeToMaps()
        {
            Debug.Log("[Map Datastore] Subscribing to Maps table");
            FirebaseDatabase.DefaultInstance.GetReference("Maps").ValueChanged += HandleValueChanged;
        }
        
        /// <summary>
        /// Save a new map to the Firebase database maps object. For now this is just for testing purposes. 
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="mapSize"></param>
        public void SaveNewMap(string mapName, float mapSize)
        {
            Debug.Log("[Map Datastore] Saving new map with name: " + mapName);
            var map = new Map(mapName, mapSize);

            DatabaseReference.Child("Maps").SetRawJsonValueAsync(JsonUtility.ToJson(map));
        }

        private void HandleValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.DatabaseError != null)
            {
                Debug.LogError("[Map Datastore] Failed to receive data from Maps: " + e.DatabaseError.Message);
                return;
            }
            
            Debug.Log("[Map Datastore] Retrieved new data: " + sender);
        }
    }
}