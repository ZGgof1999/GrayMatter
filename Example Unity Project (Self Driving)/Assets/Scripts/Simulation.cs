using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscreteSignals.GrayMatter;

public class Simulation : MonoBehaviour
{
    public GameObject spawnPoint;
    public DriverAI carPrefab;
    public int numCars;
    public int numWithTopFitness;
    public int numRandom;
    public float mutationRate;
    public float maxMutation;
    public float nextBatchTime;
    public float time = 0.0f;
    public bool nextBatchOverride = false;

    private List<DriverAI> cars;
    // Start is called before the first frame update
    void Start()
    {
        cars = new List<DriverAI>();
        for (int car = 0; car < numCars; car++)
        {
            cars.Add(Instantiate(carPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation));
            cars[cars.Count - 1].Initialize();
        }
        foreach (DriverAI c in cars) c.halted = false;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > nextBatchTime)nextBatchOverride = true;
        if(nextBatchOverride || !StillDriving())
        {
            time = 0.0f;
            nextBatchOverride = false;

            List<DriverAI> topCars = GetTopCars(numWithTopFitness);
            int car = 0;
            while (car < numCars - (numWithTopFitness + numRandom))
            {
                for (int i = 0; i < topCars.Count; i++)
                {
                    if (car == numCars - (numWithTopFitness + numRandom)) break;
                    Destroy(cars[car].gameObject);
                    cars[car] = Instantiate(carPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                    
                    cars[car].Initialize();
                    
                    cars[car].sight = new Sense(cars[car].eyes);
                    cars[car].motor = new Reaction(cars[car].gears);
                    cars[car].brain = new Brain(topCars[i].brain, cars[car].sight, cars[car].motor, mutationRate, maxMutation);
                    
                    car++;
                }
            }

            for (int i = 0; i < topCars.Count; i++)
            {
                Destroy(cars[car].gameObject);
                cars[car] = Instantiate(carPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                    
                cars[car].Initialize();
                    
                cars[car].sight = new Sense(cars[car].eyes);
                cars[car].motor = new Reaction(cars[car].gears);
                cars[car].brain = new Brain(topCars[i].brain, cars[car].sight, cars[car].motor, 0.0f);
                    
                car++;
            }

            for (int i = 0; i < numRandom; i++)
            {
                Destroy(cars[car].gameObject);
                cars[car] = Instantiate(carPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                    
                cars[car].Initialize();
                    
                car++;
            }

            for (int i = 0; i < topCars.Count; i++) Destroy(topCars[i].gameObject);
            foreach (DriverAI c in cars) c.halted = false;
        }
    }
    List<DriverAI> GetTopCars(int num)
    {
        List<DriverAI> topCars = new List<DriverAI>();

        while (topCars.Count < numWithTopFitness)
        {
            float maxFitness = 0.0f;
            int index = 0;
            for (int car = 0; car < cars.Count; car++)
            {
                if (cars[car].fitness >= maxFitness) 
                {
                    index = car;
                    maxFitness = cars[car].fitness;
                }
            }
            topCars.Add(cars[index]);
            Debug.Log("TopCar Fitness: " + cars[index].fitness);
            cars[index].fitness = -1;
        }

        return topCars;
    }

    bool StillDriving()
    {
        foreach (DriverAI car in cars) if (!car.halted) return true;
        return false;
    }
}
