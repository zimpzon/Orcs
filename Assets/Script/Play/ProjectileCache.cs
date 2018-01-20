using Assets.Script;
using UnityEngine;

public struct ProjectileInfo
{
    public ProjectileInfo(GameObject go)
    {
        Transform = go.transform;
        Renderer = go.GetComponent<SpriteRenderer>();
        Collider = go.GetComponent<CapsuleCollider2D>();
    }

    public Transform Transform;
    public SpriteRenderer Renderer;
    public CapsuleCollider2D Collider;
};

public class ProjectileCache : MonoBehaviour, IObjectFactory<ProjectileInfo>
{
    public static ProjectileCache Instance;

    public GameObject SpritePrefab;
    public SpriteData SpriteData;

    ReusableObject<ProjectileInfo> sprites_;

    private void Awake()
    {
        Instance = this;
    }

    ProjectileInfo CreateSpriteInfo()
    {
        var go = Instantiate<GameObject>(SpritePrefab);
        ProjectileInfo result = new ProjectileInfo(go);
        result.Transform.gameObject.SetActive(false);
        return result;
    }

    public ProjectileInfo CreateObject()
    {
        return CreateSpriteInfo();
    }

    void Start()
    {
        sprites_ = new ReusableObject<ProjectileInfo>(100, this);
	}

	public ProjectileInfo GetSprite()
    {
        var spriteInfo = sprites_.GetObject();
        spriteInfo.Transform.gameObject.SetActive(true);
        spriteInfo.Renderer.enabled = true;
        spriteInfo.Collider.enabled = true;
        return spriteInfo;
    }

    public void ReturnSprite(ProjectileInfo spriteInfo)
    {
        spriteInfo.Transform.gameObject.SetActive(false);
        spriteInfo.Renderer.enabled = false;
        spriteInfo.Collider.enabled = false;
        sprites_.ReturnObject(spriteInfo);
    }
}
