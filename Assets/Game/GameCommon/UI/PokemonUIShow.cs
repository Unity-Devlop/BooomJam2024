﻿using System;
using cfg;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Game
{
    public class PokemonUIShow : MonoBehaviour
    {
        public Image BG;

        public TextMeshProUGUI nameText;

        // public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI passiveSkillDescriptionText;
        public SkeletonGraphic skeletonGraphic;

        public void UnBind()
        {
            skeletonGraphic.gameObject.SetActive(false);
            nameText.text = "";
            // descriptionText.text = "";
            passiveSkillDescriptionText.text = "";
        }

        public async void Bind(HuluData data)
        {
            nameText.text = data.name;
            // descriptionText.text = data.config.Desc;
            passiveSkillDescriptionText.text = data.passiveSkillConfig.Desc;

            try
            {
                skeletonGraphic.skeletonDataAsset = await Global.Get<ResourceSystem>().LoadPokemonSpine(data.id);
            }
            catch (InvalidKeyException e)
            {
                Global.LogWarning($"没有找到精灵{data.id}的Spine资源");
            }

            skeletonGraphic.gameObject.SetActive(true);
            skeletonGraphic.Initialize(true);
            skeletonGraphic.AnimationState.SetAnimation(0, Consts.Animation.BattlePokemonIdleAnim, true);
        }
    }
}