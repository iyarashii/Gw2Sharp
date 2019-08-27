using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2Sharp.Models
{
    // data model for progress reporting
    public class ProgressReportModel
    {
        public double PercentageComplete { get; set; } = 0.0;
        public List<WebsiteDataModel> SitesDownloaded { get; set; } = new List<WebsiteDataModel>();
    }
}
