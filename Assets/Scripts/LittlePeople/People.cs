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

    private void Start()
    {
        this.agent = this.gameObject.GetComponent<NavMeshAgent>();
        homePosition = this.transform.position;
    }

    private void Update()
    {
        if (world.IsDay)
            Move();
        else
            GoHome();

        if (this.Energy <= 0.0f)
            world.Destroy(this);
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
        if (this.Energy > reproduceCost)
        {
            world.SpawnOnePeople(Speed, SenseDistance, Size, Aggresivness);
            this.Energy -= reproduceCost;
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
        if (world.Eat(FoodTarget))
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
        return targetPosition;
    }
    GameObject FindTarget()
    {
        float distance = float.MaxValue;
        int index = -1;

        for (int i = 0; i < world.foods.Count; i++)
        {
            Vector2 foodPos = new Vector2(world.foods[i].transform.position.x, world.foods[i].transform.position.z);
            Vector2 peoplePos = new Vector2(transform.position.x, transform.position.z);
            float dist;
            if ((dist = Vector2.Distance(foodPos, peoplePos)) < SenseDistance)
            {
                if (dist < distance)
                {
                    distance = dist;
                    index = i;
                }
            }
        }

        if (index > 0)
            return world.foods[index];
        else
            return null;
    }
    float CalcMovementCost()
    {
        return Mathf.Pow(Size, 3.0f) * Mathf.Pow(Speed, 2.0f) + SenseDistance;
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

        if (size > 0)
            this.Size = size;
        else
            this.Size = 0;

        this.Aggresivness = aggresiveness;
    }
}
