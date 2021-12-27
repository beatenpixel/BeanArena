using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MicroCrew/GameAssets", fileName = "GameAssets")]
public class MAssets : SingletonScriptableObject<MAssets> {

    [SerializeField] private List<NamedColor> m_colors;

    private Sprite[] m_ui_tex_0;
    public static Dictionary<string, Sprite> ui_tex_0;
    public static Dictionary<string, Color> colors;

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
    }

    [System.Serializable]
    public class NamedColor : NamedT<Color> {

    }

    [System.Serializable]
    public class NamedT<T> {
        public string name;
        public T t;
    }

}
