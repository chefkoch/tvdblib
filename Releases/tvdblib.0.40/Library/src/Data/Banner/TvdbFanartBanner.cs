﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Net;

namespace TvdbConnector.Data.Banner
{
  /// <summary>
  /// Fan Art is high quality artwork that is displayed in the background of 
  /// HTPC menus. Since fan art is displayed behind other content in most cases, 
  /// we place more restrictions on the formatting of the image. 
  /// 
  /// The resolution is either 1920x1080 or 1280x720...
  /// 
  /// More information: http://thetvdb.com/wiki/index.php/Fan_Art
  /// </summary>
  [Serializable]
  public class TvdbFanartBanner : TvdbBanner
  {
    #region private fields
    private String m_thumbPath;
    private String m_vignettePath;
    private Image m_bannerThumb;
    private Image m_vignette;
    private Point m_resolution;
    private List<Color> m_colors;
    private bool m_thumbLoaded;
    private bool m_vignetteLoaded;
    private Color m_color1;
    private Color m_color2;
    private Color m_color3;
    private bool m_thumbLoading;
    private bool m_vignetteLoading;
    private System.Object m_thumbLoadingLock = new System.Object();
    private System.Object m_vignetteLoadingLock = new System.Object();
    #endregion

    /// <summary>
    /// TvdbFanartBanner constructor
    /// </summary>
    public TvdbFanartBanner()
    {

    }

    /// <summary>
    /// TvdbFanartBanner constructor
    /// </summary>
    public TvdbFanartBanner(int _id, String _path, TvdbLanguage _lang)
    {
      this.Id = _id;
      this.BannerPath = _path;
      this.Language = _lang;
    }

    /// <summary>
    /// Is the vignette image already loaded
    /// </summary>
    public bool IsVignetteLoaded
    {
      get { return m_vignetteLoaded; }
    }

    /// <summary>
    /// Is the vignette currently beeing loaded
    /// </summary>
    public bool VignetteLoading
    {
      get { return m_vignetteLoading; }
      set { m_vignetteLoading = value; }
    }

    /// <summary>
    /// Is the thumbnail currently beeing loaded
    /// </summary>
    public bool ThumbLoading
    {
      get { return m_thumbLoading; }
      set { m_thumbLoading = value; }
    }

    /// <summary>
    /// Vignette Image
    /// </summary>
    public Image VignetteImage
    {
      get { return m_vignette; }
      set { m_vignette = value; }
    }

    /// <summary>
    /// These are the colors selected by the artist that match the image. The format is 3 colors separated by a pipe "|". This field has leading and trailing pipes. Each color is comma separated RGB, with each color portion being an integer from 1 to 255. So the format looks like |r,g,b|r,g,b|r,g,b|. The first color is the light accent color. The second color is the dark accent color. The third color is the neutral mid-tone color. 
    /// </summary>
    public List<Color> Colors
    {
      get { return m_colors; }
      set { m_colors = value; }
    }

    /// <summary>
    /// Path to the fanart vignette
    /// </summary>
    public String VignettePath
    {
      get { return m_vignettePath; }
      set { m_vignettePath = value; }
    }

    /// <summary>
    /// Path to the fanart thumbnail
    /// </summary>
    public String ThumbPath
    {
      get { return m_thumbPath; }
      set { m_thumbPath = value; }
    }

    /// <summary>
    /// Color 3 (see Colors property)
    /// </summary>
    public Color Color3
    {
      get { return m_color3; }
      set { m_color3 = value; }
    }

    /// <summary>
    /// Color 2 (see Colors property)
    /// </summary>
    public Color Color2
    {
      get { return m_color2; }
      set { m_color2 = value; }
    }

    /// <summary>
    /// Color 1 (see Colors property)
    /// </summary>
    public Color Color1
    {
      get { return m_color1; }
      set { m_color1 = value; }
    }

