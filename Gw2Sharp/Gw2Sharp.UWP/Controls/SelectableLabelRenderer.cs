using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Gw2Sharp.Controls.SelectableLabel), typeof(Gw2Sharp.UWP.Controls.SelectableLabelRenderer))]

namespace Gw2Sharp.UWP.Controls
{
    // custom renderer class that replaces SelectableLabel renderer on UWP platform
    public class SelectableLabelRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;

            // prevent from editing text inside Editor
            Control.IsReadOnly = true;
            // set border thickness to 0 to hide borders
            Windows.UI.Xaml.Thickness none = new Windows.UI.Xaml.Thickness(0,0,0,0);
            Control.BorderThickness = none;
        }
    }
}
