using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class People : MonoBehaviour
{
    const float TAU = Mathf.PI * 2.0f;


    [SerializeField]
    public float Speed = 1;
    [SerializeField]
    public float SenseDistance = 1;
    [SerializeField]
    public float Size = 1;
    [SerializeField]
    public float Aggresivness = 1;

    [SerializeField]
    private float Energy = 100, reproduceCost = 200;

    [SerializeField]
    World world;

    public World World
    {
        get { return world; }
        set { world = value; }
    }

    private float MovementCost = 0;
    private NavMeshAgent agent;

    GameObject FoodTarget = null;

    Vector3 homePosition;

    Material material;

    bool HasMoved = false;
    int kids = 0;

    private void Start()
    {
        this.agent = this.gameObject.GetComponent<NavMeshAgent>();
        homePosition = this.transform.position;
        material = this.gameObject.GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        if (world.IsDay)
        {
            Move();
            kids = 1;
            HasMoved = true;
        }
        else
            GoHome();

        if (this.Energy <= 0.0f)
            world.Destroy(this);



        material.color = new Color(Aggresivness / 10.0f, Speed / 10.0f, SenseDistance / 10.0f);
        transform.localScale = new Vector3(Size, Size, Size);
    }

    private void GoHome()
    {
        this.FoodTarget = null;

        NavMeshPath path = new NavMeshPath();
        if (this.agent.CalculatePath(homePosition, path))
        {
            this.agent.SetPath(path);
        }

        if (this.agent.remainingDistance > 1)
            return;
        if (this.Energy > reproduceCost * kids && HasMoved)
        {
            world.SpawnOnePeople(Speed, SenseDistance, Size, Aggresivness);
            this.Energy -= reproduceCost * kids;
            kids++;
        }
        

    }

    public bool IsHome()
    {
        return Vector3.Distance(transform.position, homePosition) < 1;
    }

    public void Move()
    {
        this.agent.speed = this.Speed;
        this.MovementCost = this.CalcMovementCost();

        if (this.agent.hasPath)
        {
            this.Energy -= this.MovementCost * Time.deltaTime;
        }

        if (this.agent.remainingDistance > 1)
            return;
        
        if (FoodTarget)
        {
            this.Eat();
        }

        Vector3 targetPosition;
        
        if ((FoodTarget = FindTarget()) != null)        
            targetPosition = FoodTarget.transform.position;        
        else
            targetPosition = RandomPosition();

        NavMeshPath path = new NavMeshPath();

        if (this.agent.CalculatePath(targetPosition, path))
        {
            this.agent.SetPath(path);
        }
    }

    void Eat()
    {
        if (FoodTarget.tag == "Player")
        {
            //People p = FoodTarget.GetComponent<People>();

            if (FoodTarget.transform.localScale.x * 1.2f < Size)
            {                
                world.Eat(FoodTarget.GetComponent<People>());
                Energy += 10000;
               
            }
        }
        else if (world.Eat(FoodTarget))
        {
            Energy += 100;
        }
    }

    Vector3 RandomPosition() // retuns a random position around the current position in the 2D plane between the x and z axel 
    {
        float x = Random.value * TAU, z = Random.value * TAU;
        float distance = SenseDistance * Random.value;
        if (distance < SenseDistance / 3.0f)
            distance = SenseDistance / 3.0f;
        Vector3 targetPosition = transform.position + new Vector3(Mathf.Cos(x) * distance, 0, Mathf.Sin(z) * distance);
        targetPosition.y = 1;
        return targetPosition;
    }
    GameObject FindTarget()
    {
        float distance = float.MaxValue;
        int index = -1;

        bool aggro = Random.value * 10.0f < this.Aggresivness;
        //aggro = false;

        float senseDist = SenseDistance - (Aggresivness * .5f) > 1.0f ? SenseDistance - (Aggresivness * .5f) : 1.0f;
        if (aggro)
        {
            for (int i = 0; i < world.peoples.Count; i++)
            {
                Vector2 targetPos = new Vector2(world.peoples[i].transform.position.x, world.peoples[i].transform.position.z);
                Vector2 peoplePos = new Vector2(transform.position.x, transform.position.z);
                float dist;
                if ((dist = Vector2.Distance(targetPos, peoplePos)) < senseDist)
                {
                    if (dist < distance)
                    {
                        distance = dist;
                        index = i;
                    }
                }
            }
        }
        if (index < 0 || !aggro || world.peoples[index].Size * 1.2f >= Size)
        {
            aggro = false;
            distance = float.MaxValue;
            index = -1;
            for (int i = 0; i < world.foods.Count; i++)
            {
                Vector2 foodPos = new Vector2(world.foods[i].transform.position.x, world.foods[i].transform.position.z);
                Vector2 peoplePos = new Vector2(transform.position.x, transform.position.z);
                float dist;
                if ((dist = Vector2.Distance(foodPos, peoplePos)) < senseDist)
                {
                    if (dist < distance)
                    {
                        distance = dist;
                        index = i;
                    }
                }
            }
        }



        if (index > 0)
        {
            if (aggro)
                return world.peoples[index].gameObject;
            else
                return world.foods[index];
        }
        else
            return null;
    }
    float CalcMovementCost()
    {
        return Mathf.Pow(Size, 2.2f) * Mathf.Pow(Speed, 1.2f) + SenseDistance - (Aggresivness * 0.1f);
    }

    public void SetAttributes(float speed, float sense, float size, float aggresiveness)
    {
        if (speed > 1)
            this.Speed = speed;
        else
            this.Speed = 1;

        if (sense > 1)
            this.SenseDistance = sense;
        else
            this.SenseDistance = 1;

        if (size > 0.5f)
            this.Size = size;
        else
            this.Size = 0.5f;

        this.Energy = this.Size * Energy;
        if (this.Energy > this.reproduceCost)
            this.Energy = this.reproduceCost - 1;

        if (aggresiveness > 0)
            this.Aggresivness = aggresiveness;
        else
            this.Aggresivness = 0;

        //if (Aggresivness > 10)
        //    Aggresivness = 10;
    }
}
