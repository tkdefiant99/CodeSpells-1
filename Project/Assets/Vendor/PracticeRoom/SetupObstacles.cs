using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Diagnostics;
using System.IO;

public class SetupObstacles : MonoBehaviour {
	GameObject prevObject;
	float distance = 10.0f;
	GameObject wallPrefab;
	GameObject prefab;
	GameObject ifGatePrefab;
	float height = 0.0f;
	float adjust = 0.0f;
	protected Thread java_thread;
	protected Process java_process;
	private StreamReader sr;
	string java_files_path;
	string text_files_path;
	
	public void Init (GameObject p_prevCrate) {
		prefab = Resources.Load("PracticeRoomCrate") as GameObject;
		wallPrefab = Resources.Load("Wall") as GameObject;
		ifGatePrefab = Resources.Load("IfGate") as GameObject;
		
		//The first preObject is the start crate
		prevObject = p_prevCrate;
		UnityEngine.Debug.Log("Initializing the obstacles");
		
		createWorld();
	}
	
	public void Setup (string obstacle, int direction)
	{
		String nextObstacle = "";
		int trueCounter = 0;
		int falseCounter = 0;
		distance = 5.0f;
		int ifOffset = 10.0f;
		
		//Add the obstacle 
		switch (obstacle)
		{
		case "RIVER":
			//A "river" is just a really big distance
			distance = 20.0f;
			break;
		case "WALL":
			//A "wall" is a 
			distance = 5.0f;
			height = 5.0f;
			adjust = 2.0f;
			GameObject newWall = Instantiate (wallPrefab, 
				new Vector3(prevObject.transform.position.x+distance, prevObject.transform.position.y+height, prevObject.transform.position.z), 
							wallPrefab.transform.rotation) as GameObject;
			height = -5.0f;
			distance = 10.0f;
			adjust = -2.0f;
			prevObject = newWall;
			
			break;
			
		case "IF_LEFT":
			//CREATE THE IF GATE
			GameObject newIfGate = Instantiate (ifGatePrefab, 
							new Vector3(prevObject.transform.position.x+distance, prevObject.transform.position.y+height, prevObject.transform.position.z), 
							prevObject.transform.rotation) as GameObject;
			
			prevObject = newIfGate;
			
			try {
				//CREATE A TRUE CRATE THAT IS OFFSET AND IS PREVOBJECT
				GameObject trueCrate = Instantiate (prefab, 
							new Vector3(prevObject.transform.position.x+distance, prevObject.transform.position.y+height, prevObject.transform.position.z-ifOffset), 
							prevObject.transform.rotation) as GameObject;
				
				prevObject = trueCrate;
				
				//Read what goes on the left side
				while(!(nextObstacle = sr.ReadLine()).equals("ELSE"))
				{
					setup(nextObstacle);
					trueCounter++;
				}
				
				//MAKE A MERGE FROM NODE
				GameObject mergeTrue = Instantiate (prefab, 
							new Vector3(prevObject.transform.position.x+distance, prevObject.transform.position.y+height, prevObject.transform.position.z), 
							prevObject.transform.rotation) as GameObject;
				
				prevObject = newIfGate;
				
				//CREATE A FALSE CRATE THAT IS OFFSET AND IS PREVOBJECT
				GameObject falseCrate = Instantiate (prefab, 
							new Vector3(prevObject.transform.position.x+distance, prevObject.transform.position.y+height, prevObject.transform.position.z+ifOffset), 
							prevObject.transform.rotation) as GameObject;
				
				prevObject = falseCrate;
				
				//Read what goes on the left side
				while(!(nextObstacle = sr.ReadLine()).equals("ENDIF"))
				{
					setup(nextObstacle);
					falseCounter++;
				}
				
				//MAKE A MERGE FROM NODE
				GameObject mergeFalse = Instantiate (prefab, 
							new Vector3(prevObject.transform.position.x+distance, prevObject.transform.position.y+height, prevObject.transform.position.z), 
							prevObject.transform.rotation) as GameObject;
				
				//SET PREVOBSTACLES TO IFGATE FOR MERGENODE and update distance
				prevObject = newIfGate;
				distance = 30*(Math.max(trueCounter, falseCounter));
			}
			catch (Exception e)
	        {
				UnityEngine.Debug.Log("It didn't open because: "+e.Message);
	            Console.WriteLine("The file could not be read:");
	            Console.WriteLine(e.Message);
	        }
			break;
		case "IF_RIGHT":
			//CREATE THE IF GATE
			GameObject newIfGate = Instantiate (ifGatePrefab, 
							new Vector3(prevObject.transform.position.x+distance, prevObject.transform.position.y+height, prevObject.transform.position.z), 
							prevObject.transform.rotation) as GameObject;
			
			prevObject = newIfGate;
			
			try {
				//CREATE A TRUE CRATE THAT IS OFFSET AND IS PREVOBJECT
				GameObject trueCrate = Instantiate (prefab, 
							new Vector3(prevObject.transform.position.x+distance, prevObject.transform.position.y+height, prevObject.transform.position.z+ifOffset), 
							prevObject.transform.rotation) as GameObject;
				
				prevObject = trueCrate;
				
				//Read what goes on the left side
				while(!(nextObstacle = sr.ReadLine()).equals("ELSE"))
				{
					setup(nextObstacle);
					trueCounter++;
				}
				
				//MAKE A MERGE FROM NODE
				GameObject mergeTrue = Instantiate (prefab, 
							new Vector3(prevObject.transform.position.x+distance, prevObject.transform.position.y+height, prevObject.transform.position.z), 
							prevObject.transform.rotation) as GameObject;
				
				prevObject = newIfGate;
				
				//CREATE A FALSE CRATE THAT IS OFFSET AND IS PREVOBJECT
				GameObject falseCrate = Instantiate (prefab, 
							new Vector3(prevObject.transform.position.x+distance, prevObject.transform.position.y+height, prevObject.transform.position.z-ifOffset), 
							prevObject.transform.rotation) as GameObject;
				
				prevObject = falseCrate;
				
				//Read what goes on the left side
				while(!(nextObstacle = sr.ReadLine()).equals("ENDIF"))
				{
					setup(nextObstacle);
					falseCounter++;
				}
				
				//MAKE A MERGE FROM NODE
				GameObject mergeFalse = Instantiate (prefab, 
							new Vector3(prevObject.transform.position.x+distance, prevObject.transform.position.y+height, prevObject.transform.position.z), 
							prevObject.transform.rotation) as GameObject;
				
				//SET PREVOBSTACLES TO IFGATE FOR MERGENODE and update distance
				prevObject = newIfGate;
				distance = 30*(Math.max(trueCounter, falseCounter));
			}
			catch (Exception e)
	        {
				UnityEngine.Debug.Log("It didn't open because: "+e.Message);
	            Console.WriteLine("The file could not be read:");
	            Console.WriteLine(e.Message);
	        }
			break;
		default:
			distance = 10.0f;
			break;
		}
		
		//Add Crate
		GameObject newCrate = Instantiate (prefab, 
							new Vector3(prevObject.transform.position.x+distance, prevObject.transform.position.y+height, prevObject.transform.position.z), 
							prevObject.transform.rotation) as GameObject;
		height = 0.0f;
		adjust = 0.0f;
		prevObject = newCrate;
	}
	
