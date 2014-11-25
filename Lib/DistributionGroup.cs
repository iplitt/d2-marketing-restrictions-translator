using System.Collections.Generic;

namespace Lib
{
  public class DistributionGroup
  {
    public string Name { get; set; }
    public List<DistributionChannel> DistributionChannels { get; set; }
    
		public DistributionGroup()
    {
      DistributionChannels = new List<DistributionChannel>();
      Name = "";
    }

    public DistributionChannel GetDistributionChannelByName(string channelName)
    {
      DistributionChannel result = null;
      foreach(DistributionChannel channel in DistributionChannels)
      {
        if (channel.Name == channelName)
        {
          result = channel;
          break;
        }
      }
      return result;
    }
  }
}
