using System.Runtime.InteropServices;
using UnityEngine.Networking;

public class NanoSmith_SyncComponent : NetworkBehaviour
{
	[SyncVar]
	internal int m_extraAbsorbOnVacuumBomb;

	public int Networkm_extraAbsorbOnVacuumBomb
	{
		get
		{
			return m_extraAbsorbOnVacuumBomb;
		}
		[param: In]
		set
		{
			SetSyncVar(value, ref m_extraAbsorbOnVacuumBomb, 1u);
		}
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			while (true)
			{
				switch (5)
				{
				case 0:
					break;
				default:
					writer.WritePackedUInt32((uint)m_extraAbsorbOnVacuumBomb);
					return true;
				}
			}
		}
		bool flag = false;
		if ((base.syncVarDirtyBits & 1) != 0)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.WritePackedUInt32((uint)m_extraAbsorbOnVacuumBomb);
		}
		if (!flag)
		{
			writer.WritePackedUInt32(base.syncVarDirtyBits);
		}
		return flag;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		if (initialState)
		{
			while (true)
			{
				switch (5)
				{
				case 0:
					break;
				default:
					m_extraAbsorbOnVacuumBomb = (int)reader.ReadPackedUInt32();
					return;
				}
			}
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			m_extraAbsorbOnVacuumBomb = (int)reader.ReadPackedUInt32();
		}
	}
}
