using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Game
{
    public class CardVisualShaderCode : Image
    {
        internal readonly string[] editions = new string[] { "REGULAR", "POLYCHROME", "NEGATIVE" };
        public static readonly int Rotation = Shader.PropertyToID("_Rotation");

        private Dictionary<CardVisual.ShaderType, Material> _materials =
            new Dictionary<CardVisual.ShaderType, Material>();

        private CardVisual.ShaderType currentShaderType;
        private CardVisual _visual;

        protected override void Awake()
        {
            base.Awake();
            _visual = GetComponentInParent<CardVisual>();
            UpdateCardMaterial();
        }

        private async UniTask<Material> CreateCardMaterial()
        {
            Material template =
                await Global.Get<ResourceSystem>().LoadAsync<Material>("Resource/Shaders/ShinyCard.mat");
            return new Material(template);
        }

        private bool _updating = false;

        private async void UpdateCardMaterial()
        {
            
            if (_visual.shaderType == CardVisual.ShaderType.None)
            {
                material = null;
                return;
            }
            if (_updating) return;
            _updating = true;
            if (!_materials.ContainsKey(_visual.shaderType))
            {
                Material newMaterial = await CreateCardMaterial();
                _materials.Add(_visual.shaderType, newMaterial);
            }

            material = _materials[_visual.shaderType];


            for (int i = 0; i < material.enabledKeywords.Length; i++)
            {
                material.DisableKeyword(material.enabledKeywords[i]);
            }

            switch (_visual.shaderType)
            {
                case CardVisual.ShaderType.Regular:
                    material.EnableKeyword("_EDITION_" + editions[0]);
                    break;
                case CardVisual.ShaderType.Polychrome:
                    material.EnableKeyword("_EDITION_" + editions[1]);
                    break;
                case CardVisual.ShaderType.Negative:
                    material.EnableKeyword("_EDITION_" + editions[2]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            currentShaderType = _visual.shaderType;
            _updating = false;
        }

        private void Update()
        {
            if (currentShaderType != _visual.shaderType)
            {
                UpdateCardMaterial();
            }

            // Get the current rotation as a quaternion
            Quaternion currentRotation = transform.parent.localRotation;

            // Convert the quaternion to Euler angles
            Vector3 eulerAngles = currentRotation.eulerAngles;

            // Get the X-axis angle
            float xAngle = eulerAngles.x;
            float yAngle = eulerAngles.y;

            // Ensure the X-axis angle stays within the range of -90 to 90 degrees
            xAngle = xAngle.ClampAngle(-90f, 90f);
            yAngle = yAngle.ClampAngle(-90f, 90);


            material.SetVector(Rotation,
                new Vector2(xAngle.Remap(-20, 20, -.5f, .5f),
                    yAngle.Remap(-20, 20, -.5f, .5f)));
        }
    }
}