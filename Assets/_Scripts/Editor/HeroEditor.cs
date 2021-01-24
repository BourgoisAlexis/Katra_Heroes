using UnityEditor;

[CustomEditor(typeof(Hero))]
public class HeroEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Hero hero = (Hero)target;

        string pathToObject = AssetDatabase.GetAssetPath(hero);
        string newName = (hero.Index < 10 ? "0" : string.Empty) + hero.Index.ToString() + "_" + hero.Name;

        AssetDatabase.RenameAsset(pathToObject, newName);
    }
}
