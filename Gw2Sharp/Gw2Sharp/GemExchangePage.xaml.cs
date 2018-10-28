using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2NET;
using GW2NET.Commerce;
using GW2NET.Common;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gw2Sharp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GemExchangePage : ContentPage
	{
        public string GemInfo { get; set; }
        public GemExchangePage()
		{
            InitializeComponent();
            GemInfo = "Current gem to gold exchange:";
            BindingContext = this;
        }

        // TO DO something for android
        void OnCalculateGoldToGems(object sender, EventArgs e)
        {
            BindingContext = null;
            GemInfo = "Current gem to gold exchange:\n";
            IBroker<GemQuotation> service = GW2.V2.Commerce.Exchange.ForCurrency("coins");
            if (!double.TryParse(goldAmount.Text, out double coins))
            {
                GemInfo += goldAmount.Text + " is not a number.";
                BindingContext = this;
                return;
            }
            coins = Convert.ToDouble(goldAmount.Text) * 10000.0;
            GemQuotation quotation;
            try
            {
                quotation = service.GetQuotation(Convert.ToInt64(coins));
            }
            catch (ServiceException)
            {
                GemInfo += "too few coins.";
                BindingContext = this;
                return;
            }
            catch (OverflowException)
            {
                GemInfo += "too many coins.";
                BindingContext = this;
                return;
            }
            catch (TaskCanceledException)
            {
                GemInfo += "Unknown exception!";
                BindingContext = this;
                return;
            }
            
            double goldPerGemRatio = quotation.CoinsPerGem / 10000.0;
            GemInfo += "Gold per gem: " + goldPerGemRatio + "\n";
            GemInfo += "Send: " + quotation.Send / 10000.0 + " gold\n";
            GemInfo += "To receive: " + quotation.Receive + " gems\n";
            GemInfo += "For 400 gems you have to pay about " + goldPerGemRatio * 400 + " gold\n";
            GemInfo += "For 800 gems you have to pay about " + goldPerGemRatio * 800 + " gold\n";
            GemInfo += "For 1200 gems you have to pay about " + goldPerGemRatio * 1200 + " gold\n";
            GemInfo += "For 2000 gems you have to pay about " + goldPerGemRatio * 2000 + " gold\n";
            GemInfo += "Timestamp: " + quotation.Timestamp + "\n";
            BindingContext = this;
        }
    }
}