using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HostsEdit
{
    public partial class Form1 : Form
    {
        private readonly string HOSTSfilepath = string.Format("{0}\\drivers\\etc\\hosts", Environment.GetFolderPath(Environment.SpecialFolder.System));
        private readonly Color invalidRowColor = Color.FromArgb(255, 199, 206);

        public Form1()
        {
            InitializeComponent();

            this.Text = Application.ProductName + " v" + Application.ProductVersion;

            if (!File.Exists(HOSTSfilepath))
            {
                panel1.Enabled = false;
                return;
            }

            FillGrid();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;

                txtSearch.Text = txtSearch.Text.Trim();

                if (txtSearch.Text.Length == 0)
                    return;

                dg.ClearSelection();

                var g = dg.Rows.OfType<DataGridViewRow>().Where(x => x.Cells[0].Value != null && x.Cells[0].Value.ToString().IndexOf(txtSearch.Text.Trim(), StringComparison.OrdinalIgnoreCase) > -1).ToList();

                this.Text = Application.ProductName + " v" + Application.ProductVersion + " - Found : " + g.Count;

                if (g.Count == 0)
                    return;

                foreach (DataGridViewRow item in g)
                {
                    item.Cells[0].Selected = true;
                }

                dg.FirstDisplayedScrollingRowIndex = g[0].Index;

                dg.Focus();
            }
        }

        private void dg_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dg.Rows[e.RowIndex].Cells[0].Value == null || dg.Rows[e.RowIndex].Cells[0].Value.ToString().StartsWith("#"))
                return;

            string g = dg.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();

            if (!(g.StartsWith("127.0.0.1") || g.StartsWith("0.0.0.0")))
                dg.Rows[e.RowIndex].Cells[0].Value = "127.0.0.1 " + g;
            else
                dg.Rows[e.RowIndex].Cells[0].Value = g;

            Signalize9entries();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            dg.Rows.OfType<DataGridViewRow>().ToList().ForEach(row => row.Cells[0].Selected = true);

            Clipboard.SetDataObject(dg.GetClipboardContent());

            File.Copy(HOSTSfilepath, HOSTSfilepath + ".pc.bak", true);

            File.WriteAllText(HOSTSfilepath, Clipboard.GetText().Trim());

            dg.ClearSelection();

            Shake(this);
        }

        private void brnRefresh_Click(object sender, EventArgs e)
        {
            FillGrid();
        }

        private void FillGrid()
        {
            dg.Rows.Clear();

            string[] k = File.ReadAllLines(HOSTSfilepath);

            foreach (string item in k)
                dg.Rows.Add(item);

            Signalize9entries();

            dg.Focus();
        }

        private void dg_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (dg.SelectedCells.Count > 0)
                {
                    toolStripDividePer9.Visible = (dg.SelectedCells.Count == 1 && dg.SelectedCells[0].Style.BackColor == invalidRowColor);

                    ctxGrid.Show(System.Windows.Forms.Cursor.Position);
                }
            }
        }

        private void toolStripDelete_Click(object sender, EventArgs e)
        {
            var r = dg.Rows.OfType<DataGridViewRow>().Where(x => x.Cells[0].Selected).ToList();
            foreach (var match in r)
                dg.Rows.Remove(match);
        }

        private void toolStripCopy_Click(object sender, EventArgs e)
        {
            var r = dg.Rows.OfType<DataGridViewRow>().Where(x => x.Cells[0].Value != null && x.Cells[0].Selected).Select(x => x.Cells[0].Value.ToString()).ToList();

            if (r.Count == 0)
                return;

            Copy2Clipboard(string.Join("\r\n", r));

            MessageBox.Show(r.Count + " row(s) copied to clipboard!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void Shake(Form form)
        {   //https://stackoverflow.com/a/16690934

            var original = form.Location;
            var rnd = new Random(1337);
            const int shake_amplitude = 10;
            for (int i = 0; i < 10; i++)
            {
                form.Location = new Point(original.X + rnd.Next(-shake_amplitude, shake_amplitude), original.Y + rnd.Next(-shake_amplitude, shake_amplitude));
                System.Threading.Thread.Sleep(20);
            }
            form.Location = original;
        }

        private static void Copy2Clipboard(string val)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(val, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ProductName);
            }
        }

        private void toolStripConsolidate_Click(object sender, EventArgs e)
        {
            var selectDG = dg.Rows.OfType<DataGridViewRow>().Where(x => x.Cells[0].Value != null && x.Cells[0].Selected &&
                                                                x.Cells[0].Value.ToString().IndexOf(" ") > 0 &&
                                                                !x.Cells[0].Value.ToString().StartsWith("#")
                                                            );

            var r = selectDG
                    .Select(s => s.Cells[0].Value.ToString().Split(' '))
                    .Select(x =>
                    {
                        //if (x.Length == 2)
                        //    return x[1];
                        //else
                        return string.Join(" ", x.Skip(1));

                    }).ToList();

            if (r.Count < 2)
            {
                MessageBox.Show("Please select two or more valid rows", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string k = string.Format("127.0.0.1 {0}", string.Join(" ", r));

            //delete
            var rx = selectDG.Skip(1).ToList();
            foreach (var match in rx)
                dg.Rows.Remove(match);

            //modify first
            var t = selectDG.First();
            t.Cells[0].Value = k;

            Signalize9entries();
        }

        private void toolStripDivide_Click(object sender, EventArgs e)
        {
            var selectDG = dg.Rows.OfType<DataGridViewRow>().Where(x => x.Cells[0].Value != null && x.Cells[0].Selected &&
                                                    x.Cells[0].Value.ToString().Split(' ').Length > 2 &&
                                                    !x.Cells[0].Value.ToString().StartsWith("#")
                                                ).Select(s => new { Value = s.Cells[0].Value.ToString(), RowIndex = s.Index }

                                                            ).ToList();

            if (selectDG.Count != 1)
            {
                MessageBox.Show("Please select a row has multiple hosts", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var match in selectDG)
            {
                string[] f = match.Value.Split(' ');

                foreach (string item in f.Skip(2).Reverse())
                {
                    dg.Rows.Insert(match.RowIndex + 1, string.Format("127.0.0.1 {0}", item));
                }

                dg.Rows[match.RowIndex].Cells[0].Value = string.Join(" ", f.Take(2));

                if (dg.Rows[match.RowIndex].Cells[0].Tag != null)
                {
                    dg.Rows[match.RowIndex].Cells[0].Style.BackColor = (Color)dg.Rows[match.RowIndex].Cells[0].Tag;
                    dg.Rows[match.RowIndex].Cells[0].Tag = null;
                }
            }
        }


        private void toolStripDividePer9_Click(object sender, EventArgs e)
        {
            var d = dg.SelectedCells[0].Value.ToString().Split(' ').Skip(1).ToList().SplitPer(9);

            int selectedRow = dg.SelectedCells[0].RowIndex;


            dg.Rows[selectedRow].Cells[0].Value = string.Format("127.0.0.1 {0}", string.Join(" ", d.First()));
            dg.Rows[selectedRow].Cells[0].Style.BackColor = (Color)dg.Rows[selectedRow].Cells[0].Tag;
            dg.Rows[selectedRow].Cells[0].Tag = null;

            foreach (var item in d.Skip(1).Reverse())
            {
                dg.Rows.Insert(selectedRow + 1, string.Format("127.0.0.1 {0}", string.Join(" ", item)));
            }

            Console.WriteLine(d);
        }

        private void btnHosts_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", string.Format("/select,\"{0}\"", HOSTSfilepath));

        }

        private void Signalize9entries()
        {
            dg.Rows.OfType<DataGridViewRow>().ToList().ForEach(row =>
                {
                    if (row.Cells[0].Value != null && !row.Cells[0].Value.ToString().StartsWith("#") && row.Cells[0].Value.ToString().Split(' ').Length > 10)
                    {
                        row.Cells[0].Tag = row.Cells[0].Style.BackColor;
                        row.Cells[0].Style.BackColor = invalidRowColor;
                    }
                }
            );
        }
    }

    public static class StringExtensions
    {
        public static List<List<T>> SplitPer<T>(this List<T> collection, int size)
        {   // https://codereview.stackexchange.com/a/90198 + https://codereview.stackexchange.com/a/90531
            var chunks = new List<List<T>>();
            var chunkCount = collection.Count() / size;

            if (collection.Count % size > 0)
                chunkCount++;

            for (var i = 0; i < chunkCount; i++)
                chunks.Add(collection.Skip(i * size).Take(size).ToList());

            return chunks;
        }
    }
}
