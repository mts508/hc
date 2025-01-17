using System.Collections.Generic;
using UnityEngine;

public class ScampVfxController : CopyableVfxControllerComponent
{
	[JointPopup("Joint for VFX")]
	public JointPopupProperty m_suitJoint;

	public List<AdditionalAttachedActorVfx.JointToVfx> m_persistentVfxList;

	private List<AttachedActorVFXInfo> m_vfxInstances = new List<AttachedActorVFXInfo>();

	private ActorModelData m_actorModelData;

	private void Start()
	{
		m_actorModelData = GetComponent<ActorModelData>();
		m_suitJoint.Initialize(base.gameObject);
		if (!(m_suitJoint.m_jointObject != null))
		{
			return;
		}
		for (int i = 0; i < m_persistentVfxList.Count; i++)
		{
			AdditionalAttachedActorVfx.JointToVfx jointToVfx = m_persistentVfxList[i];
			AttachedActorVFXInfo attachedActorVFXInfo = new AttachedActorVFXInfo(jointToVfx.m_vfxCommonPrefab, base.gameObject, jointToVfx.m_joint, jointToVfx.m_alignToRootOrientation, "MartyrCrystalVfx_" + jointToVfx.m_name, AttachedActorVFXInfo.FriendOrFoeVisibility.Both);
			if (attachedActorVFXInfo.HasVfxInstance())
			{
				attachedActorVFXInfo.SetInstanceLocalPosition(jointToVfx.m_localOffset);
				m_vfxInstances.Add(attachedActorVFXInfo);
			}
			else if (Application.isEditor)
			{
				Debug.LogWarning("Failed to spawn vfx on joint in " + GetType().ToString());
			}
		}
		while (true)
		{
			switch (2)
			{
			default:
				return;
			case 0:
				break;
			}
		}
	}

	private void LateUpdate()
	{
		object obj;
		if (m_actorModelData != null)
		{
			obj = m_actorModelData.m_parentActorData;
		}
		else
		{
			obj = null;
		}
		ActorData actorData = (ActorData)obj;
		if (!m_suitJoint.IsInitialized() || !(m_suitJoint.m_jointObject != null))
		{
			return;
		}
		while (true)
		{
			bool flag = !IsSuitVisuallyShown();
			int num;
			if (!(actorData == null))
			{
				if (actorData.IsActorVisibleToClient())
				{
					if (!(actorData.GetActorModelData() == null))
					{
						num = (actorData.GetActorModelData().IsVisibleToClient() ? 1 : 0);
					}
					else
					{
						num = 1;
					}
				}
				else
				{
					num = 0;
				}
			}
			else
			{
				num = 1;
			}
			bool flag2 = (byte)num != 0;
			for (int i = 0; i < m_vfxInstances.Count; i++)
			{
				AttachedActorVFXInfo attachedActorVFXInfo = m_vfxInstances[i];
				int actorVisible;
				if (flag2)
				{
					actorVisible = ((!flag) ? 1 : 0);
				}
				else
				{
					actorVisible = 0;
				}
				attachedActorVFXInfo.UpdateVisibility((byte)actorVisible != 0, true);
			}
			while (true)
			{
				switch (7)
				{
				default:
					return;
				case 0:
					break;
				}
			}
		}
	}

	public bool IsSuitVisuallyShown()
	{
		if (m_suitJoint.IsInitialized() && m_suitJoint.m_jointObject != null)
		{
			Vector3 localScale = m_suitJoint.m_jointObject.transform.localScale;
			return localScale.x > 0.1f;
		}
		return true;
	}
}
