using Cinemachine;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientPlayerMove : NetworkBehaviour
{
    [SerializeField] private PlayerInput m_PlayerInput;
    [SerializeField] private StarterAssetsInputs m_StartAssetsInputs;
    [SerializeField] private ThirdPersonController m_ThirdPersonController;
    [SerializeField] private CinemachineVirtualCamera m_VirtualCamera;

    private void Awake()
    {
        m_StartAssetsInputs.enabled = false;
        m_PlayerInput.enabled = false;
        m_ThirdPersonController.enabled = false;
        m_VirtualCamera.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            m_StartAssetsInputs.enabled = true;
            m_PlayerInput.enabled = true;
            m_VirtualCamera.enabled = true;
            m_ThirdPersonController.enabled = true;
        }
        else
        {
            m_VirtualCamera.enabled = false;
        }
        if (IsServer)
        {
            m_StartAssetsInputs.enabled = true;
            m_PlayerInput.enabled = true;
            m_ThirdPersonController.enabled = true;
        }
        else if (IsClient)
        {

            m_ThirdPersonController.enabled = true;
            m_VirtualCamera.enabled = true;
        }
        else
        {
            m_VirtualCamera.enabled = false;
        }
    }

    [Rpc(SendTo.Server)]

    private void UpdateInputServerRpc(Vector2 move, Vector2 look, bool jump, bool sprint)
    {
        m_StartAssetsInputs.MoveInput(move);
        m_StartAssetsInputs.LookInput(look);
        m_StartAssetsInputs.JumpInput(jump);
        m_StartAssetsInputs.SprintInput(sprint);
    }

    private void LateUpdate()
    {
        if (!IsOwner)
            return;
        UpdateInputServerRpc(m_StartAssetsInputs.move, m_StartAssetsInputs.look, m_StartAssetsInputs.jump, m_StartAssetsInputs.sprint);
    }
}
