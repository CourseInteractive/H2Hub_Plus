using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*****
 * VALUR - PanelSizeFromChildren ## Version 1
 * Last Change: 01.11.2015
 * By: Oli
 * 
 * Passt die Größe eines RectTransforms in der Höhe an die summierte Größe seiner Kinder an
 * benötigt in jedem Kind ein "LayOut-Element"
 *****/

public class PanelSizeFromChildren : MonoBehaviour {


	private float height = 0;


	// Use this for initialization
	public void Adjust () 
	{


		height = 0;
		for(int i = 0; i < transform.childCount; i++)
		{
            //height += transform.GetChild(i).GetComponent<LayoutElement>().minHeight;
            RectTransform tr = transform.GetChild(i).GetComponent<RectTransform>();
            if (!tr.gameObject.activeSelf)
                continue;
            height += tr.rect.height;
        }

		GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
	}
	

}
