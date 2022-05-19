// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Gw2Sharp/blob/master/LICENSE for license details.

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
