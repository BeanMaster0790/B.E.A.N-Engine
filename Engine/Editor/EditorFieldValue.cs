using System.Reflection;
using Bean.Debug;
using Bean.JsonVariables;
using Bean.Scenes;
using Bean.UI;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Bean.Editor;

public abstract class EditorFieldValue
{
    public string FieldName { get; set; }
    public IBeanJson AddonJson { get; set; }
    public Addon AttachedAddon { get; set; }
    
    public PropEditor PropEditor { get; set; }

    public EditorFieldValue(string fieldName, IBeanJson addonJson, Addon attachedAddon, PropEditor attachedPropEditor)
    {
        FieldName = fieldName;
        AddonJson = addonJson;
        AttachedAddon = attachedAddon;
        
        PropEditor = attachedPropEditor;
    }
}

public abstract class EditorFieldValue<TValue> :  EditorFieldValue
{
    public EditorFieldValue(string fieldName, IBeanJson addonJson, Addon attachedAddon, PropEditor editor) : base(fieldName, addonJson, attachedAddon, editor)
    {
    }
    
    public abstract UIAlignContainer CreateInputContainer(UIAlignContainer fieldContainer, UIScene scene);
    public abstract TValue GetValue();
    
    public TValue GetValueFromFieldName()
    {
        PropertyInfo propertyInfo = this.AddonJson.GetType().GetProperty(FieldName);
        
        if(propertyInfo != null)
            return (TValue)propertyInfo.GetValue(AddonJson);
        
        FieldInfo fieldInfo = this.AddonJson.GetType().GetField(FieldName);
        
        if (fieldInfo != null)
            return (TValue)fieldInfo.GetValue(AddonJson);

        return default(TValue);
    }

    public void SetValueFromFieldName(TValue value)
    {
        PropertyInfo propertyInfo = this.AddonJson.GetType().GetProperty(FieldName);

        if (propertyInfo != null)
        {
            propertyInfo.SetValue(AddonJson, value);
            PropEditor.UpdateProp();
            return;
        }

        throw new Exception("How did it even make it this far?");
    }

}

public class EditorStringField : EditorFieldValue<string>
{
    private UIInputText _inputText;
    
    public EditorStringField(string fieldName, IBeanJson addonJson, Addon attachedAddon, PropEditor editor) : base(fieldName, addonJson, attachedAddon, editor)
    {
    }

    public override UIAlignContainer CreateInputContainer(UIAlignContainer fieldContainer,  UIScene scene)
    {
        fieldContainer.Height = 65;
        
        UIAlignContainer fieldInputContainer = new UIAlignContainer($"{this.FieldName}-Input")
        {
            Width = fieldContainer.Width - 5,
            Height = 25,
            AlignDirection = AlignDirection.Vertical,
            Colour = new Color(45, 73, 73),
            Parent = fieldContainer,
        };

        this._inputText = new UIInputText($"{this.FieldName}-InputText")
        {
            Width = 20,
            Height = 25,
            FontSize = 24,
            InputtedText = GetValueFromFieldName(),
            PlaceHolderText = "Enter text...",
            Parent = fieldInputContainer,
            Colour = Color.White
        };
        
        this._inputText.OnTextInput += (sender, args) =>
        {
            if(string.IsNullOrEmpty(this._inputText.InputtedText))
                return;
            
            SetValueFromFieldName(this._inputText.InputtedText);
        };
        
        scene.AddUIProp(this._inputText);
        
        return fieldInputContainer;
    }

    public override string GetValue()
    {
        return this._inputText.Text;
    }
}

public class EditorVector2FieldValue : EditorFieldValue<Vector2>
{
    UIInputText _inputTextX;
    UIInputText _inputTextY;
    
    public EditorVector2FieldValue(string fieldName, IBeanJson addonJson, Addon attachedAddon, PropEditor editor) : base(fieldName, addonJson, attachedAddon, editor)
    {
    }


