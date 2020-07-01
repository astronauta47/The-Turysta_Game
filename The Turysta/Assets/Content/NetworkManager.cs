using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] Vector3[] spawnpointsPositions;
    [SerializeField] GameObject[] models;

    [SerializeField] Material skyboxLight;
    [SerializeField] Material skyboxDark;
    [SerializeField] GameObject flashLight;
    [SerializeField] GameObject dirLight;
    public bool darkMode;

    int[] spawnpointsPositionsIndexList;
    int[] ModelsIndexList;

    int index = -1;
    int modelIndex = -1;
    int modelId;
    float timer, timeStep = 5, timeStepTmp;
    PhotonView photonView;
    Text timerText;
    int minutes, secounds;

    void Start()
    {
        timeStepTmp = timeStep;
        photonView = GetComponent<PhotonView>();
        dirLight = GameObject.FindGameObjectWithTag("Light");

        spawnpointsPositionsIndexList = new int[spawnpointsPositions.Length];
        ModelsIndexList = new int[models.Length];

        if (PhotonNetwork.IsMasterClient)
        {
            Repeat(ref spawnpointsPositionsIndexList);
            Repeat(ref ModelsIndexList);
        }

        if (photonView.IsMine)
        {
            photonView.RPC("SetPlayerPosition", RpcTarget.MasterClient);   
        }

    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        timer += Time.deltaTime;

        if(timer > timeStepTmp)
        {
            timeStepTmp += timeStep;
            photonView.RPC("UpdateTimer", RpcTarget.All, (int)Mathf.Round(timer));
        }

        if(Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Q))
        {
            if(darkMode)
            {
                PhotonNetwork.LoadLevel(2);
            }
            else
            {
                PhotonNetwork.LoadLevel(1);
            }
        }
    }

    [PunRPC]
    void ActiveDarkMode(bool active)
    {
        
        RenderSettings.fog = active;
        dirLight.SetActive(!active);

        if (active)
        {
            RenderSettings.skybox = skyboxDark;
        }
        else
        {
            RenderSettings.skybox = skyboxLight;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [PunRPC]
    private void SpawnPlayer(int modelID)
    {

        GameObject player = PhotonNetwork.Instantiate("Player", spawnpointsPositions[0], Quaternion.identity);
        flashLight = player.transform.GetChild(0).GetChild(0).gameObject;
        if (darkMode) flashLight.SetActive(true);
        int id = player.GetComponent<PhotonView>().ViewID;

        if (player.GetComponent<PhotonView>().IsMine)
        {
            PhotonView.Find(id).GetComponent<PlayerMovement>().enabled = true;
            player.transform.GetChild(0).gameObject.SetActive(true);
            player.transform.GetChild(1).gameObject.SetActive(true);
            player.transform.GetChild(2).gameObject.SetActive(false);
            timerText = player.transform.GetChild(1).GetChild(4).GetComponent<Text>();
        }
        else
            player.transform.GetChild(1).gameObject.SetActive(true);

        photonView.RPC("SetStartValue", RpcTarget.MasterClient, id);
    }

    [PunRPC]
    void WaitForTP(int id, int i)
    {
        PhotonView.Find(id).transform.position = spawnpointsPositions[i];
        PhotonView.Find(id).GetComponent<PhotonTransformView>().enabled = true;
    }

    [PunRPC]
    void UpdateTimer(int time)
    {
        secounds = time;
        minutes = Mathf.FloorToInt(time / 60);
        secounds = (secounds - minutes * 60);

        string minutesText = minutes.ToString();
        string secondsText = secounds.ToString();

        if (secounds < 10)
            secondsText = '0' + secondsText;

        if (minutes < 10)
            minutesText = '0' + minutesText;


        timerText.text = minutesText + ":" + secondsText;
    }

    [PunRPC]
    void SetStartValue(int id)
    {
        index++;
        modelIndex++;

        if (index >= spawnpointsPositions.Length)
            index = 0;

        if (modelIndex >= models.Length)
            modelIndex = 0;

        photonView.RPC("WaitForTP", RpcTarget.All, id, spawnpointsPositionsIndexList[index]);
        photonView.RPC("SetRandomModel", RpcTarget.All, ModelsIndexList[modelIndex], id);
    }

    [PunRPC]
    void TpPlayers(int ViewID, int index)
    {
        PhotonView.Find(ViewID).transform.position = spawnpointsPositions[index];
    }

    [PunRPC]
    void SetPlayerPosition()
    {
        photonView.RPC("SpawnPlayer", RpcTarget.All, modelId);
    }

    [PunRPC]
    void SetRandomModel(int i, int id)
    {
        Transform player = PhotonView.Find(id).transform;

        GameObject I = Instantiate(models[i], player.position + new Vector3(0, -0.916f, 0), Quaternion.identity, player);
        if(player.GetComponent<PhotonView>().IsMine) I.SetActive(false);
    }

    void Repeat(ref int[] spawnpointsPos)
    {
        for (int i = 0; i < spawnpointsPos.Length; i++)
        {
            spawnpointsPos[i] = -1;
        }

        int count = 0;

        while (true)
        {
            int value = Random.Range(0, spawnpointsPos.Length);
            bool isNew = true;

            for (int i = 0; i < count + 1; i++)
            {
                if (spawnpointsPos[i] == value)
                {
                    isNew = false;
                }
            }

            if (isNew)
            {
                spawnpointsPos[count] = value;
                count++;
            }



            if (count >= spawnpointsPos.Length)
                break;
        }

    }
}
