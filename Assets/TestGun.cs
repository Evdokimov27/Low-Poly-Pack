using System.Collections.Generic;
using UnityEngine;

public class TestGun : MonoBehaviour
{
	public List<Camera> cameras = new List<Camera>();
	public List<Weapon> weapons = new List<Weapon>();

	private void Start()
	{
		ActivateCamera(0); // ¬ключаем первую камеру по умолчанию
	}

	public void ActivateCamera(int index)
	{
		for (int i = 0; i < cameras.Count; i++)
		{
			if (cameras[i] != null)
			{
				cameras[i].enabled = (i == index);
			}
		}
	}

	public void RandomizeAttachment()
	{
		foreach (var weapon in weapons)
		{
			if (weapon != null)
			{
				weapon.ActivateRandomAttachments();
			}
		}
	}
}
