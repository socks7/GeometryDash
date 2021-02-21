using System;
using System.Collections.Generic;
using System.Text;

namespace _
{
    class gdGraphics
    {
        public GameHack[,] gameHooks;
        public void makeGrid(int x, int y)
        {
            gameHooks = new GameHack[x,y];
            for (int y1 = 0; y1<y;y1++)
            {
                for (int x1 = 0; x1<x; x1++)
                {
                    gameHooks[x1, y1] = new GameHack();
                    gameHooks[x1, y1].attach("GeometryDash", new long[] { 0x003222D0, 0x164, 0x274, (long)(4 * (y1*x+x1)+4), 0x20, 0x8, 0x0, 0xD8 });
                }
            }
        }

        public void reAttach()
        {
            foreach (GameHack hook in gameHooks)
            {
                hook.reAttach();
            }
        }

        public void setVisible(int x, int y, bool value)
        {
            gameHooks[x, y].WriteBytes(new byte[] { BitConverter.GetBytes(value)[0] });
        }

        public bool getVisible(int x, int y)
        {
            return Convert.ToBoolean(gameHooks[x, y].getByteArray()[0]);
        }
    }
}
