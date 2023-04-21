using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class TankShooting : NetworkBehaviour
{
    public int playerNumber = 1;
    public GameObject shellRb;
    public Transform fireTransform;
    public Slider aimSlider;
    public AudioSource shootingAudio;
    public AudioClip chargingClip;
    public AudioClip fireClip;
    public float minLaunchForce = 15f;
    public float maxLaunchForce = 30f;
    public float maxChargeTime = 0.75f;

    private string _fireButton;
    private float _currentLaunchForce;
    private float _chargeSpeed;
    [Networked]
    private bool _fired { get; set; }
    [Networked]
    private bool _isDown { get; set; }

    private void OnEnable()
    {
        _currentLaunchForce = minLaunchForce;
        aimSlider.value = minLaunchForce;
    }

    private void Start()
    {
        _fireButton = "Fire" + playerNumber;
        _chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
    }

    public override void FixedUpdateNetwork()
    {
        
        aimSlider.value = minLaunchForce;
        if (GetInput(out NetworkInputPrototype input))
        {
            if (_currentLaunchForce >= maxLaunchForce && !_fired)
            {
                print(111);
                _currentLaunchForce = maxLaunchForce;
                Fire();
            }
            else if (input.IsDown(NetworkInputPrototype.BUTTON_JUMP) && !input.IsUp(NetworkInputPrototype.BUTTON_JUMP) && !_fired)
            {
                print(222);
                _isDown = true;
                _currentLaunchForce += _chargeSpeed * Runner.DeltaTime;
                aimSlider.value = _currentLaunchForce;
            }
            else if (input.IsDown(NetworkInputPrototype.BUTTON_JUMP))
            {
                print(333);
                _isDown = true;
                _fired = false;
                _currentLaunchForce = minLaunchForce;

                shootingAudio.clip = chargingClip;
                shootingAudio.Play();
            }
            else if (input.IsUp(NetworkInputPrototype.BUTTON_JUMP) && !_fired && _isDown)
            {
                print(444);
                _isDown = false;
                Fire();

            }
        }
    }

    /*private void Update()
    {
        aimSlider.value = minLaunchForce;

        if (_currentLaunchForce >= maxLaunchForce && !_fired)
        {
            print(111);
            _currentLaunchForce = maxLaunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(_fireButton))
        {
            print(222);
            _fired = false;
            _currentLaunchForce = minLaunchForce;

            shootingAudio.clip = chargingClip;
            shootingAudio.Play();
        }
        else if (Input.GetButton(_fireButton) && !_fired)
        {
            print(333);
            _currentLaunchForce += _chargeSpeed * Time.deltaTime;
            aimSlider.value = _currentLaunchForce;
        }
        else if (Input.GetButtonUp(_fireButton) && !_fired)
        {
            print(444);
            Fire();
        }
    }*/

    private void Fire()
    {
        _fired = true;

        //Rigidbody shellInstance = (Rigidbody)Instantiate(shellRb, fireTransform.position, fireTransform.rotation);
        //shellInstance.velocity = _currentLaunchForce * fireTransform.forward;

        Runner.Spawn(shellRb, fireTransform.position, fireTransform.rotation, Object.InputAuthority, (runner, o) => { o.GetComponent<ShellExplosion>().Init(_currentLaunchForce * fireTransform.forward); });

        shootingAudio.clip = fireClip;
        shootingAudio.Play();

        _currentLaunchForce = minLaunchForce;
    }
}