using Jotunn.Managers;
using OdinOnDemand.MPlayer;
using OdinOnDemand.Utils;
using OdinOnDemand.Utils.Config;
using UnityEngine;

namespace OdinOnDemand.Components
{
    internal class RemoteControlItem : MonoBehaviour
    {
        private Camera cam;
        private Player localPlayer;
        private MediaPlayerComponent mp;
        
        private SpeakerComponent selectedSpeaker;
     
        private void Start()
        {
            cam = Camera.main;
            localPlayer = Player.m_localPlayer;
        }

        private void Update()
        {
          if (ZInput.GetButtonDown(KeyConfig.UseRemoteButton.Name))
    {
        if (!ProcessRaycast())
            CheckUtilityItem();
        }

        if (ZInput.GetButtonDown(KeyConfig.LinkRemoteButton.Name))
        {
            ProcessRaycastSpeaker();
        }
}

        private bool ProcessRaycastSpeaker()
        {
            int solidRayMask = LayerMask.GetMask("piece", "Default", "terrain", "static_solid", "Default_small", "vehicle", "item");
            if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit,
                OODConfig.RemoteControlDistance.Value, solidRayMask)) return false;

            var basePlayerComponent = hit.transform.gameObject.GetComponentInParent<BasePlayer>();
            if (basePlayerComponent != null)
            {
                if (basePlayerComponent is MediaPlayerComponent || basePlayerComponent is ReceiverComponent)
                {
                    if (selectedSpeaker != null)
                    {
                        if(!basePlayerComponent.mPiece.IsCreator() && OODConfig.RemoteControlOwnerOnly.Value) return false;
                        if (!PrivateArea.CheckAccess(transform.position)) return false;
                        if (basePlayerComponent.AddSpeaker(selectedSpeaker))
                        {
                            MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Speaker added to receiver");
                            selectedSpeaker = null;
                            return true;
                        }
                        else
                        {
                            basePlayerComponent.RemoveSpeaker(selectedSpeaker);
                            MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Speaker removed from receiver");
                            selectedSpeaker = null;
                        }
                    }
                }
            }
            
            var speaker = hit.transform.gameObject.GetComponentInParent<SpeakerComponent>();
            if (speaker != null)
            {
                if (!speaker.mPiece.IsCreator() && OODConfig.RemoteControlOwnerOnly.Value) return false;
                if (!PrivateArea.CheckAccess(transform.position)) return false;
                selectedSpeaker = speaker;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Speaker selected");
                return true;
            }
            return false;
        }
        private static readonly LayerMask SolidRayMask = LayerMask.GetMask("Default", "piece", "terrain", "static_solid", "Default_small", "character", "vehicle");

        
        private bool ProcessRaycast()
        {
            
            if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit,
        OODConfig.RemoteControlDistance.Value, SolidRayMask)) return false;


            var basePlayerComponent = hit.collider.transform.gameObject.GetComponentInParent<BasePlayer>();
            
            if (basePlayerComponent != null)
            {
                if (basePlayerComponent is MediaPlayerComponent || basePlayerComponent is ReceiverComponent)
                {
                    if (selectedSpeaker != null)
                    {
                        if (basePlayerComponent.AddSpeaker(selectedSpeaker))
                        {
                            MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Speaker added to receiver");
                            selectedSpeaker = null;
                            return true;
                        }
                        {
                            basePlayerComponent.RemoveSpeaker(selectedSpeaker);
                            MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Speaker removed from receiver");
                            selectedSpeaker = null;
                            return true;
                        }
                    }
                    basePlayerComponent.UIController.ToggleMainPanel();
                    return true;
                }
                if (basePlayerComponent is CartPlayerComponent cartPlayer)
                {
                    ToggleCartPlayer(cartPlayer);
                    return true;
                }
            }
            else if(hit.transform.gameObject.GetComponent<Vagon>() != null)
            {
                var cartPlayerComponent = hit.transform.gameObject.GetComponentInChildren<CartPlayerComponent>();
                ToggleCartPlayer(cartPlayerComponent);
                return true;
            }
            
            var speaker = hit.transform.gameObject.GetComponentInParent<SpeakerComponent>();
            if (speaker != null)
            {
                selectedSpeaker = speaker;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Speaker selected");
                return true;
            }

            if (CheckAdminPrivileges(hit))
            {
                return true;
            }

            return false;
        }

        private void ToggleCartPlayer(CartPlayerComponent cartPlayer)
        {
            if (!cartPlayer.mPiece.IsCreator() && OODConfig.RemoteControlOwnerOnly.Value) return;
            if (!cartPlayer.PlayerSettings.IsGuiActive && PrivateArea.CheckAccess(transform.position))
            {
                cartPlayer.UIController.ToggleMainPanel();
            }
        }

        private bool CheckAdminPrivileges(RaycastHit hit)
        {
            if (!SynchronizationManager.Instance.PlayerIsAdmin) return false;

            var player = hit.transform.gameObject.GetComponent<Player>();
            if (player == null) return false;

            var belt = player.GetComponentInChildren<BeltPlayerComponent>();
            if (!belt) return false;
            belt.UIController.ToggleMainPanel();
            return true;
        }

        private void CheckUtilityItem()
        {
            if (localPlayer == null) return;

    // Припускаємо, що m_visEquipment містить доступ до елементів обладнання
    var equipment = localPlayer.GetComponent<VisEquipment>(); // Замінити на правильний клас, якщо такий є

    if (equipment == null) return;

    // Перевіряємо праву руку або інше місце для конкретного предмета
    var rightHandItem = equipment.m_rightHand; // Це лише приклад, якщо такий метод є

    if (rightHandItem != null)
    {
        // Дія, якщо предмет в правій руці
        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Remote Control is in the right hand.");
    }
    else
    {
        // Дія, якщо предмет не в правій руці
        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Remote Control is not in the right hand.");
    }
    {
        // Дія, якщо предмет в правій руці
        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Remote Control is in the right hand.");
    }
        }
    }
}