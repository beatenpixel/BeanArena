using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects {
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/UIEffects/UIEffect_UVScroll", 103)]
    public class UIEffect_UVScroll : BaseMeshEffect {

        public Sprite refSprite;
        public float scrollSpeed = 1f;
        public Vector2 tiling;

        [Tooltip("Flip horizontally.")]
        [SerializeField]
        private bool m_Horizontal = false;

        [Tooltip("Flip vertically.")]
        [SerializeField]
        private bool m_Veritical = false;

        public bool horizontal {
            get { return m_Horizontal; }
            set {
                if (m_Horizontal == value) return;
                m_Horizontal = value;
                SetEffectParamsDirty();
            }
        }

        public bool vertical {
            get { return m_Veritical; }
            set {
                if (m_Veritical == value) return;
                m_Veritical = value;
                SetEffectParamsDirty();
            }
        }

        public override void ModifyMesh(VertexHelper vh, Graphic graphic) {
            if (!isActiveAndEnabled) return;

            var vt = default(UIVertex);
            for (var i = 0; i < vh.currentVertCount; i++) {
                vh.PopulateUIVertex(ref vt, i);
                var pos = vt.position;
                vt.position = new Vector3(
                    m_Horizontal ? -pos.x : pos.x,
                    m_Veritical ? -pos.y : pos.y
                );

                //vt.color = new Color(vt.uv0.x,vt.uv0.y,0,1);

                Debug.Log(refSprite.textureRect.position + " " + refSprite.textureRect.size);

                Vector2 bottomLeft = new Vector2(refSprite.textureRect.xMin / refSprite.texture.width, refSprite.textureRect.yMin / refSprite.texture.height);
                Vector2 topRight = new Vector2(refSprite.textureRect.xMax / refSprite.texture.width, refSprite.textureRect.yMax / refSprite.texture.height);

                float bottomLeftPacked = Packer.ToFloat(bottomLeft.x, bottomLeft.y);
                float topRightPacked = Packer.ToFloat(topRight.x, topRight.y);

                vt.uv0 = new Vector4(vt.uv0.x, vt.uv0.y, bottomLeftPacked, topRightPacked);

                float sizeX = rectTransform.sizeDelta.x;

                vt.uv2 = new Vector4(rectTransform.sizeDelta.x * tiling.x * 0.002f, rectTransform.sizeDelta.y * tiling.y * 0.002f, 0, 0);
                Debug.Log("uv2: " + vt.uv2.x);
                vh.SetUIVertex(vt, i);
            }
        }
    }
}
