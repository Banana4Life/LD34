using UnityEngine;
using System.Collections;

public class Village_Taken : MonoBehaviour
{
    public SpriteRenderer renderer;
    public Sprite castleAlien;
    private Village village;

    public void Adapt ()
    {
        village = gameObject.GetComponentInParent(typeof (Village)) as Village;
        float maxRadius = village.minRadius + village.radiusStepScale * (village.radiusSteps - 1);
        Debug.Log(village.radius + " / " + maxRadius);
        if (village.faction == Village.Faction.ENEMY && Mathf.Abs(village.radius - maxRadius) <= 0.01)
        {
            renderer.sprite = castleAlien;
        }
        else
        {
            renderer.sprite = null;
        }
    }
}
