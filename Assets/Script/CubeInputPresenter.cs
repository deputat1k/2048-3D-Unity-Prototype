using UnityEngine;

public class CubeInputPresenter : MonoBehaviour
{
    private Cube cube;
    private IInputHandler inputHandler;

    public void Construct(Cube cube, IInputHandler inputHandler)
    {
        this.cube = cube;
        this.inputHandler = inputHandler;

        // Підписуємось на управління
        Subscribe();
    }

    private void Subscribe()
    {
        if (inputHandler != null)
        {
            inputHandler.OnDrag += OnDrag;
            inputHandler.OnRelease += OnRelease;
        }
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
        if (cube != null)
        {
            cube.Move(deltaX);
        }
    }

    private void OnRelease()
    {
        if (cube != null)
        {
            cube.Shoot();
        }
        
        Unsubscribe();

        this.enabled = false;
    }

    private void OnDisable()
    { 
        Unsubscribe();
    }
}   