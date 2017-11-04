using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPool : MonoBehaviour {
    
    #region Public members

    public static ObjectPool instance;

    [Header("Pool item settings")]
    public GameObject pooledObject;
    public int numberOfPooledObjects = 20;

    [Header("Pool mode")]
    [Tooltip("If this is set to true the numberOfPooledObjects can be extended when needed.")]
    public bool isExtendable = false;
    [Tooltip("If this is set to true the first pooled object will be deactivated and reused for the last call to the pool.")]
    public bool isOverridable = false;

    #endregion

    #region Private members

    private IList<GameObject> pool;
    private int lastOverridableObjectIndex = 0;  //this index is used when isOverridable = true to keep track of the last object that was reused.

    #endregion

    void Awake () {
        instance = this;

        if ((isExtendable == true) && (isOverridable == true))
        {
            throw new System.Exception("Only one of the two modes can be set to true!");
        }
	}

    void Start()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < numberOfPooledObjects; i++)
        {
            InstantiateNewPooledObject();
        }
    }

    public GameObject GetPooledObject()
    {
        GameObject result = pool.Where(i => !i.activeInHierarchy).FirstOrDefault();

        if(result == null)
        {
            if (isExtendable)
            {
                result = InstantiateNewPooledObject();
            }
            else if (isOverridable)
            {
                result = pool.ElementAt(lastOverridableObjectIndex);
                result.SetActive(false);

                if (lastOverridableObjectIndex == numberOfPooledObjects - 1)
                    lastOverridableObjectIndex = 0;
                else
                    lastOverridableObjectIndex++;
            }
        }
        
        return result;
    }

    private GameObject InstantiateNewPooledObject()
    {
        GameObject result = Instantiate(pooledObject, transform);
        result.SetActive(false);
        pool.Add(result);

        return result;
    }

    private void Update()
    {
        foreach (var item in pool)
        {
            if(item == null)
                Debug.Log("THere is a null Object in Pool!");
        }
    }
}
