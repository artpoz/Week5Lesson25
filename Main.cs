﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
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

            _groups = GroupsHelper.GetGroups("Wszystkie");

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

        public void RefreshDiary()
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
            //dgvDiary.Columns[nameof(Student.Id)].HeaderText = "Numer";
            dgvDiary.Columns[nameof(Student.FirstName)].HeaderText = "Imię";
            dgvDiary.Columns[nameof(Student.LastName)].HeaderText = "Nazwisko";
            dgvDiary.Columns[nameof(Student.Comments)].HeaderText = "Uwagi";
            dgvDiary.Columns[nameof(Student.Math)].HeaderText = "Matematyka";
            dgvDiary.Columns[nameof(Student.Technology)].HeaderText = "Technologia";
            dgvDiary.Columns[nameof(Student.Physics)].HeaderText = "Fizyka";
            dgvDiary.Columns[nameof(Student.PolishLang)].HeaderText = "Język polski";
            dgvDiary.Columns[nameof(Student.ForeignLang)].HeaderText = "Język obcy";
            dgvDiary.Columns[nameof(Student.Extra)].HeaderText = "Zajęcia dodatkowe";
            //dgvDiary.Columns[10].HeaderText = "Grupa";
        }

        private void HideColumns()
        {
            dgvDiary.Columns[nameof(Student.Id)].Visible = false;
            dgvDiary.Columns[nameof(Student.GroupId)].Visible = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addEditStudent = new AddEditStudent();
            addEditStudent.FormClosing += AddEditStudent_FormClosing;
            addEditStudent.ShowDialog();
        }

        private void AddEditStudent_FormClosing(object sender, FormClosingEventArgs e)
        {
            RefreshDiary();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvDiary.SelectedRows.Count == 0)
            {
                MessageBox.Show("Proszę zaznacz ucznia, którego dane chcesz edytować");
                return;
            }

            var addEditStudent = new AddEditStudent(Convert.ToInt32(dgvDiary.SelectedRows[0].Cells[0].Value));
            addEditStudent.FormClosing += AddEditStudent_FormClosing;
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

        private void cbFilter_SelectedValueChanged(object sender, EventArgs e)
        {
            RefreshDiary();
        }
    }
}
