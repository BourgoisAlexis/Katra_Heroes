using System.Collections.Generic;
using UnityEngine;

public class DeckManager : DragDrop
{
    #region Variables
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private LayerMask zoneLayer;

    private CardPiece selectedCard;
    private CardZone zone;

    private List<int> deck = new List<int>();
    private List<int> trash = new List<int>();
    private List<CardPiece> currentCards = new List<CardPiece>();

    private List<Square> inRange = new List<Square>();

    private GameplayManager gameplayManager;
    private BoardUtility utility => gameplayManager.Utility;
    #endregion


    protected override void Awake()
    {
        base.Awake();
        gameplayManager = GetComponent<GameplayManager>();
    }

    public void GameStart()
    {
        foreach (Card card in gameplayManager.Data.Cards)
            card.Set();

        TextAsset deckDoc = Resources.Load<TextAsset>("Decks/Deck_01");
        string[] index = deckDoc.text.Split(':');

        foreach (string s in index)
            deck.Add(int.Parse(s));
    }


    public void DrawCard()
    {
        if (deck.Count <= 0)
            return;

        int drawAt = Random.Range(0, deck.Count);
        Card drawn = gameplayManager.Data.Cards[deck[drawAt]];
        CardPiece card = Instantiate(cardPrefab, cardParent).GetComponent<CardPiece>();

        card.transform.localPosition = new Vector3(750, -350);

        card.Setup(drawn);
        currentCards.Add(card);

        CardsPosition();

        deck.RemoveAt(drawAt);
    }

    private void CardsPosition()
    {
        int width = 500;

        for (int i = 0; i < currentCards.Count; i++)
        {
            float step = width / currentCards.Count;
            currentCards[i].GetComponent<CardPiece>().ChangePosition(new Vector3(width / 2 - step * i, 0, i));
        }
    }


    public void ClickOnCard(CardPiece _card)
    {
        if (selectedCard != null)
            selectedCard.ReturnInHand(0.1f);

        selectedCard = _card;
        ClickDrag(_card.transform);
    }

    public void TargetBoard(Square _square)
    {
        if (inRange.Contains(_square))
        {
            Square[] targets = new Square[] { _square };
            selectedCard.Card.Use(targets, null);
            UsedCard();
        }
    }

    public void TargetHero(HeroPiece _piece)
    {
        Square square = utility.Board[_piece.Position.x, _piece.Position.y];
        if (inRange.Contains(square))
        {
            Square[] targets = new Square[] { square };
            selectedCard.Card.Use(targets, null);
            UsedCard();
        }
    }


    public override void Drag()
    {
        base.Drag();

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapCircle(mousePos, 0.2f, zoneLayer);

        if (col != null)
        {
            CardZone tempoZone = col.GetComponent<CardZone>();

            if (tempoZone != null && tempoZone != zone)
            {
                tempoZone.ShowZone(true);
                zone = tempoZone;
            }
        }
        else if (col == null && zone != null)
        {
            zone.ShowZone(false);
            zone = null;
        }
    }

    public override void DragEnd()
    {
        base.DragEnd();

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapCircle(mousePos, 0.2f, zoneLayer);

        if (col != null)
        {
            if (selectedCard.Card.Cost <= gameplayManager.Mana)
            {
                CardInDropZone();
                return;
            }
        }

        Unselection();
    }


    private void CardInDropZone()
    {
        //Disparition de la carte
        selectedCard.gameObject.SetActive(false);
        //A retaper

        Ability ability = selectedCard.Card;

        if (ability.Targetting == e_targetting.Default)
        {
            if (zone != null)
                zone.ShowZone(false);

            inRange = utility.GetRange(inRange, ability);

            foreach (Square s in inRange)
                s.HighLight(1);

            gameplayManager.ChangeStep(e_step.UseCard);
        }
        else if (ability.Targetting == e_targetting.AutomaticTarget)
        {
            inRange = utility.GetRange(inRange, ability);
            ability.Use(inRange.ToArray(), null);

            UsedCard();
        }
        else
        {
            ability.Use(null, null);
            UsedCard();
        }
    }

    private void UsedCard()
    {
        gameplayManager.ModifyMana(-selectedCard.Card.Cost);

        currentCards.Remove(selectedCard);
        trash.Add(selectedCard.Card.Index);
        Destroy(selectedCard.gameObject);
        selectedCard = null;

        CardsPosition();
        Unselection();

        gameplayManager.ChangeStep(e_step.Card);
    }


    public void Unselection()
    {
        if (selectedCard != null)
        {
            selectedCard.ReturnInHand(0.1f);
            selectedCard = null;
        }

        inRange.Clear();
        toDrag = null;

        if (zone != null)
            zone.ShowZone(false);

        foreach (Square s in utility.Board)
            s.UnHighLight();
    }
}