    /// <summary>
    /// Resolution of the fanart
    /// </summary>
    public Point Resolution
    {
      get { return m_resolution; }
      set { m_resolution = value; }
    }

    /// <summary>
    /// Image of the thumbnail
    /// </summary>
    public Image BannerThumb
    {
      get { return m_bannerThumb; }
      set { m_bannerThumb = value; }
    }

    /// <summary>
    /// Load the vignette from tvdb
    /// </summary>
    /// <returns></returns>
    public bool LoadVignette()
    {
      return LoadVignette(false);
    }

    /// <summary>
    /// Load the vignette from tvdb
    /// </summary>
    /// <returns></returns>
    public bool LoadVignette(bool _replaceOld)
    {
      bool wasLoaded = m_vignetteLoaded;//is the banner already loaded at this point
      lock (m_vignetteLoadingLock)
      {//if another thread is already loading THIS banner, the lock will block this thread until the other thread
        //has finished loading
        if (!wasLoaded && !_replaceOld && m_vignetteLoaded)
        {////the banner has already been loaded from a different thread and we don't want to replace it
          return false;
        }
        m_vignetteLoading = true;
        try
        {
          Image img = LoadImage(TvdbLinks.CreateBannerLink(m_vignettePath));

          if (img != null)
          {
            m_vignette = img;
            m_vignetteLoaded = true;
            m_vignetteLoading = false;
            return true;
          }
        }
        catch (WebException ex)
        {
          Log.Error("Couldn't load banner thumb" + m_thumbPath, ex);
        }
        m_vignetteLoaded = false;
        m_vignetteLoading = false;
        return false;
      }
    }

    /// <summary>
    /// Load vignette with given image
    /// </summary>
    /// <param name="_img"></param>
    /// <returns></returns>
    public bool LoadVignette(Image _img)
    {
      if (_img != null)
      {
        m_vignette = _img;
        m_vignetteLoaded = true;
        return true;
      }
      else
      {
        m_vignetteLoaded = false;
        return false;
      }
    }

    /// <summary>
    /// Load the thumb from tvdb
    /// </summary>
    /// <returns></returns>
    public bool LoadThumb()
    {
      return LoadThumb(false);
    }


    /// <summary>
    /// Load the thumb from tvdb
    /// </summary>
    /// <returns></returns>
    public bool LoadThumb(bool _replaceOld)
    {
      bool wasLoaded = m_thumbLoaded;//is the banner already loaded at this point
      lock (m_thumbLoadingLock)
      {//if another thread is already loading THIS banner, the lock will block this thread until the other thread
        //has finished loading
        if (!wasLoaded && !_replaceOld && m_thumbLoaded)
        {////the banner has already been loaded from a different thread and we don't want to replace it
          return false;
        }
        m_thumbLoading = true;
        if (m_thumbPath != null)
        {
          try
          {
            Image img = LoadImage(TvdbLinks.CreateBannerLink(m_thumbPath));

            if (img != null)
            {
              m_bannerThumb = img;
              m_thumbLoaded = true;
              m_thumbLoading = false;
              return true;
            }
          }
          catch (WebException ex)
          {
            Log.Error("Couldn't load banner thumb" + m_thumbPath, ex);
          }
        }
        m_thumbLoaded = false;
        m_thumbLoading = false;
        return false;
      }
    }

    /// <summary>
    /// Load thumbnail with given image
    /// </summary>
    /// <param name="_img"></param>
    /// <returns></returns>
    public bool LoadThumb(Image _img)
    {
      if (_img != null)
      {
        m_bannerThumb = _img;
        m_thumbLoaded = true;
        return true;
      }
      else
      {
        m_thumbLoaded = false;
        return false;
      }
    }

    /// <summary>
    /// Is the Image of the thumb already loaded
    /// </summary>
    public bool IsThumbLoaded
    {
      get { return m_thumbLoaded; }
    }
  }
}
