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
    private Village village;

    public void Adapt ()
    {
        village = gameObject.GetComponentInParent(typeof (Village)) as Village;

        gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        gameObject.transform.localScale = new Vector3(1, 1, 1);

        if (village.faction != null)
        {
            if (village.size == Village.Size.CAMP)
            {
                if (village.faction == Village.Faction.ENEMY)
                {
                    renderer.sprite = campAlien;
                }
                else if (village.faction == Village.Faction.FRIENDLY)
                {
                    renderer.sprite = campLegionnaire;
                }
            }
            else if (village.size == Village.Size.VILLAGE)
            {
                if (village.faction == Village.Faction.ENEMY)
                {
                    renderer.sprite = villageAlien;
                }
                else if (village.faction == Village.Faction.FRIENDLY)
                {
                    renderer.sprite = villageLegionnaire;
                }
            }
            else if (village.size == Village.Size.CASTLE)
            {
                if (village.faction == Village.Faction.ENEMY)
                {
                    renderer.sprite = castleAlien;
                }
                else if (village.faction == Village.Faction.FRIENDLY)
                {
                    renderer.sprite = castleLegionnaire;
                }
            }
        }
    }
}
