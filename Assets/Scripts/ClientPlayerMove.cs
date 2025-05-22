using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using StarterAssets;

public class ClientPlayerMove : NetworkBehaviour
{
    [SerializeField] private PlayerInput m_PlayerInput;
    [SerializeField] private StarterAssetsInputs m_StartAssetsInputs;
    [SerializeField] private ThirdPersonController m_ThirdPersonController;

    private void Awake()
    {
        m_StartAssetsInputs.enabled = false;
        m_PlayerInput.enabled = false;
        m_ThirdPersonController.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            m_StartAssetsInputs.enabled = true;
            m_PlayerInput.enabled = true;
        }
        if (IsServer)
        {


            m_ThirdPersonController.enabled = true;
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
