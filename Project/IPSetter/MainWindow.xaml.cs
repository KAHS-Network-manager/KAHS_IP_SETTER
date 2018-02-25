using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Security.Principal;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            tb_grade.Focus();

            tb_grade.MaxLength = tb_class.MaxLength = 1;
            tb_number.MaxLength = 2;
        }

        private void bt_set_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(tb_grade.Text, out var gd) || gd > 3 || gd < 1)
            {
                MessageBox.Show("학년을 제대로 입력 해 주세요", "입력 오류", MessageBoxButton.OK);
                return;
            }
            if (!int.TryParse(tb_class.Text, out var cs) || cs > 4 || cs < 1)
            {
                MessageBox.Show("반을 제대로 입력 해 주세요", "입력 오류", MessageBoxButton.OK);
                return;
            }
            if (!int.TryParse(tb_number.Text, out var nb) || nb >= 28 || nb < 1)
            {
                MessageBox.Show("번호를 제대로 입력 해 주세요", "입력 오류", MessageBoxButton.OK);
                return;
            }

            var sb = new StringBuilder();

            sb.Append("10.44."); 
            sb.Append($"{63 - gd / 3}.{9 + nb + 108 * (gd / 2) - 38 * (gd / 3) + (cs - 1) * 28}");

            SetIp(sb.ToString());
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    return;
                case Key.Enter:
                    bt_set_Click(sender, new RoutedEventArgs());
                    break;
            }
        }

        private void SetIp(string ip)
        {
            const string subnetMask = "255.255.252.0";
            const string dns = "168.126.63.1,168.126.63.2";
            const string gateway = "10.44.60.1";
                
            var mos = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var moc = mos.GetInstances();

            var isSet = false;

            foreach (var o in moc)
            {
                using (var mo = (ManagementObject) o)
                {
                    if (!(bool) mo["IPEnabled"] || 
                        mo["Description"].ToString().IndexOf("Realtek", StringComparison.Ordinal) < 0 &&
                        mo["Description"].ToString().IndexOf("Killer", StringComparison.Ordinal) < 0 &&
                        mo["Description"].ToString().IndexOf("Atheros", StringComparison.Ordinal) < 0 &&
                        mo["Description"].ToString().IndexOf("Broadcom", StringComparison.Ordinal) < 0
                    )
                        continue;
                    try
                    {
                        var newIp = mo.GetMethodParameters("EnableStatic");
                        newIp["IPAddress"] = new[] {ip};
                        newIp["SubnetMask"] = new[] {subnetMask};

                        mo.InvokeMethod("EnableStatic", newIp, null);

                        var newGateway = mo.GetMethodParameters("SetGateways");

                        newGateway["DefaultIPGateway"] = new[] {gateway};
                        newGateway["GatewayCostMetric"] = new[] {1};

                        mo.InvokeMethod("SetGateways", newGateway, null);

                        var newDns = mo.GetMethodParameters("SetDNSServerSearchOrder");
                        newDns["DNSServerSearchOrder"] = dns.Split(',');
                        mo.InvokeMethod("SetDNSServerSearchOrder", newDns, null);

                        MessageBox.Show("설정 완료!", $"IP : {ip}", MessageBoxButton.OK);
                        isSet = true;
                        break;
                    }
                    catch (Exception e)
                    {
                        System.Windows.MessageBox.Show(e.Message, "에러");
                        Close();
                    }
                }
            }
            if (!isSet)
            {
                MessageBox.Show("카카오톡ID xklest로 문의 해 주십시오.", "개발자에 문의하십시오.");
            }
            Close();
        }

        private void tb_grade_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (tb_grade.Text.Length > 0)
                tb_class.Focus();
        }

        private void tb_class_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (tb_class.Text.Length > 0)
                tb_number.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mos = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var moc = mos.GetInstances();

            bool isSet = false;

            foreach (var o in moc)
            {
                using (var mo = (ManagementObject) o)
                {
                    if (!(bool) mo["IPEnabled"] ||
                        mo["Description"].ToString().IndexOf("Realtek", StringComparison.Ordinal) < 0 &&
                        mo["Description"].ToString().IndexOf("Killer", StringComparison.Ordinal) < 0 &&
                        mo["Description"].ToString().IndexOf("Atheros", StringComparison.Ordinal) < 0 &&
                        mo["Description"].ToString().IndexOf("Broadcom", StringComparison.Ordinal) < 0
                    )
                        continue;
                    try
                    {
                        mo.InvokeMethod("EnableDHCP", null);

                        var newDns = mo.GetMethodParameters("SetDNSServerSearchOrder");
                        newDns["DNSServerSearchOrder"] = null;
                        mo.InvokeMethod("SetDNSServerSearchOrder", newDns, null);

                        MessageBox.Show("설정 완료!", "", MessageBoxButton.OK);
                        isSet = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "에러");
                        Close();
                    }
                }
            }
            if (!isSet)
            {
                MessageBox.Show("카카오톡ID xklest로 문의 해 주십시오.", "개발자 에게 문의 하십시오.");
            }
            Close();
        }
    }
}
