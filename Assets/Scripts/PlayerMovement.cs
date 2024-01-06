using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

  public InputActionProperty inputActionProperty;

  public AudioClip [] moveSound;
  [SerializeField] private bool isMoving = false;
  GameObject activeCard;

  private void Update() {
    activeCard = GameObject.FindGameObjectWithTag("Active Card");
    if(CardEffectManager.isDead) {
      inputActionProperty.action.Disable();
    }
    else {
      inputActionProperty.action.Enable();
    }

    if(inputActionProperty.action.ReadValue<Vector2>() != Vector2.zero) {
      if(!isMoving) {
        InvokeRepeating(nameof(MovingSound), 0.15f, 0.35f);
      }
      isMoving = true;
    }
    else {
      isMoving = false;
      CancelInvoke(nameof(MovingSound));
    }
  }

  private void MovingSound() {
    int randomSound = Mathf.FloorToInt(Random.Range(0f, moveSound.Length));
    gameObject.GetComponent<AudioSource>().PlayOneShot(moveSound[randomSound]);
  }

}
