using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "MicroCrew/GameAssets", fileName = "GameAssets")]
public class MAssets : SingletonScriptableObject<MAssets> {

    [SerializeField] private List<NamedColor> m_colors;
    [SerializeField] private List<TextMeshProPreset> m_TextPresets;

    private Sprite[] m_ui_tex_0;
    public Dictionary<string, Sprite> ui_tex_0;
    public Dictionary<string, Color> colors;
    public Dictionary<TextStylePreset, TextMeshProPreset> textPresets;

    public SO_AssetDB<SO_ItemInfo, ItemType> itemsInfo;
    public SO_AssetDB<SO_ChestInfo, ChestType> chestsInfo;
    public SO_AssetDB<SO_HeroInfo, HeroType> heroesInfo;
    public SO_AssetDB<SO_HeroRarenessInfo, ItemRareness> heroesRarenessInfo;

    public override void Init() {
        m_ui_tex_0 = Resources.LoadAll<Sprite>("Sprites/ui_tex_0");
        ui_tex_0 = new Dictionary<string, Sprite>();

        for (int i = 0; i < m_ui_tex_0.Length; i++) {
            ui_tex_0.Add(m_ui_tex_0[i].name, m_ui_tex_0[i]);
        }

        colors = new Dictionary<string, Color>();
        for (int i = 0; i < m_colors.Count; i++) {
            colors.Add(m_colors[i].name, m_colors[i].t);
        }

        textPresets = new Dictionary<TextStylePreset, TextMeshProPreset>();
        for (int i = 0; i < m_TextPresets.Count; i++) {
            textPresets.Add(m_TextPresets[i].preset, m_TextPresets[i]);
        }

        itemsInfo = new SO_AssetDB<SO_ItemInfo, ItemType>("ItemsInfo/Weapon", "ItemsInfo/Head");
        chestsInfo = new SO_AssetDB<SO_ChestInfo, ChestType>("ChestInfo");
        heroesInfo = new SO_AssetDB<SO_HeroInfo, HeroType>("HeroInfo");
        heroesRarenessInfo = new SO_AssetDB<SO_HeroRarenessInfo, ItemRareness>("HeroRarenessInfo");
    }

    [System.Serializable]
    public class NamedColor : NamedT<Color> {

    }

    [System.Serializable]
    public class NamedT<T> {
        public string name;
        public T t;
    }

    [System.Serializable]
    public class TextMeshProPreset {
        public TextStylePreset preset;
        public TMP_FontAsset fontAsset;
        public Material material;
    }

    public const string COLOR_BUTTON_GREEN = "BUTTON_GREEN";
    public const string COLOR_BUTTON_RED = "BUTTON_RED";
    public const string COLOR_BUTTON_GRAY = "BUTTON_GRAY";
    public const string COLOR_BUTTON_GOLD = "BUTTON_GOLD";
    public const string COLOR_BUTTON_MAGENTA = "BUTTON_MAGENTA";

}

public interface ITypeKey<TKey> {
    TKey GetKey();
}

public class SO_AssetDB<TObject, TKey> where TObject : ScriptableObject, ITypeKey<TKey> {

    private Dictionary<TKey, TObject> assetDict;
    private List<TObject> assetList;

    public SO_AssetDB(string resourcesLoadPath) {
        assetDict = new Dictionary<TKey, TObject>();
        assetList = new List<TObject>(Resources.LoadAll<TObject>(resourcesLoadPath));

        foreach (var a in assetList) {
            assetDict[a.GetKey()] = a;
        }
    }

    public SO_AssetDB(params string[] resourcesLoadPath) {
        assetDict = new Dictionary<TKey, TObject>();
        assetList = new List<TObject>();

        for (int i = 0; i < resourcesLoadPath.Length; i++) {
            assetList.AddRange(Resources.LoadAll<TObject>(resourcesLoadPath[i]));
        }

        foreach (var a in assetList) {
            assetDict[a.GetKey()] = a;
        }
    }

    public TObject GetAsset(TKey _type) {
        return assetDict[_type];
    }

    public List<TObject> GetAllAssets() {
        return assetList;
    }

}