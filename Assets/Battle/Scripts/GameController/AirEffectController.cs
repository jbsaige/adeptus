using UnityEngine;
using System.Collections;

public class AirEffectController : MonoBehaviour 
{

    private PlayerStats thePlayer;
    private bool _isAlive;

    public float startWait;
    public float thunderSpawnRate;
    public Vector3 spawnValues;
    public GameObject ThunderHazard;
    public GameObject Cyclone;

    // Use this for initialization
    void Start()
    {
        thePlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        StartCoroutine(SpawnThunderEvent());
    }

    IEnumerator SpawnThunderEvent()
    {
        yield return new WaitForSeconds(startWait);
        _isAlive = true;

        while (_isAlive)
        {
            SpawnThunder();
            //SpawnCyclone();
            yield return new WaitForSeconds(thunderSpawnRate);
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

    private void SpawnThunder()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, Random.Range(-spawnValues.z, spawnValues.z));
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(ThunderHazard, spawnPosition, spawnRotation);
    }

    private void SpawnCyclone()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, Random.Range(-spawnValues.z, spawnValues.z));
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(Cyclone, spawnPosition, spawnRotation);
    }
}