    public override UIAlignContainer CreateInputContainer(UIAlignContainer fieldContainer, UIScene scene)
    {
        fieldContainer.Height = 65;
        
        UIAlignContainer fieldInputContainer = new UIAlignContainer($"{this.FieldName}-Input")
        {
            Width = fieldContainer.Width - 5,
            Height = 25,
            AlignDirection = AlignDirection.Horizontal,
            Colour = new Color(64, 65, 70),
            Parent = fieldContainer,
            Spacing = 5
        };

        UIAlignContainer XInputContainer = new UIAlignContainer($"{this.FieldName}-XInputContainer")
        {
            Width = fieldInputContainer.Width / 2 - 5,
            Height = 25,
            Colour = new Color(45, 73, 73),
            Parent = fieldInputContainer,
        };
        
        this._inputTextX = new UIInputText($"{this.FieldName}-XInputText")
        {
            Width = 20,
            Height = 25,
            FontSize = 24,
            InputtedText = GetValueFromFieldName().X.ToString(),
            PlaceHolderText = "Enter text...",
            Parent = XInputContainer,
            Colour = Color.White,
            InputType = UIInputText.TextInputType.Decimal
        };
        
        this._inputTextX.OnTextInput += (sender, args) =>
        {
            SetValueFromFieldName(GetValue());
        };
        
        UIAlignContainer YInputContainer = new UIAlignContainer($"{this.FieldName}-YInputContainer")
        {
            Width = fieldInputContainer.Width / 2 - 5,
            Height = 25,
            Colour = new Color(45, 73, 73),
            Parent =  fieldInputContainer,
        };
        
        this._inputTextY = new UIInputText($"{this.FieldName}-XInputText")
        {
            Width = 20,
            Height = 25,
            FontSize = 24,
            InputtedText = GetValueFromFieldName().Y.ToString(),
            PlaceHolderText = "Enter text...",
            Parent = YInputContainer,
            Colour = Color.White,
            InputType = UIInputText.TextInputType.Decimal
        };
        
        this._inputTextY.OnTextInput += (sender, args) =>
        {
            SetValueFromFieldName(GetValue());
        };
        
        scene.AddUIProp(XInputContainer);
        scene.AddUIProp(this._inputTextX);
        scene.AddUIProp(YInputContainer);
        scene.AddUIProp(this._inputTextY);

        return fieldInputContainer;
    }

    public override Vector2 GetValue()
    {
        float x;
        float y;
        
        if (float.TryParse(this._inputTextX.InputtedText, out float valueX))
        {
            x = valueX;
        }
        else
        {
            x = 0f;
        }
        
        if (float.TryParse(this._inputTextY.InputtedText, out float valueY))
        {
            y = valueY;
        }
        else
        {
            y = 0f;
        }
        
        return new Vector2(x, y);
    }
}

public class EditorColourFieldValue : EditorFieldValue<JsonColour>
{
    UIInputText _inputTextR;
    UIInputText _inputTextG;
    UIInputText _inputTextB;
    UIInputText _inputTextA;
    
    public EditorColourFieldValue(string fieldName, IBeanJson addonJson, Addon attachedAddon, PropEditor editor) : base(fieldName, addonJson, attachedAddon, editor)
    {
    }


    public override UIAlignContainer CreateInputContainer(UIAlignContainer fieldContainer, UIScene scene)
    {
        fieldContainer.Height = 65;
        
        UIAlignContainer fieldInputContainer = new UIAlignContainer($"{this.FieldName}-Input")
        {
            Width = fieldContainer.Width - 5,
            Height = 25,
            AlignDirection = AlignDirection.Horizontal,
            Colour = new Color(64, 65, 70),
            Parent = fieldContainer,
            Spacing = 2
        };

        UIAlignContainer RInputContainer = new UIAlignContainer($"{this.FieldName}-RInputContainer")
        {
            Width = fieldInputContainer.Width / 4 - 2,
            Height = 25,
            Colour = new Color(45, 73, 73),
            Parent = fieldInputContainer,
        };
        
        this._inputTextR = new UIInputText($"{this.FieldName}-RInputText")
        {
            Width = 20,
            Height = 25,
            FontSize = 24,
            InputtedText = GetValueFromFieldName().R.ToString(),
            PlaceHolderText = "Enter text...",
            Parent = RInputContainer,
            Colour = Color.White,
            InputType = UIInputText.TextInputType.Int
        };
        
        this._inputTextR.OnTextInput += (sender, args) =>
        {
            SetValueFromFieldName(GetValue());
        };
        
        UIAlignContainer GInputContainer = new UIAlignContainer($"{this.FieldName}-GInputContainer")
        {
            Width = fieldInputContainer.Width / 4 - 2,
            Height = 25,
            Colour = new Color(45, 73, 73),
            Parent =  fieldInputContainer,
        };
        
        this._inputTextG = new UIInputText($"{this.FieldName}-GInputText")
        {
            Width = 20,
            Height = 25,
            FontSize = 24,
            InputtedText = GetValueFromFieldName().G.ToString(),
            PlaceHolderText = "Enter text...",
            Parent = GInputContainer,
            Colour = Color.White,
            InputType = UIInputText.TextInputType.Int
        };
        
        this._inputTextG.OnTextInput += (sender, args) =>
        {
            SetValueFromFieldName(GetValue());
        };
        
        UIAlignContainer BInputContainer = new UIAlignContainer($"{this.FieldName}-BInputContainer")
        {
            Width = fieldInputContainer.Width / 4 - 2,
            Height = 25,
            Colour = new Color(45, 73, 73),
            Parent =  fieldInputContainer,
        };
        
        this._inputTextB = new UIInputText($"{this.FieldName}-BInputText")
        {
            Width = 20,
            Height = 25,
            FontSize = 24,
            InputtedText = GetValueFromFieldName().B.ToString(),
            PlaceHolderText = "Enter text...",
            Parent = BInputContainer,
            Colour = Color.White,
            InputType = UIInputText.TextInputType.Int
        };
        
        this._inputTextB.OnTextInput += (sender, args) =>
        {
            SetValueFromFieldName(GetValue());
        };
        
        UIAlignContainer AInputContainer = new UIAlignContainer($"{this.FieldName}-AInputContainer")
        {
            Width = fieldInputContainer.Width / 4 - 2,
            Height = 25,
            Colour = new Color(45, 73, 73),
            Parent =  fieldInputContainer,
        };
        
        this._inputTextA = new UIInputText($"{this.FieldName}-AInputText")
        {
            Width = 20,
            Height = 25,
            FontSize = 24,
            InputtedText = GetValueFromFieldName().A.ToString(),
            PlaceHolderText = "Enter text...",
            Parent = AInputContainer,
            Colour = Color.White,
            InputType = UIInputText.TextInputType.Int
        };
        
        this._inputTextA.OnTextInput += (sender, args) =>
        {
            SetValueFromFieldName(GetValue());
        };
        
        scene.AddUIProp(RInputContainer);
        scene.AddUIProp(this._inputTextR);
        scene.AddUIProp(GInputContainer);
        scene.AddUIProp(this._inputTextG);
        scene.AddUIProp(BInputContainer);
        scene.AddUIProp(this._inputTextB);
        scene.AddUIProp(AInputContainer);
        scene.AddUIProp(this._inputTextA);

        return fieldInputContainer;
    }

