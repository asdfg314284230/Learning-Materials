using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditLoadCheck{
    static Dictionary<string, bool> spriteDic; 

    static public Dictionary<string, bool> GetSpriteDic()
    {
        if (null == spriteDic)
        {
            spriteDic = new Dictionary<string, bool>();
            spriteDic.Add("load", true);
            spriteDic.Add("sprite_load_level", true);
            spriteDic.Add("sprite_load_login", true);
            spriteDic.Add("sprite_load_smithy",true);
            spriteDic.Add("sprite_load_icon_item", true);
            spriteDic.Add("sprite_load_email", true);
            spriteDic.Add("sprite_load_shop", true);
            spriteDic.Add("sprite_load_adv_reward", true);
            spriteDic.Add("sprite_load_card", true);
        }

        return spriteDic;
    }

}
