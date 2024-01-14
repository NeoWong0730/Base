using Framework;
using UnityEngine;
using System.Collections.Generic;

public class UIAtlas : ScriptableObject
{    
    [SerializeField]
    private Sprite[] _sprites;
    private Dictionary<string, Sprite> _spriteDics;    

    public Sprite GetSprite(string spriteName)
    {
        if (_spriteDics == null && _sprites != null)
        {
            _spriteDics = new Dictionary<string, Sprite>(_sprites.Length);
            for (int i = 0; i < _sprites.Length; ++i)
            {
                _spriteDics[_sprites[i].name] = _sprites[i];
            }
        }

        if (_spriteDics != null)
        {
            _spriteDics.TryGetValue(spriteName, out Sprite sprite);
            return sprite;
        }
        return null;
    }

#if UNITY_EDITOR
    public void InternalSetData(Sprite[] sprites)
    {        
        _sprites = sprites;
    }
#endif
}
