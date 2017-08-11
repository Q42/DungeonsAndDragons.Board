using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;

public class Datastore
{
	/// <summary>
	/// Default database reference for the Dungeons and Dragons application.
	/// </summary>
	protected DatabaseReference DatabaseReference;

	/// <summary>
	/// Constructor creating the connection to the Firebase database and getting de default reference.
	/// </summary>
	public Datastore() {
		Debug.Log("[Datastore] Setting up connection to the Firebase database");
		
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://dungeonsanddragons-6a9b1.firebaseio.com/");
		
		DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;
	}
}
