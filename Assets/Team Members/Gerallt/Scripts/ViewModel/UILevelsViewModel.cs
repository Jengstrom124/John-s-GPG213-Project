using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gerallt
{
    public class UILevelsViewModel : ManagerBase<UILevelsViewModel>
    {
        [SerializeField] private GameObject view;
        [SerializeField] private GameObject scrollViewContent;
        [SerializeField] private GameObject levelUIPrefab;
        [SerializeField] private Button buttonTestPopulateLevels;
        
        public static string managerScene = "ManagerScene";

        public mayaSpawner spawner;
        
        public void LoadLevel(string levelName)
        {
            //SceneManager.sceneLoaded += SceneManager_OnsceneLoaded;
            //SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            
            // Tell all the clients to load the specified level.
            NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
            NetworkManager.Singleton.SceneManager.LoadScene(levelName, LoadSceneMode.Additive); // Wouldn't synchronise the correct LoadSceneMode well for players that joined late
            spawner.SpawnStuff();
        }

        [ClientRpc]
        public void SetActiveSceneClientRpc(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            
            SceneManager.SetActiveScene(scene);
        }
        
        [ClientRpc]
        public void UnloadSceneClientRpc(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);

            // Wouldn't synchronise the correct LoadSceneMode well for players that joined late
            // So had to make RPCs like this doing everything myself
            
            SceneManager.UnloadSceneAsync(scene);
        }
        
        private void SceneManager_OnsceneLoaded(Scene newScene, LoadSceneMode loadSceneMode)
        {
            SceneManager.sceneLoaded -= SceneManager_OnsceneLoaded;
            
            string sceneName = newScene.name;
            
            LoadEventCompleted(sceneName);
        }
        
        private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                //case SceneEventType.SynchronizeComplete: // Clients have loaded all scenes that were loaded by the server.
                case SceneEventType.LoadEventCompleted:
                    NetworkManager.Singleton.SceneManager.OnSceneEvent -= SceneManager_OnSceneEvent;

                    string sceneName = sceneEvent.SceneName;
                    LoadEventCompleted(sceneName);
                    
                    // if(!GameManager.Instance.hasGameStarted.Value)
                    // {
                    //     GameManager.Instance.StartGame(); // Spawn the characters in the level
                    // }
                    
                    break;
            }
        }

        public void LoadEventCompleted(string sceneName, bool lateJoin = false)
        {
            if (sceneName == managerScene) return;
            
            // Unload the old scene if it's not the ManagerScene
            Scene oldScene = SceneManager.GetActiveScene();
            if (oldScene.name != managerScene && sceneName != oldScene.name)
            {
                //UnloadSceneClientRpc(oldScene.name);
                NetworkManager.SceneManager.UnloadScene(oldScene);
            }

            if (ServerManager.Singleton.IsServer)
            {
                // Update the selected level to the new one.
                GameManager.Instance.selectedLevel.Value = sceneName;
            }

            // Level has loaded so hide all UIs.
            GameManager.Instance.RaiseChangeLevelsVisibilityClientRpc(false);
            
            // Destroy cameras on new scene for each client
            DestroyCamerasClientRpc(sceneName);
     
            // Tell all the clients to set this new scene as the active one.
            //SetActiveSceneClientRpc(sceneName);

            // GameManager.Instance.hasGameStarted.OnValueChanged += delegate(bool value, bool newValue)
            // {
            //     if (newValue)
            //     {
            //         StartPlayerServerRpc();
            //     }
            //     
            // };
            //
            // GameManager.Instance.GetGameStartedServerRpc();

            
            
            //if (!HasGameStartedState.Instance.hasGameStarted.Value)
            if(!GameManager.Instance.hasGameStarted.Value) // lateJoin == false && 
            {
                GameManager.Instance.StartGame();
                
                // if (ServerManager.Singleton.IsServer)
                // {
                //     HasGameStartedState.Instance.hasGameStarted.Value = true;
                // }
                
            }
            else
            {
                //GameManager.Instance.StartPlayerServerRpc();
            }
        }

        public void DestroyCameras()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.name != managerScene)
                {
                    DestroyCamerasClientRpc(scene.name);
                }
            }
        }
        
        [ClientRpc]
        public void DestroyCamerasClientRpc(string sceneName)
        {
            // Destroy all cameras in scene, since Manager Scene has the main camera
            Scene currentScene = SceneManager.GetSceneByName(sceneName);
            
            GameObject[] roots = currentScene.GetRootGameObjects();
            for (int i = 0; i < currentScene.rootCount; i++)
            {
                GameObject rootObj = roots[i];

                Camera annoyingCamera2 = rootObj.GetComponent<Camera>();
                if (annoyingCamera2 != null)
                {
                    Destroy(annoyingCamera2.gameObject);
                }

                PlayerController annoyingPlayerController = rootObj.GetComponent<PlayerController>();
                if (annoyingPlayerController != null)
                {
                    annoyingPlayerController.enabled = false;
                    
                    Destroy(annoyingPlayerController);
                }

                Camera[] cameras = rootObj.GetComponentsInChildren<Camera>();

                for (int j = 0; j < cameras.Length; j++)
                {
                    Camera annoyingCamera = cameras[j];
                            
                    Destroy(annoyingCamera.gameObject);
                }
            }
        }
        
        public void PopulateLevels()
        {
            // Clear everything in view.
            for (int i = 0; i < scrollViewContent.transform.childCount; i++)
            {
                GameObject child = scrollViewContent.transform.GetChild(i).gameObject;
                
                Destroy(child);
            }
            
            // Show levels in view.
            List<Object> levels = GameManager.Instance.levels;
            for (int i = 0; i < levels.Count; i++)
            {
                GameObject levelInstance = Instantiate(levelUIPrefab, scrollViewContent.transform);

                Button itemButton = levelInstance.GetComponentInChildren<Button>();
                TextMeshProUGUI buttonText = itemButton.GetComponentInChildren<TextMeshProUGUI>();
                
                Object levelObj = levels[i];
                //SceneAsset sceneAsset = levelObj as SceneAsset;
                //string sceneName = sceneAsset.name;
                string sceneName = levelObj.name;
                
                buttonText.text = sceneName;
                
                itemButton.onClick.AddListener(() =>
                {
                    LoadLevel(sceneName);
                });
            }
        }
        
        private void Start()
        {
            // view.SetActive(false);
            
            GameManager.Instance.OnChangeLevelsVisibility += GameManager_OnChangeLevelsVisibility;
            
            buttonTestPopulateLevels.onClick.AddListener(TestPopulateLevels);
            
            PopulateLevels();
        }

        private void GameManager_OnChangeLevelsVisibility(bool visibility)
        {
            // view.SetActive(visibility);

            // if (visibility)
            // {
                //PopulateLevels();
            // }
        }

        private void TestPopulateLevels()
        {
            PopulateLevels();
        }
    }
}