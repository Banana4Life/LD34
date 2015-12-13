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
        float maxRadius = Village.MIN_RADIUS + Village.RADIUS_STEP_SCALE * (Village.RADIUS_STEPS - 1);

        if (village.faction != null)
        {
            if (village.faction == Village.Faction.ENEMY)
            {
                if (Mathf.Abs(village.radius - maxRadius) <= 0.01)
                {
                    renderer.sprite = castleAlien;
                }
            }
            else if (village.faction == Village.Faction.FRIENDLY)
            {
                if (Mathf.Abs(village.radius - maxRadius) <= 0.01)
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
