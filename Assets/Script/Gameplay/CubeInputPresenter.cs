using UnityEngine;
using Zenject;
using Cube2048.Input;

namespace Cube2048.Gameplay
{
    public enum CubeState
    {
        Idle,
        Dragging,
        Launched
    }

    public class CubeInputPresenter : MonoBehaviour
    {
        private Cube cube;
        private IInputHandler inputHandler;
        private CubeState currentState = CubeState.Idle;

        [Inject]
        public void Construct(IInputHandler inputHandler)
        {
            this.inputHandler = inputHandler;
            if (gameObject.activeInHierarchy && this.enabled)
            {
                Initialize();
            }
        }

        private void Awake()
        {
            cube = GetComponent<Cube>();
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Initialize()
        {
            currentState = CubeState.Idle;
            Subscribe();
        }

        private void Subscribe()
        {
            if (inputHandler == null) return;
            Unsubscribe();
            inputHandler.OnDrag += OnDrag;
            inputHandler.OnRelease += OnRelease;
        }

        private void Unsubscribe()
        {
            if (inputHandler != null)
            {
                inputHandler.OnDrag -= OnDrag;
                inputHandler.OnRelease -= OnRelease;
            }
        }

        private void OnDrag(float deltaX)
        {
            if (currentState == CubeState.Launched) return;
            currentState = CubeState.Dragging;
            if (cube != null) cube.Move(deltaX);
        }

        private void OnRelease()
        {
            if (currentState == CubeState.Launched) return;

            currentState = CubeState.Launched;
            if (cube != null) cube.Shoot();

            Unsubscribe();
            this.enabled = false;
        }
    }
}