using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    [SerializeField]
    Vector2 min = new Vector2(-50, -50), max = new Vector2(50, 50);

    [SerializeField]
    uint FoodSize = 100;
    uint MaxFood = 1000;

    [SerializeField]
    private GameObject Food;
    public List<GameObject> foods;


    [SerializeField]
    private GameObject people;
    [SerializeField]
    uint basePeople = 10;
    [SerializeField]
    float baseSpeed = 1, baseSense = 1, baseSize = 1, baseAggresivness = 1;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    float deviation = 0.1f;

    [SerializeField]
    bool day, spawnFood = false;

    [SerializeField]
    float DayTime = 30.0f;
    float dayTimer = 0;
    float homeTimer = 0;

    [SerializeField]
    public List<People> peoples;

    [SerializeField]
    [Range(1.0f, 10.0f)]
    private float TimeScale = 1.0f;

    public Slider TimeScaleSlider;
    public Slider FoodSlider;
    public Slider Deviation;

    public bool IsDay
    {
        get { return day; }
    }

    public uint CurrentDay = 0;
    public uint DeathCount = 0;
    public uint NomCount = 0;

    private void Start()
    {
        SpawnPeople();
        SpawnFood();
        FoodSlider.value = (float)FoodSize / (float)MaxFood;
    }
    private void Update()
    {
        Time.timeScale = TimeScale;
        dayTimer += Time.deltaTime;
        this.day = dayTimer < DayTime;

        TimeScale = this.TimeScaleSlider.value * 9 + 1;
        FoodSize = (uint)(this.FoodSlider.value * MaxFood + 1);
        deviation = Deviation.value * 9 + 1;

        if (dayTimer >= DayTime)
        {
            bool allHome = true;
            for (int i = 0; i < peoples.Count && allHome; i++)
            {
                allHome = peoples[i].IsHome();
            }

            homeTimer += Time.deltaTime;

            if (allHome || homeTimer > 5.0f)
            {
                SpawnFood();
                CurrentDay++;
                dayTimer = 0;
                homeTimer = 0;
            }
        }

        if (peoples.Count <= 0)
            SpawnOnePeople(baseSpeed, baseSense, baseSize, baseAggresivness);

        if (spawnFood)
        {
            SpawnFood();
        }
    }
    void SpawnPeople()
    {
        for (int i = 0; i < basePeople; i++)
        {
            Vector2 position = new Vector2();
            if (Random.Range(0.0f, 1.0f) > 0.5f)
            {
                position.x = Mathf.Sign(Random.value - 0.5f);
                position *= max;
                position.y = (Random.value - 0.5f) * (max.x * 2.0f);

            }
            else
            {
                position.y = Mathf.Sign(Random.value - 0.5f);
                position *= max;
                position.x = (Random.value - 0.5f) * (max.x * 2.0f);
            }
            GameObject instance = Instantiate(people, new Vector3(position.x, 1, position.y), Quaternion.identity);
            People p = instance.GetComponent<People>();
            p.World = this;
            p.SetAttributes(baseSpeed, baseSense, baseSize, baseAggresivness);
            peoples.Add(p);
        }
    }

    public void SpawnOnePeople(float speed, float sense, float size, float aggresiveness)
    {
        Vector2 position = new Vector2();
        if (Random.Range(0.0f, 1.0f) > 0.5f)
        {
            position.x = Mathf.Sign(Random.value - 0.5f);
            position *= max;
            position.y = (Random.value - 0.5f) * (max.x * 2.0f);

        }
        else
        {
            position.y = Mathf.Sign(Random.value - 0.5f);
            position *= max;
            position.x = (Random.value - 0.5f) * (max.x * 2.0f);
        }
        GameObject instance = Instantiate(people, new Vector3(position.x, 1, position.y), Quaternion.identity);
        People p = instance.GetComponent<People>();
        p.World = this;
        p.SetAttributes(speed + Random.Range(-deviation, deviation), 
            sense + Random.Range(-deviation, deviation), 
            size + Random.Range(-deviation, deviation),
            aggresiveness + Random.Range(-deviation, deviation));
        peoples.Add(p);

    }

    public void Destroy(People p)
    {
        peoples.Remove(p);
        Destroy(p.gameObject);
        DeathCount++;
    }
    public void Eat(People p)
    {
        peoples.Remove(p);
        Destroy(p.gameObject);
        NomCount++;
    }

    void SpawnFood()
    {
        while (foods.Count > 1)
        {
            Destroy(foods[0]);
            foods.RemoveAt(0);
        }
        for (int i = 0; i < FoodSize; i++)
        {
            Vector3 position = new Vector3(Random.Range(min.x, max.x), 1, Random.Range(min.y, max.y));

            GameObject go = Instantiate(Food, position, Quaternion.identity, transform);
            go.hideFlags = HideFlags.HideInHierarchy;
            go.transform.localScale = new Vector3(1 / transform.localScale.x , 1 / transform.localScale.y, 1 / transform.localScale.z);
            foods.Add(go);

        }
    }

    public bool Eat(GameObject food)
    {
        if (foods.Remove(food))
        {
            Destroy(food);
            return true;
        }
        return false;

    }
    
}
