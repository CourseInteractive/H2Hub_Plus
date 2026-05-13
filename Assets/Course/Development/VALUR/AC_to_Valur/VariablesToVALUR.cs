using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using AC;

public class VariablesToVALUR : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		VALUR.ConsoleTopic GVarTopic = new VALUR.ConsoleTopic();
		GVarTopic.token = "gVars";
		GVarTopic.name = "Global Variables";
		VALUR.Data.IntroduceTopic(GVarTopic);
		VALUR.Data.AddFetchFctToTopic("gVars", FetchDataGVAR);

		VALUR.ConsoleTopic LVarTopic = new VALUR.ConsoleTopic();
		LVarTopic.token = "lVars";
		LVarTopic.name = "Local Variables";
		VALUR.Data.IntroduceTopic(LVarTopic);
		VALUR.Data.AddFetchFctToTopic("lVars", FetchDataLVAR);
	}

	public void FetchDataGVAR()
	{

		/*List<GVar> gVar = AC.GlobalVariables.GetAllVars();
		foreach (GVar variable in gVar)
		{
			ShowVariableInfo(variable);
		}*/

	}


	public void FetchDataLVAR()
	{

		VALUR.Data.AddConsoleInfo("Active Scene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
		//List<GVar> lVars = AC.LocalVariables.GetAllVars();
		/*foreach (GVar variable in lVars)
		{
			ShowVariableInfo(variable);
		}*/

	}

	/*void ShowVariableInfo(GVar variable)
    {
		switch (variable.type)
		{
			case VariableType.String:
				VALUR.Data.AddConsoleInfo(variable.label, variable.TextValue);
				break;
			case VariableType.Boolean:
				VALUR.Data.AddConsoleInfo(variable.label, variable.BooleanValue.ToString());
				break;
			case VariableType.Float:
				VALUR.Data.AddConsoleInfo(variable.label, variable.floatVal.ToString());
				break;
			case VariableType.Integer:
				VALUR.Data.AddConsoleInfo(variable.label, variable.IntegerValue.ToString());
				break;
			case VariableType.Vector3:
				VALUR.Data.AddConsoleInfo(variable.label, variable.vector3Val.ToString());
				break;
			case VariableType.PopUp:
				VALUR.Data.AddConsoleInfo(variable.label, variable.popUps[variable.val]);
				break;
		}
	}*/

}