	public void createWorld()
	{
		UnityEngine.Debug.Log("About to spawn a thread for compiling the java: "+Application.dataPath);
		
		if(Application.isEditor)
			{
				text_files_path = Application.dataPath + "/Vendor/PracticeRoom/World.txt";
				java_files_path = Application.dataPath + "/Vendor/PracticeRoom/src/";
			} else {
				//After deployment, the the directories /Vendor/June/MyJune/ and /Vendor/June/ are 
				//  moved to the root directory of the unity executable.
				//  This is done by the Editor/PostprocessBuildPlayer script
				java_files_path = Application.dataPath + "/../../PracticeRoom/src/";
				text_files_path = Application.dataPath + "../../PracticeRoom/World.txt";
			}
		UnityEngine.Debug.Log("The javaFilesPath: "+java_files_path);
		UnityEngine.Debug.Log("The textFilesPath: "+text_files_path);
		UnityEngine.Debug.Log("I have the text file: "+File.Exists(text_files_path));

		//Compile and run the Java code that creates a random world and the code
		java_thread = (new Thread(javaCompileAndRun));
		java_thread.IsBackground = true;
		java_thread.Start();
		
		UnityEngine.Debug.Log("I still have the new text file: "+File.Exists(text_files_path));
		
		string firstObstacle = "";
		try{
			sr = new StreamReader(text_files_path);
			while((firstObstacle = sr.ReadLine()) != null)
			{
				//Actually build the obstacle in Unity
				setup(firstObstacle);;
			}
		}
		catch (Exception e)
        {
			UnityEngine.Debug.Log("It didn't open because: "+e.Message);
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }	
	}
	
	void javaCompileAndRun()
	{
		bool success = true;
		try{
			string class_name = "PracticeRoom";
			
			UnityEngine.Debug.Log("Running");
			UnityEngine.Debug.Log("java -classpath "+java_files_path+" PracticeRoom " + text_files_path);
			java_process = Shell.shell_no_start("java", "-classpath "+java_files_path+" PracticeRoom " + text_files_path);	
			//Generating the world file
			java_process = Shell.shell_no_start("java", "-classpath "+java_files_path+" PracticeRoom " + text_files_path);
			//Actually running the Java code
			java_process.Start();
			java_process.WaitForExit();
			
			var output3 = java_process.StandardOutput.ReadToEnd();
	   		var error3 = java_process.StandardError.ReadToEnd();
			
			UnityEngine.Debug.Log ("RUNNING DONE: "+output3 + " " + error3);
			
		}catch(Exception e){
			UnityEngine.Debug.Log("Exception: "+e.Message);
		}
	}
}
