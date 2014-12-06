using UnityEngine;
using System.Collections;

public class FireWorldEffects : MonoBehaviour {

    private PlayerStats thePlayer;
    private bool _isAlive;

    public float startWait;
    public float fireSpawnRate;
    public Vector3 spawnValues;
    public GameObject FireBurstHazard;

	// Use this for initialization
	void Start () 
    {
        thePlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        StartCoroutine(SpawnFireEvent());
	}

    IEnumerator SpawnFireEvent()
    {
        yield return new WaitForSeconds(startWait);
        _isAlive = true;

        while (_isAlive)
        {
            SpawnFireBursts();

            yield return new WaitForSeconds(fireSpawnRate);
            if (!thePlayer)
            {
                _isAlive = false;
            }

            if (!_isAlive)
            {
                break;
            }
        }
    }

    private void SpawnFireBursts()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, Random.Range(-spawnValues.z, spawnValues.z));
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(FireBurstHazard, spawnPosition, spawnRotation);
    }

}
