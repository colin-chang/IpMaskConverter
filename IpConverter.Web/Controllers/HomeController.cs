using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IpConverter.Web.Models;

namespace IpConverter.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public IActionResult Convert2UintNumber(IEnumerable<string> ips)
        //{
        //    var result = new Dictionary<string, long>();
        //    foreach (string ip in ips)
        //        result[ip] = ip.ToLong();

        //    txtResult.Text = string.Join("\r\n", result.Values.Where(n => n >= 0));

        //    var errors = result.Keys.Where(k => result[k] < 0);
        //}

        /*
        private void btn2Number_Click(object sender, EventArgs e)
        {
            IEnumerable<string> ips;
            if (!TryGetSource(out ips))
                return;

            var result = new Dictionary<string, long>();
            foreach (string ip in ips)
                result[ip] = ip.ToLong();

            txtResult.Text = string.Join("\r\n", result.Values.Where(n => n >= 0));

            var errors = result.Keys.Where(k => result[k] < 0);
            HandleErrors(errors);
        }

        private void btn2Whole_Click(object sender, EventArgs e)
        {
            IEnumerable<string> ips;
            if (!TryGetSource(out ips))
                return;

            var result = new Dictionary<string, string>();
            IPAddress addr;
            foreach (string ip in ips)
                result[ip] = IPAddress.TryParse(ip, out addr) ? addr.ToString() : ConvertUtil.ToLong(ip, -1).ToIP();

            txtResult.Text = string.Join("\r\n", result.Values.Where(n => !string.IsNullOrWhiteSpace(n)));

            var errors = result.Keys.Where(k => string.IsNullOrWhiteSpace(result[k]));
            HandleErrors(errors);
        }


        private void btn2Location_Click(object sender, EventArgs e)
        {
            IEnumerable<string> ips;
            if (!TryGetSource(out ips))
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择IP地址库文件";
            ofd.Filter = "文本文件(*.txt)|*.txt";
            if (ofd.ShowDialog(this) != DialogResult.OK)
                return;

            var ipBase = File.ReadAllLines(ofd.FileName).Select(ln =>
            {
                var parts = ln.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 4)
                    throw new Exception($"{ln} 格式不正确");

                string location;
                if (parts.Length < 5)
                    location = "未知";
                else
                {
                    var la = new List<string>();
                    for (int i = 4; i < parts.Length; i++)
                        la.Add(parts[i]);
                    location = string.Join("-", la);
                }

                return new IpPeriod(parts[2], parts[3], location);
            });

            var summary = new Dictionary<string, int>();
            var errors = new List<string>();
            foreach (string ip in ips)
            {
                var ipn = ip.ToLong();
                if (ipn < 0)
                {
                    errors.Add(ip);
                    continue;
                }

                string loc = ipBase.FirstOrDefault(b => ipn >= b.Start && ipn <= b.End)?.Location ?? "未知";
                txtResult.AppendText($"{ipn.ToIP()}/{ipn}, {loc}\r\n");
                summary[loc] = summary.ContainsKey(loc) ? summary[loc] + 1 : 1;
            }

            txtResult.AppendText($"\r\n****************************************\r\n");
            foreach (string loca in summary.Keys)
                txtResult.AppendText($"{loca}\t{summary[loca]}\r\n");

            HandleErrors(errors);
        }

        private void btn2IpMask_Click(object sender, EventArgs e)
        {
            IEnumerable<string> periods;
            if (!TryGetSource(out periods))
                return;

            var errors = new List<string>();
            foreach (string period in periods)
            {
                var se = period.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(ip => ip.ToLong()).ToList();
                if (se.Count() < 2)
                    se.Add(se.FirstOrDefault());
                if (se.Min() < 0 || se.FirstOrDefault() > se.LastOrDefault())
                {
                    errors.Add(period);
                    continue;
                }

                txtResult.AppendText(string.Join("\r\n", IpConverter.ToIpAndMask((uint)se.FirstOrDefault(), (uint)se.LastOrDefault())) + "\r\n");
            }

            HandleErrors(errors);
        }

        private void btn2IpList_Click(object sender, EventArgs e)
        {
            IEnumerable<string> ims;
            if (!TryGetSource(out ims))
                return;

            var errors = new List<string>();
            foreach (string im in ims)
            {
                try
                {
                    txtResult.AppendText(string.Join("\r\n", IpConverter.ToIpList(im)));
                }
                catch
                {
                    errors.Add(im);
                }
            }

            HandleErrors(errors);
        }

        private void btn2IpPeriod_Click(object sender, EventArgs e)
        {
            IEnumerable<string> ims;
            if (!TryGetSource(out ims))
                return;

            var errors = new List<string>();
            foreach (string im in ims)
            {
                try
                {
                    var period = im.ToIpPeriod();
                    txtResult.AppendText($"{period.StartIp},{period.EndIp}\r\n");
                }
                catch
                {
                    errors.Add(im);
                }
            }

            HandleErrors(errors);
        }

        private void lkbtnExport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string content = txtResult.Text;
            if (string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show(this, "没有任何结果，不能导出数据", "消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "导出转换结果";
            sfd.Filter = "文本文件(*.txt)|*.txt";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                File.WriteAllText(sfd.FileName, content);
                MessageBox.Show(this, "数据导出成功", "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"数据导出失败,错误消息：\r\n{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSource.Clear();
            txtResult.Clear();
        }

        private bool TryGetSource(out IEnumerable<string> lines)
        {
            txtResult.Text = string.Empty;
            lines = null;

            string src = txtSource.Text.Trim();
            if (string.IsNullOrWhiteSpace(src))
                return false;

            lines = src.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Where(ln=>!string.IsNullOrWhiteSpace(ln));
            if (lines == null || !lines.Any())
                return false;

            return true;
        }

        private void HandleErrors(IEnumerable<string> errors, string errorMessage = null)
        {
            if (errors == null || !errors.Any())
                return;

            errorMessage = errorMessage ?? "转换完成，但有错误";
            MessageBox.Show(this, $"{errorMessage}。以下内容转换失败：\r\n{string.Join("\r\n", errors)}", "消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        
        */
    }
}
