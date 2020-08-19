using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DeckManager : DragDrop
{
    #region Variables
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField]
    private Transform cardParent;
    [SerializeField]
    private LayerMask zoneLayer;

    private GameplayManager gameplayManager;
    private BoardUtility utility;
    private CardPiece selectedCard;
    private bool usingCard;
    private int currentIndex;

    private CardZone zone;

    [SerializeField]
    private List<int> deck = new List<int>();
    [SerializeField]
    private List<CardPiece> currentCards = new List<CardPiece>();
    [SerializeField]
    private List<int> trash = new List<int>();
    private List<Square> activeList = new List<Square>();

    //Accessors
    public bool UsingCard => usingCard;
    #endregion


    protected override void Awake()
    {
        base.Awake();

        gameplayManager = GetComponent<GameplayManager>();
    }

    public void Setup(BoardUtility _utility)
    {
        utility = _utility;
        currentIndex = 0;
        TextAsset deckDoc = Resources.Load<TextAsset>("Decks/Deck_01");

        string[] ui = deckDoc.text.Split(':');
        foreach (string s in ui)
            deck.Add(int.Parse(s));

        StartCoroutine(DebuteDraw());
    }

    private IEnumerator DebuteDraw()
    {
        int t = 5;
        yield return new WaitForSeconds(1);

        for (int i = 0; i < t; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(0.1f);
        }
    }


    public void DrawCard()
    {
        if (deck.Count > 0)
        {
            int drawAt = Random.Range(0, deck.Count);
            Card drawn = GameplayManager.Instance.Data.Cards[deck[drawAt]];
            CardPiece card = Instantiate(cardPrefab, cardParent).GetComponent<CardPiece>();

            card.Setup(drawn, currentIndex);
            currentCards.Add(card);

            CardsPosition();

            deck.RemoveAt(drawAt);
            currentIndex++;
        }
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
        gameplayManager.Board.Unselection();

        if (selectedCard != null)
            selectedCard.ReturnInHand();

        selectedCard = _card;
    }

    public void TargetBoard(Square _square)
    {
        if (activeList.Contains(_square))
            selectedCard.Card.Ability.Use(_square);

        UsedCard();
    }

    public void TargetHero(HeroPiece _piece)
    {
        Square square = gameplayManager.Board.Board[_piece.Position.x, _piece.Position.y];
        if (activeList.Contains(square))
            selectedCard.Card.Ability.Use(square);

        UsedCard();
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
        Collider2D[] cols = Physics2D.OverlapCircleAll(mousePos, 0.2f);

        if (cols.Length > 1)
            foreach (Collider2D c in cols)
                if (c.GetComponent<CardZone>())
                {
                    CardInDropZone();
                    return;
                }

        Unselection();
    }

    private void CardInDropZone()
    {
        //Disparition de la carte
        selectedCard.gameObject.SetActive(false);
        if (zone != null)
            zone.ShowZone(false);
        //A retaper


        Ability ability = selectedCard.Card.Ability;

        if (ability.AbilityType != e_abilityType.Creation && ability.AbilityType != e_abilityType.Draw)
        {
            activeList = utility.GetRangeSpe(activeList, ability);

            foreach (Square s in activeList)
                s.HighLight(1);

            usingCard = true;
        }
        else
        {
            ability.Use(null);
            UsedCard();
        }
    }

    private void UsedCard()
    {
        currentCards.Remove(selectedCard);
        trash.Add(selectedCard.Card.Index);
        Destroy(selectedCard.gameObject);
        selectedCard = null;
        Unselection();

        CardsPosition();
    }

    public void Unselection()
    {
        if (selectedCard)
        {
            selectedCard.ReturnInHand();
            selectedCard = null;
        }

        foreach (Square s in activeList)
            s.UnHighLight();

        activeList.Clear();
        usingCard = false;

        toDrag = null;

        if (zone != null)
            zone.ShowZone(false);
    }
}
