﻿using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
public class Manager : MonoBehaviour {
	
	private GameObject[] servoArray;
	
	private string fileMotions = @"Assets\Data\motions.txt";
	private string fileMovements = @"Assets\Data\movements.txt";
	private string fileServoLimits = @"Assets\Data\servoLimits.txt";
	
	private List<Motion> motionsList = new List<Motion>();
	private List<Movement> movementsList = new List<Movement>();
	
	private static int clickCounter = 0;
	private static int movementCounter = 0;
	
	// Use this for initialization
	void Start () {
		servoArray = GameObject.FindGameObjectsWithTag("ConfigJoint").OrderBy(go => go.name).ToArray();
		if (File.Exists(fileMotions)) {
			motionsList = CreateMotions(ReadFromFile(fileMotions));			
		} else {
			print ("File doesn't exist");
			
		}
		movementsList = CreateMovements(ReadFromFile(fileMovements));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	List<string> ReadFromFile(string fileName) {
		List<string> readStringsList = new List<string>();
		string[] s = File.ReadAllLines(fileName);
		for (int i = 0; i < s.Length; i++) {
			readStringsList.Add(s[i]);
		}
		return readStringsList;
	}
	
	List<Motion> CreateMotions(List<string> stringsList) {
		List<Motion> motionsList = new List<Motion>();
		foreach(string currentString in stringsList) {
			string[] splittedString = currentString.Split(',');
			string motionName = splittedString[0];
			
			Dictionary<String, int> dictionaryOfServoAngles = new Dictionary<String, int>();
			for (int i = 1; i < splittedString.Length; i++) {
				string[] servoAnglePair = splittedString[i].Split(':');
				int parsedAngle = 0;
				if (Int32.TryParse(servoAnglePair[1], out parsedAngle))
					dictionaryOfServoAngles.Add(servoAnglePair[0],parsedAngle);		
				else
					print ("Parsed string " + servoAnglePair[1] + " wasn't parsed");		
			}
			List<float> v = new List<float>(3);
			for (int i = 0; i < 3; i++) 
				v.Add (0f);
			if(!motionName.Equals(""))
				motionsList.Add(new Motion(motionName, dictionaryOfServoAngles, v));
		}
		
		return motionsList;
	}
	
	List<Movement> CreateMovements(List<string> stringList) {
		List<Movement> resultList = new List<Movement>();
		foreach(string currentString in stringList) {
			string[] splittedString = currentString.Split(',');
			string movementName = splittedString[0];
			Movement m = new Movement(movementName);
			
			for (int i = 1; i < splittedString.Length; i++) {
				foreach (Motion currentMotion in motionsList) {
					if (splittedString[i].Equals(currentMotion.name)) {
						m.Add (currentMotion);
					}
				}
			}
			resultList.Add(m);
		}
		return resultList;
	}
	
	public void OnMotionButton() {
		print ("ClickCounter: " + clickCounter.ToString("00"));		
		foreach (GameObject g in servoArray) {	
			if (clickCounter < motionsList.Count) {
				Dictionary<string, int> t = motionsList[clickCounter].servoAngles;
				foreach(KeyValuePair<string, int> servoAnglePair in t) {
					if (g.name.Substring(g.name.Length - 2).Equals(servoAnglePair.Key)) {
						g.GetComponent<ServoMotorConfigurable>().Setup(servoAnglePair.Value);
					}
				}		
			}			
		}
		
		clickCounter++;
		
	}
	
	public void OnMovementButton() {
		foreach (Motion m in movementsList[movementCounter].motionList) {	
			
			foreach (GameObject g in servoArray) {	
				Dictionary<string, int> t = m.servoAngles;
				foreach(KeyValuePair<string, int> servoAnglePair in t) {
					if (g.name.Substring(g.name.Length - 2).Equals(servoAnglePair.Key)) {
						g.GetComponent<ServoMotorConfigurable>().Setup(servoAnglePair.Value);
					}
				}				
			}
		}
		
		movementCounter++;
	}
}
