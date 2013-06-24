using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2MumbleLinkTesting
{
    class MumbleLink
    {
        public uint uiVersion { get; set; }
        public uint uiTick { get; set; }
        public float[] fAvatarPosition { get; set; }
        public float[] fAvatarFront { get; set; }
        public float[] fAvatarTop { get; set; }
        public char[] name { get; set; }
        public float[] fCameraPosition { get; set; }
        public float[] fCameraFront { get; set; }
        public float[] fCameraTop { get; set; }
        public char[] identity { get; set; }
        public uint context_len { get; set; }
        public char[] context { get; set; }
        public char[] description { get; set; }

        public MumbleLink() { }

        public MumbleLink(uint uiVersion, uint uiTick, float[] fAvatarPosition, float[] fAvatarFront, float[] fAvatarTop, char[] name,
                            float[] fCameraPosition, float[] fCameraFront, float[] fCameraTop, char[] identity, uint context_len,
                            char[] context, char[] description)
        {
            this.uiVersion = uiVersion;
            this.uiTick = uiTick;
            this.fAvatarPosition = fAvatarPosition;
            this.fAvatarFront = fAvatarFront;
            this.fAvatarTop = fAvatarTop;
            this.name = name;
            this.fCameraPosition = fCameraPosition;
            this.fCameraFront = fCameraFront;
            this.fCameraTop = fCameraTop;
            this.identity = identity;
            this.context_len = context_len;
            this.context = context;
            this.description = description;
        }
    }
}
