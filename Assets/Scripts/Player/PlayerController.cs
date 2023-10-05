using UnityEngine;

/* Based on Unity documentation for CharacterController.Move */

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _speed = 5f;
    [SerializeField] float _gravity = 2f;
    [SerializeField] float _gravityModifier = 2f;

    public float maximumSpeed = 40f;

    CharacterController _characterController;
    Vector3 _moveDirection = Vector3.zero;

    [Header("ability rings")]
    [SerializeField] ParticleSystem RedAbilityRing;
    [SerializeField] ParticleSystem BlueAbilityRing;
    [SerializeField] ParticleSystem GreenAbilityRing;

    void Start() => _characterController = GetComponent<CharacterController>();

    void Update()
    {
        if (_characterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes
            _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            _moveDirection = Vector3.ClampMagnitude(_moveDirection, 1f);
            _moveDirection *= _speed;

            // Face in the right direction
            if (_moveDirection != Vector3.zero)
            {
                transform.forward = _moveDirection;
            }

        }

        // Need to continually apply gravity to player
        _moveDirection.y -= _gravityModifier * _gravity * Time.deltaTime;

        // Move the controller
        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    public void emitAbilityRing(float size, int abilityNum)
    {
        ParticleSystem ps;
        if (abilityNum == 1) { ps = RedAbilityRing; }
        else if (abilityNum == 2) { ps = BlueAbilityRing; }
        else { ps = GreenAbilityRing; }

        var main = ps.main;
        main.startSize = size;
        ps.Play();
    }

    public bool upgradeMovementSpeed(float amount)
    {
        _speed += amount;

        if (_speed >= maximumSpeed)
        {
            _speed = maximumSpeed;
            return true;
        }
        return false;
    }
}
