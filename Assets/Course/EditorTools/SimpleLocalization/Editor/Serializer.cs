/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2019
 *	
 *	"Serializer.cs"
 * 
 *	This script serializes saved game data and performs the file handling.
 * 
 * 	It is partially based on Zumwalt's code here:
 * 	http://wiki.unity3d.com/index.php?title=Save_and_Load_from_XML
 *  and uses functions by Nitin Pande:
 *  http://www.eggheadcafe.com/articles/system.xml.xmlserialization.asp 
 * 
 */

#if !UNITY_WEBPLAYER
#define CAN_DELETE
#endif

using UnityEngine;
using System.Collections.Generic;
using System.IO;


	/**
	 * All of AC's actual file handling, serialising and deserialising is performed within this script.
	 * Its public functions are static, so it does not need to be placed on any scene object.
	 */
	#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0)
	[HelpURL("http://www.adventurecreator.org/scripting-guide/class_a_c_1_1_serializer.html")]
	#endif
	public class Serializer : MonoBehaviour
	{

		

		#if UNITY_EDITOR

		public static bool SaveFile (string fullFileName, string _data)
		{
			try
			{
				StreamWriter writer; // = new 
				FileInfo t = new FileInfo (fullFileName);
				
				if (!t.Exists)
				{
					writer = t.CreateText ();
				}
				
				else
				{
					#if CAN_DELETE
					t.Delete ();
					#endif
					writer = t.CreateText ();
				}
				
				writer.Write (_data);
				writer.Close ();

				//ACDebug.Log ("File written: " + fullFileName);
			}
			catch (System.Exception e)
 			{
				//ACDebug.LogWarning ("Unable to save file '" + fullFileName + "'. Exception: " + e);
				return false;
 			}
			return true;
		}

		#endif


		public static string LoadFile (string fullFilename, bool doLog = true)
		{
			string _data = "";
			
			if (File.Exists (fullFilename))
			{
				StreamReader r = File.OpenText (fullFilename);

				string _info = r.ReadToEnd ();
				r.Close ();
				_data = _info;
			}
			
			if (_data != "" && doLog)
			{
				//ACDebug.Log ("File Read: " + fullFilename);
			}
			return (_data);
		}



	}