    public override JsonColour GetValue()
    {
        int r;
        int g;
        int b;
        int a;
        
        if (int.TryParse(this._inputTextR.InputtedText, out int valueR))
        {
            r = valueR;
        }
        else
        {
            r = 0;
        }
        
        if (int.TryParse(this._inputTextG.InputtedText, out int valueG))
        {
            g = valueG;
        }
        else
        {
            g = 0;
        }
        
        if (int.TryParse(this._inputTextB.InputtedText, out int valueB))
        {
            b = valueB;
        }
        else
        {
            b = 0;
        }
        
        if (int.TryParse(this._inputTextA.InputtedText, out int valueA))
        {
            a = valueB;
        }
        else
        {
            a = 0;
        }
        
        return new JsonColour{R = r, G = g, B = b, A = a};
    }
}

class EditorIntFieldValue : EditorFieldValue<int>
{
    private UIInputText _inputText;
    
    public EditorIntFieldValue(string fieldName, IBeanJson addonJson, Addon attachedAddon, PropEditor editor) : base(fieldName, addonJson, attachedAddon, editor)
    {
    }

    public override UIAlignContainer CreateInputContainer(UIAlignContainer fieldContainer,  UIScene scene)
    {
        fieldContainer.Height = 65;
        
        UIAlignContainer fieldInputContainer = new UIAlignContainer($"{this.FieldName}-Input")
        {
            Width = fieldContainer.Width - 5,
            Height = 25,
            AlignDirection = AlignDirection.Vertical,
            Colour = new Color(45, 73, 73),
            Parent = fieldContainer,
        };

        this._inputText = new UIInputText($"{this.FieldName}-InputText")
        {
            Width = 20,
            Height = 25,
            FontSize = 24,
            InputtedText = GetValueFromFieldName().ToString(),
            PlaceHolderText = "Enter text...",
            Parent = fieldInputContainer,
            Colour = Color.White,
            InputType = UIInputText.TextInputType.Int
        };

        this._inputText.OnTextInput += (sender, args) =>
        {
            SetValueFromFieldName(GetValue());
        };
        
        scene.AddUIProp(this._inputText);
        
        return fieldInputContainer;
    }

    public override int GetValue()
    {
        if (int.TryParse(this._inputText.InputtedText, out int value))
        {
            return value;
        }
        else
        {
            return 0;
        }
    }
}

class EditorFloatFieldValue : EditorFieldValue<float>
{
    private UIInputText _inputText;
    
    public EditorFloatFieldValue(string fieldName, IBeanJson addonJson, Addon attachedAddon, PropEditor editor) : base(fieldName, addonJson, attachedAddon, editor)
    {
    }

    public override UIAlignContainer CreateInputContainer(UIAlignContainer fieldContainer,  UIScene scene)
    {
        fieldContainer.Height = 65;
        
        UIAlignContainer fieldInputContainer = new UIAlignContainer($"{this.FieldName}-Input")
        {
            Width = fieldContainer.Width - 5,
            Height = 25,
            AlignDirection = AlignDirection.Vertical,
            Colour = new Color(45, 73, 73),
            Parent = fieldContainer,
        };

        this._inputText = new UIInputText($"{this.FieldName}-InputText")
        {
            Width = 20,
            Height = 25,
            FontSize = 24,
            InputtedText = GetValueFromFieldName().ToString(),
            PlaceHolderText = "Enter text...",
            Parent = fieldInputContainer,
            Colour = Color.White,
            InputType = UIInputText.TextInputType.Decimal
        };

        this._inputText.OnTextInput += (sender, args) =>
        {
            SetValueFromFieldName(GetValue());
        };
        
        scene.AddUIProp(this._inputText);
        
        return fieldInputContainer;
    }

    public override float GetValue()
    {
        if (float.TryParse(this._inputText.InputtedText, out float value))
        {
            return value;
        }
        else
        {
            return 0f;
        }
    }
}