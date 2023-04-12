using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectLobby
{

    public struct Size
    {
        public double m_Width;
        public double m_Height;

        public Size() { }

        public Size(double i_Width, double i_Height)
        {
            m_Width = i_Width;
            m_Height = i_Height;
        }

        public Size SetAndGetSize(double i_Width, double i_Height)
        {
            m_Width = i_Width;
            m_Height = i_Height;

            return this;
        }

        public void SetSize(double i_Width, double i_Height)
        {
            m_Width = i_Width;
            m_Height = i_Height;
        }

        public static bool operator ==(Size i_P1, Size i_P2)
        {
            return (i_P1.m_Width == i_P2.m_Width) && (i_P1.m_Height == i_P2.m_Height);
        }

        public static bool operator !=(Size i_P1, Size i_P2)
        {
            return (i_P1.m_Width != i_P2.m_Width) || (i_P1.m_Height != i_P2.m_Height);
        }
    }
}
