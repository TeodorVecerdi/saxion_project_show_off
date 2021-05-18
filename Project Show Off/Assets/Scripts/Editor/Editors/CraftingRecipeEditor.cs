using System;
using Editor.Utils;
using Editor.Utils.Components;
using Runtime.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor {
    // [CustomEditor(typeof(CraftingRecipe))]
    public class CraftingRecipeEditor : UnityEditor.Editor {
        private CraftingRecipe recipe;
        private VisualElement rootElement;

        private ScrollView inputsContainer;
        private ScrollView outputsContainer;

        private void OnEnable() {
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnDisable() {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        public override VisualElement CreateInspectorGUI() {
            recipe = target as CraftingRecipe;
            rootElement = new VisualElement {name = "CraftingRecipeEditor"};

            var inputs = new VisualElement {name = "Inputs"};
            inputsContainer = inputs.AddGet(new ScrollView(ScrollViewMode.Vertical) {name = "InputsContainer"});
            foreach (var recipeInput in recipe!.Ingredients) {
                inputsContainer.Add(new ItemStackElement(this, recipeInput) {userData = true});
            }
            
            var middle = new VisualElement {name = "Middle"};
            middle.Add(new Image {name = "Arrow"});
            
            var outputs = new VisualElement {name = "Outputs"};
            outputsContainer = outputs.AddGet(new ScrollView(ScrollViewMode.Vertical) {name = "OutputsContainer"});
           
            
            rootElement.Add(inputs);
            rootElement.Add(middle);
            rootElement.Add(outputs);
            rootElement.AddStylesheet(Resources.Load<StyleSheet>("Stylesheets/CraftingRecipeEditor"));
            return rootElement;
        }

        private void OnUndoRedoPerformed() {
            Debug.Log("Undo/Redo performed");
            // Todo: Rebuild UI
        }

        public void RemoveItemStack(ItemStackElement itemStackElement) {
            var isInput = (bool) itemStackElement.userData;
            Undo.RegisterCompleteObjectUndo(recipe, "Removed item from recipe");
            if (isInput) {
                inputsContainer.Remove(itemStackElement);
                recipe.Ingredients.Remove(itemStackElement.ItemStack);
            }
        }
    }
}