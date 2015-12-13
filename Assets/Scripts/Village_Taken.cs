using UnityEngine;
using System.Collections;

public class Village_Taken : MonoBehaviour
{
    public SpriteRenderer renderer;
    public Sprite castleAlien;
    public Sprite castleLegionnaire;
    private Village village;

    public void Adapt ()
    {
        village = gameObject.GetComponentInParent(typeof (Village)) as Village;

        if (village.faction != null)
        {
            if (village.faction == Village.Faction.ENEMY)
            {
                if (village.size == Village.Size.CASTLE)
                {
                    renderer.sprite = castleAlien;
                }
            }
            else if (village.faction == Village.Faction.FRIENDLY)
            {
                if (village.size == Village.Size.CASTLE)
                {
                    renderer.sprite = castleLegionnaire;
                }
            }
            else
            {
                renderer.sprite = null;
            }
        }
    }
}
