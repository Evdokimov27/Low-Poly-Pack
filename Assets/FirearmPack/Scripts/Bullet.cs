using UnityEngine;

public enum CalibreType
{
	mm9,
	mm12,
	mm45,
	mm357,
	mm5_45,
	mm5_56,
	mm7_51,
	mm7_62,
	mm50,
	granade32,
	rocket
}

public class Bullet : MonoBehaviour
{
	public CalibreType calibre;
}
