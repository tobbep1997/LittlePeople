using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistic : MonoBehaviour
{
    World world;

    public Text text;

    [SerializeField]
    float AverageSpeed = 0, AverageSense = 0, AverageSize = 0, AverageAggression = 0;

    private void Start()
    {
        this.world = this.gameObject.GetComponent<World>();
    }

    private void Update()
    {
        float sp = 0, se = 0, si = 0, agg = 0;


       
        for (int i = 0; i < world.peoples.Count; i++)
        {
            sp += world.peoples[i].Speed;
            se += world.peoples[i].SenseDistance;
            si += world.peoples[i].Size;
            agg += world.peoples[i].Aggresivness;
        }
        AverageSpeed = sp / world.peoples.Count;
        AverageSense = se / world.peoples.Count;
        AverageSize = si / world.peoples.Count;
        AverageAggression = agg / world.peoples.Count;

        text.text = "Generation : " + world.CurrentDay.ToString() +
            "\nDeath Count : " + world.DeathCount.ToString() +
            "\nTinyPeople : " + world.peoples.Count.ToString() +
            "\nFood : " + world.foods.Count.ToString() +
            "\nAverage Speed : " + AverageSpeed.ToString() + 
            "\nAverage Sense : " + AverageSense.ToString();

        
    }
}
