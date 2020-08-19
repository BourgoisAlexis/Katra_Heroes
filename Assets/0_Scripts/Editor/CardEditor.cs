using UnityEditor;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Card card = (Card)target;

        string pathToObject = AssetDatabase.GetAssetPath(card);

        string newName = (card.Index < 10 ? "0" : string.Empty) + card.Index.ToString() + "_" + card.Name;

        AssetDatabase.RenameAsset(pathToObject, newName);
    }
}
