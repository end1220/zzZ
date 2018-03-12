using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Float.Pages.Settings
{
	/// <summary>
	/// Interaction logic for About.xaml
	/// </summary>
	public partial class About : UserControl
	{
		public About()
		{
			InitializeComponent();

			aboutText.Text = "ABOUT是韓國時尚男女裝行動購物品牌 時尚潮流兼具 打造一個全新的自己。\n將生活的感觸化成專業舒適時尚的衣著,分享最新商品、最新消息與即時優惠外,\n也與您分享日常中的各種感觸,一起成長學習。";
		}
	}
}
