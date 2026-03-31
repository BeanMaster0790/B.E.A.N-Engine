using Bean.UI;
using Microsoft.Xna.Framework;

namespace Bean.Editor;

public static class EditorUIObjects
{
#if DEBUG
     
     public static Color InputColour = new Color(45, 73, 73);
     public static Color ButtonColour = Color.DimGray;
     public static Color BaseColour = new Color(64, 65, 70);
     
     public static UIAlignContainer CreateFakePaddingBox(UIAlignContainer parent)
     {
          return new UIAlignContainer($"{parent.Name}-FakePadding")
          {
               Width = parent.Width - 5, Height = 1, Colour = parent.Colour, Parent = parent,
          };
     }
     
     public static UIText CreateTopBarOption(UIAlignContainer parent, string text)
     {
          UIText newPropButton = new UIText(text)
          {
               Parent = parent,
               Colour = Color.White,
               Width = 100,
               Height = 25,
               Text = text,
               FontSize = 16
          };

          newPropButton.OnHoverEnter += (sender, args) => { newPropButton.Colour = Color.Wheat; };

          newPropButton.OnHoverExit += (sender, args) => { newPropButton.Colour = Color.White; };

          return newPropButton;
     }

     public static UIAlignContainer CreateListOption(UIAlignContainer parent, string text, AlignDirection alignDirection, HorizontalAlign horizontalAlign, VerticalAlign verticalAlign, Color colour, out UIText optionText)
     {
          UIAlignContainer addonContainer;
          
          if (parent != null)
          {
               addonContainer = new UIAlignContainer($"AddonContainer-{text}")
               {
                    Width = parent.Width - 5,
                    Height = 25,
                    Colour = colour,
                    Parent = parent,
                    AlignDirection = alignDirection,
                    HorizontalAlign = horizontalAlign,
                    VerticalAlign = verticalAlign,
                    Layer = 0.1f
               };
          }
          else
          {
               addonContainer = new UIAlignContainer($"AddonContainer-{text}")
               {
                    Width = 100,
                    Height = 25,
                    Colour = colour,
                    AlignDirection = alignDirection,
                    HorizontalAlign = horizontalAlign,
                    VerticalAlign = verticalAlign,
                    Layer = 0.1f
               };
          }
          

          optionText = new UIText($"AddonNameText-{text}")
          {
               Width = 20,
               Height = 25,
               FontSize = 24,
               Text = text,
               Parent = addonContainer,
               Colour = Color.White,
               Layer = 0.15f
          };
          
          return addonContainer;
     }
#endif
}
