using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gerallt
{
    public class UILevelsViewModel : NetworkBehaviour
    {
        [SerializeField] private GameObject view;
        [SerializeField] private GameObject scrollViewContent;
        [SerializeField] private GameObject levelUIPrefab;
        [SerializeField] private string managerScene = "ManagerScene";
        
        public void LoadLevel(string levelName)
        {
            // Tell all the clients to load the specified level.
            NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
            NetworkManager.Singleton.SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
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

            SceneManager.UnloadSceneAsync(scene);
        }
        
        private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                //case SceneEventType.SynchronizeComplete: // Clients have loaded all scenes that were loaded by the server.
                case SceneEventType.LoadEventCompleted:
                    NetworkManager.Singleton.SceneManager.OnSceneEvent -= SceneManager_OnSceneEvent;

                    // Unload the old scene if it's not the ManagerScene
                    Scene oldScene = SceneManager.GetActiveScene();
                    if (oldScene.name != managerScene)
                    {
                        UnloadSceneClientRpc(oldScene.name);
                    }
            
                    string sceneName = sceneEvent.SceneName;
            
                    // Tell all the clients to set this new scene as the active one.
                    SetActiveSceneClientRpc(sceneName);
            
                    // Update the selected level to the new one.
                    GameManager.Instance.selectedLevel.Value = sceneName;
                    
                    // Level has loaded so hide all UIs.
                    GameManager.Instance.RaiseChangeLevelsVisibilityClientRpc(false);

                    // Destroy cameras on new scene for each client
                    DestroyCamerasClientRpc(sceneName);

                    
                    GameManager.Instance.StartGame(); // Spawn the characters in the level

                    break;
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
                SceneAsset sceneAsset = levelObj as SceneAsset;
                string sceneName = sceneAsset.name;
                
                buttonText.text = sceneName;
                
                itemButton.onClick.AddListener(() =>
                {
                    LoadLevel(sceneName);
                });
            }
        }
        
        private void Start()
        {
            view.SetActive(false);
            
            GameManager.Instance.OnChangeLevelsVisibility += GameManager_OnChangeLevelsVisibility;
        }

        private void GameManager_OnChangeLevelsVisibility(bool visibility)
        {
            view.SetActive(visibility);

            if (visibility)
            {
                PopulateLevels();
            }
        }
    }
}