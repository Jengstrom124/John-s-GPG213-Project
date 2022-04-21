using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Gerallt
{
    public class InGameUI : ManagerBase<InGameUI>
    {
        [SerializeField] private GameObject view;
        [SerializeField] private Button backToLobbyButton;

        public void SetVisibility(bool visibility)
        {
            view.SetActive(visibility);
        }

        public void Lobby_OnClick()
        {
            SetVisibility(false);
            GameManager.Instance.RaiseChangeLobbyVisibility(true);
        }
        
        private void Start()
        {
            SetVisibility(false);
            
            backToLobbyButton.onClick.AddListener(Lobby_OnClick);
            
            GameManager.Instance.OnChangeInGameUIVisibility += OnChangeInGameUIVisibility;
        }

        private void OnChangeInGameUIVisibility(bool visibility)
        {
            SetVisibility(visibility);
        }
    }
}