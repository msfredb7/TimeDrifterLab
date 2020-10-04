using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;
using System;

public class ReadyButton : GamePresentationBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;

    [SerializeField] private string _cantReadyYetText = "Wait";
    [SerializeField] private Color _cantReadyYetColor = Color.white;
    [SerializeField] private string _waitingForReadyText = "Not Ready";
    [SerializeField] private Color _waitingForReadyColor = Color.green;
    [SerializeField] private string _readyText = "Ready";
    [SerializeField] private Color _readyColor = Color.white;


    public enum TurnState
    {
        Ready,
        NotReady,
        NotMyTurn
    }

    private DirtyValue<TurnState> _viewState;
    private float _updateTimer = 0;
    private const float UPDATE_DELAY = 0.5f;

    public TurnState GetState() { return _viewState.Get(); }

    public event Action ButtonPressed;

    protected override void Awake()
    {
        _button.onClick.AddListener(OnReadyClicked);

        _updateTimer = UPDATE_DELAY;

        _viewState.Set(TurnState.NotMyTurn); // default value

        base.Awake();
    }

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.HasSingleton<NewTurnEventData>())
        {
            _updateTimer = UPDATE_DELAY; // reset timer as we're manually updating

            UpdateState();
        }
        else
        {
            _updateTimer -= Time.deltaTime;
            if (_updateTimer <= 0)
            {
                _updateTimer = UPDATE_DELAY;

                UpdateState();
            }
        }

        UpdateView();

        // Shortcut
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnReadyClicked();
        }
    }

    private void UpdateState()
    {
        if (SimWorldCache.LocalPawn == Entity.Null)
        {
            // If we don't have a pawn
            _viewState.Set(TurnState.NotMyTurn);
        }
        else if (SimWorldCache.CurrentTeam != SimWorldCache.LocalPawnTeam && SimWorldCache.CurrentTeam.Value != -1)
        {
            // if it's not our turn to play
            _viewState.Set(SimWorld.HasSingleton<GameStartedTag>() ? TurnState.NotMyTurn : TurnState.NotReady);
        }
        else
        {
            // if it's our turn to play
            if (SimWorld.TryGetComponentData(SimWorldCache.LocalController, out ReadyForNextTurn ready) && ready.Value)
            {
                _viewState.Set(TurnState.Ready);
            }
            else
            {
                _viewState.Set(TurnState.NotReady);
            }
        }
    }

    private void UpdateView()
    {
        if (_viewState.ClearDirty())
        {
            switch (_viewState.Get())
            {
                case TurnState.Ready:
                    _text.text = _readyText;
                    _image.color = _readyColor;
                    break;
                case TurnState.NotReady:
                    _text.text = _waitingForReadyText;
                    _image.color = _waitingForReadyColor;
                    break;
                default:
                case TurnState.NotMyTurn:
                    _text.text = _cantReadyYetText;
                    _image.color = _cantReadyYetColor;
                    break;
            }
        }
    }

    private void OnReadyClicked()
    {
        ToggleReady();
    }

    private void ToggleReady()
    {
        if (_viewState.Get() == TurnState.NotMyTurn)
            return;

        bool currentlyReady = _viewState.Get() == TurnState.Ready;

        SimWorld.SubmitInput(new SimPlayerInputNextTurn(!currentlyReady));

        _viewState.Set(currentlyReady ? TurnState.NotReady : TurnState.Ready);
        
        _updateTimer = UPDATE_DELAY; // reset timer as we're manually updating

        ButtonPressed?.Invoke();
    }
}
