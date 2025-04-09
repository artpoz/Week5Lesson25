using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Week5Lesson25.Properties;

namespace Week5Lesson25
{
    public partial class Main : Form
    {
        private string _filePath = Program.FilePath;

        private FileHelper<List<Student>> _fileHelper = 
            new FileHelper<List<Student>>(Program.FilePath);

        private List<Group> _groups;

        public bool IsMaximize
        {
            get
            {
                return Settings.Default.IsMaximize;
            }
            set
            {
                Settings.Default.IsMaximize = value;
            }
        }
        public Main()
        {
            InitializeComponent();
                       
            _groups = new List<Group>();
            _groups.Add(new Group { Id = 0, Name = "Wszystkie" });
            _groups.Add(new Group { Id = 1, Name = "A1" });
            _groups.Add(new Group { Id = 2, Name = "A2" });
            _groups.Add(new Group { Id = 3, Name = "A3" });

            cbFilter.DataSource = _groups;
            cbFilter.DisplayMember = "Name";
            cbFilter.ValueMember = "Id";

            RefreshDiary();

            SetColumnHeader();

            if (IsMaximize)
            {
                WindowState = FormWindowState.Maximized;
            }

            HideColumns();
        }

        private void RefreshDiary()
        {
            var students = _fileHelper.DeserializeFromFile();
            
            var selectedGroupId = (cbFilter.SelectedItem as Group).Id;

            if (selectedGroupId != 0)
            {
                students = students.Where(x => x.GroupId == selectedGroupId).ToList();
            }            

            dgvDiary.DataSource = students;
        }

        private void SetColumnHeader()
        {
            dgvDiary.Columns[0].HeaderText = "Numer";
            dgvDiary.Columns[1].HeaderText = "Imię";
            dgvDiary.Columns[2].HeaderText = "Nazwisko";
            dgvDiary.Columns[3].HeaderText = "Uwagi";
            dgvDiary.Columns[4].HeaderText = "Matematyka";
            dgvDiary.Columns[5].HeaderText = "Technologia";
            dgvDiary.Columns[6].HeaderText = "Fizyka";
            dgvDiary.Columns[7].HeaderText = "Język polski";
            dgvDiary.Columns[8].HeaderText = "Język obcy";
            dgvDiary.Columns[9].HeaderText = "Zajęcia dodatkowe";
            dgvDiary.Columns[10].HeaderText = "Grupa";
        }

        private void HideColumns()
        {
            dgvDiary.Columns[0].Visible = false;
            dgvDiary.Columns[10].Visible = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addEditStudent = new AddEditStudent();
            addEditStudent.ShowDialog();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvDiary.SelectedRows.Count == 0)
            {
                MessageBox.Show("Proszę zaznacz ucznia, którego dane chcesz edytować");
                return;
            }

            var addEditStudent = new AddEditStudent(Convert.ToInt32(dgvDiary.SelectedRows[0].Cells[0].Value));
            addEditStudent.ShowDialog();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvDiary.SelectedRows.Count == 0)
            {
                MessageBox.Show("Proszę zaznacz ucznia, którego dane chcesz usunąć");
                return;
            }

            var selectedStudent = dgvDiary.SelectedRows[0];

            var confirmDelete = MessageBox.Show($"Czy na pewno chcesz usunąć ucznia " +
                $"{selectedStudent.Cells[1].Value.ToString()} " +
                $"{selectedStudent.Cells[2].Value.ToString()}",
                "Usuwanie ucznia",
                MessageBoxButtons.OKCancel);

            if (confirmDelete == DialogResult.OK)
            {
                DeleteStudent(Convert.ToInt32(selectedStudent.Cells[0].Value));
                RefreshDiary();
            }
        }

        private void DeleteStudent(int id)
        {
            var students = _fileHelper.DeserializeFromFile();
            students.RemoveAll(x => x.Id == id);            
            _fileHelper.SerializeToFile(students);
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDiary();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                IsMaximize = true;
            }
            else
            {
                IsMaximize = false;
            }

            Settings.Default.Save();
        }
    }
}
