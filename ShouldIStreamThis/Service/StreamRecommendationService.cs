namespace ShouldIStreamThis.Service
{
    public class StreamRecommendationService
    {
        public string CheckActiveStreams(int totalChannels) => totalChannels switch
        {
            < 12 => "No",
            < 15 => "Maybe",
            < 80 => "Yes",
            < 90 => "Maybe",
            _ => "No" // Covers 90 and everything above
        };

        public string CheckViewersPerChannel(int totalViewers, int totalChannels)
        {
            // Guard clause to prevent invalid division/infinity
            if (totalChannels <= 0) return "Red";

            double average = (double)totalViewers / totalChannels;

            return average switch
            {
                < 10 => "Red",
                < 15 => "YellowGreen",
                < 20 => "Green",
                < 25 => "YellowGreen",
                _ => "Red"
            };
        }
    }
}
