using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomLevelGenerator : MonoBehaviour {

    [SerializeField]
    [Tooltip("x")]
    int numOfLargePods = 3;

    [Tooltip("y")]
    [SerializeField]
    int largeGridSize = 4;

    [Tooltip("z")]
    [SerializeField]
    int numOfSmallPods = 10;

    int smallerGridSize;

    [SerializeField]
    GameObject testPodBlank;

    [SerializeField]
    GameObject testPodSmall;

    [SerializeField]
    GameObject testPod3xFill;

    [SerializeField]
    GameObject testPod3xSpawnPoint;

    [SerializeField]
    GameObject test3xPod;

    [SerializeField]
    Transform podParent;

    [SerializeField]
    GameObject doorObject;
    [SerializeField]
    GameObject filledObject;
    [SerializeField]
    GameObject largeSpawnObject;
    
    struct Location
    {
       public int locY;
       public int locX;

        public override string ToString()
        {
            return "Y location is " + locY + " and X location is " + locX;
        }
    }


	// Use this for initialization
	void Start () { 
        #region Making sure the serialized Variables meet the requirements
        if(largeGridSize <= numOfLargePods)
        {
            largeGridSize = numOfLargePods + 1;
        }
        if(numOfSmallPods < numOfLargePods)
        {
            numOfSmallPods = numOfLargePods + 1;
        }
        if(numOfSmallPods >= largeGridSize * largeGridSize)
        {
            numOfSmallPods = (largeGridSize * largeGridSize) - 2;
        }

        smallerGridSize = 3 * largeGridSize;
#endregion
        #region Declaring various arrays to be used
        bool[,] largePodsSpawned_Array = new bool[largeGridSize, largeGridSize];
        int[,] smallPodsSpawned_Array = new int[smallerGridSize, smallerGridSize];
        int[] largeYPos, largeXPos;
        largeYPos = new int[numOfLargePods]; 
        largeXPos = new int[numOfLargePods];
#endregion
        #region Randomly Adding Large Pods to Large Grid
        for(int i = 0; i < numOfLargePods; i++)
        {
            int tempY;
            int tempX;

            tempY = Random.Range(0, largeGridSize);
            tempX = Random.Range(0, largeGridSize);
            while(largePodsSpawned_Array[tempY, tempX])
            {
                tempY = Random.Range(0, largeGridSize);
                tempX = Random.Range(0, largeGridSize);
            }
            largePodsSpawned_Array[tempY, tempX] = true;
            largeYPos[i] = tempY;
            largeXPos[i] = tempX;
        }
#endregion
        #region Filling smallPodsSpawned_Array with "empty" slots
        for(int i = 0; i < smallerGridSize; i++)
        {
            for (int j = 0; j < smallerGridSize; j++)
            {
                smallPodsSpawned_Array[i, j] = 0;
            }
        }
#endregion
        #region Converting Large pods to Small Grid locations
        for(int i = 0; i < numOfLargePods; i++)
        {
            int tempY = largeYPos[i];
            int tempX = largeXPos[i];

            smallPodsSpawned_Array[((tempY + 1) * 3) - 3, ((tempX + 1) * 3) - 3] = 2;
            smallPodsSpawned_Array[((tempY + 1) * 3) - 3, ((tempX + 1) * 3) - 2] = 1;
            smallPodsSpawned_Array[((tempY + 1) * 3) - 3, ((tempX + 1) * 3) - 1] = 2;
            smallPodsSpawned_Array[((tempY + 1) * 3) - 2, ((tempX + 1) * 3) - 3] = 1;
            smallPodsSpawned_Array[((tempY + 1) * 3) - 2, ((tempX + 1) * 3) - 2] = 3;
            smallPodsSpawned_Array[((tempY + 1) * 3) - 2, ((tempX + 1) * 3) - 1] = 1;
            smallPodsSpawned_Array[((tempY + 1) * 3) - 1, ((tempX + 1) * 3) - 3] = 2;
            smallPodsSpawned_Array[((tempY + 1) * 3) - 1, ((tempX + 1) * 3) - 2] = 1;
            smallPodsSpawned_Array[((tempY + 1) * 3) - 1, ((tempX + 1) * 3) - 1] = 2;
        }
        #endregion
        #region Declaring Counter variables and a gameObject array to store pods
        int doorCounter, filledCounter, largeSpawnCounter, emptyCounter;
        doorCounter = filledCounter = largeSpawnCounter = emptyCounter = 0;
        GameObject[,] smallPodsGameObject_Array = new GameObject[smallerGridSize, smallerGridSize];
        #endregion
        #region Filling the gameObject array with doors, filled spaces, or the spawnLocation, and increasing the counters
        for(int i = 0; i < smallPodsSpawned_Array.GetLength(0); i++)
        {
            for(int j = 0; j < smallPodsSpawned_Array.GetLength(1); j++)
            {
                switch (smallPodsSpawned_Array[i, j])
                {
                    case 0:
                        smallPodsGameObject_Array[i, j] = null;
                        emptyCounter++;
                        break;
                    case 1:
                        smallPodsGameObject_Array[i, j] = doorObject;
                        doorCounter++;
                        break;
                    case 2:
                        smallPodsGameObject_Array[i, j] = filledObject;
                        filledCounter++;
                        break;
                    case 3:
                        smallPodsGameObject_Array[i, j] = largeSpawnObject;
                        largeSpawnCounter++;
                        break;
                }
            }
        }
        #endregion
        #region Randomly Adding small pods to the small grid, avoiding filled spots
                for(int i = 0; i < numOfSmallPods; i++)
                {
                    int tempY, tempX;

                    tempY = Random.Range(0, smallerGridSize);
                    tempX = Random.Range(0, smallerGridSize);
                    while(smallPodsSpawned_Array[tempY, tempX] != 0)
                    {
                        tempY = Random.Range(0, smallerGridSize);
                        tempX = Random.Range(0, smallerGridSize);
                    }
                    smallPodsSpawned_Array[tempY, tempX] = 1;
                    smallPodsGameObject_Array[tempY, tempX] = doorObject;
                    doorCounter++;
                }
        #endregion

        Location middleLocation = new Location();

        float minDistance_Float = 10000f;

        int totalNumOfPodsAndDoors = numOfSmallPods + (numOfLargePods * 4);
        Location[] smallPodsLocations = new Location[totalNumOfPodsAndDoors];

        bool[,] isSpaceConnected = new bool[smallerGridSize, smallerGridSize];

        #region Filling an array of Locations with all of the spawned Pods' locations or doors
        int locationCounter = 0;
        for(int i = 0; i < smallerGridSize; i++)
        {
            for(int j = 0; j < smallerGridSize; j++)
            {
                if(smallPodsSpawned_Array[j, i] == 1)
                {
                    smallPodsLocations[locationCounter].locY = j;
                    smallPodsLocations[locationCounter].locX = i;
                    //Debug.Log("Pod " + (locationCounter + 1) + " location: " + smallPodsLocations[locationCounter].ToString());
                    locationCounter++;                    
                }

            }
        }
#endregion


        for(int i = 0; i < smallPodsLocations.Length; i++)
        {
            float tempDistance = Mathf.Sqrt(((((smallerGridSize / 2)) - smallPodsLocations[i].locX) * (((smallerGridSize / 2)) - smallPodsLocations[i].locX)) + ((((smallerGridSize / 2)) - smallPodsLocations[i].locY) * (((smallerGridSize / 2)) - smallPodsLocations[i].locY)));
            if(tempDistance < minDistance_Float)
            {
                
                minDistance_Float = tempDistance;
                middleLocation.locX = smallPodsLocations[i].locX;
                middleLocation.locY = smallPodsLocations[i].locY;
            }
        }
        Debug.Log("The size of the grid is " + smallerGridSize + "x" + smallerGridSize + ". The middle of the grid is " + (smallerGridSize / 2) + ", " + (smallerGridSize / 2) + ". The closest pod to the middle: " + middleLocation.ToString() + ". And it's distance from the center is " + minDistance_Float);

        //#region Connecting Close Pods together to form a more coherent level
        //int connectionCounter = 0;
        //for (int connectorCounter = 0; connectorCounter <= 1; connectorCounter++)
        //{
        //    Debug.Log(connectorCounter + " time running the connector");
        //    Debug.Log("Number of connections made: " + connectionCounter);
        //    for (int i = 0; i < smallerGridSize; i++)
        //    {
        //        for (int j = 0; j < smallerGridSize; j++)
        //        {
        //            if (smallPodsSpawned_Array[j, i] == 1)
        //            {
        //                if ((j + 2 < smallerGridSize) && smallPodsSpawned_Array[j + 2, i] == 1 && smallPodsSpawned_Array[j + 1, i] == 0)
        //                {
        //                    smallPodsSpawned_Array[j + 1, i] = 1;
        //                    connectionCounter++;
        //                }
        //                if ((j - 2 > 0) && smallPodsSpawned_Array[j - 2, i] == 1 && smallPodsSpawned_Array[j - 1, i] == 0)
        //                {
        //                    smallPodsSpawned_Array[j - 1, i] = 1;
        //                    connectionCounter++;
        //                }
        //                if ((j + 1 < smallerGridSize && i + 1 < smallerGridSize) && smallPodsSpawned_Array[j + 1, i + 1] == 1 && (smallPodsSpawned_Array[j, i + 1] == 0 || smallPodsSpawned_Array[j + 1, i] == 0))
        //                {
        //                    if (smallPodsSpawned_Array[j, i + 1] == 0)
        //                        smallPodsSpawned_Array[j, i + 1] = 1;
        //                    else
        //                        smallPodsSpawned_Array[j + 1, i] = 1;
        //                    connectionCounter++;

        //                }
        //                if ((j - 1 > 0 && i - 1 > 0) && smallPodsSpawned_Array[j - 1, i - 1] == 1 && (smallPodsSpawned_Array[j, i - 1] == 0 || smallPodsSpawned_Array[j - 1, i] == 0))
        //                {
        //                    if (smallPodsSpawned_Array[j, i - 1] == 0)
        //                        smallPodsSpawned_Array[j, i - 1] = 1;
        //                    else
        //                        smallPodsSpawned_Array[j - 1, i] = 1;
        //                    connectionCounter++;
        //                }
        //                if ((i + 2 < smallerGridSize) && smallPodsSpawned_Array[j, i + 2] == 1 && smallPodsSpawned_Array[j, i + 1] == 0)
        //                {
        //                    smallPodsSpawned_Array[j, i + 1] = 1;
        //                    connectionCounter++;
        //                }
        //                if ((i - 2 >= 0) && smallPodsSpawned_Array[j, i - 2] == 1 && smallPodsSpawned_Array[j, i - 1] == 0)
        //                {
        //                    smallPodsSpawned_Array[j, i - 1] = 1;
        //                    connectionCounter++;
        //                }
        //            }
        //        }
        //    }
        //}
        //#endregion

        Location lastLocation = new Location();
        Vector3[,] lastPostion = new Vector3[smallerGridSize, smallerGridSize];
        GameObject[,] spawnedPods = new GameObject[smallerGridSize, smallerGridSize];
        for(int i = 0; i < smallerGridSize; i++)
        {
            for(int j = 0; j < smallerGridSize; j++)
            {
                if(j > 0)
                { 
                    lastLocation.locY = j - 1;
                }
                else
                {
                    lastLocation.locY = j;
                }
                GameObject tempObject;
                switch (smallPodsSpawned_Array[j, i])
                {
                    case 0:
                        tempObject = Instantiate(testPodBlank, podParent.transform.position, podParent.transform.rotation, podParent);
                       
                            break;
                    case 1:
                        tempObject = Instantiate(testPodSmall, podParent.transform.position, podParent.transform.rotation, podParent);
                        break;
                    case 2:
                        tempObject = Instantiate(testPod3xFill, podParent.transform.position, podParent.transform.rotation, podParent);
                        
                        break;
                    case 3:
                        tempObject = Instantiate(testPod3xSpawnPoint, podParent.transform.position, podParent.transform.rotation, podParent);
                        
                        break;
                    default:
                        tempObject = Instantiate(testPodBlank, podParent.transform.position, podParent.transform.rotation, podParent);
                        break;
                }
                spawnedPods[j, i] = tempObject;

                if (i == 0 && j == 0)
                {
                    tempObject.transform.position = podParent.transform.position;
                }
                    
                if(i == 0 && j == 0)
                {
                        tempObject.transform.Translate(0, 0, -10.05f);
                }
                else if(j == 0 && i > 0)
                {
                tempObject.transform.position = lastPostion[j, i - 1];
                tempObject.transform.Translate(10.05f, 0, 0);
                }
                else if(i == 0 && j > 0 && j != smallerGridSize -1)
                {
                    tempObject.transform.position = lastPostion[j - 1, i];
                    tempObject.transform.Translate(0, 0, -10.05f);
                }
                else if (i > 0 && j > 0 && j != smallerGridSize - 1)
                {
                    tempObject.transform.position = lastPostion[j - 1, i];
                    tempObject.transform.Translate(0, 0, -10.05f);
                }
                else if(j == smallerGridSize - 1)
                {
                    tempObject.transform.position = lastPostion[j - 1, i];
                    tempObject.transform.Translate(0, 0, -10.05f);
                }

                lastPostion[j, i] = tempObject.transform.position;
                
                
            }

            lastLocation.locX = i - 1;
        }

        for(int i = 0; i < smallerGridSize; i++)
        {
            for(int j = 0; j < smallerGridSize; j++)
            {
                if(smallPodsSpawned_Array[j,i] == 3)
                {
                    GameObject tempObject =  Instantiate(test3xPod, spawnedPods[j, i].transform.position, spawnedPods[j, i].transform.rotation, podParent);
                    Destroy(spawnedPods[j, i]);
                    Destroy(spawnedPods[j, i + 1]);
                    Destroy(spawnedPods[j, i - 1]);
                    Destroy(spawnedPods[j + 1, i]);
                    Destroy(spawnedPods[j - 1, i]);
                    Destroy(spawnedPods[j + 1, i + 1]);
                    Destroy(spawnedPods[j - 1, i - 1]);
                    Destroy(spawnedPods[j + 1, i - 1]);
                    Destroy(spawnedPods[j - 1, i + 1]);
                    spawnedPods[j, i] = tempObject;
                    
                }
                if(smallPodsSpawned_Array[j,i] == 0)
                {
                    Destroy(spawnedPods[j, i]);
                }
            }
        }
        
        //Cycling through the grid, and activating doors if there is an adjacent pod
        for(int i = 0; i < smallerGridSize; i++)
        {
            for(int j = 0; j < smallerGridSize; j++)
            {
                if(smallPodsSpawned_Array[j,i] == 1)
                {
                    if (j + 1 < smallerGridSize && smallPodsSpawned_Array[j + 1, i] == 1)
                    {
                        if(smallPodsSpawned_Array[j+1,i + 1] == 2)
                        {
                            if (spawnedPods[j + 2, i].GetComponent<PodProperties>())
                                spawnedPods[j + 2, i].GetComponent<PodProperties>().ActivateTopDoor();
                        }
                        else
                            spawnedPods[j + 1, i].GetComponent<PodProperties>().ActivateTopDoor();
                    }
                    if (j - 1 >= 0 && smallPodsSpawned_Array[j-1, i] == 1)
                    {
                        if(smallPodsSpawned_Array[j-1, i+1] == 2)
                        {
                            if(spawnedPods[j-2,i].GetComponent<PodProperties>())
                                spawnedPods[j - 2, i].GetComponent<PodProperties>().ActivateBottomDoor();
                        }
                        else
                            spawnedPods[j - 1, i].GetComponent<PodProperties>().ActivateBottomDoor();
                    }
                    if ((i + 1 < smallerGridSize && j + 1 < smallerGridSize) &&  smallPodsSpawned_Array[j,i+1] == 1)
                    {
                        if (smallPodsSpawned_Array[j + 1, i + 1] == 2)
                        {
                            if (spawnedPods[j, i + 2].GetComponent<PodProperties>())
                                spawnedPods[j, i + 2].GetComponent<PodProperties>().ActivateLeftDoor();
                        }
                        else
                            spawnedPods[j, i + 1].GetComponent<PodProperties>().ActivateLeftDoor();
                    }
                    if(i - 1 >= 0 && smallPodsSpawned_Array[j, i - 1] == 1)
                    {
                        if(i - 2 >= 0 && smallPodsSpawned_Array[j + 1, i - 1] == 2)
                        {
                            if (spawnedPods[j, i - 2].GetComponent<PodProperties>())
                                spawnedPods[j, i - 2].GetComponent<PodProperties>().ActivateRightDoor();
                        }
                        else
                            spawnedPods[j, i - 1].GetComponent<PodProperties>().ActivateRightDoor();
                    }
                }
            }
        }
        //Location tempLocation = new Location();
        //bool foundFirstPod = false; ;
        //for(int i = 0; i < smallerGridSize; i++)
        //{

        //   for(int j = 0; j < smallerGridSize; j++)
        //    {
        //        if(smallPodsSpawned_Array[j,i] == 1)
        //        {
        //            if (!foundFirstPod)
        //            {
                        
        //                tempLocation.locY = j;
        //                tempLocation.locX = i;
        //                foundFirstPod = true;
        //            }
        //        }

        //    } 
        //}
        //spawnedPods[tempLocation.locY, tempLocation.locX].GetComponentInChildren<SpriteRenderer>().color = new Color(255, 255, 0);

        //Debug.Log("Number of empty spaces: " + emptyCounter);
        //Debug.Log("Number of Door spaces: " + doorCounter);
        //Debug.Log("Number of filled spaces: " + filledCounter);
        //Debug.Log("Number of spawn spaces: " + largeSpawnCounter);

    }
	
    List<Location> FindMostEfficientPath(int[,] gridToSearch, int startingLocationY, int startingLocationX, int nextLocationY, int nextLocationX)
    {
        List<Location> spotsAlongPath = new List<Location>();
        #region Filling isObstacle with spots from 3x pods that don't contain doors i.e filledObjects and spawnPointObjects
        bool[,] isObstacle = new bool[smallerGridSize, smallerGridSize];
        for(int i = 0; i < smallerGridSize; i++)
        {
            for(int j = 0; j < smallerGridSize; j++)
            {
                if(gridToSearch[i,j] > 1)
                {
                    isObstacle[i, j] = true;
                }
            }
        }
        #endregion

        int yDistance = startingLocationY - nextLocationY;
        int xDistance = startingLocationX - nextLocationX;

        bool isAbove = yDistance < 0;
        bool isLeft = xDistance < 0;

        //If yDistance is positive, then the starting location is below the next location
        //If xDistance is positive, then the starging location is to the right of the next location 

        if(yDistance == 0)
        {
            for(int i = 0; i < Mathf.Abs(xDistance); i++)
            {

            }
        }
        

        return spotsAlongPath;
    }
}
