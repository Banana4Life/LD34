using UnityEngine;
using System.Collections;

public class Village_Taken : MonoBehaviour
{
    public SpriteRenderer renderer;
    public Sprite campAlien;
    public Sprite campLegionnaire;
    public Sprite villageAlien;
    public Sprite villageLegionnaire;
    public Sprite castleAlien;
    public Sprite castleLegionnaire;

    public void Adapt(Size s, Faction f)
    {
        gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        
        if (s == Size.CAMP)
        {
            if (f == Faction.ENEMY)
            {
                renderer.sprite = campAlien;
            }
            else if (f == Faction.FRIENDLY)
            {
                renderer.sprite = campLegionnaire;
            }
        }
        else if (s == Size.VILLAGE)
        {
            if (f == Faction.ENEMY)
            {
                renderer.sprite = villageAlien;
            }
            else if (f == Faction.FRIENDLY)
            {
                renderer.sprite = villageLegionnaire;
            }
        }
        else if (s == Size.CASTLE)
        {
            if (f == Faction.ENEMY)
            {
                renderer.sprite = castleAlien;
            }
            else if (f == Faction.FRIENDLY)
            {
                renderer.sprite = castleLegionnaire;
            }
        }
    }
}
