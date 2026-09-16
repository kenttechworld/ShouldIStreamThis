using ShouldIStreamThis.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ShouldIStreamThis.Handlers
{
    public class ToStreamOrNotToStreamHandler
    {
        public string DetermineStreamingDecision(int totalViewers, int totalChannels)
        {
            StreamRecommendationService streamRecommendationService = new();

            if (streamRecommendationService.CheckActiveStreams(totalChannels) == "Yes")
            {
                return streamRecommendationService.CheckViewersPerChannel(totalViewers, totalChannels);
            }
            else if (streamRecommendationService.CheckActiveStreams(totalChannels) == "Maybe")
            {
                if (streamRecommendationService.CheckViewersPerChannel(totalViewers, totalChannels) == "Red")
                {
                    return "red";
                }
                else
                {
                    return "YellowGreen";
                }
            }
            return "red";
        }
    }
}
