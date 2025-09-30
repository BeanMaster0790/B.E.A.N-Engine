using System;
using System.Collections.Generic;
using System.Linq;
using Bean.Graphics;
using Bean.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bean.UI;

public class UIAlignContainer : UIContainer
{

    public int Spacing;

    public VerticalAlign VerticalAlign;

    public HorizontalAlign HorizontalAlign;

    public AlignDirection AlignDirection;

    public bool isScrollable;


    public float _scroll;

    public override void Start()
    {
        base.Start();


        this.OnHover += (object sender, EventArgs e) => 
        {
            ScreenProp[] children = this._children.ToArray();

            if(!this.isScrollable)
                return;


            if(InputManager.Instance.HasScrolledDown() && this._children.Count > 0)
            {
                if(this.AlignDirection == AlignDirection.Horizontal)
                {
                    if(children[children.Length - 1].LocalPosition.X + children[children.Length - 1].Width * this._UIScene.RenderScale > this.Width * this._UIScene.RenderScale)
                        this._scroll += 7;
                }
                else
                {
                    if(children[children.Length - 1].LocalPosition.Y + children[children.Length - 1].Height * this._UIScene.RenderScale > this.Height * this._UIScene.RenderScale)
                        this._scroll += 7;
                }
            }
            if(InputManager.Instance.HasScrolledUp()  && this._children.Count > 0)
            {
                if(this.AlignDirection == AlignDirection.Horizontal)
                {
                    if(children[0].LocalPosition.X < 0)
                        this._scroll -= 7;
                }
                else
                {
                    if(children[0].LocalPosition.Y < 0)
                        this._scroll -= 7;
                }
            }

        };

    }

    public override void Update()
    {
        base.Update();

        ScreenProp[] currentChildren = this._children.ToArray();


        if (currentChildren.Length == 0)
            return;

        if (this.AlignDirection == AlignDirection.Horizontal)
            ChildrenStackHorizontal(currentChildren);
        else
            ChildrenStackVertical(currentChildren);


        AlignVertical(currentChildren);
        AlignHorizontal(currentChildren);

        if(this.AlignDirection == AlignDirection.Horizontal)
            SnapHorizontalScroll(currentChildren);
        else
            SnapVerticalScroll(currentChildren);
    }

    private void SnapHorizontalScroll(ScreenProp[] currentChildren)
    {
        float childrenWidth = currentChildren[currentChildren.Length - 1].LocalPosition.X + this._scroll + currentChildren[currentChildren.Length - 1].Width * this._UIScene.RenderScale;

        float maxScroll = (childrenWidth - (this.Width * this._UIScene.RenderScale)) + 10 * this._UIScene.RenderScale;

        float minScroll = currentChildren[0].LocalPosition.X + this._scroll;

        if (this._scroll > maxScroll && maxScroll > 0)
            this._scroll = maxScroll;

        else if (this._scroll < minScroll && minScroll < 0)
            this._scroll = minScroll;

        else if (maxScroll <= 0 && minScroll >= 0)
            this._scroll = 0;
    }

    private void SnapVerticalScroll(ScreenProp[] currentChildren)
    {
        float childrenHeight = currentChildren[currentChildren.Length - 1].LocalPosition.Y + this._scroll + currentChildren[currentChildren.Length - 1].Height * this._UIScene.RenderScale;

        float maxScroll = (childrenHeight - (this.Height * this._UIScene.RenderScale)) + 10 * this._UIScene.RenderScale;

        float minScroll = currentChildren[0].LocalPosition.Y + this._scroll;

        if (this._scroll > maxScroll && maxScroll > 0)
            this._scroll = maxScroll;

        else if (this._scroll < minScroll && minScroll < 0)
            this._scroll = minScroll;

        else if (maxScroll <= 0 && minScroll >= 0)
            this._scroll = 0;

    }

    private void ChildrenStackVertical(ScreenProp[] currentChildren)
    {
        Vector2 currentPosition =  new Vector2(0, -this._scroll);

        foreach (ScreenProp prop in currentChildren)
        {
            prop.LocalPosition = currentPosition;

            currentPosition += new Vector2(0, (prop.Height * this._UIScene.RenderScale) + (Spacing * this._UIScene.RenderScale));
        }
    }


    private void ChildrenStackHorizontal(ScreenProp[] currentChildren)
    {
        Vector2 currentPosition =  new Vector2(-this._scroll, 0);

        foreach (ScreenProp prop in currentChildren)
        {
            prop.LocalPosition = currentPosition;

            currentPosition += new Vector2((prop.Width * this._UIScene.RenderScale) + (Spacing * this._UIScene.RenderScale), 0);
        }
    }

    private void AlignVertical(ScreenProp[] currentChildren)
    {
        float MaxY = currentChildren[currentChildren.Length - 1].LocalPosition.Y + currentChildren[currentChildren.Length - 1].Height * this._UIScene.RenderScale;

        float totalHeight = MaxY + ((this.AlignDirection == AlignDirection.Vertical) ? this._scroll : 0);

        if (this.VerticalAlign == VerticalAlign.Center)
        {
            float totalHalfHeight = totalHeight / 2;

            float finalHeight = totalHalfHeight - this.Height * this._UIScene.RenderScale / 2;

            foreach (ScreenProp prop in currentChildren)
            {
                prop.LocalPosition.Y -= finalHeight;

                if (this.AlignDirection != AlignDirection.Vertical)
                    prop.LocalPosition.Y = this.LocalPosition.Y * this._UIScene.RenderScale + ((this.Height * this._UIScene.RenderScale) / 2) - ((prop.Height * this._UIScene.RenderScale) / 2);
            }
        }
        else if (this.VerticalAlign == VerticalAlign.Bottom)
        {
            float finalHeight = totalHeight - this.Height * this._UIScene.RenderScale;

            foreach (ScreenProp prop in currentChildren)
            {
                prop.LocalPosition.Y -= finalHeight;
            }
        }
    }

    private void AlignHorizontal(ScreenProp[] currentChildren)
    {
        float MaxX = currentChildren[currentChildren.Length - 1].LocalPosition.X + currentChildren[currentChildren.Length - 1].Width * this._UIScene.RenderScale;

        float totalWidth = MaxX + ((this.AlignDirection == AlignDirection.Horizontal) ? this._scroll : 0);

        if (this.HorizontalAlign == HorizontalAlign.Center)
        {
            float totalHalfWidth = totalWidth / 2;

            float finalWidth = totalHalfWidth - this.Width * this._UIScene.RenderScale / 2;

            foreach (ScreenProp prop in currentChildren)
            {
                prop.LocalPosition.X -= finalWidth;

                if (this.AlignDirection != AlignDirection.Vertical)
                    prop.LocalPosition.X = this.LocalPosition.X * this._UIScene.RenderScale + ((this.Width * this._UIScene.RenderScale) / 2) - ((prop.Width * this._UIScene.RenderScale) / 2);
            }
        }
        else if (this.HorizontalAlign == HorizontalAlign.Right)
        {
            float finalWidth = totalWidth - this.Width * this._UIScene.RenderScale;

            foreach (ScreenProp prop in currentChildren)
            {
                prop.LocalPosition.X -= finalWidth;
            }
        }
    }
}

public enum VerticalAlign
{
    Top,
    Center,
    Bottom
}

public enum HorizontalAlign
{
    Left,
    Center,
    Right
}

public enum AlignDirection
{
    Vertical,
    Horizontal
}
