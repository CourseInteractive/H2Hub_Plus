using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using AC;

public class InventoryToVALUR : MonoBehaviour
{
    // Start is called before the first frame update
  /*  void Start()
    {
        VALUR.ConsoleTopic topic = new VALUR.ConsoleTopic();
        topic.token = "inventory";
        topic.name = "Inventar";
        VALUR.Data.IntroduceTopic(topic);
        VALUR.Data.AddFetchFctToTopic("inventory", FetchDataInv);
    }

    public void FetchDataInv()
    {

        InvItem[] items = AC.KickStarter.inventoryManager.GetItemsInCategory(0);
        foreach (InvItem item in items)
        {
            VALUR.Data.AddConsoleButton(item.label, AddToInvOrder, item.id.ToString(), new Vector2(80,80), item.iconAsSprite);
        }

    }

    public void AddToInvOrder(string data)
    {
        KickStarter.runtimeInventory.Add(int.Parse(data), 1, false, AC.KickStarter.player.ID, false);
    }

    Sprite GetSpriteFromTex(Texture tex_)
    {
        Texture2D tex = tex_ as Texture2D;
        return Sprite.Create(tex, new Rect(0, 0,  tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
  */
}
