using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



internal class NonActorTargetInfo_LaserCoords : NonActorTargetInfo
{
    public VectorUtils.LaserCoords m_laserCoords;

    public NonActorTargetInfo_LaserCoords(VectorUtils.LaserCoords laserCoords)
    {
        this.m_laserCoords = laserCoords;
    }
}
