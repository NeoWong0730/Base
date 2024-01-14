using Lib.Core;
using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageLoader : MonoBehaviour
{
    static string sAtlasPathFormat = "Atlas/{0}.spriteatlas";

    public enum EAssetType
    {
        None = 0,
        SpriteAtlas = 1,
        Sprite = 2,
    }

    private AsyncOperationHandle _handle;
    private string _sAtlasName;
    private string _sSpriteName;
    private bool bAutoSetNativeSize;
    private EAssetType eCurrentAssetType = EAssetType.None;
    private Sprite _sprite;
    
    private Image _image;
    private bool CheckImage()
    {
        if (_image)
            return true;

        if (TryGetComponent<Image>(out _image))
        {
            if (_image.sprite == null)
                _image.enabled = false;

            return true;
        }

        return false;
    }

    //private void Start()
    //{
    //    if (!_image)
    //    {
    //        if (TryGetComponent<Image>(out _image))
    //        {
    //            if (_image.sprite == null)
    //                _image.enabled = false;
    //
    //            _Load();
    //        }
    //        else
    //        {
    //            DebugUtil.LogErrorFormat("ImageLoader {0} 获取Image失败", name);
    //        }
    //    }
    //}

    public void LoadSprite(string atlasName, string spriteName, bool autoSetNativeSize = false)
    {
        if (string.Equals(_sAtlasName, atlasName, StringComparison.Ordinal) && string.Equals(_sSpriteName, spriteName, StringComparison.Ordinal))
        {
            return;
        }

        bAutoSetNativeSize = autoSetNativeSize;
        EAssetType aimAssetType = EAssetType.None;
        //如果 spriteName 为空,则 atlasName 设置为空，没有精灵就不需要加载图集
        if (string.IsNullOrWhiteSpace(spriteName))
        {
            atlasName = null;
        }
        else
        {
            aimAssetType = string.IsNullOrWhiteSpace(atlasName) ? EAssetType.Sprite : EAssetType.SpriteAtlas;
        }

        //判断是否需要重新加载资源
        bool needReload = false;
        if (aimAssetType != eCurrentAssetType)
        {
            needReload = true;
        }
        else
        {
            if (aimAssetType == EAssetType.SpriteAtlas)
            {
                if (!atlasName.Equals(_sAtlasName))
                {
                    needReload = true;
                }
            }
            else if (aimAssetType == EAssetType.Sprite)
            {
                if (!spriteName.Equals(_sSpriteName))
                {
                    needReload = true;
                }
            }
        }

        //设置新的name
        _sAtlasName = atlasName;
        _sSpriteName = spriteName;

        if (needReload)
        {
            _Release();
            eCurrentAssetType = aimAssetType;
            _Load();
        }
        else if (eCurrentAssetType == EAssetType.SpriteAtlas && _handle.IsDone && _handle.IsValid())
        {
            ImageSpriteAtlasLoader_Completed(_handle.Convert<SpriteAtlas>());
        }
    }

    private void ImageSpriteAtlasLoader_Completed(AsyncOperationHandle<SpriteAtlas> obj)
    {
        SpriteAtlas spriteAtlas = obj.Result;
        if (spriteAtlas)
        {
            if (CheckImage())
            {
                _image.enabled = true;
                if (_sprite)
                {
                    DestroyImmediate(_sprite);
                }
                _image.sprite = _sprite = spriteAtlas.GetSprite(_sSpriteName);
                if (!_sprite)
                {
                    DebugUtil.LogErrorFormat("ImageLoader {0} GetSprite失败 Atlas = {1} Sprite = {2}", name, _sAtlasName, _sSpriteName);
                }
                if (bAutoSetNativeSize)
                {
                    _image.SetNativeSize();
                }
            }
            else
            {
                DebugUtil.LogErrorFormat("ImageLoader {0} 获取Image失败 Atlas = {1} Sprite = {2}", name, _sAtlasName, _sSpriteName);
            }
        }
        else
        {
            DebugUtil.LogErrorFormat("ImageLoader {0} 资源Atlas失败 Atlas = {1} Sprite = {2}", name, _sAtlasName, _sSpriteName);
        }
    }

    private void ImageSpriteLoader_Completed(AsyncOperationHandle<Sprite> obj)
    {
        Sprite sprite = obj.Result;
        if (sprite)
        {
            if (CheckImage())
            {
                _image.enabled = true;
                _image.sprite = sprite;
                if (bAutoSetNativeSize)
                {
                    _image.SetNativeSize();
                }
            }
            else
            {
                DebugUtil.LogErrorFormat("ImageLoader {0} 获取Image失败 Sprite = {1}", name, _sSpriteName);
            }
        }
        else
        {
            DebugUtil.LogErrorFormat("ImageLoader {0} 加载Sprite失败  Sprite = {1}", name, _sSpriteName);
        }
    }

    private void OnDestroy()
    {
        _Release();
    }

    private void _Load()
    {
        if (!CheckImage())
            return;

        if (EAssetType.SpriteAtlas == eCurrentAssetType)
        {
            AsyncOperationHandle<SpriteAtlas> handle = default;
            AddressablesUtil.LoadAssetAsync<SpriteAtlas>(ref handle, string.Format(sAtlasPathFormat, _sAtlasName), ImageSpriteAtlasLoader_Completed);
            _handle = handle;
        }
        else if (EAssetType.Sprite == eCurrentAssetType)
        {
            AsyncOperationHandle<Sprite> handle = default;
            AddressablesUtil.LoadAssetAsync<Sprite>(ref handle, _sSpriteName, ImageSpriteLoader_Completed);
            _handle = handle;
        }
    }

    private void _Release()
    {        
        if (CheckImage())
        {
            _image.sprite = null;
            _image.enabled = false;
        }

        if (_sprite)
        {
            DestroyImmediate(_sprite);
        }
        _sprite = null;

        if (EAssetType.SpriteAtlas == eCurrentAssetType)
        {
            AddressablesUtil.Release<SpriteAtlas>(ref _handle, ImageSpriteAtlasLoader_Completed);
        }
        else if (EAssetType.Sprite == eCurrentAssetType)
        {
            AddressablesUtil.Release<Sprite>(ref _handle, ImageSpriteLoader_Completed);
        }
        eCurrentAssetType = EAssetType.None;
    }
}
