using System.Collections.Generic;

namespace Lib
{
  public class DistributionChannel
  {
    public string Name { get; set; }
    public Product Product { get; set; }
    public List<Asset> Assets { get; set; }
    
		public DistributionChannel()
    {
      Assets = new List<Asset>();
    }

    public Asset GetAssetByTitleAndTrackNumber(string title, string trackNumber)
    {
      Asset returnedAsset = null;
      foreach(Asset asset in Assets)
      {
        if(asset.TrackTitle == title && asset.TrackNumber == trackNumber)
        {
          returnedAsset = asset;
          break;
        }
      }
      return returnedAsset;
    }

    public Asset GetAssetByTrackTitleVolumeAndTrackNumber(string trackTitle, string volume, string trackNumber)
    {
      Asset returnedAsset = null;
      foreach(Asset asset in Assets)
      {
        if(asset.TrackTitle == trackTitle && asset.Volume == volume && asset.TrackNumber == trackNumber)
        {
          returnedAsset = asset;
          break;
        }
      }
      return returnedAsset;
    }
  }
}