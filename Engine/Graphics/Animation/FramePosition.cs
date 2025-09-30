using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bean.Graphics.Animation
{
    public class FramePosition
    {
        public int x;
        public int y;
        public int w;
        public int h;
    }

    public class Frame
    {
        public string filename;
        public FramePosition frame;
    }

    public class FrameTag
    {
        public string name;
    }

    public class MetaData
    {
        public FrameTag[] frameTags;

    }

    public class ImageFrames
    {
        public Frame[] frames;

        public MetaData meta;
    }

}
